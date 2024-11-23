using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;

namespace WordPad
{
    internal class FontManager
    {
        private RichTextBox _richTextBox;
        private int[] fontSize = new int[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
        public event Action<double> FontSizeChanged;

        // Khởi tạo với tham chiếu tới RichTextBox 
        public FontManager(RichTextBox richTextBox)
        {
            _richTextBox = richTextBox;
        }

        // Lấy danh sách font chữ
        public List<string> GetFontFamilies()
        {
            var fontFamilies = new List<string>();
            foreach (var font in Fonts.SystemFontFamilies)
            {
                fontFamilies.Add(font.Source);
            }
            return fontFamilies;
        }

        // Thay đổi kiểu font
        public void ChangeFontFamily(string fontFamily)
        {
            if (_richTextBox.Selection.IsEmpty)
                return;

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            var currentFont = selection.GetPropertyValue(TextElement.FontFamilyProperty) as FontFamily;
            selection.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily(fontFamily));
        }

        // Thay đổi kích thước font
        public void ChangeFontSize(double fontSize)
        {
            if (_richTextBox.Selection.IsEmpty)
                return;
            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
        }

        // Thay đổi màu văn bản
        public void ChangeFontColor(Color color)
        {
            if (_richTextBox.Selection.IsEmpty)
                return;

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
        }

        // Thay đổi màu nền (highlight) văn bản
        public void ChangeHighlightColor(Color color)
        {
            if (_richTextBox.Selection.IsEmpty)
                return;

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            selection.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(color));
        }

        // Chuyển văn bản sang in đậm
        public void ToggleBold()
        {
            if (_richTextBox.Selection.IsEmpty)
                return;

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            var currentFontWeight = (FontWeight)selection.GetPropertyValue(TextElement.FontWeightProperty);
            selection.ApplyPropertyValue(TextElement.FontWeightProperty, currentFontWeight == FontWeights.Bold ? FontWeights.Normal : FontWeights.Bold);
        }

        // Chuyển văn bản sang in nghiêng
        public void ToggleItalic()
        {
            if (_richTextBox.Selection.IsEmpty)
                return;

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            var currentFontStyle = (FontStyle)selection.GetPropertyValue(TextElement.FontStyleProperty);
            selection.ApplyPropertyValue(TextElement.FontStyleProperty, currentFontStyle == FontStyles.Italic ? FontStyles.Normal : FontStyles.Italic);
        }

        // Chuyển văn bản sang gạch chân
        public void ToggleUnderline()
        {
            if (_richTextBox.Selection.IsEmpty)
                return;

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            var currentTextDecoration = (TextDecorationCollection)selection.GetPropertyValue(Inline.TextDecorationsProperty);
            
            // Kiểm tra xem có TextDecoration là Underline không
            if (currentTextDecoration.Any(decoration => decoration == TextDecorations.Underline[0]))
            {
                selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
            }
            else
            {
                selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            }
        }

