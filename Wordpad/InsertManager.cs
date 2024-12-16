using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wordpad;
using Image = System.Windows.Controls.Image;

namespace Wordpad
{
    public class InsertManager
    {
        private RichTextBox _richTextBox;

        // Dictionary để quản lý hình ảnh
        private Dictionary<Guid, Image> imageDictionary = new Dictionary<Guid, Image>();


        // Kho lưu trữ vị trí icon để nhận dạng loại icon
        public static Dictionary<Image, string> iconLinks { get; private set; } = new Dictionary<Image, string>();

        // Khởi tạo với tham chiếu tới RichTextBox
        public InsertManager(RichTextBox richTextBox)
        {
            _richTextBox = richTextBox;
        }

        // Hàm tính toán tỷ lệ ảnh và tạo đối tượng Image với tỷ lệ gốc

        private double CalculateAspectRatio(BitmapImage image)
        {
            // Lấy kích thước thực tế của RichTextBox(- 10 vì khi chỉ trừ 2 padding thì nó vẫn bị tràn 1 xíu)
            double maxWidth = _richTextBox.ActualWidth - _richTextBox.Padding.Left - _richTextBox.Padding.Right - 10;
            double maxHeight = _richTextBox.ActualHeight - _richTextBox.Padding.Top;

            // Tính toán tỷ lệ chiều rộng và chiều cao
            double widthRatio = maxWidth / image.PixelWidth;
            double heightRatio = maxHeight / image.PixelHeight;

            double originalRatio = image.PixelWidth / image.PixelHeight;

            // Chọn tỷ lệ nhỏ hơn để đảm bảo ảnh không bị vỡ tỷ lệ
            double imageRatio = Math.Min(widthRatio, heightRatio);

            // Nếu ảnh nhỏ hơn khung RichTextBox, giữ nguyên kích thước
            if (image.PixelWidth <= maxWidth && image.PixelHeight <= maxHeight)
            {
                imageRatio = 1;  // Giữ nguyên tỷ lệ 100%
            }

            return imageRatio;
        }

        // Phương thức tạo khung ảnh phù hợp với khung rtb
        private Image GetImageWithAspectRatio(BitmapImage image)
        {
            double imageRatio = CalculateAspectRatio(image);

            // Tính toán kích thước mới của KHUNG CHỨA ảnh
            // Lưu ý: Kích thước thực tế của ảnh không thay đổi
            double newWidth = image.PixelWidth * imageRatio;
            double newHeight = image.PixelHeight * imageRatio;

            // Tạo đối tượng Image và thiết lập kích thước mới cho khung ảnh
            Image imgControl = new Image
            {
                Source = image, // Đây mới là ảnh gốc
                Width = newWidth,
                Height = newHeight
            };

            return imgControl;
        }

        // Phương thức xóa hình tại vị trí bất kì
        public void DeleteImageAtPosition(TextPointer selectedContent)
        {
            var (existed, para, inline, entry) = CheckIfImageExistInDictionary(selectedContent);
            if (existed && entry.HasValue)
            {
                // Xóa InlineUIContainer chứa hình ảnh
                para.Inlines.Remove(inline);

                // Xóa hình ảnh khỏi Dictionary
                imageDictionary.Remove(entry.Value.Key);
            }
        }

        public void MakeContainerForImageAndInsert(BitmapImage image)
        {
            // Tính toán tỷ lệ và lấy ảnh với tỷ lệ duy trì
            Image imgControl = GetImageWithAspectRatio(image);

            // Tạo một ID duy nhất cho mỗi hình ảnh
            Guid imageId = Guid.NewGuid();

            // Gắn ID duy nhất của tấm ảnh vào tag của nó
            imgControl.Tag = imageId;

            // Lưu ảnh vào dictionary với ID duy nhất của nó
            imageDictionary[imageId] = imgControl;

            // Tạo một InlineUIContainer để chứa hình ảnh
            InlineUIContainer container = new InlineUIContainer(imgControl);

            // Lấy vị trí con trỏ hiện tại và chèn hình ảnh tại đó
            TextSelection selection = _richTextBox.Selection;
            if (!selection.IsEmpty) // Nếu có văn bản được chọn, ta có thể thay thế nó bằng hình ảnh
            {
                // Lấy TextPointer từ vị trí con trỏ hiện tại (hoặc vùng chọn)
                TextPointer selectedContent = selection.Start;

                selection.Text = "";

                // Gọi hàm để tìm hình ảnh tại vị trí con trỏ (nếu có)
                DeleteImageAtPosition(selectedContent);
            }

            // Chèn InlineUIContainer vào RichTextBox tại vị trí con trỏ
            Paragraph para = new Paragraph();
            para.Inlines.Add(container);

            // Chèn đoạn văn bản (chứa hình ảnh) vào RichTextBox
            _richTextBox.Document.Blocks.Add(para);
        }
     

