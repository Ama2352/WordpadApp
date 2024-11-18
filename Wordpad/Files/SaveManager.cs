using System.IO;
using System.Windows.Controls; // RichTextBox của WPF
using Microsoft.Win32; // SaveFileDialog của WPF
using System.Windows.Documents; // TextRange để xử lý RichTextBox
using DocumentFormat.OpenXml.Packaging; // Thư viện OpenXML
using DocumentFormat.OpenXml.Wordprocessing; // OpenXML cho .docx

namespace Wordpad
{
    internal class SaveManager
    {
        private RichTextBox richTextBox1;
        public static string CurrentFilePath { get; set; } = string.Empty;

        public SaveManager(RichTextBox RTX)
        {
            richTextBox1 = RTX;
        }

        public void SaveFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            switch (extension)
            {
                case ".rtf":
                    SaveAsRtf(filePath);
                    break;
                case ".txt":
                    SaveAsText(filePath);
                    break;
                case ".xaml":
                    SaveAsXaml(filePath);
                    break;
                case ".html":
                    SaveAsHtml(filePath);
                    break;
                case ".docx":
                    SaveAsDocx(filePath);
                    break;
                default:
                    System.Windows.MessageBox.Show("Định dạng không được hỗ trợ!", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    break;
            }
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(CurrentFilePath))
            {
                SaveFile(CurrentFilePath);
            }
            else
            {
                SaveAs();
            }
        }

        public void SaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|XAML Files (*.xaml)|*.xaml|HTML Files (*.html)|*.html|Word Document (*.docx)|*.docx",
                Title = "Save As"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                CurrentFilePath = saveFileDialog.FileName;
                SaveFile(CurrentFilePath);
            }
        }

        private void SaveAsRtf(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                TextRange textRange = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd);
                textRange.Save(stream, System.Windows.DataFormats.Rtf);
            }
        }

        private void SaveAsText(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                TextRange textRange = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd);
                textRange.Save(stream, System.Windows.DataFormats.Text);
            }
        }

        private void SaveAsXaml(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                TextRange textRange = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd);
                textRange.Save(stream, System.Windows.DataFormats.Xaml);
            }
        }

        private void SaveAsHtml(string filePath)
        {
            string htmlContent = $"<html><body><p>{System.Net.WebUtility.HtmlEncode(new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd).Text).Replace("\n", "<br>")}</p></body></html>";
            File.WriteAllText(filePath, htmlContent);
        }

        private void SaveAsDocx(string filePath)
        {
            using (DocumentFormat.OpenXml.Packaging.WordprocessingDocument wordDocument =
                DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Create(filePath,
                DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            {
                var mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document(
                    new DocumentFormat.OpenXml.Wordprocessing.Body(
                        new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                            new DocumentFormat.OpenXml.Wordprocessing.Run(
                                new DocumentFormat.OpenXml.Wordprocessing.Text(
                                    new System.Windows.Documents.TextRange(
                                        richTextBox1.Document.ContentStart,
                                        richTextBox1.Document.ContentEnd).Text)
                            )
                        )
                    )
                );
            }
        }

    }
}
