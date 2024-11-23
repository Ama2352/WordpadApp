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

namespace WordPad
{
    public class InsertManager
    {
        private RichTextBox _richTextBox;

        // Dictionary để quản lý hình ảnh
        private Dictionary<Guid, Image> imageDictionary = new Dictionary<Guid, Image>();


        // Kho lưu trữ vị trí icon để nhận dạng loại icon
        public static Dictionary<int, string> iconLinks { get; private set; } = new Dictionary<int, string>();

        // Khởi tạo với tham chiếu tới RichTextBox
        public InsertManager(RichTextBox richTextBox)
        {
            _richTextBox = richTextBox;
        }

        // Hàm tính toán tỷ lệ ảnh và tạo đối tượng Image với tỷ lệ gốc

        private double CalculateAspectRatio(BitmapImage image)
        {
            // Lấy kích thước thực tế của RichTextBox
            double maxWidth = _richTextBox.ActualWidth;
            double maxHeight = _richTextBox.ActualHeight;

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

        public void InsertImage()
        {
            // Mở hộp thoại chọn file hình ảnh
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                BitmapImage image = new BitmapImage(new Uri(filePath));

                // Tính toán tỷ lệ và lấy ảnh với tỷ lệ duy trì
                Image imgControl = GetImageWithAspectRatio(image);

                // Tạo một ID duy nhất cho mỗi hình ảnh
                Guid imageId = Guid.NewGuid();

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

                    // Gọi hàm để tìm hình ảnh tại vị trí con trỏ
                    DeleteImageAtPosition(selectedContent);
                }

                // Chèn InlineUIContainer vào RichTextBox tại vị trí con trỏ
                Paragraph para = new Paragraph();
                para.Inlines.Add(container);

                // Chèn đoạn văn bản (chứa hình ảnh) vào RichTextBox
                _richTextBox.Document.Blocks.Add(para);
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
                                if (entry.Value == selectedImage)
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
        public void InsertDateTime()
        {
            string dateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            _richTextBox.AppendText(dateTime);
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
            string imageName = "";
            switch (iconType)
            {
                case "Foxit PhantomPDF Document":
                    imageName = "pdf.png";
                    break;
                case "Microsoft Word Document":
                    imageName = "word.png";
                    break;
                case "Wordpad Document":
                    imageName = "word.png";
                    break;
                case "Microsoft Excel":
                    imageName = "excel.png";
                    break;
                case "Microsoft PowerPoint Presentation":
                    imageName = "powerpoint.png";
                    break;
                case "Paint":
                    imageName = "paint.png";
                    break;
                default:
                    imageName = "file.png";
                    break;
            }

            string imagePath = Path.Combine(ClipboardManager.ImageDirectory, imageName);

            // Tạo đối tượng BitmapSource từ ImagePath
            BitmapSource icon = new BitmapImage(new Uri(imagePath));

            // Thay đổi kích thước ảnh
            BitmapSource resizedIcon = new BitmapImage(new Uri(imagePath));
            resizedIcon = new TransformedBitmap(resizedIcon, new ScaleTransform(0.1, 0.1)); // Giảm kích thước

            Clipboard.SetImage(resizedIcon);

            // Chuyển TextPointer thành chỉ số văn bản (int)
            int iconPosition = _richTextBox.Selection.Start.GetOffsetToPosition(_richTextBox.Selection.End);

            if (directory != null)
                iconLinks[iconPosition] = directory;
            else
                iconLinks[iconPosition] = iconType;

            _richTextBox.Paste();
        }

        public void OpenDocumentIfLinkIconClicked(object sender, MouseButtonEventArgs e)
        {
            if (_richTextBox != null)
            {
                var mousePosition = e.GetPosition(_richTextBox);

                // Lấy TextPointer tại vị trí chuột
                TextPointer textPointer = _richTextBox.GetPositionFromPoint(mousePosition, true);

                // Kiểm tra xem chỉ số ký tự có khớp với vị trí ảnh trong Dictionary iconLinks không
                if (textPointer != null)
                {
                    // Tìm vị trí chỉ số ký tự từ TextPointer
                    int charIndex = textPointer.GetOffsetToPosition(_richTextBox.Document.ContentStart);

                    // Kiểm tra xem chỉ số ký tự có khớp với vị trí ảnh trong Dictionary iconLinks không
                    if (iconLinks.ContainsKey(charIndex) || (charIndex > 0 && iconLinks.ContainsKey(charIndex - 1)))
                    { 
                        // Chọn ký tự tại vị trí chuột
                        _richTextBox.Selection.Select(textPointer, textPointer.GetPositionAtOffset(1)); // Chọn ký tự tại vị trí chuột
                        CheckAndOpenApplication(charIndex);
                    }
                }
            }
        }

        private void CheckAndOpenApplication(int charIndex)
        {
            // Lấy TextPointer tại vị trí nhấp chuột
            TextPointer pointer = _richTextBox.Document.ContentStart.GetPositionAtOffset(charIndex);

            if (pointer != null)
            {
                // Kiểm tra xem phần tử hiện tại có phải là InlineUIContainer không
                var element = pointer.GetAdjacentElement(LogicalDirection.Forward);

                // Nếu phần tử này là InlineUIContainer và chứa Image
                if (element is InlineUIContainer inlineUIContainer && inlineUIContainer.Child is System.Windows.Controls.Image image)
                {
                    // Kiểm tra xem Image có phải là BitmapImage không
                    if (image.Source is BitmapImage bitmapImage)
                    {
                        // Lấy iconType từ iconLinks dựa trên charIndex
                        string iconType = iconLinks.ContainsKey(charIndex) ? iconLinks[charIndex] : null;

                        // Nếu có iconType, mở ứng dụng tương ứng
                        if (iconType != null)
                        {
                            OpenApplicationFromIconType(iconType);
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

            Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                UseShellExecute = true
            });
        }
    }
}
