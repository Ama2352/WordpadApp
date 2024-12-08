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

        // Phương thức thiết lập vùng chọn cho trường hợp không có văn bản được chọn để tận dụng chức năng thao tác trên selection có sẵn của richtextbox
        public void SettingForEmptySelectionCase()
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
                SettingForEmptySelectionCase();

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
                SettingForEmptySelectionCase();

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
                SettingForEmptySelectionCase();

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));

            // Đảm bảo RichTextBox có focus để con trỏ luôn hiển thị
            _richTextBox.Focus();
        }


        // Thay đổi màu nền (highlight) văn bản
        public void ChangeHighlightColor(Color color)
        {
            if (_richTextBox.Selection.IsEmpty)
                SettingForEmptySelectionCase();

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            selection.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(color));

            // Đảm bảo RichTextBox có focus để con trỏ luôn hiển thị
            _richTextBox.Focus();

        }

        // Chuyển văn bản sang in đậm
        public void ToggleBold()
        {
            if (_richTextBox.Selection.IsEmpty)
                SettingForEmptySelectionCase();

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);

            // Lấy giá trị fontWeight của đoạn văn bản
            var currentFontWeight = selection.GetPropertyValue(TextElement.FontWeightProperty);

            // Kiểm tra xem giá trị có phải là DependencyProperty.UnsetValue hay không
            if (currentFontWeight == null
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
                SettingForEmptySelectionCase();

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

        #region Processing Functions for Grow/Shrink Font, TextDecorations(Underline/Strikethrough) 
        // Phương thức đếm số paragraph đã áp dụng định dạng cần thao tác
        private int CountFormattedParagraphs(TextRange originalFormatRange, string formatOption)
        {
            int totalFormattedParagraphs = 0;

            var paragraphs = _richTextBox.Document.Blocks.OfType<Paragraph>().ToList();
            foreach (Paragraph paragraph in paragraphs)
            {
                // Lấy TextPointer của bắt đầu và kết thúc paragraph
                TextPointer paragraphStart = paragraph.ContentStart;
                TextPointer paragraphEnd = paragraph.ContentEnd;

                // Kiểm tra xem vùng chọn có giao với đoạn văn này không
                if (originalFormatRange.Start.CompareTo(paragraphEnd) < 0 && originalFormatRange.End.CompareTo(paragraphStart) > 0)
                {
                    // Vùng chọn có phần giao với paragraph này
                    TextPointer startPointer = originalFormatRange.Start.CompareTo(paragraphStart) < 0 ? paragraphStart : originalFormatRange.Start;
                    TextPointer endPointer = originalFormatRange.End.CompareTo(paragraphEnd) > 0 ? paragraphEnd : originalFormatRange.End;

                    // Bây giờ startPointer và endPointer đại diện cho phần văn bản được chọn trong paragraph này
                    TextRange selectedInParagraph = new TextRange(startPointer, endPointer);

                    int charactersAppliedFormat = CountCharacterApplyTextDecoration(selectedInParagraph, formatOption);
                    bool isFullyApplied = true;

                    /* Nếu tổng số kí tự áp dụng định dạng == tổng số kí tự trong đoạn văn bản được chọn thì tức là
                     cả đoạn văn bản đó đã áp dụng loại định dạng cần thao tác*/
                    if (charactersAppliedFormat != selectedInParagraph.Text.Length)
                        isFullyApplied = false;

                    if (isFullyApplied)
                        totalFormattedParagraphs++;
                }
            }

            return totalFormattedParagraphs;
        }

        // Phương thức đếm số paragraph cần format
        private int CountNeededFormatParagraphs(TextRange originalFormatRange, string formatOption)
        {
            int formatParagraphs = 0;
            var paragraphs = _richTextBox.Document.Blocks.OfType<Paragraph>().ToList();
            foreach (Paragraph paragraph in paragraphs)
            {
                // Lấy TextPointer của bắt đầu và kết thúc paragraph
                TextPointer paragraphStart = paragraph.ContentStart;
                TextPointer paragraphEnd = paragraph.ContentEnd;

                // Kiểm tra xem vùng chọn có giao với đoạn văn này không
                if (originalFormatRange.Start.CompareTo(paragraphEnd) < 0 && originalFormatRange.End.CompareTo(paragraphStart) > 0)
                {
                    formatParagraphs++;
                }
            }
            return formatParagraphs;
        }

        // Phương thức định dạng cho các đoạn văn bản được chọn (Trường hợp vùng chọn có nhiều đoạn văn bản)
        private void FormatParagraphs(TextRange formatRange, string formatOption)
        {

            bool applied = true;
            int totalParagraphs = CountNeededFormatParagraphs(formatRange, formatOption);
            int totalFulliedApplyFormat = CountFormattedParagraphs(formatRange, formatOption);

            if (totalParagraphs != totalFulliedApplyFormat)
                applied = false;

            var paragraphs = _richTextBox.Document.Blocks.OfType<Paragraph>().ToList();
            foreach (Paragraph paragraph in paragraphs)
            {
                // Lấy TextPointer của bắt đầu và kết thúc paragraph
                TextPointer paragraphStart = paragraph.ContentStart;
                TextPointer paragraphEnd = paragraph.ContentEnd;

                // Kiểm tra xem vùng chọn có giao với đoạn văn này không
                if (formatRange.Start.CompareTo(paragraphEnd) < 0 && formatRange.End.CompareTo(paragraphStart) > 0)
                {
                    // Vùng chọn có phần giao với paragraph này
                    TextPointer startPointer = formatRange.Start.CompareTo(paragraphStart) < 0 ? paragraphStart : formatRange.Start;
                    TextPointer endPointer = formatRange.End.CompareTo(paragraphEnd) > 0 ? paragraphEnd : formatRange.End;

                    // Bây giờ startPointer và endPointer đại diện cho phần văn bản được chọn trong paragraph này
                    TextRange selectedInParagraph = new TextRange(startPointer, endPointer);

                    FormatTextDecorationsForParagraph(selectedInParagraph, applied, formatOption);
                }
            }
        }
        
        // Phương thức thao tác định dạng loại TextDecorations (Underline/Strikethrough) cho từng paragraph
        private void FormatTextDecorationsForParagraph(TextRange selectedParagraph, bool applied, string formatOption)
        {
            // Lấy giá trị TextDecorations của đoạn văn bản
            var currentTextDecoration = selectedParagraph.GetPropertyValue(Inline.TextDecorationsProperty);

            // Kiểm tra xem giá trị trả về có phải là UnsetValue không (chưa xác định kiểu)
            if (currentTextDecoration == null
                || currentTextDecoration.Equals(DependencyProperty.UnsetValue))
                ManipulateFormatForPropertyCases(selectedParagraph, applied, formatOption);
            else
            {
                // Trường hợp != null và UnsetValue tức là định dạng đã được áp dụng trên cả đoạn văn bản được chọn
                // bool applied = CheckIfFullyApplyTextDecorationsOrNot(selection, formatOption);
                //ManipulateUnderlineOrStrikethrough(selection, formatOption, applied);

                ManipulateFormatForPropertyCases(selectedParagraph, applied, formatOption);
            }
        }

        // Phương thức áp dụng định dạng cho các vùng văn bản trong cả trường hợp xác định được và không xác định được kiểu
        private void ManipulateFormatForPropertyCases(TextRange formatField, bool applied, string formatOption)
        {
            var start = formatField.Start;
            var end = formatField.End;

            while (start.CompareTo(end) < 0)
            {
                var next = start.GetPositionAtOffset(1, LogicalDirection.Forward);
                if (next == null)
                    break;

                var formatRange = new TextRange(start, next);

                switch (formatOption)
                {
                    case "Grow Font":
                        MakeGrowOrShrink(formatRange, true, false, out double newGrowSize);
                        break;
                    case "Shrink Font":
                        MakeGrowOrShrink(formatRange, false, true, out double newShrinkSize);
                        break;
                    case "Underline":
                        ManipulateUnderlineOrStrikethrough(formatRange, formatOption, applied);
                        break;
                    case "Strikethrough":
                        ManipulateUnderlineOrStrikethrough(formatRange, formatOption, applied);
                        break;
                    default:
                        break;
                }

                start = next;
            }
        }

        // Phương thức đếm số kí tự đã áp dụng định dạng cần thao tác trên đoạn văn bản được chọn
        private int CountCharacterApplyTextDecoration(TextRange formatField, string formatOption)
        {
            var start = formatField.Start;
            var end = formatField.End;

            int totalAppliedDecorations = 0;
            string formatText = formatField.Text;
            int formatTextIndex = 0;

            while (start.CompareTo(end) < 0)
            {
                var next = start.GetPositionAtOffset(1, LogicalDirection.Forward);
                if (next == null)
                    break;

                var formatRange = new TextRange(start, next);
                if (formatRange.Text.CompareTo(formatText[formatTextIndex].ToString()) != 0)
                {
                    start = next;
                    continue;
                }

                else
                    formatTextIndex++;

                var currentTextDecoration = formatRange.GetPropertyValue(Inline.TextDecorationsProperty);

                // Nếu kí tự đó là null hoặc unsetValue thì skip qua lần lặp tiếp theo
                if (currentTextDecoration == null
                    || currentTextDecoration.Equals(DependencyProperty.UnsetValue))
                    continue;

                var textDecorations = currentTextDecoration as TextDecorationCollection;
                var checkedDecorations = new TextDecorationCollection(textDecorations);

                switch (formatOption)
                {
                    case "Underline":
                        checkedDecorations.Remove(TextDecorations.Strikethrough[0]);
                        bool hasUnderline = checkedDecorations.Contains(TextDecorations.Underline[0]);
                        if (hasUnderline)
                            totalAppliedDecorations++;
                        break;
                    case "Strikethrough":
                        checkedDecorations.Remove(TextDecorations.Underline[0]);
                        bool hasStrikethrough = checkedDecorations.Contains(TextDecorations.Strikethrough[0]);
                        if (hasStrikethrough)
                            totalAppliedDecorations++;
                        break;
                    default:
                        break;
                }

                start = next;
            }

            return totalAppliedDecorations;
        }

        // Phương thức quyết định thực hiện thao tác áp dụng hoặc hủy bỏ Underline/Strikethrough
        // Lưu ý: Phương thức này dành cho trường hợp TextDecorationsProperty xác định (!= null và UnsetValue)
        private void ManipulateUnderlineOrStrikethrough(TextRange formatRange, string formatOption, bool applied)
        {
            // Xác định loại định dạng cần thao tác
            TextDecoration neededDecoration = null;
            TextDecoration removedDecoration = null;
            if (formatOption == "Underline")
            {
                neededDecoration = TextDecorations.Underline[0];
                removedDecoration = TextDecorations.Strikethrough[0];
            }
            else if (formatOption == "Strikethrough")
            {
                neededDecoration = TextDecorations.Strikethrough[0];
                removedDecoration = TextDecorations.Underline[0];
            }

            // Không cần xét định đạng được lấy ra nữa khi gọi hàm này là đã xét != null và UnsetValue rồi
            var currentTextDecoration = formatRange.GetPropertyValue(Inline.TextDecorationsProperty);
            var textDecorations = currentTextDecoration as TextDecorationCollection;

            TextDecorationCollection newTextDecorations = new TextDecorationCollection(textDecorations);

            if (applied)
                newTextDecorations.Remove(neededDecoration); // Xóa bỏ nếu đã có
            else
            {
                /*Có 2 trường hợp trong câu else này:
                 1. Toàn bộ đoạn văn được chọn hoàn toàn chưa được định dạng
                 2. Đoạn văn bản được định dạng vài kí tự nhưng chưa phải toàn bộ
                 --> Trong trường hợp 2: Cần xét nếu kí tự đó đã có định dạng chưa để không gán trùng định dạng nữa lên nó nữa (Tránh tích stack)
                 */
                var checkedDecorations = new TextDecorationCollection(textDecorations);
                checkedDecorations.Remove(removedDecoration);
                bool hasDecoration = checkedDecorations.Contains(neededDecoration);
                if (!hasDecoration)
                    newTextDecorations.Add(neededDecoration);
            }

            formatRange.ApplyPropertyValue(Inline.TextDecorationsProperty, newTextDecorations);
        }

        // Phương thức chỉnh grow/shrink font
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
        #endregion

        // Chuyển văn bản sang gạch chân
        public void ToggleUnderline()
        {
            if (_richTextBox.Selection.IsEmpty)
                SettingForEmptySelectionCase();

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            string formatOption = "Underline";

            FormatParagraphs(selection, formatOption);

            // Đảm bảo RichTextBox có focus để con trỏ luôn hiển thị
            _richTextBox.Focus();
        }

        // Thay đổi kiểu gạch ngang văn bản
        public void ToggleStrikethrough()
        {
            if (_richTextBox.Selection.IsEmpty)
                SettingForEmptySelectionCase();

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            string formatOption = "Strikethrough";

            FormatParagraphs(selection, formatOption);

            // Đảm bảo RichTextBox có focus để con trỏ luôn hiển thị
            _richTextBox.Focus();
        }

        // Chuyển văn bản thành chỉ số dưới (Subscript)
        public void ToggleSubscript()
        {
            if (_richTextBox.Selection.IsEmpty)
                SettingForEmptySelectionCase();

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
            catch (Exception ex)
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
                SettingForEmptySelectionCase();

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

        // Tăng kích thước font
        public void GrowFont()
        {
            if (_richTextBox.Selection.IsEmpty)
                SettingForEmptySelectionCase();

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);

            var isVariousFormat = selection.GetPropertyValue(TextElement.FontSizeProperty);
            if (isVariousFormat == null || isVariousFormat == DependencyProperty.UnsetValue)
            {
                string formatOption = "Grow Font";
                ManipulateFormatForPropertyCases(selection, false, formatOption);
            }
            else
            {
                MakeGrowOrShrink(selection, true, false, out double newSize);
                FontSizeChanged?.Invoke(newSize);
            }
            _richTextBox.Focus();
        }

        // Giảm kích thước font
        public void ShrinkFont()
        {
            if (_richTextBox.Selection.IsEmpty)
                SettingForEmptySelectionCase();

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);

            var isVariousFormat = selection.GetPropertyValue(TextElement.FontSizeProperty);
            if (isVariousFormat == null || isVariousFormat == DependencyProperty.UnsetValue)
            {
                string formatOption = "Shrink Font";
                ManipulateFormatForPropertyCases(selection, false, formatOption);
            }
            else
            {
                MakeGrowOrShrink(selection, false, true, out double newSize);
                FontSizeChanged?.Invoke(newSize);
            }
            _richTextBox.Focus();
        }
    }
}
