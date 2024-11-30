using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Media;
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
        private double zoomFactor = 1.0;

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
            richTextBox.SizeChanged += (sender, e) => SyncRulerWithRichTextBox();
        }

        public void ZoomIn()
        {
            zoomFactor += 0.1;
            ApplyZoom();
        }

        public void ZoomOut()
        {
            zoomFactor = Math.Max(0.1, zoomFactor - 0.1);
            ApplyZoom();
        }

        public void SetZoom(double factor)
        {
            zoomFactor = factor;
            ApplyZoom();
        }

        public double ZoomFactor
        {
            get { return zoomFactor; }
            set { zoomFactor = value; ApplyZoom(); }
        }

        public void UpdateRuler()
        {
            // Đồng bộ chiều rộng rulerCanvas với RichTextBox
            double scaledWidth = richTextBox.ActualWidth / zoomFactor; // Điều chỉnh chiều rộng theo zoom
            rulerCanvas.Width = scaledWidth;

            double scale = rulerCanvas.Width / 100; // Default scale
            if (unitComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string unit = selectedItem.Content.ToString();
                switch (unit)
                {
                    case "Inch":
                        scale = rulerCanvas.Width / 10; // Scale cho inch
                        break;
                    case "Cm":
                        scale = rulerCanvas.Width / 25; // Scale cho cm
                        break;
                    case "Points":
                        scale = rulerCanvas.Width / 72; // Scale cho points
                        break;
                    case "Picas":
                        scale = rulerCanvas.Width / 6; // Scale cho picas
                        break;
                }
            }
            DrawRuler(scale);
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

        private void ApplyZoom()
        {
            // Cập nhật zoom cho RichTextBox
            if (richTextBox.Document != null)
            {
                foreach (var block in richTextBox.Document.Blocks)
                {
                    block.FontSize = 12 * zoomFactor; // Phóng to nội dung dựa trên zoomFactor
                }
            }

            // Zoom nội dung chính bằng ScaleTransform
            double scaleX = zoomFactor;
            double scaleY = zoomFactor;
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(scaleX, scaleY));
            richTextBox.LayoutTransform = transformGroup;

            // Cập nhật lại thước đo
            SyncRulerWithRichTextBox(); // Đảm bảo thước kẻ được đồng bộ hóa
            UpdateRuler(); // Cập nhật lại thước đo
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

        public void SyncRulerWithRichTextBox()
        {
            // Đồng bộ chiều rộng rulerCanvas với kích thước RichTextBox
            rulerCanvas.Width = richTextBox.ActualWidth;
            UpdateRuler(); // Cập nhật lại thước đo
        }

        private void DrawRuler(double scale = 1)
        {
            rulerCanvas.Children.Clear();
            int maxMarks = (int)(rulerCanvas.Width / scale); // Số vạch tối đa dựa trên chiều rộng

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
                    Canvas.SetLeft(text, i * scale - 5); // Dịch sang trái để căn giữa
                    Canvas.SetTop(text, 20);
                    rulerCanvas.Children.Add(text);
                }
            }
        }

    }
}
