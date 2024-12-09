using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;

namespace Wordpad
{
    internal class Ruler
    {
        private Canvas marginCanvas;
        private Canvas tickCanvas;
        private Canvas thumbCanvas;
        private Canvas rulerCanvas;
        private Thumb firstLineIndentThumb;
        private Thumb hangingIndentThumb;
        private Thumb paragraphIndentThumb;

        public double rulerLength;
        private double leftMargin;
        private double rightMargin;

        private RichTextBox richTextBox;
        private DockPanel dockPanel;

        private string currentUnit = "Inches"; // Đơn vị đo hiện tại
        private readonly Dictionary<string, double> unitConversion;

        //Thành phần để quan lý từng phần tử trong ruler
        private List<UIElement> rulerElements = new List<UIElement>(); // Các thành phần thuộc ruler
        private List<UIElement> marginElements = new List<UIElement>(); // Các thành phần thuộc margins


        public Ruler(Canvas margin, Canvas tick, Canvas thumb,Canvas ruler, RichTextBox richTextBox, DockPanel dockPanel)
        {
            marginCanvas = margin;
            tickCanvas = tick;
            thumbCanvas = thumb;
            rulerCanvas = ruler;
            this.richTextBox = richTextBox;
            this.dockPanel = dockPanel;

            rulerLength = dockPanel.Width;
            unitConversion = new Dictionary<string, double>
            {
            { "Inches", 96 },
            { "Centimeters", 37.7952755906 },
            { "Points", 1.333333333 },
            { "Picas", 16 }
            };

            dockPanel.SizeChanged += (s, e) =>
            {
                rulerLength = dockPanel.Width;

                //DrawMargins(richTextBox.Padding.Left, richTextBox.Padding.Right);
                //DrawRuler(); // Vẽ lại ruler khi kích thước thay đổi
                //

            };

            //MessageBox.Show("Canvas Children Count: " + rulerCanvas.Children.Count);
            //MessageBox.Show($"Ruler Length: {rulerLength}, Canvas Width: {rulerCanvas.Width}");
        }


        // Cập nhật đơn vị đo và vẽ lại ruler
        public void UpdateUnit(string unit)
        {
            if (!unitConversion.ContainsKey(unit))
                throw new ArgumentException("Unsupported unit");

            currentUnit = unit;
            DrawRuler();
        }

        // Vẽ ruler dựa trên đơn vị đo
        public void DrawRuler()
        {
            // Xóa các thành phần cũ trong tickCanvas
            //MessageBox.Show($"Dock width: {dockPanel.Width}\nTick canvas length: {tickCanvas.Width}\nRuler Lenght: {rulerLength}");
            tickCanvas.Children.Clear();

            double pixelsPerUnit = unitConversion[currentUnit];
            double totalUnits = tickCanvas.ActualWidth / pixelsPerUnit;

            double canvasCenterY = tickCanvas.Height / 2;

            // Vẽ các nhãn số và các vạch phụ
            for (int i = 0; i <= (totalUnits + 1) && i * pixelsPerUnit < rulerLength; i++)
            {
                double xPosition = i * pixelsPerUnit;
                //Tại vạch -đầu tiên của ruler thì k in số và vạch chính.
                if(i != 0)
                {
                   // Vạch chính
                    Line mainTick = new Line
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        X1 = xPosition,
                        X2 = xPosition,
                        Y1 = 0,
                        Y2 = 4
                    };

                    Line mainTick2 = new Line
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        X1 = xPosition,
                        X2 = xPosition,
                        Y1 = 16,
                        Y2 = 20
                    };
                    tickCanvas.Children.Add(mainTick);
                    tickCanvas.Children.Add(mainTick2);

                    // Nhãn chính (Số thay thế vạch chính)
                    TextBlock label = new TextBlock
                    {
                        Text = (i - 1).ToString(), // Nhãn chính
                        FontSize = 12,
                        Foreground = Brushes.Black
                    };

                    Canvas.SetLeft(label, xPosition - 3);
                    Canvas.SetTop(label, canvasCenterY - 9);
                    tickCanvas.Children.Add(label);
                }
 

