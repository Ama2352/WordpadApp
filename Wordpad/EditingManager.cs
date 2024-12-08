using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Linq;
using System.Text.RegularExpressions;

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
                MessageBox.Show("Không tìm thấy từ khóa.", "Thông báo");
                _lastSearchPosition = null;
            }
        }
    }
}
