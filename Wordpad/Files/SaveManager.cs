using DocumentFormat.OpenXml.Wordprocessing; // OpenXML cho .docx
using Microsoft.Win32; // SaveFileDialog của WPF
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls; // RichTextBox của WPF
using System.Windows.Documents; // TextRange để xử lý RichTextBox

namespace Wordpad
{
    public class SaveManager
    {
        private RichTextBox richTextBox1;
        public static string CurrentFilePath { get; set; } = string.Empty;
        public static FileStream fileStream;

        public SaveManager(RichTextBox RTX)
        {
            richTextBox1 = RTX;
        }

        public void SaveFile()
        {
            string extension = Path.GetExtension(CurrentFilePath).ToLower();

            switch (extension)
            {
                case ".rtf":
                    SaveAsRtf();
                    break;
                case ".txt":
                    SaveAsText();
                    break;
                case ".xaml":
                    SaveAsXaml();
                    break;
                case ".html":
                    SaveAsHtml();
                    break;
                case ".docx":
                    SaveAsDocx();
                    break;
                default:
                    System.Windows.MessageBox.Show("Unsupported format!", "Error!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    break;
            }
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(CurrentFilePath))
            {
                SaveFile();
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
                SaveFile();
            }
        }

        /*  private void SaveAsRtf(string filePath)
          {
              using (FileStream stream = new FileStream(filePath, FileMode.Create))
              {
                  TextRange textRange = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd);
                  textRange.Save(stream, System.Windows.DataFormats.Rtf);
              }
          }*/
        private void SaveAsRtf()
        {
            try
            {
                // Dùng khối using để tự động giải phóng FileStream
                using (FileStream fileStream = new FileStream(CurrentFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    TextRange textRange = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd);
                    textRange.Save(fileStream, System.Windows.DataFormats.Rtf);
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show($"The file is being used by another process: {ex.Message}", "File Access Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void SaveAsText()
        {
            try
            {
                // Dùng khối using để tự động giải phóng FileStream
                using (FileStream fileStream = new FileStream(CurrentFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    TextRange textRange = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd);
                    textRange.Save(fileStream, System.Windows.DataFormats.Text);
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show($"The file is being used by another process: {ex.Message}", "File Access Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveAsXaml()
        {
            try
            {
                // Dùng khối using để tự động giải phóng FileStream
                using (FileStream fileStream = new FileStream(CurrentFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    TextRange textRange = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd);
                    textRange.Save(fileStream, System.Windows.DataFormats.Xaml);
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show($"The file is being used by another process: {ex.Message}", "File Access Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveAsHtml()
        {
            string htmlContent = $"<html><body><p>{System.Net.WebUtility.HtmlEncode(new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd).Text).Replace("\n", "<br>")}</p></body></html>";
            File.WriteAllText(CurrentFilePath, htmlContent);
        }

        private void SaveAsDocx()
        {
            // Tạo tệp .docx
            using (DocumentFormat.OpenXml.Packaging.WordprocessingDocument wordDocument =
                   DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Create(CurrentFilePath,
                   DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            {
                // Thêm phần chính (Main Document Part)
                var mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document(new Body());

                // Lấy nội dung từ RichTextBox
                string fullText = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd).Text;

                // Chia nội dung thành các dòng bằng cách tách theo ký tự xuống dòng
                string[] lines = fullText.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

                // Tạo các đoạn văn (Paragraph) cho mỗi dòng
                foreach (string line in lines)
                {
                    // Sử dụng Paragraph từ namespace DocumentFormat.OpenXml.Wordprocessing
                    DocumentFormat.OpenXml.Wordprocessing.Paragraph paragraph =
                        new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                            new DocumentFormat.OpenXml.Wordprocessing.Run(
                                new DocumentFormat.OpenXml.Wordprocessing.Text(line)));

                    // Thêm đoạn văn vào nội dung chính
                    mainPart.Document.Body.Append(paragraph);
                }

                // Lưu tài liệu
                mainPart.Document.Save();
            }
        }

    }
}
