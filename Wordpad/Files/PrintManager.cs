using System;
using System.Drawing.Printing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using Wordpad.Files;
using con = System.Windows.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Wordpad
{
    internal class PrintManager
    {
        private PrintDocument printDocument;
        private PrintPreviewDialog printPreviewDialog;
        private PageSetupDialog pageSetupDialog;
        private FlowDocument flowDocument;
        private bool printPageNumber = false;
        private DockPanel dockPanel;
        private con.RichTextBox richTextBox;
        private InsertManager insertManager;
        private Ruler ruler;

        public PrintManager(DockPanel DP, con.RichTextBox richTextBox, Ruler ruler)
        {
            printDocument = new PrintDocument();
            printPreviewDialog = new PrintPreviewDialog { Document = printDocument };
            pageSetupDialog = new PageSetupDialog { Document = printDocument };
            insertManager = new InsertManager(richTextBox);

            this.dockPanel = DP;
            this.richTextBox = richTextBox;
            this.ruler = ruler;


        }

        public void PrintRichTextBoxContent()
        {
            // Hiển thị hộp thoại in của WPF
            System.Windows.Controls.PrintDialog printDialog = new System.Windows.Controls.PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                try
                {
                    // Chuẩn bị nội dung in (FlowDocument)
                    PreparePrint();
                    IDocumentPaginatorSource paginator = flowDocument;

                    // Gói DocumentPaginator với lớp PaginatorWithPageNumbers
                    var wrappedPaginator = new PaginatorWithPageNumbers(paginator.DocumentPaginator, new Typeface("Arial"), 12, System.Windows.Media.Brushes.Black, 96, printPageNumber);

                    // Thiết lập các tùy chọn in từ PrintDialog
                    wrappedPaginator.PageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);

                    // Thực hiện in tài liệu
                    printDialog.PrintDocument(wrappedPaginator, "Printing RichTextBox Content");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while printing: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PreparePrint()
        {
            // Chuyển nội dung từ RichTextBox sang FlowDocument
            flowDocument = CreateFlowDocumentFromRichTextBox(richTextBox);

            // Điều chỉnh FlowDocument theo PageSetupDialog
            var pageSettings = printDocument.DefaultPageSettings;
            flowDocument.PageHeight = pageSettings.PaperSize.Height;
            flowDocument.PageWidth = pageSettings.PaperSize.Width;
            flowDocument.PagePadding = new Thickness(
                pageSettings.Margins.Left,
                pageSettings.Margins.Top,
                pageSettings.Margins.Right,
                pageSettings.Margins.Bottom
            );
            flowDocument.ColumnWidth = flowDocument.PageWidth - flowDocument.PagePadding.Left - flowDocument.PagePadding.Right; // Không chia cột

        }

        private FlowDocument CreateFlowDocumentFromRichTextBox(con.RichTextBox richTextBox)
        {
            var flowDocument = new FlowDocument();

            // Chuyển nội dung từ RichTextBox sang FlowDocument
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                textRange.Save(memoryStream, System.Windows.DataFormats.XamlPackage);
                memoryStream.Seek(0, SeekOrigin.Begin);

                TextRange flowDocumentRange = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                flowDocumentRange.Load(memoryStream, System.Windows.DataFormats.XamlPackage);
            }

            // Đồng bộ font
            flowDocument.FontSize = richTextBox.FontSize;
            flowDocument.FontFamily = richTextBox.FontFamily;
            flowDocument.FontStretch = richTextBox.FontStretch;
            flowDocument.FontStyle = richTextBox.FontStyle;
            flowDocument.FontWeight = richTextBox.FontWeight;

            // Đồng bộ padding
            flowDocument.PagePadding = richTextBox.Padding;

            return flowDocument;
        }




        public void ShowPrintPreview()
        {
            // Chuẩn bị FlowDocument
            PreparePrint(); // Tạo FlowDocument từ nội dung RichTextBox

            // Tạo FixedDocument từ FlowDocument
            var paginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;
            FixedDocument fixedDoc = ConvertPaginatorToFixedDocument(paginator);

            // Tạo DocumentViewer để hiển thị Print Preview
            var window = new Window
            {
                Title = "Print Preview",
                Width = 800,
                Height = 600,
                Content = new DocumentViewer { Document = fixedDoc }
            };

            window.ShowDialog();
        }

        private FixedDocument ConvertPaginatorToFixedDocument(DocumentPaginator paginator)
        {
            FixedDocument fixedDoc = new FixedDocument();
            paginator.ComputePageCount(); // Tính tổng số trang

            for (int pageIndex = 0; pageIndex < paginator.PageCount; pageIndex++)
            {
                DocumentPage page = paginator.GetPage(pageIndex);
                FixedPage fixedPage = new FixedPage();
                fixedPage.Width = page.Size.Width;
                fixedPage.Height = page.Size.Height;

                // Sử dụng Canvas để chứa nội dung Visual của trang
                Canvas canvas = new Canvas();
                canvas.Width = page.Size.Width;
                canvas.Height = page.Size.Height;

                // Dùng VisualBrush để render nội dung của DocumentPage.Visual
                VisualBrush visualBrush = new VisualBrush(page.Visual);
                canvas.Background = visualBrush;

                // Nếu tùy chọn in số trang được chọn, thêm footer số trang
                if (printPageNumber)
                {
                    TextBlock pageNumberText = new TextBlock
                    {
                        Text = $"Page {pageIndex + 1}",
                        FontSize = 12,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new Thickness(0, 0, 0, 20), // Cách lề dưới 20px
                        Foreground = System.Windows.Media.Brushes.Black
                    };

                    FixedPage.SetLeft(pageNumberText, (page.Size.Width - 100) / 2); // Canh giữa
                    FixedPage.SetTop(pageNumberText, page.Size.Height - 30); // Vị trí cách dưới cùng 30px
                    fixedPage.Children.Add(pageNumberText);
                }

                // Thêm Canvas vào FixedPage
                fixedPage.Children.Add(canvas);

                // Thêm FixedPage vào FixedDocument
                PageContent pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(fixedPage);
                fixedDoc.Pages.Add(pageContent);
            }

            return fixedDoc;
        }

        // Hiển thị Page Setup
        public void ShowPageSetupDialog()
        {
            if (pageSetupDialog.ShowDialog() == DialogResult.OK)
            {
                // Mở hộp thoại bổ sung cho các thiết lập khác
                PrintPageNumberWindow printPageNumberWindow = new PrintPageNumberWindow();
                if (printPageNumberWindow.ShowDialog() == true) // WPF sử dụng true cho OK
                {
                    // Lưu cờ printPageNumber từ ExtraSettingsDialog
                    printPageNumber = printPageNumberWindow.PrintPageNumberSelected;
                }

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

            dockPanel.Width = pageSettings.Landscape
                ? pageSettings.PaperSize.Height
                : pageSettings.PaperSize.Width;

            // Cập nhật margin trên ruler
            Ruler.oriRulerWidth = dockPanel.Width;
            ViewManagment.DPWidthOri = dockPanel.Width;
            ruler.UpdateCanvasSize(dockPanel.Width);
            ruler.DrawMargins(pageSettings.Margins.Left, pageSettings.Margins.Right);
            ruler.DrawRuler();
        }

    }
}