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
using Shape = System.Windows.Shapes.Shape;
using System.Windows.Input;
using Wordpad.View;

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
        private Thumb rightIndentThumb;
        //Các đường gạch đứt khi kéo thumb
        private Line DashedLine;
        private GlobalDashedLineAdorner _adorner;


        private double rulerLength;
        private double leftMargin;
        private double rightMargin;
        private double textLength;
        private int thumbSize = 15;
        //Kich thước ban đầu của ruler (dùng để scale theo zoom)
        public static double oriRulerWidth;

        private RichTextBox richTextBox;
        private DockPanel dockPanel;
        private DockPanel mainContainer;

        private string currentUnit = "Inches"; // Đơn vị đo hiện tại
        private readonly Dictionary<string, double> unitConversion;

        //Thành phần để quan lý từng phần tử trong ruler
        private List<UIElement> rulerElements = new List<UIElement>(); // Các thành phần thuộc ruler
        private List<UIElement> marginElements = new List<UIElement>(); // Các thành phần thuộc margins



        public Ruler(Canvas margin, Canvas tick, Canvas thumb,Canvas ruler, RichTextBox richTextBox, DockPanel dockPanel, DockPanel mainContainer,
            GlobalDashedLineAdorner adorner)
        {
            marginCanvas = margin;
            tickCanvas = tick;
            thumbCanvas = thumb;
            rulerCanvas = ruler;
            this.richTextBox = richTextBox;
            this.dockPanel = dockPanel;
            this.mainContainer = mainContainer;
            _adorner = adorner;

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

            };
            
            //Copy indent của paragraph trước áp dụng cho paragraph sau.
            richTextBox.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    var caretParagraph = richTextBox.CaretPosition.Paragraph;
                    if (caretParagraph != null)
                    {
                        var newParagraph = new Paragraph();
                        newParagraph.TextIndent = caretParagraph.TextIndent;
                        newParagraph.Margin = caretParagraph.Margin;
                        richTextBox.Document.Blocks.InsertAfter(caretParagraph, newParagraph);
                    }
                }
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
            // Tạo Thumb cho First Line Indent - tam giác trên
            firstLineIndentThumb = CreateCustomThumb(0);
            Canvas.SetLeft(firstLineIndentThumb, leftMargin - thumbSize);
            thumbCanvas.Children.Add(firstLineIndentThumb);

            // Tạo Thumb cho Hanging Indent - tam giác dưới
            hangingIndentThumb = CreateCustomThumb(2);
            Canvas.SetLeft(hangingIndentThumb, leftMargin);
            thumbCanvas.Children.Add(hangingIndentThumb);

            // Tạo Thumb cho Paragraph Indent - chữ nhật dưới
            paragraphIndentThumb = CreateCustomThumb(1);
            Canvas.SetLeft(paragraphIndentThumb, leftMargin);
            thumbCanvas.Children.Add(paragraphIndentThumb);

            // Tạo Thumb cho Right Indent - tam giác dưới
            rightIndentThumb = CreateCustomThumb(2);
            Canvas.SetLeft(rightIndentThumb, leftMargin + textLength + thumbSize);
            thumbCanvas.Children.Add(rightIndentThumb);

            // Gắn sự kiện DragDelta cho các Thumb
            firstLineIndentThumb.DragDelta += FirstLineIndentThumb_DragDelta;
            hangingIndentThumb.DragDelta += HangingIndentThumb_DragDelta;
            paragraphIndentThumb.DragDelta += ParagraphIndentThumb_DragDelta;
            rightIndentThumb.DragDelta += RightIndentThumb_DragDelta;
            //Gắn sự kiện để hiển thị đường gạch đứt khi kéo
            
            firstLineIndentThumb.DragDelta += Thumb_DragDelta;
            hangingIndentThumb.DragDelta += Thumb_DragDelta;
            paragraphIndentThumb.DragDelta += Thumb_DragDelta;
            rightIndentThumb.DragDelta += Thumb_DragDelta;

            firstLineIndentThumb.DragStarted += Thumb_DragStarted;
            hangingIndentThumb.DragStarted += Thumb_DragStarted;
            paragraphIndentThumb.DragStarted += Thumb_DragStarted;
            rightIndentThumb.DragStarted += Thumb_DragStarted;

            firstLineIndentThumb.DragCompleted += Thumb_DragCompleted;
            hangingIndentThumb.DragCompleted += Thumb_DragCompleted;
            paragraphIndentThumb.DragCompleted += Thumb_DragCompleted;
            rightIndentThumb.DragCompleted += Thumb_DragCompleted;
            InitializeDashLines();
        }

        // Hàm tạo Thumb với giao diện tùy chỉnh
        private Thumb CreateCustomThumb(int thumbNumb)
        {
            var thumb = new Thumb
            {
                Width = 10,
                Height = 10,
                Template = CreateThumbTemplate(thumbNumb)
            };
            return thumb;
        }

        // Hàm tạo ControlTemplate cho Thumb
        private ControlTemplate CreateThumbTemplate(int thumbNumb)
        {
            var template = new ControlTemplate(typeof(Thumb));
            var factory = new FrameworkElementFactory(typeof(Canvas));
            switch(thumbNumb)
            {
                case 0:
                    // Tam giác trên
                    var topTriangle = new FrameworkElementFactory(typeof(Polygon));
                    topTriangle.SetValue(Polygon.PointsProperty, new PointCollection
                    {
                        new Point(0, 0),   //      10^  *(0,0)   10  *(10,0)
                        new Point(10, 0),   //        |          
                        new Point(5, 10)   //        v          *(5,10)
                    });
                    topTriangle.SetValue(System.Windows.Shapes.Shape.FillProperty, Brushes.Gray);
                    factory.AppendChild(topTriangle);
                    break;
                case 1:
                    // Hình chữ nhật dưới
                    var rectangle = new FrameworkElementFactory(typeof(Rectangle));
                    rectangle.SetValue(Rectangle.WidthProperty, 10.0);
                    rectangle.SetValue(Rectangle.HeightProperty, 10.0);
                    rectangle.SetValue(Canvas.TopProperty, 20.0);
                    rectangle.SetValue(Shape.FillProperty, Brushes.Gray);
                    factory.AppendChild(rectangle);
                    break;
                case 2:
                    // Tam giác trên hình chữ nhật
                    var bottomTriangle = new FrameworkElementFactory(typeof(Polygon));
                    bottomTriangle.SetValue(Polygon.PointsProperty, new PointCollection
                    {
                        new Point(0, 20),   //  10^          *(5,10)
                        new Point(10, 20),  //    |  
                        new Point(5, 10)    //    v  *(0,20)   10   *(10,20)
                    });
                    bottomTriangle.SetValue(Shape.FillProperty, Brushes.Gray);

                    // Thêm các phần tử vào Canvas

                    factory.AppendChild(bottomTriangle);
                    break;

            }
            template.VisualTree = factory;
            return template;
        }


        public void DrawMargins(double leftMarginPx, double rightMarginPx)
        {
            //MessageBox.Show($"Dock width: {dockPanel.Width}\nMargin canvas length: {marginCanvas.Width}\nRuler Lenght: {rulerLength}");
            // Xóa các thành phần cũ trong marginCanvas
            marginCanvas.Children.Clear();

            // Vẽ vùng margin trái
            Rectangle leftMarginRec = new Rectangle
            {
                Width = leftMarginPx,
                Height = marginCanvas.Height,
                Fill = Brushes.Yellow
            };
            Canvas.SetLeft(leftMarginRec, 0);
            marginCanvas.Children.Add(leftMarginRec);

            // Vẽ vùng margin phải
            Rectangle rightMarginRec = new Rectangle
            {
                Width = rightMarginPx,
                Height = marginCanvas.Height,
                Fill = Brushes.Green
            };
            Canvas.SetRight(rightMarginRec, 0);
            marginCanvas.Children.Add(rightMarginRec);
            leftMargin = leftMarginPx;
            rightMargin = rightMarginPx;
            textLength = rulerCanvas.Width - leftMargin - rightMargin;
            //MessageBox.Show($"Left margin: {leftMargin}\nRight margin: {rightMargin}\n Text length: {textLength}");
        }


        private void FirstLineIndentThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newIndent = Canvas.GetLeft(firstLineIndentThumb) + e.HorizontalChange;

            //Giới hạn cần - cho kích thước của thumb
            if (newIndent >= (leftMargin - thumbSize) && newIndent <= Canvas.GetLeft(rightIndentThumb) - 10)
            {
                Canvas.SetLeft(firstLineIndentThumb, newIndent);
                ApplyIndent();
            }
        }
        //Hanging và Paragraph indent sẽ dính với nhau
        private void HangingIndentThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newIndent = Canvas.GetLeft(hangingIndentThumb) + e.HorizontalChange;

            // Giới hạn di chuyển giữa FirstLineIndent và ParagraphIndent
            if (newIndent >= (leftMargin - 5) && newIndent <= Canvas.GetLeft(rightIndentThumb))
            {
                Canvas.SetLeft(hangingIndentThumb, newIndent);

                // Di chuyển cả Paragraph Indent cùng lúc
                Canvas.SetLeft(paragraphIndentThumb, newIndent);

                ApplyIndent();
            }
        }
        //Chỉnh indent cả đoạn và đồng thời cũng sẽ kéo theo các thumb khác theo --> kiểm tra vị trí của các thumb khác khi kéo(chỉ có first line thumb)
        private void ParagraphIndentThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newIndent = Canvas.GetLeft(paragraphIndentThumb) + e.HorizontalChange;
            double delta = newIndent - Canvas.GetLeft(paragraphIndentThumb);
            double setHanging = Canvas.GetLeft(hangingIndentThumb) + delta;
            double setFirstLine = Canvas.GetLeft(firstLineIndentThumb) + delta;
            // Khi di chuyển first line thumb thì cũng phải xét vị trí của nó so với text length của ruler
            if (newIndent >= (leftMargin - 5) && newIndent <= Canvas.GetLeft(rightIndentThumb)
                && setFirstLine >= (leftMargin - thumbSize) && setFirstLine <= Canvas.GetLeft(rightIndentThumb) - 10)
            {
                // Di chuyển toàn bộ các Thumb trái

                Canvas.SetLeft(paragraphIndentThumb, newIndent);
                Canvas.SetLeft(hangingIndentThumb, setHanging);
                Canvas.SetLeft(firstLineIndentThumb, setFirstLine);

                ApplyIndent();
            }
        }

        private void RightIndentThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newIndent = Canvas.GetLeft(rightIndentThumb) + e.HorizontalChange;

            // Giới hạn di chuyển trong khoảng hợp lệ
            if (newIndent >= (Canvas.GetLeft(firstLineIndentThumb) + 10) && newIndent >= (Canvas.GetLeft(hangingIndentThumb)) &&
                newIndent <= (rulerLength - rightMargin - 5))
            {
                Canvas.SetLeft(rightIndentThumb, newIndent);
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
            double firstLineIndent = (Canvas.GetLeft(firstLineIndentThumb) - (leftMargin - thumbSize));
            double hangingIndent = (Canvas.GetLeft(hangingIndentThumb) - (leftMargin - 5));
            double paragraphIndent = (Canvas.GetLeft(paragraphIndentThumb) - (leftMargin - 5));
            double rightIndent = (rulerLength - Canvas.GetLeft(rightIndentThumb));

            // Cập nhật đoạn văn
            paragraph.TextIndent = firstLineIndent;
            paragraph.Margin = new Thickness(paragraphIndent, 0, 0, 0);
        }



        public void IsVisible(bool visible)
        {
            rulerCanvas.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        //Scale ruler khi zoom
        public void ScaleRuler(double zoomScale, double preDPWidth)
        {
            double targetWidth = dockPanel.Width; // Kích thước cố định mong muốn

            // Tính toán tỷ lệ
            double scaleX = targetWidth / oriRulerWidth;
            double delta = dockPanel.Width - preDPWidth;

            // Áp dụng scale theo kích thước ban đầu của ruler
            rulerCanvas.LayoutTransform = new ScaleTransform(scaleX, 1);
            // Tính lại vị trí dockPanel (nếu bị dịch chuyển do zoom)
            dockPanel.UpdateLayout();
            Point dockPanelPosition = dockPanel.TransformToAncestor(mainContainer)
                                             .Transform(new Point(0, 0));

            // Cập nhật Margin của rulerCanvas để nó luôn nằm đúng vị trí so với dockPanel
            if(dockPanelPosition.X > 0)
            {
                Thickness newMargin = new Thickness(dockPanelPosition.X - 15, 0, dockPanelPosition.X, 0);
                rulerCanvas.Margin = newMargin;
            }
            else
            {
                Thickness newMargin = new Thickness(rulerCanvas.Margin.Left + delta, 0, 0, 0);
                rulerCanvas.Margin = newMargin;
            }
            //MessageBox.Show($"dock panel X: {dockPanelPosition.X}\nRuler canvas margin: {rulerCanvas.Margin}");
        }
        
        //Hàm cập nhật chiều dài của canvas do khi đang chạy hàm adjust dockpanel, kích thước của dock panel chưa thực sự được đổi nên cần đổi trực
        //tiếp trong class Ruler
        public void UpdateCanvasSize(double length)
        {
            rulerLength = length;
            marginCanvas.Width = length;
            tickCanvas.Width = length;
            thumbCanvas.Width = length;
        }

        //Tạo các nét đứt
        private void InitializeDashLines()
        {
            DashedLine = CreateDashLine();
        }

        private Line CreateDashLine()
        {
            return new Line
            {
                Stroke = Brushes.Gray,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection { 2, 2 }, // Tạo đường gạch đứt
                Y1 = 0,
                Y2 = rulerCanvas.Height
            };
        }
        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                if(rulerCanvas.Margin.Right != 0)
                {
                    double left = Canvas.GetLeft(thumb) + 29;
                    // Cập nhật đường gạch đứt, bắt đầu từ dưới RulerCanvas
                    _adorner.UpdateLine(left, rulerCanvas.ActualHeight + 10, true);
                }
                else
                {
                    double left = Canvas.GetLeft(thumb) + 29 + rulerCanvas.Margin.Left;
                    // Cập nhật đường gạch đứt, bắt đầu từ dưới RulerCanvas
                    _adorner.UpdateLine(left, rulerCanvas.ActualHeight + 10, true);
                }
            }
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                if (rulerCanvas.Margin.Right != 0)
                {
                    double left = Canvas.GetLeft(thumb) + 29;
                    // Cập nhật đường gạch đứt, bắt đầu từ dưới RulerCanvas
                    _adorner.UpdateLine(left, rulerCanvas.ActualHeight + 10, true);
                }
                else
                {
                    double left = Canvas.GetLeft(thumb) + 29 + rulerCanvas.Margin.Left;
                    // Cập nhật đường gạch đứt, bắt đầu từ dưới RulerCanvas
                    _adorner.UpdateLine(left, rulerCanvas.ActualHeight + 10, true);
                }
            }
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _adorner.UpdateLine(0, 0, false); // Ẩn đường gạch đứt
        }

        public void SetAdorner(GlobalDashedLineAdorner adorner)
        {
            _adorner = adorner;
            // Nếu cần, cập nhật lại giao diện hoặc các logic khác liên quan đến Adorner
        }
    }
}