                // Vạch phụ (Sub Ticks) giữa các đơn vị chính
                int subDivisions = GetSubDivisions(currentUnit); // Số lượng vạch phụ giữa các nhãn số
                for (int j = 1; j < subDivisions && xPosition + j * (pixelsPerUnit / subDivisions) < rulerLength; j++)
                {
                    double subXPosition = xPosition + j * (pixelsPerUnit / subDivisions);

                    Line subTick = new Line
                    {
                        Stroke = Brushes.Gray,
                        StrokeThickness = 1,
                        X1 = subXPosition,
                        X2 = subXPosition,
                        Y1 = canvasCenterY - 5, // Vạch phụ ngắn hơn, căn giữa
                        Y2 = canvasCenterY + 5
                    };
                    tickCanvas.Children.Add(subTick);
                }
            }
        }



        // Lấy số vạch phụ giữa các đơn vị chính, tùy theo đơn vị đo
        private int GetSubDivisions(string unit)
        {
            int result;

            switch (unit)
            {
                case "Inches":
                    result = 8; // 8 vạch phụ cho 1 inch
                    break;
                case "Centimeters":
                    result = 10; // 10 vạch phụ cho 1 cm
                    break;
                case "Points":
                    result = 4; // 4 vạch phụ cho 1 point
                    break;
                case "Picas":
                    result = 6; // 6 vạch phụ cho 1 pica
                    break;
                default:
                    result = 4; // Mặc định
                    break;
            }

            return result;
        }

        public void InitializeThumbs()
        {
            // Tạo các Thumb và thêm chúng vào thumbCanvas
            firstLineIndentThumb = CreateThumb();
            Canvas.SetLeft(firstLineIndentThumb, leftMargin + 30);
            thumbCanvas.Children.Add(firstLineIndentThumb);

            hangingIndentThumb = CreateThumb();
            Canvas.SetLeft(hangingIndentThumb, leftMargin + 60);
            thumbCanvas.Children.Add(hangingIndentThumb);

            paragraphIndentThumb = CreateThumb();
            Canvas.SetLeft(paragraphIndentThumb, leftMargin + 90);
            thumbCanvas.Children.Add(paragraphIndentThumb);

            firstLineIndentThumb.DragDelta += FirstLineIndentThumb_DragDelta;
            hangingIndentThumb.DragDelta += HangingIndentThumb_DragDelta;
            paragraphIndentThumb.DragDelta += ParagraphIndentThumb_DragDelta;
        }

        private Thumb CreateThumb()
        {
            return new Thumb
            {
                Width = 10,
                Height = 20,
                Background = Brushes.Gray
            };
        }

        public void DrawMargins(double leftMarginPx, double rightMarginPx)
        {
            //MessageBox.Show($"Dock width: {dockPanel.Width}\nMargin canvas length: {marginCanvas.Width}\nRuler Lenght: {rulerLength}");
            // Xóa các thành phần cũ trong marginCanvas
            marginCanvas.Children.Clear();

            // Vẽ vùng margin trái
            Rectangle leftMargin = new Rectangle
            {
                Width = leftMarginPx,
                Height = marginCanvas.Height,
                Fill = Brushes.Yellow
            };
            Canvas.SetLeft(leftMargin, 0);
            marginCanvas.Children.Add(leftMargin);

            // Vẽ vùng margin phải
            Rectangle rightMargin = new Rectangle
            {
                Width = rightMarginPx,
                Height = marginCanvas.Height,
                Fill = Brushes.Green
            };
            Canvas.SetRight(rightMargin, 0);
            marginCanvas.Children.Add(rightMargin);
        }


        private void FirstLineIndentThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newIndent = Canvas.GetLeft(firstLineIndentThumb) + e.HorizontalChange;

            if (newIndent >= leftMargin && newIndent <= rulerLength - rightMargin)
            {
                Canvas.SetLeft(firstLineIndentThumb, newIndent);
                ApplyIndent();
            }
        }

        private void HangingIndentThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newIndent = Canvas.GetLeft(hangingIndentThumb) + e.HorizontalChange;

            if (newIndent >= leftMargin && newIndent <= rulerLength - rightMargin)
            {
                Canvas.SetLeft(hangingIndentThumb, newIndent);
                ApplyIndent();
            }
        }

        private void ParagraphIndentThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newIndent = Canvas.GetLeft(paragraphIndentThumb) + e.HorizontalChange;

            if (newIndent >= leftMargin && newIndent <= rulerLength - rightMargin)
            {
                Canvas.SetLeft(paragraphIndentThumb, newIndent);
                ApplyIndent();
            }
        }

        private void ApplyIndent()
        {
            // Kiểm tra đơn vị hiện tại và lấy hệ số chuyển đổi
            if (!unitConversion.TryGetValue(currentUnit, out double pixelsPerUnit))
                throw new InvalidOperationException("Unsupported unit or unitConversion not initialized.");

            // Lấy vùng văn bản được chọn
            var selection = richTextBox.Selection;

            if (selection == null || selection.IsEmpty)
            {
                // Nếu không có vùng chọn, áp dụng indent cho đoạn văn tại con trỏ
                var caretPosition = richTextBox.CaretPosition;
                var paragraph = caretPosition.Paragraph;

                if (paragraph != null)
                {
                    ApplyIndentToParagraph(paragraph, pixelsPerUnit);
                }
                else
                {
                    MessageBox.Show("No paragraph available to apply indent.");
                }
            }
            else
            {
                // Nếu có vùng chọn, duyệt qua các đoạn văn trong tài liệu và kiểm tra vùng chọn
                TextPointer selectionStart = selection.Start;
                TextPointer selectionEnd = selection.End;

                // Lấy tài liệu từ RichTextBox
                var document = richTextBox.Document;

                // Duyệt qua các khối văn bản (Blocks) trong tài liệu
                foreach (var block in document.Blocks)
                {
                    if (block is Paragraph paragraph)
                    {
                        // Kiểm tra nếu đoạn văn nằm trong vùng chọn
                        var paragraphStart = paragraph.ContentStart;
                        var paragraphEnd = paragraph.ContentEnd;

                        if (paragraphStart.CompareTo(selectionEnd) <= 0 && paragraphEnd.CompareTo(selectionStart) >= 0)
                        {
                            ApplyIndentToParagraph(paragraph, pixelsPerUnit);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Hàm áp dụng indent cho một đoạn văn
        /// </summary>
        private void ApplyIndentToParagraph(Paragraph paragraph, double pixelsPerUnit)
        {
            double firstLineIndent = (Canvas.GetLeft(firstLineIndentThumb) - leftMargin) / pixelsPerUnit;
            double hangingIndent = (Canvas.GetLeft(hangingIndentThumb) - Canvas.GetLeft(firstLineIndentThumb)) / pixelsPerUnit;
            double paragraphIndent = (Canvas.GetLeft(paragraphIndentThumb) - leftMargin) / pixelsPerUnit;

            // Cập nhật các giá trị indent
            paragraph.TextIndent = firstLineIndent; // Indent dòng đầu tiên
            paragraph.Margin = new Thickness(paragraphIndent, 0, 0, 0);

            // Áp dụng Hanging Indent nếu cần
            if (hangingIndent > 0)
            {
                paragraph.TextIndent = firstLineIndent;
                paragraph.Margin = new Thickness(hangingIndent, 0, 0, 0);
            }
        }


        public void IsVisible(bool visible)
        {
            rulerCanvas.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public void ScaleRuler(double zoomScale)
        {
            // Scale toàn bộ rulerCanvas
            rulerCanvas.RenderTransform = new ScaleTransform(zoomScale, 1);

            // Điều chỉnh kích thước rulerCanvas theo dockPanel nếu cần
            rulerCanvas.Width = dockPanel.Width / zoomScale;
            //MessageBox.Show($"Dock width: {dockPanel.Width}\nRuler canvas length: {rulerCanvas.Width}\nRuler Lenght: {rulerLength}");
            //// Điều chỉnh vị trí các thành phần Thumb
            //if (firstLineIndentThumb != null)
            //{
            //    Canvas.SetLeft(firstLineIndentThumb, leftMargin * zoomScale + 30 * zoomScale);
            //}
            //if (hangingIndentThumb != null)
            //{
            //    Canvas.SetLeft(hangingIndentThumb, leftMargin * zoomScale + 60 * zoomScale);
            //}
            //if (paragraphIndentThumb != null)
            //{
            //    Canvas.SetLeft(paragraphIndentThumb, leftMargin * zoomScale + 90 * zoomScale);
            //}
        }
        public void UpdateCanvasSize()
        {
            marginCanvas.Width = rulerLength;
            tickCanvas.Width = rulerLength;
            thumbCanvas.Width = rulerLength;
        }
    }
}
