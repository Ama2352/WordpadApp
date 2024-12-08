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
        private Canvas rulerCanvas;
        private Thumb firstLineIndentThumb;
        private Thumb hangingIndentThumb;
        private Thumb paragraphIndentThumb;

        private double rulerLength;
        private double leftMargin;
        private double rightMargin;

        private RichTextBox richTextBox;
        private DockPanel dockPanel;

        private string currentUnit = "Inches"; // Đơn vị đo hiện tại
        private readonly Dictionary<string, double> unitConversion;

        //Thành phần để quan lý từng phần tử trong ruler
        private List<UIElement> rulerElements = new List<UIElement>(); // Các thành phần thuộc ruler
        private List<UIElement> marginElements = new List<UIElement>(); // Các thành phần thuộc margins


        public Ruler(Canvas canvas, RichTextBox richTextBox, DockPanel dockPanel)
        {
            rulerCanvas = canvas;
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
                rulerCanvas.Width = dockPanel.Width;
                DrawMargins(richTextBox.Padding.Left, richTextBox.Padding.Right);
                DrawRuler(); // Vẽ lại ruler khi kích thước thay đổi
                //MessageBox.Show($"Dock width: {dockPanel.Width}\nRUler canvas length: {rulerCanvas.Width}\nRUler Lenght: {rulerLength}");

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
            // Xóa các phần tử thuộc ruler
            foreach (var element in rulerElements)
            {
                rulerCanvas.Children.Remove(element);
            }
            rulerElements.Clear();

            double pixelsPerUnit = unitConversion[currentUnit];
            double totalUnits = rulerCanvas.ActualWidth / pixelsPerUnit;

            double canvasCenterY = rulerCanvas.Height / 2;

            // Vẽ các nhãn số và các vạch phụ
            for (int i = 0; i <= totalUnits && i * pixelsPerUnit < rulerLength; i++)
            {
                double xPosition = i * pixelsPerUnit;

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
                rulerCanvas.Children.Add(mainTick);
                rulerCanvas.Children.Add(mainTick2);
                rulerElements.Add(mainTick);
                rulerElements.Add(mainTick2);

                // Nhãn chính (Số thay thế vạch chính)
                TextBlock label = new TextBlock
                {
                    Text = i.ToString(), // Nhãn chính
                    FontSize = 12,
                    Foreground = Brushes.Black
                };

                Canvas.SetLeft(label, xPosition - 3);
                Canvas.SetTop(label, canvasCenterY - 9);
                rulerCanvas.Children.Add(label);
                rulerElements.Add(label);

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
                    rulerCanvas.Children.Add(subTick);
                    rulerElements.Add(subTick);
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
            // First Line Indent Thumb
            firstLineIndentThumb = CreateThumb();
            Canvas.SetLeft(firstLineIndentThumb, leftMargin + 30); // Khoảng cách mặc định
            firstLineIndentThumb.DragDelta += FirstLineIndentThumb_DragDelta;

            // Hanging Indent Thumb
            hangingIndentThumb = CreateThumb();
            Canvas.SetLeft(hangingIndentThumb, leftMargin + 60);
            hangingIndentThumb.DragDelta += HangingIndentThumb_DragDelta;

            // Paragraph Indent Thumb
            paragraphIndentThumb = CreateThumb();
            Canvas.SetLeft(paragraphIndentThumb, leftMargin + 90);
            paragraphIndentThumb.DragDelta += ParagraphIndentThumb_DragDelta;

            // Thêm các Thumb vào rulerCanvas
            rulerCanvas.Children.Add(firstLineIndentThumb);
            rulerCanvas.Children.Add(hangingIndentThumb);
            rulerCanvas.Children.Add(paragraphIndentThumb);
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
            // Xóa các phần tử thuộc margins
            foreach (var element in marginElements)
            {
                rulerCanvas.Children.Remove(element);
            }
            marginElements.Clear();

            // Vẽ vùng margin trái
            Rectangle leftMargin = new Rectangle
            {
                Width = leftMarginPx,
                Height = rulerCanvas.Height,
                Fill = Brushes.Yellow
            };
            Canvas.SetLeft(leftMargin, 0);
            rulerCanvas.Children.Add(leftMargin);
            marginElements.Add(leftMargin);

            // Vẽ vùng margin phải
            Rectangle rightMargin = new Rectangle
            {
                Width = rulerCanvas.ActualWidth - (rulerCanvas.ActualWidth - leftMarginPx - rightMarginPx) - leftMarginPx,
                Height = rulerCanvas.Height,
                Fill = Brushes.Green
            };
            Canvas.SetRight(rightMargin, rightMarginPx);
            rulerCanvas.Children.Add(rightMargin);
            marginElements.Add(rightMargin);
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


    }
}
