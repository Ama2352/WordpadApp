using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Wordpad
{
    public class EditingManager
    {
        private RichTextBox _richTextBox;
        private TextPointer _lastSearchPosition;

        // Khởi tạo với tham chiếu tới RichTextBox
        public EditingManager(RichTextBox richTextBox)
        {
            _richTextBox = richTextBox;
        }

        // Chọn tất cả văn bản
        public void SelectAllText()
        {
            _richTextBox.SelectAll();
            _richTextBox.Focus();
        }

        public TextRange FindText(string searchText, TextPointer startPointer, bool matchCase, bool matchWholeWord)
        {
            if (string.IsNullOrEmpty(searchText) || startPointer == null)
                return null;

            TextPointer currentPointer = startPointer;

            while (currentPointer != null && currentPointer.CompareTo(_richTextBox.Document.ContentEnd) < 0)
            {
                if (currentPointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string text = currentPointer.GetTextInRun(LogicalDirection.Forward);
                    StringComparison comparison = matchCase
                        ? StringComparison.CurrentCulture
                        : StringComparison.CurrentCultureIgnoreCase;

                    int index = text.IndexOf(searchText, comparison);

                    while (index != -1)
                    {
                        // Lấy vị trí bắt đầu và kết thúc
                        TextPointer start = currentPointer.GetPositionAtOffset(index);
                        TextPointer end = start.GetPositionAtOffset(searchText.Length);

                        // Kiểm tra từ nguyên vẹn nếu cần
                        if (!matchWholeWord || IsWholeWord(start, end))
                        {
                            return new TextRange(start, end);
                        }

                        // Tìm tiếp trong đoạn văn bản còn lại
                        index = text.IndexOf(searchText, index + 1, comparison);
                    }
                }
                currentPointer = currentPointer.GetNextContextPosition(LogicalDirection.Forward);
            }
            return null;
        }

        private bool IsWholeWord(TextPointer start, TextPointer end)
        {
            char? beforeChar = GetCharAtPosition(start, LogicalDirection.Backward);
            char? afterChar = GetCharAtPosition(end, LogicalDirection.Forward);

            // Kiểm tra ký tự xung quanh không phải là chữ cái hoặc số
            return (!beforeChar.HasValue || !char.IsLetterOrDigit(beforeChar.Value)) &&
                   (!afterChar.HasValue || !char.IsLetterOrDigit(afterChar.Value));
        }

        private char? GetCharAtPosition(TextPointer position, LogicalDirection direction)
        {
            TextPointerContext context = position.GetPointerContext(direction);
            if (context == TextPointerContext.Text)
            {
                string text = position.GetTextInRun(direction);
                if (!string.IsNullOrEmpty(text))
                {
                    return direction == LogicalDirection.Backward
                        ? text[text.Length - 1]  // Lấy ký tự cuối
                        : text[0];  // Lấy ký tự đầu
                }
            }
            return null;
        }

        public void FindNext(string searchText, bool matchCase, bool matchWholeWord)
        {
            TextPointer startPosition = _lastSearchPosition ?? _richTextBox.Document.ContentStart;

            TextRange foundRange = FindText(searchText, startPosition, matchCase, matchWholeWord);

            if (foundRange != null)
            {
                _richTextBox.Selection.Select(foundRange.Start, foundRange.End);
                _richTextBox.Focus();
                _lastSearchPosition = foundRange.End;
            }
            else
            {
                MessageBox.Show("Keyword not found.", "Notification", MessageBoxButton.OK, MessageBoxImage.Error);
                _lastSearchPosition = null;
            }
        }

        private Dictionary<string, Tuple<DependencyProperty, object>> SaveExistingFormat(TextRange formatRange)
        {
            Dictionary<string, Tuple<DependencyProperty, object>> existingFormats = new Dictionary<string, Tuple<DependencyProperty, object>>();

            // Lưu lại các định dạng đã có
            existingFormats["existingFontFamily"] = new Tuple<DependencyProperty, object>(TextElement.FontFamilyProperty, formatRange.GetPropertyValue(TextElement.FontFamilyProperty));
            existingFormats["existingFontSize"] = new Tuple<DependencyProperty, object>(TextElement.FontSizeProperty, formatRange.GetPropertyValue(TextElement.FontSizeProperty));
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
            foreach (string formatType in existingFormats.Keys)
            {
                if (existingFormats.ContainsKey(formatType))
                    formatRange.ApplyPropertyValue(existingFormats[formatType].Item1, existingFormats[formatType].Item2);
            }
        }

        public void Replace(string searchText, string replaceText, bool matchCase, bool matchWholeWord)
        {
            // Xác định vị trí bắt đầu tìm kiếm, có thể là vị trí con trỏ hiện tại hoặc vị trí cuối cùng đã tìm kiếm
            TextPointer startPosition = _lastSearchPosition ?? _richTextBox.Document.ContentStart;

            // Tìm kiếm văn bản đầu tiên
            TextRange foundRange = FindText(searchText, startPosition, matchCase, matchWholeWord);

            if (foundRange != null)
            {
                // Lưu lại định dạng văn bản vào dictionary trước khi thay thế văn bản
                Dictionary<string, Tuple<DependencyProperty, object>> existingFormats = SaveExistingFormat(foundRange);

                // Nếu tìm thấy, thay thế văn bản
                _richTextBox.Selection.Select(foundRange.Start, foundRange.End);

                _richTextBox.Selection.Text = replaceText; // Thay thế văn bản

                // Áp dụng lại các định dạng vốn có của văn bản được thay thế
                ReApplyExistingFormats(existingFormats, _richTextBox.Selection);

                // Cập nhật vị trí sau khi thay thế để tiếp tục tìm kiếm nếu cần
                _lastSearchPosition = foundRange.End;
            }
            else
            {
                // Nếu không tìm thấy từ cần thay thế
                MessageBox.Show("WordPad has finished searching the document.", "WordPad", MessageBoxButton.OK, MessageBoxImage.Information);
                _lastSearchPosition = null;
            }

            _richTextBox.Focus();
        }

        public void ReplaceAll(string searchText, string replaceText, bool matchCase, bool matchWholeWord)
        {
            // Xác định vị trí bắt đầu tìm kiếm
            TextPointer startPosition = _richTextBox.Document.ContentStart;

            // Tiến hành tìm kiếm và thay thế trong toàn bộ tài liệu
            TextRange foundRange = FindText(searchText, startPosition, matchCase, matchWholeWord);

            while (foundRange != null)
            {
                // Lưu lại định dạng văn bản vào dictionary trước khi thay thế văn bản
                Dictionary<string, Tuple<DependencyProperty, object>> existingFormats = SaveExistingFormat(foundRange);

                // Nếu tìm thấy, thay thế văn bản
                _richTextBox.Selection.Select(foundRange.Start, foundRange.End);

                _richTextBox.Selection.Text = replaceText; // Thay thế văn bản

                // Áp dụng lại các định dạng vốn có của văn bản được thay thế
                ReApplyExistingFormats(existingFormats, _richTextBox.Selection);

                // Tiếp tục tìm kiếm từ vị trí kết thúc của từ đã thay thế
                startPosition = foundRange.End;
                foundRange = FindText(searchText, startPosition, matchCase, matchWholeWord);
            }

            // Thông báo nếu không có từ nào được thay thế
            if (_richTextBox.Selection.Text == string.Empty)
            {
                MessageBox.Show("WordPad has finished searching the document.", "WordPad", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            _richTextBox.Focus();
        }
    }
}
