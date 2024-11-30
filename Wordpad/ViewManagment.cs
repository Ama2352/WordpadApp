using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace Wordpad
{
    public class ViewManagement
    {
        private readonly Canvas rulerCanvas;
        private readonly StatusBar statusBar;
        private readonly StatusBarItem statusBarItem;
        private readonly RichTextBox richTextBox;
        private double zoomFactor = 1.0;
        private readonly ComboBox unitComboBox;

        public ViewManagement(Canvas rulerCanvas, StatusBar statusBar, StatusBarItem statusBarItem, RichTextBox richTextBox, ComboBox unitComboBox)
        {
            this.rulerCanvas = rulerCanvas;
            this.statusBar = statusBar;
            this.statusBarItem = statusBarItem;
            this.richTextBox = richTextBox;

            DrawRuler();

            // Đăng ký sự kiện
            richTextBox.SizeChanged += (sender, e) => SyncRulerWithRichTextBox();
            this.unitComboBox = unitComboBox;
        }

        // Zoom In
        public void ZoomIn()
        {
            zoomFactor = Math.Min(zoomFactor + 0.1, 3.0); // Giới hạn zoom tối đa là 300%
            ApplyZoom();
        }

        // Zoom Out
        public void ZoomOut()
        {
            zoomFactor = Math.Max(zoomFactor - 0.1, 0.5); // Giới hạn zoom tối thiểu là 50%
            ApplyZoom();
        }
        public void SetZoom(double factor)
        {
            zoomFactor = factor;
            zoomFactor = Math.Max(zoomFactor - 0.1, 0.5); // Giới hạn zoom tối thiểu là 50%
            ApplyZoom();
        }

        public double ZoomFactor
        {
            get { return zoomFactor; }
            set { zoomFactor = value; ApplyZoom(); }
        }
        public void ShowRuler(bool show)
        {
            rulerCanvas.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        public void ShowStatusBar(bool show)
        {
            statusBar.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        // Áp dụng zoom và đồng bộ hóa
        private void ApplyZoom()
        {
            // Sử dụng ScaleTransform để zoom
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(zoomFactor, zoomFactor));
            richTextBox.LayoutTransform = transformGroup;

            // Cập nhật trạng thái zoom trên thanh trạng thái
            statusBarItem.Content = $"Zoom: {(int)(zoomFactor * 100)}%";

            // Đồng bộ hóa thước đo
            SyncRulerWithRichTextBox();
        }

        // Cập nhật thước đo theo zoom
        public void SyncRulerWithRichTextBox()
        {
            rulerCanvas.Width = richTextBox.ActualWidth * zoomFactor;
            UpdateRuler();
        }

        // Vẽ lại thước đo
        public void UpdateRuler()
        {
            double scale = rulerCanvas.Width / 100; // Chia thành 100 phần
            DrawRuler(scale);
        }

        private void DrawRuler(double scale = 1)
        {
            rulerCanvas.Children.Clear();
            int maxMarks = (int)(rulerCanvas.Width / scale);

            for (int i = 0; i <= maxMarks; i++)
            {
                Line line = new Line
                {
                    X1 = i * scale,
                    Y1 = 0,
                    X2 = i * scale,
                    Y2 = i % 10 == 0 ? 20 : 10, // Vạch dài hơn ở bội số của 10
                    Stroke = Brushes.Black,
                    StrokeThickness = i % 10 == 0 ? 2 : 1
                };
                rulerCanvas.Children.Add(line);

                if (i % 10 == 0)
                {
                    TextBlock text = new TextBlock
                    {
                        Text = (i / 10).ToString(),
                        Foreground = Brushes.Black,
                        FontSize = 10
                    };
                    Canvas.SetLeft(text, i * scale - 5); // Căn giữa text
                    Canvas.SetTop(text, 20);
                    rulerCanvas.Children.Add(text);
                }
            }
        }
    }
}