        // Thay đổi kiểu gạch ngang văn bản
        public void ToggleStrikethrough()
        {
            if (_richTextBox.Selection.IsEmpty)
                return;

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            var currentTextDecoration = (TextDecorationCollection)selection.GetPropertyValue(Inline.TextDecorationsProperty);
            
            // Kiểm tra xem có TextDecoration là Strikethrough không
            if (currentTextDecoration.Any(decoration => decoration == TextDecorations.Strikethrough[0]))
            {
                selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
            }
            else
            {
                selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Strikethrough);
            }
        }

        // Chuyển văn bản thành chỉ số dưới (Subscript)
        public void ToggleSubscript()
        {
            if (_richTextBox.Selection.IsEmpty)
                return;
            try
            {
                var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);

                // Lấy kích cỡ văn bản được chọn
                var currentFontSize = selection.GetPropertyValue(TextElement.FontSizeProperty);

                // Xác định kiểu định dạng (Subscript, Superscript, hoặc Baseline)
                var currentAlignment = selection.GetPropertyValue(Inline.BaselineAlignmentProperty);

                // Kiểm tra giá trị FontSize hợp lệ
                if (currentFontSize == DependencyProperty.UnsetValue || !(currentFontSize is double fontSize))
                    fontSize = 11.0; // Giá trị mặc định nếu không xác định được kích thước font
                var scaleSubscript = 0.75;

                // Kiểm tra giá trị Alignment hợp lệ
                BaselineAlignment alignment = BaselineAlignment.Baseline;
                if (currentAlignment != DependencyProperty.UnsetValue && currentAlignment is BaselineAlignment)
                    alignment = (BaselineAlignment)currentAlignment;

                // Thay đổi định dạng
                if (alignment == BaselineAlignment.Baseline)
                {
                    double newFontSize = Math.Max(fontSize * scaleSubscript, 8.0); // Đặt giới hạn font tối thiểu là 8.0
                    selection.ApplyPropertyValue(TextElement.FontSizeProperty, newFontSize);
                    selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Subscript);
                }
                else if (alignment == BaselineAlignment.Superscript)
                {
                    selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Subscript);
                }
                else
                {
                    double newFontSize = Math.Min(fontSize / scaleSubscript, 72.0); // Giới hạn font tối đa là 72.0
                    selection.ApplyPropertyValue(TextElement.FontSizeProperty, newFontSize);
                    selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Baseline);
                }
            }
            catch(Exception ex)
            {
                // Log hoặc xử lý lỗi (ví dụ: hiển thị thông báo lỗi)
                MessageBox.Show($"An error occurred: {ex.Message}");
            } 
            
            
        }


        // Chuyển văn bản thành chỉ số trên (Superscript)
        public void ToggleSuperscript()
        {
            if (_richTextBox.Selection.IsEmpty)
                return;

            try
            {
                var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);

                // Lấy kích cỡ văn bản được chọn
                var currentFontSize = selection.GetPropertyValue(TextElement.FontSizeProperty);

                // Xác định kiểu định dạng (Subscript, Superscript, hoặc Baseline)
                var currentAlignment = selection.GetPropertyValue(Inline.BaselineAlignmentProperty);

                // Kiểm tra giá trị FontSize hợp lệ
                if (currentFontSize == DependencyProperty.UnsetValue || !(currentFontSize is double fontSize))
                    fontSize = 11.0; // Giá trị mặc định nếu không xác định được kích thước font
                var scaleSuperscript = 0.75;

                // Kiểm tra giá trị Alignment hợp lệ
                BaselineAlignment alignment = BaselineAlignment.Baseline;
                if (currentAlignment != DependencyProperty.UnsetValue && currentAlignment is BaselineAlignment)
                    alignment = (BaselineAlignment)currentAlignment;

                // Thay đổi định dạng
                if (alignment == BaselineAlignment.Baseline)
                {
                    double newFontSize = Math.Max(fontSize * scaleSuperscript, 8.0); // Đặt giới hạn font tối thiểu là 8.0
                    selection.ApplyPropertyValue(TextElement.FontSizeProperty, newFontSize);
                    selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                }
                else if (alignment == BaselineAlignment.Subscript)
                {
                    selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                }
                else
                {
                    double newFontSize = Math.Min(fontSize / scaleSuperscript, 72.0); // Giới hạn font tối đa là 72.0
                    selection.ApplyPropertyValue(TextElement.FontSizeProperty, newFontSize);
                    selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Baseline);
                }
            }
            catch(Exception ex)
            {
                // Log hoặc xử lý lỗi (ví dụ: hiển thị thông báo lỗi)
                MessageBox.Show($"An error occurred: {ex.Message}");
            }        
            
        }

       

        // Tăng kích thước font
        public void GrowFont()
        {
            if (_richTextBox.Selection.IsEmpty)
                return;

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            var currentFontSize = (double)selection.GetPropertyValue(TextElement.FontSizeProperty);
            int currentIndex = Array.FindIndex(fontSize, size => size >= currentFontSize);

            if (currentIndex >= 0 && currentIndex < fontSize.Length - 1)
            {
                double newSize = fontSize[currentIndex + 1];
                selection.ApplyPropertyValue(TextElement.FontSizeProperty, newSize);

                FontSizeChanged?.Invoke(newSize);
            }
        }

        // Giảm kích thước font
        public void ShrinkFont()
        {
            if (_richTextBox.Selection.IsEmpty)
                return;

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            var currentFontSize = (double)selection.GetPropertyValue(TextElement.FontSizeProperty);
            int currentIndex = Array.FindIndex(fontSize, size => size >= currentFontSize);

            if (currentIndex > 0)
            {
                double newSize = fontSize[currentIndex - 1];
                selection.ApplyPropertyValue(TextElement.FontSizeProperty, newSize);

                FontSizeChanged?.Invoke(newSize);
            }
        }
    }
}