        public void InsertImage()
        {
            // Mở hộp thoại chọn file hình ảnh
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                BitmapImage image = new BitmapImage(new Uri(filePath));

                // Phương thức gán ảnh vào InlineUIContainer và chèn vào rtb
               MakeContainerForImageAndInsert(image);
            }
        }

        // Phương thức thay đổi hình ảnh
        public void ChangeImage()
        {
            // Kiểm tra xem có đoạn văn bản nào được chọn không
            if (_richTextBox.Selection.IsEmpty)
            {
                MessageBox.Show("Please select an image to change.", "Change Image", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Gọi lại phương thức chèn hình ảnh
            InsertImage();
        }

        // Phương thức kiểm tra nếu hình ảnh có tồn tại trong Dictionary và trả về các trị để thao tác với nó
        public (bool, Paragraph, Inline, KeyValuePair<Guid, Image>?) CheckIfImageExistInDictionary(TextPointer textPointer)
        {
            Paragraph para = textPointer.Paragraph;
            if (para != null)
            {
                var inlines = para.Inlines.ToList();
                foreach (Inline inline in inlines)
                {
                    if (inline is InlineUIContainer imageContainer)
                    {
                        if (imageContainer.Child is Image selectedImage)
                        {
                            foreach (var entry in imageDictionary)
                            {                                                           
                                if (entry.Value.Tag == selectedImage.Tag)
                                {                              
                                    // Trả về true, para, inline nếu tìm thấy hình ảnh
                                    return (true, para, inline, entry);
                                }
                            }
                        }
                    }
                }
            }
            // Nếu không tìm thấy, trả về false, null, null
            return (false, null, null, null);
        }

        public void ResizeImage()
        {
            TextPointer selectedContent = _richTextBox.Selection.Start;
            var (existed, para, inline, entry) = CheckIfImageExistInDictionary(selectedContent);
            if (existed && entry.HasValue)
            {
                /*
                  Vì entry là kiểu KeyValuePair<Guid, Image>?  => Tức là một kiểu nullable
                --> Nếu kiểm tra entry.HasValue == true thì dùng entry.Value để lấy đối tượng KeyValuePair<Guid, Image>
                --> entry.Value chứa thông tin của KeyValuePair<Guid, Image>, có hai phần:
                    + entry.Value.Key: Chính là khóa Guid của ảnh.
                    + entry.Value.Value: Chính là giá trị, tức là đối tượng Image cần thao tác.
                 */
                Image imageToResize = entry.Value.Value;

                BitmapImage resizedImage = imageToResize.Source as BitmapImage;
                double originalAspectRatio = CalculateAspectRatio(resizedImage);

                ResizeWindow resizeWindow = new ResizeWindow(
                    imageToResize,
                    _richTextBox.ActualWidth,
                    _richTextBox.ActualHeight,
                    originalAspectRatio);

                if (resizeWindow.ShowDialog() == true)
                {
                    // Thay đổi kích thước của KHUNG CHỨA ảnh trong InlineUIContainer
                    // Lưu ý: Kích thước thực tế của ảnh không thay đổi
                    if (inline is InlineUIContainer imageContainer)
                    {
                        if (imageContainer.Child is Image selectedImage && selectedImage == imageToResize)
                        {
                            selectedImage.Width = resizeWindow.newWidth;
                            selectedImage.Height = resizeWindow.newHeight;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No image found at the selected position.", "Resize Image", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }


        // Phương thức chèn ngày/giờ
        public void InsertDateTime(string chosenDateTime)
        {
            if(_richTextBox.Selection.IsEmpty)
            {
                FontManager fontManager = new FontManager(_richTextBox);
                fontManager.SettingForEmptySelectionCase();
            }

            TextRange selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            selection.Text = chosenDateTime;
        }

        public void DisplayDateTimeFormats(ListBox _listBox)
        {
            DateTime now = DateTime.Now;

            // Xóa nội dung hiện tại của _listBox
            _listBox.Items.Clear();

            // Thêm các loại định dạng
            _listBox.Items.Add(now.ToString("MM/d/yyyy"));           // 11/5/2024
            _listBox.Items.Add(now.ToString("MM/d/yy"));            // 11/5/24
            _listBox.Items.Add(now.ToString("M/dd/yy"));              // 11/05/24
            _listBox.Items.Add(now.ToString("MM/dd/yyyy"));           // 11/05/2024
            _listBox.Items.Add(now.ToString("yy/MM/dd"));            // 24/11/05
            _listBox.Items.Add(now.ToString("yyyy-MM-dd"));           // 2024-11-05
            _listBox.Items.Add(now.ToString("dd-MMM-yy"));           // 05-Nov-24
            _listBox.Items.Add(now.ToString("dddd, MMMM dd, yyyy"));   // Tuesday, November 5, 2024
            _listBox.Items.Add(now.ToString("MMMM dd, yyyy"));        // November 5, 2024
            _listBox.Items.Add(now.ToString("dddd, d MMMM, yyyy"));   // Tuesday, 5 November, 2024
            _listBox.Items.Add(now.ToString("d MMMM, yyyy"));         // 5 November, 2024
            _listBox.Items.Add(now.ToString("h:mm:ss tt"));          // 3:08:27 PM
            _listBox.Items.Add(now.ToString("hh:mm:ss tt"));            // 03:08:27 PM 
            _listBox.Items.Add(now.ToString("HH:mm:ss"));            // 15:08:27 
        }

        public void AddObjectTypes(ListBox listBox)
        {
            string[] objectTypes = {
                "Foxit PhantomPDF Document",
                "Microsoft Word Document",
                "Wordpad Document",
                "Microsoft Excel",
                "Microsoft PowerPoint Presentation",
                "Paint"
            };

            // Lặp qua từng mục và thêm vào ListBox
            foreach (var type in objectTypes)
            {
                listBox.Items.Add(type);
            }
        }

        public void InsertObjectAsIcon(string iconType, string directory)
        {
            string iconName = "";
            string appName = "";
            switch (iconType)
            {
                case "Foxit PhantomPDF Document":
                    iconName = "pdf.png";
                    appName = "Foxit PhantomPDF Document";
                    break;
                case "Microsoft Word Document":
                    iconName = "word.png";
                    appName = "Microsoft Word Document";
                    break;
                case "Wordpad Document":
                    iconName = "word.png";
                    appName = "Wordpad Document";
                    break;
                case "Microsoft Excel":
                    iconName = "excel.png";
                    appName = "Microsoft Excel";
                    break;
                case "Microsoft PowerPoint Presentation":
                    iconName = "powerpoint.png";
                    appName = "Microsoft PowerPoint Presentation";
                    break;
                case "Paint":
                    iconName = "paint.png";
                    appName = "Paint";
                    break;
                default:
                    iconName = "file.png";
                    break;
            }

            string iconPath = Path.Combine(ClipboardManager.imageDirectory, iconName);

            // Thay đổi kích thước icon cho phù hợp (size ảnh gốc to)
            BitmapSource resizedIcon = new BitmapImage(new Uri(iconPath));
            resizedIcon = new TransformedBitmap(resizedIcon, new ScaleTransform(0.1, 0.1)); // Giảm kích thước

            // Tạo khung chứa icon
            Image linkIcon = new Image
            {
                Source = resizedIcon,
                Width = resizedIcon.Width,
                Height = resizedIcon.Height,
            };
            
            if(directory != null)
            {
                iconLinks[linkIcon] = directory; // Nếu là loại file thì lưu đường dẫn đến file đó
            }    
            else
            {
                // Nếu là loại app office
                iconLinks[linkIcon] = appName;
            }    
           

            // Tạo container chứa khung icon
            InlineUIContainer container = new InlineUIContainer(linkIcon);

            // Lấy vùng chọn hiện tại
            TextSelection selection = _richTextBox.Selection;
            if(!selection.IsEmpty)
            {
                // Lấy textPointer từ vị trí con trỏ hiện tại (hoặc vùng chọn)
                TextPointer selectedText = selection.Start;

                selection.Text = "";

                // Phương thức xóa hình ảnh tại vị trí (vùng chọn) của con trỏ (nếu có)
                DeleteImageAtPosition(selectedText);
            }

            var para = selection.Start.Paragraph;

            // Nếu không có đoạn văn, tạo mới một đoạn văn
            if (para == null)
            {
                para = new Paragraph();
                _richTextBox.Document.Blocks.Add(para);
            }

            // Chèn InlineUIContainer vào Paragraph
            para.Inlines.Add(container);
        }

        public void OpenDocumentIfLinkIconClicked(object sender, MouseButtonEventArgs e)
        {
            string fileType = null;

            TextPointer selection = _richTextBox.Selection.Start;
            Paragraph para = selection.Paragraph;
            if (para != null)
            {
                var inlines = para.Inlines.ToList();
                foreach (Inline inline in inlines)
                {
                    if (inline is InlineUIContainer iconContainer)
                    {
                        if (iconContainer.Child is Image selectedIcon)
                        {
                            if (iconLinks.ContainsKey(selectedIcon))
                            {
                                fileType = iconLinks[selectedIcon];
                                OpenApplicationFromIconType(fileType);
                                return;
                            }
                        }
                    }
                }
            }
        }

        public void OpenApplicationFromIconType(string iconType)
        {
            string fileName = "";
            switch (iconType)
            {
                case "Wordpad Document":
                    fileName = "wordpad";
                    break;
                case "Foxit PhantomPDF Document":
                    fileName = @"C:\Program Files (x86)\Foxit Software\Foxit PhantomPDF\FoxitPhantomPDF.exe";
                    break;
                case "Microsoft Word Document":
                    fileName = "winword";
                    break;
                case "Microsoft Excel":
                    fileName = "excel";
                    break;
                case "Microsoft PowerPoint Presentation":
                    fileName = "powerpnt";
                    break;
                case "Paint":
                    fileName = "mspaint";
                    break;
                default:
                    fileName = iconType;
                    break;
            }

            // Mở ứng dụng từ fileName
            Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                UseShellExecute = true
            });
        }
    }
}
