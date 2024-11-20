using System;
using con = System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Drawing;
using System.Collections.Generic;

namespace WordPad
{
    internal class PrintManager
    {
        private PrintDocument printDocument;
        private PageSetupDialog pageSetupDialog;
        private PrintPreviewDialog printPreviewDialog;
        private bool printPageNumber = false;
        private int currentPageNumber = 1;
        private con.DockPanel dockPanel;
        private con.RichTextBox richTextBox;

        public PrintManager(con.DockPanel dockPanel, con.RichTextBox richTextBox)
        {
            this.dockPanel = dockPanel;
            this.richTextBox = richTextBox;

            // Initialize PrintDocument and dialogs
            printDocument = new PrintDocument();
            pageSetupDialog = new PageSetupDialog { Document = printDocument };
            printPreviewDialog = new PrintPreviewDialog { Document = printDocument };

            // Attach event handlers
            printDocument.PrintPage += PrintDocument_PrintPage;
        }

        // Hiển thị Page Setup
        public void ShowPageSetupDialog()
        {
            if (pageSetupDialog.ShowDialog() == DialogResult.OK)
            {
                AdjustDockPanelToPageSetup();
            }
        }

        public void AdjustDockPanelToPageSetup()
        {
            var pageSettings = printDocument.DefaultPageSettings;
            richTextBox.Padding = new Thickness(
                pageSettings.Margins.Left,
                pageSettings.Margins.Top,
                pageSettings.Margins.Right,
                pageSettings.Margins.Bottom
            );

            if (pageSettings.Landscape)
            {
                dockPanel.Width = pageSettings.PaperSize.Height;
                dockPanel.Height = pageSettings.PaperSize.Width;
            }
            else
            {
                dockPanel.Width = pageSettings.PaperSize.Width;
                dockPanel.Height = pageSettings.PaperSize.Height;
            }
        }
        //Lấy line spacing của từng đoạn văn (paragraph) dựa vào line height và font size của từng đoạn
        private List<(string Text, System.Drawing.Font Font, double LineSpacing)> GetRichTextBoxParagraphsWithStyles()
        {
            var paragraphs = new List<(string Text, System.Drawing.Font Font, double LineSpacing)>();

            foreach (var block in richTextBox.Document.Blocks)
            {
                if (block is Paragraph paragraph)
                {
                    // Lấy văn bản
                    var range = new TextRange(paragraph.ContentStart, paragraph.ContentEnd);
                    string text = range.Text;

                    // Lấy thông tin Font
                    string fontFamily = paragraph.FontFamily?.Source ?? "Arial";
                    double fontSize = paragraph.FontSize > 0 ? paragraph.FontSize : 12;
                    bool isBold = paragraph.FontWeight == FontWeights.Bold;
                    bool isItalic = paragraph.FontStyle == FontStyles.Italic;

                    // Tạo đối tượng Font
                    var fontStyle = System.Drawing.FontStyle.Regular;
                    if (isBold) fontStyle |= System.Drawing.FontStyle.Bold;
                    if (isItalic) fontStyle |= System.Drawing.FontStyle.Italic;

                    var font = new System.Drawing.Font(fontFamily, (float)fontSize, fontStyle);

                    // Lấy LineSpacing
                    double lineSpacing = paragraph.LineHeight > 1
                        ? paragraph.LineHeight + fontSize
                        : fontSize * 1.2; // Giá trị mặc định nếu không có LineHeight

                    paragraphs.Add((text, font, lineSpacing));
                }
            }

            return paragraphs;
        }
        // Xử lý sự kiện PrintPage
        public void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            var pageBounds = e.MarginBounds;
            var graphics = e.Graphics;

            // Lấy thông tin từng Paragraph
            var paragraphs = GetRichTextBoxParagraphsWithStyles();

            float x = pageBounds.Left;
            float y = pageBounds.Top;

            foreach (var (text, font, lineSpacing) in paragraphs)
            {
                // Chia đoạn văn bản thành các dòng
                string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                foreach (var line in lines)
                {

                    // Vẽ dòng lên trang
                    graphics.DrawString(line, font, System.Drawing.Brushes.Black, x, y);
                    y += (float)lineSpacing * 1.75f; // Dịch y xuống theo khoảng cách dòng
                }

                //// Thêm khoảng cách giữa các Paragraph (nếu cần)
                //y += (float)lineSpacing / 2;
            }

            e.HasMorePages = false;

            // In số trang nếu cần
            if (printPageNumber)
            {
                PrintPageNumber(e);
            }
        }
        public void PrintDoc()
        {
            currentPageNumber = 1; // Reset page number
            PrintDialog printDialog = new PrintDialog
            {
                Document = printDocument // Gán tài liệu cho hộp thoại in
            };

            // Hiển thị hộp thoại in, nếu người dùng xác nhận thì thực hiện in
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print(); // Thực hiện in tài liệu 
            }
        }

        private void PrintPageNumber(PrintPageEventArgs e)
        {
            var graphics = e.Graphics;
            using (var font = new System.Drawing.Font("Arial", 10))
            {
                string pageNumberText = $"Page {currentPageNumber}";
                float x = e.MarginBounds.Left + e.MarginBounds.Width / 2 - graphics.MeasureString(pageNumberText, font).Width / 2;
                float y = e.MarginBounds.Bottom + 10;

                graphics.DrawString(pageNumberText, font, System.Drawing.Brushes.Black, x, y);
            }

            currentPageNumber++;
        }

        // Hiển thị Print Preview
        public void ShowPrintPreview()
        {
            currentPageNumber = 1;
            printPreviewDialog.ShowDialog();
        }

        // In nhanh
        public void QuickPrint()
        {
            currentPageNumber = 1;
            try
            {
                printDocument.Print();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error during Quick Print: {ex.Message}");
            }
        }

        // In với lề bằng PrintDialog
        /*public void PrintWithMargins()
        {
            con.PrintDialog printDialog = new con.PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                // Sao chép nội dung từ RichTextBox
                FlowDocument printDocument = new FlowDocument();
                TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                printDocument.Blocks.Add(new Paragraph(new Run(range.Text)));

                // Thiết lập lề
                printDocument.PagePadding = new Thickness(
                    dockPanel.Margin.Left,
                    dockPanel.Margin.Top,
                    dockPanel.Margin.Right,
                    dockPanel.Margin.Bottom
                );

                // Thiết lập khổ giấy
                printDocument.ColumnWidth = printDialog.PrintableAreaWidth;

                // In tài liệu
                IDocumentPaginatorSource paginator = printDocument;
                printDialog.PrintDocument(paginator.DocumentPaginator, "RichTextBox Print");
            }
        }*/
    }
}
