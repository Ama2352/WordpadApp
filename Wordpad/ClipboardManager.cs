using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Wordpad
{
    public class ClipboardManager
    {
        private RichTextBox _richTextBox;
        public static string imageDirectory { get; private set; }
        public string linkText { get; private set; }
        public string typeOfLink { get; private set; }

        // Khởi tạo với tham chiếu tới RichTextBox
        public ClipboardManager(RichTextBox richTextBox)
        {
            _richTextBox = richTextBox;
            imageDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
        }

        // Phương thức Cắt
        public void Cut()
        {
            TextRange textRange = _richTextBox.Selection;
            if (!textRange.IsEmpty)
            {
                // Copy RTF text
                MemoryStream stream = new MemoryStream();
                textRange.Save(stream, DataFormats.Rtf);
                string rtfText = Encoding.UTF8.GetString(stream.ToArray());

                // Set to Clipboard and remove text
                Clipboard.SetData(DataFormats.Rtf, rtfText);
                textRange.Text = string.Empty;  // Delete selected text
            }
            else
            {
                MessageBox.Show("No text selected to cut.");
            }
        }


        // Phương thức Sao chép
        public void Copy()
        {
            TextRange textRange = _richTextBox.Selection;
            if (!textRange.IsEmpty)
            {
                // Save RTF format
                MemoryStream stream = new MemoryStream();
                textRange.Save(stream, DataFormats.Rtf);
                string rtfText = Encoding.UTF8.GetString(stream.ToArray());

                // Set to Clipboard
                Clipboard.SetData(DataFormats.Rtf, rtfText);
            }
            else
            {
                MessageBox.Show("No text selected to copy.");
            }
        }


        // Phương thức Dán
        public void Paste()
        {
            if (Clipboard.ContainsData(DataFormats.Rtf))
            {
                string rtfText = (string)Clipboard.GetData(DataFormats.Rtf);
                TextRange textRange = _richTextBox.Selection;
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(rtfText)))
                {
                    textRange.Load(stream, DataFormats.Rtf);
                }
            }
            else if (Clipboard.ContainsText())
            {
                // Fallback to plain text if RTF is unavailable
                TextRange textRange = _richTextBox.Selection;
                textRange.Text = Clipboard.GetText();
            }
            else
            {
                MessageBox.Show("Clipboard does not contain valid text.");
            }
        }


        // Phương thức lấy các tùy chọn dán từ clipboard
        public List<string> GetPasteOptions()
        {
            List<string> options = new List<string>();

            if (Clipboard.ContainsData(DataFormats.Rtf))
            {
                options.Add("Rich Text (RTF)");
            }

            if (Clipboard.ContainsText())
            {
                options.Add("Unformatted Text");
            }

            if (Clipboard.ContainsImage())
            {
                options.Add("Bitmap");
            }

            return options;
        }

        // Lấy mô tả và hình ảnh minh họa cho tùy chọn được chọn
        public (string Description, BitmapImage Illustration, BitmapImage DisplayIcon) GetOptionDetails(string option)
        {
            string imagePath = string.Empty;
            string iconPath = string.Empty;
            switch (option)
            {
                case "Rich Text (RTF)":
                    imagePath = Path.Combine(imageDirectory, "paste.png");
                    return ("Inserts the contents of the clipboard into your document as text with font and paragraph formatting.",
                            LoadImage(imagePath), null);

                case "Unformatted Text":
                    imagePath = Path.Combine(imageDirectory, "paste.png");
                    return ("Inserts the contents of the clipboard into your document as text without any formatting.",
                            LoadImage(imagePath), null);

                case "Bitmap":
                    imagePath = Path.Combine(imageDirectory, "paste.png");
                    return ("Inserts the contents of the clipboard into your document as a bitmap.",
                            LoadImage(imagePath), null);

                case "Wordpad Document":
                    imagePath = Path.Combine(imageDirectory, "paste.png");
                    iconPath = Path.Combine(imageDirectory, "word.png");
                    return ("Inserts the contents of the clipboard into your document so that you may activate it using WordPad.",
                            LoadImage(imagePath), LoadImage(iconPath));

                case "Foxit PhantomPDF Document":
                    imagePath = Path.Combine(imageDirectory, "paste.png");
                    iconPath = Path.Combine(imageDirectory, "pdf.png");
                    return ("Inserts a new Foxit PhantomPDF Document object into your document. It will be displayed as an icon.",
                            LoadImage(imagePath), LoadImage(iconPath));

                case "Microsoft Word Document":
                    imagePath = Path.Combine(imageDirectory, "paste.png");
                    iconPath = Path.Combine(imageDirectory, "word.png");
                    return ("Inserts a new Microsoft Word Document object into your document.",
                            LoadImage(imagePath), LoadImage(iconPath));

                case "Microsoft Excel":
                    imagePath = Path.Combine(imageDirectory, "paste.png");
                    iconPath = Path.Combine(imageDirectory, "excel.png");
                    return ("Inserts a new Microsoft Excel object into your document.",
                            LoadImage(imagePath), LoadImage(iconPath));

                case "Microsoft PowerPoint Presentation":
                    imagePath = Path.Combine(imageDirectory, "paste.png");
                    iconPath = Path.Combine(imageDirectory, "powerpoint.png");
                    return ("Inserts a new Microsoft PowerPoint Presentation object into your document.",
                            LoadImage(imagePath), LoadImage(iconPath));

                case "Paint":
                    imagePath = Path.Combine(imageDirectory, "paste.png");
                    iconPath = Path.Combine(imageDirectory, "paint.png");
                    return ("Inserts a new Paint object into your document.",
                        LoadImage(imagePath), LoadImage(iconPath));

                default:
                    return ("No description for this option.", null, null);
            }
        }

        // Phương thức tải ảnh từ đường dẫn, kiểm tra nếu ảnh tồn tại
        public BitmapImage LoadImage(string path)
        {
            if (!File.Exists(path))
                return null;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path, UriKind.Absolute);
            bitmap.EndInit();
            return bitmap;
        }

        public void OpenDocumentIfLinkClicked(MouseButtonEventArgs e)
        {
            if (_richTextBox != null && linkText != null)
            {
                TextPointer pointer = _richTextBox.Document.ContentStart;
                TextPointer clickedPosition = _richTextBox.GetPositionFromPoint(e.GetPosition(_richTextBox), true);

                if (clickedPosition != null)
                {
                    int offset = pointer.GetOffsetToPosition(clickedPosition);
                    if (offset >= 0 && offset < linkText.Length)
                    {
                        try
                        {
                            string fileName;
                            switch (typeOfLink)
                            {
                                case "Wordpad Document":
                                    fileName = "wordpad";
                                    break;
                                case "Foxit PhantomPDF Document":
                                    fileName = "FoxitPhantomPDF";
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
                                default:
                                    throw new NotSupportedException("Unsupported document type.");
                            }

                            Process.Start(new ProcessStartInfo
                            {
                                FileName = fileName,
                                UseShellExecute = true
                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Cannot open the application: " + ex.Message);
                        }
                    }
                }
            }
        }

        public void PasteSpecial(string selectedOption)
        {
            TextRange textRange = _richTextBox.Selection;

            if ((selectedOption == "Rich Text (RTF)" || selectedOption == "Wordpad Document") && Clipboard.ContainsData(DataFormats.Rtf))
            {
                    Paste();
            }
            else if ((selectedOption == "Unformatted Text" || selectedOption == "Wordpad Document") && Clipboard.ContainsText())
            {
                textRange.Text = Clipboard.GetText();
            }
            else if (selectedOption == "Bitmap" && Clipboard.ContainsImage())
            {
                InsertManager insertManager = new InsertManager(_richTextBox);
                BitmapSource bitmapSource = Clipboard.GetImage();
                BitmapImage bitmapImage = new BitmapImage();

                using (var memoryStream = new MemoryStream())
                {
                    // Mã hóa hình ảnh từ kiểu BitmapSource sang png và lưu vào MemoryStream (encode để lưu trữ hình ành ở dạng png)
                    BitmapEncoder encoder = new PngBitmapEncoder(); // Định dạng png
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource)); // Tạo và thêm khung hình vào bộ mã hóa (encoder)
                    encoder.Save(memoryStream); // Ghi vào bộ nhớ

                    /*Đặt lại con trỏ vị trí của MemoryStream về vị trí bắt đầu để đọc lại từ đầu dòng bộ nhớ (Vì khi ghi dữ liệu vào bộ nhớ thì con trỏ
                     đã di chuyển đến cuối dữ liệu vừa ghi)*/
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Bắt đầu khởi tạo
                    bitmapImage.BeginInit();

                    // Đặt tùy chọn bộ nhớ cache của bitmapImage để tải hình ảnh ngay lập tức vào bộ nhớ khi khởi tạo.
                    // Điều này đảm bảo hình ảnh sẽ được tải hoàn toàn từ memoryStream ngay khi bắt đầu khởi tạo, không cần phải tải lại sau đó.
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;

                    // Xác định nguồn dữ liệu cho bitmapImage là một memory stream chứa hình ảnh.
                    // Đây là nơi BitmapImage sẽ đọc dữ liệu hình ảnh từ (trong trường hợp này là từ bộ nhớ, không phải từ file hoặc URI).
                    bitmapImage.StreamSource = memoryStream;

                    // Hoàn thành khởi tạo 
                    bitmapImage.EndInit();
                    
                }

                insertManager.MakeContainerForImageAndInsert(bitmapImage);
            }    
            else
            {
                MessageBox.Show("No valid paste option selected.", "Paste Special", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
