using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Globalization;

namespace Wordpad.Files
{
    public class PaginatorWithPageNumbers : DocumentPaginator
    {
        private readonly DocumentPaginator _paginator;
        private readonly Typeface _typeface;
        private readonly double _fontSize;
        private readonly Brush _brush;
        private readonly double _pixelsPerDip;
        private readonly bool _printPageNumbers;

        public PaginatorWithPageNumbers(DocumentPaginator paginator, Typeface typeface, double fontSize, Brush brush, double pixelsPerDip, bool printPageNumbers)
        {
            _paginator = paginator;
            _typeface = typeface;
            _fontSize = fontSize;
            _brush = brush;
            _pixelsPerDip = pixelsPerDip;
            _printPageNumbers = printPageNumbers;
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            DocumentPage page = _paginator.GetPage(pageNumber);

            // Create a container to hold the original page content
            ContainerVisual container = new ContainerVisual();
            container.Children.Add(page.Visual);

            if(_printPageNumbers)
            {
                // Add page number to the container
                DrawingVisual pageNumberVisual = new DrawingVisual();
                using (DrawingContext dc = pageNumberVisual.RenderOpen())
                {
                    double pixelsPerDip = VisualTreeHelper.GetDpi(page.Visual).PixelsPerDip;
                    FormattedText formattedText = new FormattedText(
                        $"Page {pageNumber + 1}",
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        _typeface,
                        _fontSize,
                        _brush,
                        pixelsPerDip);

                    double x = page.ContentBox.Left + (page.ContentBox.Width - formattedText.Width) / 2;
                    double y = page.ContentBox.Bottom - formattedText.Height - 10; // Adjust the position as needed

                    dc.DrawText(formattedText, new Point(x, y));
                }

                container.Children.Add(pageNumberVisual);
            }
            // Return a new DocumentPage with the modified container
            return new DocumentPage(container, page.Size, page.BleedBox, page.ContentBox);
        }



        public override bool IsPageCountValid => _paginator.IsPageCountValid;

        public override int PageCount => _paginator.PageCount;

        public override Size PageSize
        {
            get => _paginator.PageSize;
            set => _paginator.PageSize = value;
        }

        public override IDocumentPaginatorSource Source => _paginator.Source;
    }

}
