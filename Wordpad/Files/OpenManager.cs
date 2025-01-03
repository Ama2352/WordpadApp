using System.IO; // Làm việc với tệp
using Microsoft.Win32; // Sử dụng OpenFileDialog trong WPF
using System.Windows.Controls; // RichTextBox
using System.Windows.Documents; // TextRange
using System.Windows; // MessageBox
using DocumentFormat.OpenXml.Packaging; // Đọc file DOCX

namespace Wordpad
{
    internal class OpenManager
    {
        private RichTextBox richTextBox; // RichTextBox được tham chiếu
        public static bool isOpened = false;

        public OpenManager(RichTextBox rtx)
        {
            richTextBox = rtx;
        }

        /// <summary>
        /// Mở file và tải nội dung vào RichTextBox.
        /// </summary>
        public void Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Supported Files (*.txt;*.rtf;*.docx;*.odt)|*.txt;*.rtf;*.docx;*.odt|" +
                         "Text Files (*.txt)|*.txt|" +
                         "Rich Text Format (*.rtf)|*.rtf|" +
                         "Word Documents (*.docx)|*.docx|" +
                         "OpenDocument Text (*.odt)|*.odt",
                Title = "Open File"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                isOpened = true;

                string filePath = openFileDialog.FileName;

                try
                {
                    string fileExtension = Path.GetExtension(filePath).ToLower();

                    // Xử lý theo định dạng file
                    switch (fileExtension)
                    {
                        case ".rtf":
                            LoadRtfFile(filePath);
                            break;

                        case ".txt":
                            LoadTxtFile(filePath);
                            break;

                        case ".docx":
                            LoadDocxFile(filePath);
                            break;

                        case ".odt":
                            LoadOdtFile(filePath);
                            break;

                        default:
                            MessageBox.Show("File format not supported!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                    }

                    // Cập nhật đường dẫn file hiện tại cho SaveManager
                    SaveManager.CurrentFilePath = filePath;
                    isOpened = false;
                    MainWindow.IsTextChanged = true;
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Tải file RTF vào RichTextBox.
        /// </summary>
        private void LoadRtfFile(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                textRange.Load(fileStream, DataFormats.Rtf);
            }
        }

        /// <summary>
        /// Tải file TXT vào RichTextBox.
        /// </summary>
        private void LoadTxtFile(string filePath)
        {
            string text = File.ReadAllText(filePath);
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            textRange.Text = text;
        }

        /// <summary>
        /// Tải file DOCX vào RichTextBox.
        /// </summary>
        private void LoadDocxFile(string filePath)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                string text = wordDoc.MainDocumentPart.Document.Body.InnerText;
                TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                textRange.Text = text;
            }
        }

        /// <summary>
        /// Tải file ODT vào RichTextBox.
        /// </summary>
        private void LoadOdtFile(string filePath)
        {
            string odtContent = File.ReadAllText(filePath);
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            textRange.Text = odtContent;
        }
    }
}
