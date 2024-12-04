using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;

namespace Wordpad
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
            {
                // Nếu không có vùng chọn, áp dụng font size tại vị trí con trỏ
                TextPointer caretPosition = _richTextBox.CaretPosition;

                // Lưu lại vị trí con trỏ trước khi chèn dấu cách
                TextPointer startPointer = caretPosition;

                // Chèn dấu cách vào vị trí con trỏ (không di chuyển con trỏ chuột)
                caretPosition.InsertTextInRun(" ");

                // Dịch chuyển con trỏ chuột ra sau dấu cách vừa chèn vào
                _richTextBox.CaretPosition = _richTextBox.CaretPosition.GetPositionAtOffset(1, LogicalDirection.Forward);

                // Lấy vị trí con trỏ sau khi chèn dấu cách
                TextPointer endPointer = _richTextBox.CaretPosition;

                // Chọn vùng từ startPointer đến endPointer (vùng vừa chèn dấu cách)
                _richTextBox.Selection.Select(startPointer, endPointer);

            } 

            // Áp dụng font family cho vùng chọn
            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            selection.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily(fontFamily));

            // Đảm bảo RichTextBox có focus để con trỏ luôn hiển thị
            _richTextBox.Focus();
        }

        // Thay đổi kích thước font
        public void ChangeFontSize(double fontSize)
        {
            if (_richTextBox.Selection.IsEmpty)
            {
                // Nếu không có vùng chọn, áp dụng font size tại vị trí con trỏ
                TextPointer caretPosition = _richTextBox.CaretPosition;

                // Lưu lại vị trí con trỏ trước khi chèn dấu cách
                TextPointer startPointer = caretPosition;

                // Chèn dấu cách vào vị trí con trỏ (không di chuyển con trỏ chuột)
                caretPosition.InsertTextInRun(" ");

                // Dịch chuyển con trỏ chuột ra sau dấu cách vừa chèn vào
                _richTextBox.CaretPosition = _richTextBox.CaretPosition.GetPositionAtOffset(1, LogicalDirection.Forward);

                // Lấy vị trí con trỏ sau khi chèn dấu cách
                TextPointer endPointer = _richTextBox.CaretPosition;

                // Chọn vùng từ startPointer đến endPointer (vùng vừa chèn dấu cách)
                _richTextBox.Selection.Select(startPointer, endPointer);

            } 

            // Áp dụng font size cho vùng chọn
            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);

            // Đảm bảo RichTextBox có focus để con trỏ luôn hiển thị
            _richTextBox.Focus();
        }


        // Thay đổi màu văn bản
        public void ChangeFontColor(Color color)
        {
            if (_richTextBox.Selection.IsEmpty)
            {
                // Nếu không có vùng chọn, áp dụng font size tại vị trí con trỏ
                TextPointer caretPosition = _richTextBox.CaretPosition;

                if (caretPosition != null && caretPosition.IsAtInsertionPosition)
                {
                    // Lưu lại vị trí con trỏ trước khi chèn dấu cách
                    TextPointer startPointer = caretPosition;

                    // Chèn dấu cách vào vị trí con trỏ (không di chuyển con trỏ chuột)
                    caretPosition.InsertTextInRun(" ");

                    // Dịch chuyển con trỏ chuột ra sau dấu cách vừa chèn vào
                    _richTextBox.CaretPosition = _richTextBox.CaretPosition.GetPositionAtOffset(1, LogicalDirection.Forward);

                    // Lấy vị trí con trỏ sau khi chèn dấu cách
                    TextPointer endPointer = _richTextBox.CaretPosition;

                    // Chọn vùng từ startPointer đến endPointer (vùng vừa chèn dấu cách)
                    _richTextBox.Selection.Select(startPointer, endPointer);
                }
            }

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));

            // Đảm bảo RichTextBox có focus để con trỏ luôn hiển thị
            _richTextBox.Focus();
        }

        // Thay đổi màu nền (highlight) văn bản
        public void ChangeHighlightColor(Color color)
        {
            if (_richTextBox.Selection.IsEmpty)
            {
                // Nếu không có vùng chọn, áp dụng font size tại vị trí con trỏ
                TextPointer caretPosition = _richTextBox.CaretPosition;

                if (caretPosition != null && caretPosition.IsAtInsertionPosition)
                {
                    // Lưu lại vị trí con trỏ trước khi chèn dấu cách
                    TextPointer startPointer = caretPosition;
                    // Chèn dấu cách vào vị trí con trỏ (không di chuyển con trỏ chuột)
                    caretPosition.InsertTextInRun(" ");

                    // Dịch chuyển con trỏ chuột ra sau dấu cách vừa chèn vào
                    _richTextBox.CaretPosition = _richTextBox.CaretPosition.GetPositionAtOffset(1, LogicalDirection.Forward);

                    // Lấy vị trí con trỏ sau khi chèn dấu cách
                    TextPointer endPointer = _richTextBox.CaretPosition;

                    // Chọn vùng từ startPointer đến endPointer (vùng vừa chèn dấu cách)
                    _richTextBox.Selection.Select(startPointer, endPointer);
                }
            }

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            selection.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(color));

            // Đảm bảo RichTextBox có focus để con trỏ luôn hiển thị
            _richTextBox.Focus();

        }

        // Chuyển văn bản sang in đậm
        public void ToggleBold()
        {
            if (_richTextBox.Selection.IsEmpty)
            {
                // Nếu không có vùng chọn, áp dụng font size tại vị trí con trỏ
                TextPointer caretPosition = _richTextBox.CaretPosition;

                if (caretPosition != null && caretPosition.IsAtInsertionPosition)
                {
                    // Lưu lại vị trí con trỏ trước khi chèn dấu cách
                    TextPointer startPointer = caretPosition;

                    // Chèn dấu cách vào vị trí con trỏ (không di chuyển con trỏ chuột)
                    caretPosition.InsertTextInRun(" ");

                    // Dịch chuyển con trỏ chuột ra sau dấu cách vừa chèn vào
                    _richTextBox.CaretPosition = _richTextBox.CaretPosition.GetPositionAtOffset(1, LogicalDirection.Forward);

                    // Lấy vị trí con trỏ sau khi chèn dấu cách
                    TextPointer endPointer = _richTextBox.CaretPosition;

                    // Chọn vùng từ startPointer đến endPointer (vùng vừa chèn dấu cách)
                    _richTextBox.Selection.Select(startPointer, endPointer);
                }
            }

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);

            // Lấy giá trị fontWeight của đoạn văn bản
            var currentFontWeight = selection.GetPropertyValue(TextElement.FontWeightProperty);

            // Kiểm tra xem giá trị có phải là DependencyProperty.UnsetValue hay không
            if(currentFontWeight == null 
                || (currentFontWeight != null && currentFontWeight.Equals(DependencyProperty.UnsetValue)))
            {
                // Áp dụng kiểu định dạng Bold
                selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }    
            else
            {
                // Ép kiểu giá trị trả về thành FontWeight và kiểm tra
                var fontWeight = (FontWeight)currentFontWeight;
                selection.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight == FontWeights.Bold ? FontWeights.Normal : FontWeights.Bold);
            }
            // Đảm bảo RichTextBox có focus để con trỏ luôn hiển thị
            _richTextBox.Focus();

        }

        // Chuyển văn bản sang in nghiêng
        public void ToggleItalic()
        {
            if (_richTextBox.Selection.IsEmpty)
            {
                // Nếu không có vùng chọn, áp dụng font size tại vị trí con trỏ
                TextPointer caretPosition = _richTextBox.CaretPosition;

                if (caretPosition != null && caretPosition.IsAtInsertionPosition)
                {
                    // Lưu lại vị trí con trỏ trước khi chèn dấu cách
                    TextPointer startPointer = caretPosition;

                    // Chèn dấu cách vào vị trí con trỏ (không di chuyển con trỏ chuột)
                    caretPosition.InsertTextInRun(" ");

                    // Dịch chuyển con trỏ chuột ra sau dấu cách vừa chèn vào
                    _richTextBox.CaretPosition = _richTextBox.CaretPosition.GetPositionAtOffset(1, LogicalDirection.Forward);

                    // Lấy vị trí con trỏ sau khi chèn dấu cách
                    TextPointer endPointer = _richTextBox.CaretPosition;

                    // Chọn vùng từ startPointer đến endPointer (vùng vừa chèn dấu cách)
                    _richTextBox.Selection.Select(startPointer, endPointer);
                }
            }

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);

            var currentFontStyle = selection.GetPropertyValue(TextElement.FontStyleProperty);

            // Kiểm tra xem giá trị có phải là DependencyProperty.UnsetValue hay không
            if (currentFontStyle == null
                || (currentFontStyle != null && currentFontStyle.Equals(DependencyProperty.UnsetValue)))
            {
                // Áp dụng kiểu định dạng Italic
                selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
            }
            else
            {
                // Ép kiểu giá trị trả về thành FontStyle và kiểm tra
                var fontStyle = (FontStyle)currentFontStyle;
                selection.ApplyPropertyValue(TextElement.FontStyleProperty, fontStyle == FontStyles.Italic ? FontStyles.Normal : FontStyles.Italic);
            }               

            // Đảm bảo RichTextBox có focus để con trỏ luôn hiển thị
            _richTextBox.Focus();

        }

        // Chuyển văn bản sang gạch chân
        public void ToggleUnderline()
        {
            if (_richTextBox.Selection.IsEmpty)
            {
                // Nếu không có vùng chọn, áp dụng font size tại vị trí con trỏ
                TextPointer caretPosition = _richTextBox.CaretPosition;

                if (caretPosition != null && caretPosition.IsAtInsertionPosition)
                {
                    // Lưu lại vị trí con trỏ trước khi chèn dấu cách
                    TextPointer startPointer = caretPosition;

                    // Chèn dấu cách vào vị trí con trỏ (không di chuyển con trỏ chuột)
                    caretPosition.InsertTextInRun(" ");

                    // Dịch chuyển con trỏ chuột ra sau dấu cách vừa chèn vào
                    _richTextBox.CaretPosition = _richTextBox.CaretPosition.GetPositionAtOffset(1, LogicalDirection.Forward);

                    // Lấy vị trí con trỏ sau khi chèn dấu cách
                    TextPointer endPointer = _richTextBox.CaretPosition;

                    // Chọn vùng từ startPointer đến endPointer (vùng vừa chèn dấu cách)
                    _richTextBox.Selection.Select(startPointer, endPointer);
                }
            }

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);

            // Lấy giá trị TextDecorations của đoạn văn bản
            var currentTextDecoration = selection.GetPropertyValue(Inline.TextDecorationsProperty);

            // Kiểm tra xem giá trị trả về có phải là UnsetValue không (chưa xác định kiểu)
            if (currentTextDecoration == null
                || currentTextDecoration.Equals(DependencyProperty.UnsetValue))
            {
                // Nếu không xác định kiểu, áp dụng định dạng Underline mặc định
                selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            }
            else
            {
                // Kiểm tra xem có TextDecoration là Underline không
                var textDecorationCollection = currentTextDecoration as TextDecorationCollection;

                if (textDecorationCollection != null)
                {
                    // Kiểm tra xem đã có Underline chưa
                    bool hasUnderline = textDecorationCollection.Any(decoration => decoration == TextDecorations.Underline[0]);

                    if (hasUnderline)
                    {
                        // Nếu đã có Underline, gỡ bỏ Underline
                        var newTextDecorations = new TextDecorationCollection(textDecorationCollection);
                        newTextDecorations.Remove(TextDecorations.Underline[0]);

                        // Áp dụng bộ sưu tập TextDecoration mới
                        selection.ApplyPropertyValue(Inline.TextDecorationsProperty, newTextDecorations);
                    }
                    else
                    {
                        // Nếu chưa có Underline, thêm Underline vào
                        var newTextDecorations = new TextDecorationCollection(textDecorationCollection);
                        newTextDecorations.Add(TextDecorations.Underline[0]);

                        // Áp dụng bộ sưu tập TextDecoration mới
                        selection.ApplyPropertyValue(Inline.TextDecorationsProperty, newTextDecorations);
                    }
                }
                else
                {
                    // Nếu không phải TextDecorationCollection, áp dụng TextDecoration Strikethrough một cách trực tiếp
                    selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Strikethrough);
                }
            }

            // Đảm bảo RichTextBox có focus để con trỏ luôn hiển thị
            _richTextBox.Focus();
        }

        // Thay đổi kiểu gạch ngang văn bản
        public void ToggleStrikethrough()
        {
            if (_richTextBox.Selection.IsEmpty)
            {
                // Nếu không có vùng chọn, áp dụng font size tại vị trí con trỏ
                TextPointer caretPosition = _richTextBox.CaretPosition;

                if (caretPosition != null && caretPosition.IsAtInsertionPosition)
                {
                    // Lưu lại vị trí con trỏ trước khi chèn dấu cách
                    TextPointer startPointer = caretPosition;

                    // Chèn dấu cách vào vị trí con trỏ (không di chuyển con trỏ chuột)
                    caretPosition.InsertTextInRun(" ");

                    // Dịch chuyển con trỏ chuột ra sau dấu cách vừa chèn vào
                    _richTextBox.CaretPosition = _richTextBox.CaretPosition.GetPositionAtOffset(1, LogicalDirection.Forward);

                    // Lấy vị trí con trỏ sau khi chèn dấu cách
                    TextPointer endPointer = _richTextBox.CaretPosition;

                    // Chọn vùng từ startPointer đến endPointer (vùng vừa chèn dấu cách)
                    _richTextBox.Selection.Select(startPointer, endPointer);
                }
            }

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            var currentTextDecoration = selection.GetPropertyValue(Inline.TextDecorationsProperty);

            // Kiểm tra xem giá trị trả về có phải là UnsetValue không (chưa xác định kiểu)
            if (currentTextDecoration == null
               || currentTextDecoration.Equals(DependencyProperty.UnsetValue))
            {
                // Nếu không xác định kiểu, áp dụng định dạng Strikethrough mặc định
                selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Strikethrough);
            }
            else
            {
                // Kiểm tra xem giá trị lấy được có phải là TextDecorationCollection hay không
                var textDecorationCollection = currentTextDecoration as TextDecorationCollection;

                if (textDecorationCollection != null)
                {
                    // Kiểm tra xem đã có Strikethrough chưa
                    bool hasStrikethrough = textDecorationCollection.Any(decoration => decoration == TextDecorations.Strikethrough[0]);

                    if (hasStrikethrough)
                    {
                        // Nếu đã có Strikethrough, gỡ bỏ Strikethrough
                        var newTextDecorations = new TextDecorationCollection(textDecorationCollection);
                        newTextDecorations.Remove(TextDecorations.Strikethrough[0]);

                        // Áp dụng bộ sưu tập TextDecoration mới
                        selection.ApplyPropertyValue(Inline.TextDecorationsProperty, newTextDecorations);
                    }
                    else
                    {
                        // Nếu chưa có Strikethrough, thêm Strikethrough vào
                        var newTextDecorations = new TextDecorationCollection(textDecorationCollection);
                        newTextDecorations.Add(TextDecorations.Strikethrough[0]);

                        // Áp dụng bộ sưu tập TextDecoration mới
                        selection.ApplyPropertyValue(Inline.TextDecorationsProperty, newTextDecorations);
                    }
                }
                else
                {
                    // Nếu không phải TextDecorationCollection, áp dụng TextDecoration Strikethrough một cách trực tiếp
                    selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Strikethrough);
                }
            }

            // Đảm bảo RichTextBox có focus để con trỏ luôn hiển thị
            _richTextBox.Focus();
        }

        // Chuyển văn bản thành chỉ số dưới (Subscript)
        public void ToggleSubscript()
        {
            if (_richTextBox.Selection.IsEmpty)
            {
                // Nếu không có vùng chọn, áp dụng font size tại vị trí con trỏ
                TextPointer caretPosition = _richTextBox.CaretPosition;

                if (caretPosition != null && caretPosition.IsAtInsertionPosition)
                {
                    // Lưu lại vị trí con trỏ trước khi chèn dấu cách
                    TextPointer startPointer = caretPosition;

                    // Chèn dấu cách vào vị trí con trỏ (không di chuyển con trỏ chuột)
                    caretPosition.InsertTextInRun(" ");

                    // Dịch chuyển con trỏ chuột ra sau dấu cách vừa chèn vào
                    _richTextBox.CaretPosition = _richTextBox.CaretPosition.GetPositionAtOffset(1, LogicalDirection.Forward);

                    // Lấy vị trí con trỏ sau khi chèn dấu cách
                    TextPointer endPointer = _richTextBox.CaretPosition;

                    // Chọn vùng từ startPointer đến endPointer (vùng vừa chèn dấu cách)
                    _richTextBox.Selection.Select(startPointer, endPointer);
                }
            }

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

            // Đảm bảo RichTextBox có focus để con trỏ luôn hiển thị
            _richTextBox.Focus();
        }


        // Chuyển văn bản thành chỉ số trên (Superscript)
        public void ToggleSuperscript()
        {
            if (_richTextBox.Selection.IsEmpty)
            {
                // Nếu không có vùng chọn, áp dụng font size tại vị trí con trỏ
                TextPointer caretPosition = _richTextBox.CaretPosition;

                if (caretPosition != null && caretPosition.IsAtInsertionPosition)
                {
                    // Lưu lại vị trí con trỏ trước khi chèn dấu cách
                    TextPointer startPointer = caretPosition;

                    // Chèn dấu cách vào vị trí con trỏ (không di chuyển con trỏ chuột)
                    caretPosition.InsertTextInRun(" ");

                    // Dịch chuyển con trỏ chuột ra sau dấu cách vừa chèn vào
                    _richTextBox.CaretPosition = _richTextBox.CaretPosition.GetPositionAtOffset(1, LogicalDirection.Forward);

                    // Lấy vị trí con trỏ sau khi chèn dấu cách
                    TextPointer endPointer = _richTextBox.CaretPosition;

                    // Chọn vùng từ startPointer đến endPointer (vùng vừa chèn dấu cách)
                    _richTextBox.Selection.Select(startPointer, endPointer);
                }
            }

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
            catch (Exception ex)
            {
                // Log hoặc xử lý lỗi (ví dụ: hiển thị thông báo lỗi)
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

            // Đảm bảo RichTextBox có focus để con trỏ luôn hiển thị
            _richTextBox.Focus();
        }

        private Dictionary<string, Tuple<DependencyProperty, object>> SaveExistingFormat(TextRange formatRange)
        {
            Dictionary<string, Tuple<DependencyProperty, object>> existingFormats = new Dictionary<string, Tuple<DependencyProperty, object>>();

            // Lưu lại các định dạng đã có
            existingFormats["existingFontFamily"] = new Tuple<DependencyProperty, object>(TextElement.FontFamilyProperty, formatRange.GetPropertyValue(TextElement.FontFamilyProperty));          
            existingFormats["existingFontColor"] = new Tuple<DependencyProperty, object>(TextElement.ForegroundProperty, formatRange.GetPropertyValue(TextElement.ForegroundProperty));
            existingFormats["existingFontHighlight"] = new Tuple<DependencyProperty, object>(TextElement.BackgroundProperty, formatRange.GetPropertyValue(TextElement.BackgroundProperty));
            existingFormats["existingBold"] = new Tuple<DependencyProperty, object>(TextElement.FontWeightProperty, formatRange.GetPropertyValue(TextElement.FontWeightProperty));
            existingFormats["existingItalic"] = new Tuple<DependencyProperty, object>(TextElement.FontStyleProperty, formatRange.GetPropertyValue(TextElement.FontStyleProperty));
            existingFormats["existingUnderline"] = new Tuple<DependencyProperty, object>(Inline.TextDecorationsProperty, formatRange.GetPropertyValue(Inline.TextDecorationsProperty));
            existingFormats["existingSuScript"] = new Tuple<DependencyProperty, object>(Inline.BaselineAlignmentProperty, formatRange.GetPropertyValue(Inline.BaselineAlignmentProperty));

            return existingFormats;
        }

        private void ReApplyExistingFormats(Dictionary<string, Tuple<DependencyProperty, object>> existingFormats, TextRange formatRange)
        {
            foreach(string formatType in  existingFormats.Keys)
            {
                if(existingFormats.ContainsKey(formatType))
                {
                    formatRange.ApplyPropertyValue(existingFormats[formatType].Item1, existingFormats[formatType].Item2);
                }
            }
        }

        private void MakeGrowOrShrink(TextRange formatRange, bool isGrow, bool isShrink, out double newSize)
        {
            newSize = 11;

            if (isGrow)
            {
                var currentFontSize = (double)formatRange.GetPropertyValue(TextElement.FontSizeProperty);
                int currentIndex = Array.FindIndex(fontSize, size => size >= currentFontSize);

                if (currentIndex >= 0 && currentIndex < fontSize.Length - 1)
                {
                    newSize = fontSize[currentIndex + 1];
                    formatRange.ApplyPropertyValue(TextElement.FontSizeProperty, newSize);
                }
            }
            else
            {
                var currentFontSize = (double)formatRange.GetPropertyValue(TextElement.FontSizeProperty);
                int currentIndex = Array.FindIndex(fontSize, size => size >= currentFontSize);

                if (currentIndex > 0 && currentIndex <= fontSize.Length - 1)
                {
                    newSize = fontSize[currentIndex - 1];
                    formatRange.ApplyPropertyValue(TextElement.FontSizeProperty, newSize);
                }
            }                 
        }

        // Tăng kích thước font
        public void GrowFont()
        {
            if (_richTextBox.Selection.IsEmpty)
            {
                TextPointer startPointer = _richTextBox.CaretPosition;
                startPointer.InsertTextInRun(" ");
                _richTextBox.CaretPosition = _richTextBox.CaretPosition.GetPositionAtOffset(1, LogicalDirection.Forward);
                TextPointer endPointer = _richTextBox.CaretPosition;
                _richTextBox.Selection.Select(startPointer, endPointer);
            }

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            var start = selection.Start;
            var end = selection.End;

            double newSize;

            var isVariousFormat = selection.GetPropertyValue(TextElement.FontSizeProperty);
            if(isVariousFormat == null || isVariousFormat == DependencyProperty.UnsetValue)
            {
                while (start.CompareTo(end) < 0)
                {
                    var next = start.GetPositionAtOffset(1, LogicalDirection.Forward);
                    if (next == null)
                        break;

                    var formatRange = new TextRange(start, next);

                    Dictionary<string, Tuple<DependencyProperty, object>> existingFormats = SaveExistingFormat(formatRange);

                    MakeGrowOrShrink(formatRange, true, false, out newSize);

                    ReApplyExistingFormats(existingFormats, formatRange);
                    start = next;
                }
            }  
            else
            {
                MakeGrowOrShrink(selection, true, false, out newSize);
                FontSizeChanged?.Invoke(newSize);
            }    
            _richTextBox.Focus();
        }

        // Giảm kích thước font
        public void ShrinkFont()
        {
            if (_richTextBox.Selection.IsEmpty)
            {
                TextPointer startPointer = _richTextBox.CaretPosition;
                startPointer.InsertTextInRun(" ");
                _richTextBox.CaretPosition = _richTextBox.CaretPosition.GetPositionAtOffset(1, LogicalDirection.Forward);
                TextPointer endPointer = _richTextBox.CaretPosition;
                _richTextBox.Selection.Select(startPointer, endPointer);
            }

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            var start = selection.Start;
            var end = selection.End;

            double newSize;

            var isVariousFormat = selection.GetPropertyValue(TextElement.FontSizeProperty);
            if (isVariousFormat == null || isVariousFormat == DependencyProperty.UnsetValue)
            {
                while (start.CompareTo(end) < 0)
                {
                    var next = start.GetPositionAtOffset(1, LogicalDirection.Forward);
                    if (next == null)
                        break;

                    var formatRange = new TextRange(start, next);

                    Dictionary<string, Tuple<DependencyProperty, object>> existingFormats = SaveExistingFormat(formatRange);

                    MakeGrowOrShrink(formatRange, false, true, out newSize);

                    ReApplyExistingFormats(existingFormats, formatRange);
                    start = next;
                }
            }
            else
            {
                MakeGrowOrShrink(selection, false, true, out newSize);
                FontSizeChanged?.Invoke(newSize);
            }
            _richTextBox.Focus();
        }
    }
}
