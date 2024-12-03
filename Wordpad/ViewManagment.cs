using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
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
        private int zoomLevel = 100;

        public ViewManagment(Canvas rulerCanvas, StatusBar statusBar, StatusBarItem statusBarItem, RichTextBox richTextBox, ComboBox unitComboBox)
        {
            this.rulerCanvas = rulerCanvas;
            this.statusBar = statusBar;
            this.statusBarItem = statusBarItem;
            this.richTextBox = richTextBox;
            this.unitComboBox = unitComboBox;

            DrawRuler();

            // Đăng ký sự kiện TextChanged cho RichTextBox
            richTextBox.TextChanged += RichTextBox_TextChanged;
            richTextBox.SizeChanged += (s, e) => DrawRuler();
        }

        public void ZoomIn()
        {
            zoomLevel += 10;
            ApplyZoom();
        }

        public void ZoomOut()
        {
            zoomLevel -= 10;
            ApplyZoom();
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

        public void SetZoom(int zoomLevel)
        {
            if (zoomLevel < 50 || zoomLevel > 200)
                return;

            this.zoomLevel = zoomLevel;
            ApplyZoom();
        }

        private void ApplyZoom()
        {
            // Cập nhật zoom bằng cách thay đổi kích thước phông chữ trong FlowDocument
            if (richTextBox.Document != null)
            {
                foreach (var block in richTextBox.Document.Blocks)
                {
                    block.FontSize = 12 * zoomLevel / 100; // Font size gốc là 12
                }
            }
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
            double richTextBoxWidth = richTextBox.ActualWidth; // Get the actual width of the RichTextBox
            double maxUnits = richTextBoxWidth / scale; // Calculate the maximum units visible

            for (int i = 0; i <= maxUnits; i++)
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
                    Canvas.SetLeft(text, i * scale - text.ActualWidth / 2); // Center-align the text
                    Canvas.SetTop(text, 20);
                    rulerCanvas.Children.Add(text);
                }
            }
        }

    }
}
