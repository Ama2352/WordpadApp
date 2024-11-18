using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;

namespace Wordpad
{
    internal class PrintManager
    {
        // Hàm in với cách lề
        RichTextBox richTextBox;
        public PrintManager(RichTextBox richTextBox)
        {
            this.richTextBox = richTextBox;
        }

        public void PrintWithMargins()
        {
            // Tạo PrintDialog
            PrintDialog printDialog = new PrintDialog();

            // Nếu người dùng chọn máy in
            if (printDialog.ShowDialog() == true)
            {
                // Tạo FlowDocument để in
                FlowDocument printDocument = new FlowDocument();

                // Sao chép nội dung từ RichTextBox
                TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                printDocument.Blocks.Add(new Paragraph(new Run(range.Text)));

                // Thiết lập lề trang in
                printDocument.PagePadding = new Thickness(50, 30, 50, 30); // Cách lề như Border

                // Căn chỉnh nội dung
                printDocument.TextAlignment = TextAlignment.Left;

                // Thiết lập khổ giấy
                printDocument.ColumnWidth = printDialog.PrintableAreaWidth;

                // In tài liệu
                IDocumentPaginatorSource paginator = printDocument;
                printDialog.PrintDocument(paginator.DocumentPaginator, "RichTextBox Print");
            }
        }
    }
}
