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

namespace WordPad
{
    public class ClipboardManager
    {
        private RichTextBox _richTextBox;
        public static string ImageDirectory { get; private set; }
        public static bool IsCopiedFromApplication; // Thuộc tính theo dõi nguồn

        public string LinkText { get; private set; }
        public string TypeOfLink { get; private set; }

        // Khởi tạo với tham chiếu tới RichTextBox
        public ClipboardManager(RichTextBox richTextBox)
        {
            _richTextBox = richTextBox;
            ImageDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "C:/Users/Admin/Documents/GitHub/WordPad/WordPad/WordPad/Images");
            IsCopiedFromApplication = false; // Khởi tạo giá trị mặc định
        }

        // Phương thức Cắt
        public void Cut()
        {
            TextRange textRange = _richTextBox.Selection;
            if (!textRange.IsEmpty)
            {
                Clipboard.SetText(textRange.Text);
                textRange.Text = string.Empty; // Xóa nội dung được cắt
                IsCopiedFromApplication = true;
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
                Clipboard.SetText(textRange.Text);
                IsCopiedFromApplication = true;
            }
            else
            {
                MessageBox.Show("No text selected to copy.");
            }
        }

        // Phương thức Dán
        public void Paste()
        {
            if (Clipboard.ContainsText())
            {
                TextRange textRange = _richTextBox.Selection;
                textRange.Text = Clipboard.GetText();
                IsCopiedFromApplication = false;
            }
            else
            {
                MessageBox.Show("Clipboard is empty.");
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
                    imagePath = Path.Combine(ImageDirectory, "paste.png");
                    return ("Inserts the contents of the clipboard into your document as text with font and paragraph formatting.",
                            LoadImage(imagePath), null);

                case "Unformatted Text":
                    imagePath = Path.Combine(ImageDirectory, "paste.png");
                    return ("Inserts the contents of the clipboard into your document as text without any formatting.",
                            LoadImage(imagePath), null);

                case "Bitmap":
                    imagePath = Path.Combine(ImageDirectory, "paste.png");
                    return ("Inserts the contents of the clipboard into your document as a bitmap.",
                            LoadImage(imagePath), null);

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
            if (_richTextBox != null && LinkText != null)
            {
                TextPointer pointer = _richTextBox.Document.ContentStart;
                TextPointer clickedPosition = _richTextBox.GetPositionFromPoint(e.GetPosition(_richTextBox), true);

                if (clickedPosition != null)
                {
                    int offset = pointer.GetOffsetToPosition(clickedPosition);
                    if (offset >= 0 && offset < LinkText.Length)
                    {
                        try
                        {
                            string fileName;
                            switch (TypeOfLink)
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
                string rtf = Clipboard.GetData(DataFormats.Rtf) as string;
                if (!string.IsNullOrEmpty(rtf))
                {
                    MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(rtf));
                    textRange.Load(stream, DataFormats.Rtf);
                }
            }
            else if ((selectedOption == "Unformatted Text" || selectedOption == "Wordpad Document") && Clipboard.ContainsText())
            {
                textRange.Text = Clipboard.GetText();
            }
            else
            {
                MessageBox.Show("No valid paste option selected.", "Paste Special", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
