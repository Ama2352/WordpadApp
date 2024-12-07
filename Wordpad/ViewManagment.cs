using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using static System.Windows.MessageBox;

namespace Wordpad
{
    public class ViewManagment
    {
        private readonly Canvas rulerCanvas;
        private readonly StatusBar statusBar;
        private readonly StatusBarItem statusBarItem;
        private readonly RichTextBox richTextBox;
        private readonly ComboBox unitComboBox;
        private DockPanel dockPanel;
        private Slider zoomSlider;
        private double zoomScale = 1.0; // Giá trị ban đầu (100%)
        //Kích thước ban đầu cảu dockpanel
        private double DPWidthOri;
        private double DPHeightOri;

        public ViewManagment(Canvas rulerCanvas, StatusBar statusBar, StatusBarItem statusBarItem, 
            RichTextBox richTextBox, ComboBox unitComboBox, DockPanel DP, Slider slider)
        {
            this.rulerCanvas = rulerCanvas;
            this.statusBar = statusBar;
            this.statusBarItem = statusBarItem;
            this.richTextBox = richTextBox;
            this.unitComboBox = unitComboBox;
            this.dockPanel = DP;
            this.zoomSlider = slider;
            DPWidthOri = dockPanel.Width;
            DPHeightOri = dockPanel.Height;

            DrawRuler();

            // Đăng ký sự kiện TextChanged cho RichTextBox
            richTextBox.TextChanged += RichTextBox_TextChanged;
        }


        public void UpdateRuler()
        {
            if (unitComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string unit = selectedItem.Content.ToString();
                double scale = rulerCanvas.ActualWidth / 100; // Default scale
                switch (unit)
                {
                    case "Inch":
                        scale = rulerCanvas.ActualWidth / 10; // Example scale for inches
                        break;
                    case "Cm":
                        scale = rulerCanvas.ActualWidth / 25; // Example scale for cm
                        break;
                    case "Points":
                        scale = rulerCanvas.ActualWidth / 72; // Example scale for points
                        break;
                    case "Picas":
                        scale = rulerCanvas.ActualWidth / 6; // Example scale for picas
                        break;
                }
                DrawRuler(scale);
            }
        }

        public void ShowRuler(bool show)
        {
            rulerCanvas.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        public void ShowStatusBar(bool show)
        {
            statusBar.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        public void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int charCount = GetCharacterCount();
            int lineCount = GetLineCount();

            statusBarItem.Content = $"Line: {lineCount} | Characters: {charCount}";
        }

        public void WordCount_Click(object sender, RoutedEventArgs e)
        {
            int charCount = GetCharacterCount();
            int lineCount = GetLineCount();

            Show($"Character Count: {charCount}\nLine Count: {lineCount}", "Character and Line Count");
            statusBarItem.Content = $"Line: {lineCount} | Characters: {charCount}";
        }

        public void ZoomIn()
        {
            if (zoomScale < 5) // Đảm bảo mức zoom không xuống dưới 10%
            {
                zoomScale += 0.1; // Giảm 10% mức zoom
                ApplyZoom();
            }
        }

        public void ZoomOut()
        {
            if (zoomScale > 0.1) // Đảm bảo mức zoom không xuống dưới 10%
            {
                zoomScale -= 0.1; // Giảm 10% mức zoom
                ApplyZoom();
            }
        }

        public void Set100()
        {
            zoomScale = 1;
            ApplyZoom();
        }

        public void SetZoom(double zoomLevel)
        {
            if (zoomLevel < 0.1 || zoomLevel > 5)
                return;

            zoomScale = zoomLevel;
            ApplyZoom();
        }

        private void ApplyZoom()
        {
            // Phóng to cả RichTextBox bằng cách thay đổi kích thước.
            dockPanel.Width = DPWidthOri * zoomScale; // Kích thước mặc định là 743.
            dockPanel.Height = DPHeightOri * zoomScale; // Chiều cao mặc định là 500.
            // Áp dụng zoom cho RichTextBox qua LayoutTransform
            richTextBox.LayoutTransform = new ScaleTransform(zoomScale, zoomScale);

            // Cập nhật giá trị trên thanh trạng thái
            zoomSlider.Value = zoomScale * 100;
        }

        private int GetCharacterCount()
        {
            // Tính tổng số ký tự trong RichTextBox
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            return textRange.Text.TrimEnd('\r', '\n').Length; // Loại bỏ ký tự xuống dòng cuối cùng
        }

        private int GetLineCount()
        {
            // Dựa vào số dòng logic trong FlowDocument
            return richTextBox.Document.Blocks.Count;
        }

        private void DrawRuler(double scale = 1)
        {
            rulerCanvas.Children.Clear();
            for (int i = 0; i <= 100; i++)
            {
                Line line = new Line
                {
                    X1 = i * scale,
                    Y1 = 0,
                    X2 = i * scale,
                    Y2 = i % 10 == 0 ? 20 : 10, // Longer lines for every 10 units
                    Stroke = System.Windows.Media.Brushes.Black,
                    StrokeThickness = i % 10 == 0 ? 2 : 1
                };
                rulerCanvas.Children.Add(line);

                if (i % 10 == 0)
                {
                    TextBlock text = new TextBlock
                    {
                        Text = (i / 10).ToString(),
                        Foreground = System.Windows.Media.Brushes.Black,
                        FontSize = 10
                    };
                    Canvas.SetLeft(text, i * scale);
                    Canvas.SetTop(text, 20);
                    rulerCanvas.Children.Add(text);
                }
            }
        }
    }
}
