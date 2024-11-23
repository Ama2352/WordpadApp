using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Linq;

namespace WordPad
{
    public class EditingManager
    {
        private RichTextBox _richTextBox;
        private int _currentFindIndex = 0;  // Chỉ mục tìm kiếm hiện tại

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

        // Hàm để xóa bỏ màu nền của các từ đã được làm nổi bật trước đó
        public void ClearPreviousHighlight()
        {
            // Đặt lại màu nền về không màu (mặc định)
            TextRange selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);
        }

        // Hàm tìm kiếm
        public void FindText(string searchText, bool matchCase, bool matchWholeWord)
        {
            // Dọn dẹp màu nền của các từ trước đó (nếu có)
            ClearPreviousHighlight();

            // Tìm kiếm lần lượt từ startIndex
            int startIndex = _currentFindIndex;
            StringComparison comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            // Tìm kiếm lần lượt từ startIndex
            string text = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd).Text;
            while (startIndex < text.Length)
            {
                int foundIndex = FindNextMatch(text, searchText, startIndex, comparison, matchWholeWord);

                if (foundIndex != -1)
                {
                    TextPointer startPointer = _richTextBox.Document.ContentStart.GetPositionAtOffset(foundIndex);
                    TextPointer endPointer = startPointer.GetPositionAtOffset(searchText.Length);
                    _richTextBox.Selection.Select(startPointer, endPointer); // Chọn từ khóa
                    _richTextBox.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Aquamarine);  // Thay đổi màu nền (nổi bật)

                    _currentFindIndex = foundIndex + searchText.Length;  // Cập nhật chỉ mục để tìm tiếp
                    break;
                }
                else
                {
                    MessageBox.Show("Wordpad has finished searching the document.");
                    _currentFindIndex = 0;  // Nếu không tìm thấy, reset lại chỉ mục
                    break;
                }
            }
            if (startIndex >= text.Length)
            {
                MessageBox.Show("Wordpad has finished searching the document.");
                _currentFindIndex = 0; // Nếu đã tìm hết văn bản thì reset chỉ mục
            }
        }

        // Tìm và thay thế lần lượt
        public void FindAndReplace(string searchText, string replaceText, bool matchCase, bool matchWholeWord)
        {
            // Dọn dẹp màu nền của các từ trước đó (nếu có)
            ClearPreviousHighlight();

            int startIndex = _currentFindIndex;
            StringComparison comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            string text = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd).Text;
            while (startIndex < text.Length)
            {
                int foundIndex = FindNextMatch(text, searchText, startIndex, comparison, matchWholeWord);

                if (foundIndex != -1)
                {
                    TextPointer startPointer = _richTextBox.Document.ContentStart.GetPositionAtOffset(foundIndex);
                    TextPointer endPointer = startPointer.GetPositionAtOffset(searchText.Length);
                    _richTextBox.Selection.Select(startPointer, endPointer); // Chọn từ khóa
                    _richTextBox.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Aquamarine);  // Thay đổi màu nền (nổi bật)

                    _richTextBox.Selection.Text = replaceText;  // Thay thế từ khóa
                    _currentFindIndex = foundIndex + replaceText.Length;  // Cập nhật chỉ mục để tìm tiếp
                    break;
                }
                else
                {
                    MessageBox.Show("Wordpad has finished replacing the document.");
                    _currentFindIndex = 0;  // Nếu không tìm thấy, reset lại chỉ mục
                    break;
                }
            }
            if (startIndex >= text.Length)
            {
                MessageBox.Show("Wordpad has finished searching the document.");
                _currentFindIndex = 0; // Nếu đã tìm hết văn bản thì reset chỉ mục
            }
        }

        // Thay thế tất cả các từ
        public void ReplaceAll(string searchText, string replaceText, bool matchCase, bool matchWholeWord)
        {
            int startIndex = 0;
            StringComparison comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            string text = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd).Text;
            while (startIndex < text.Length)
            {
                int foundIndex = FindNextMatch(text, searchText, startIndex, comparison, matchWholeWord);

                if (foundIndex != -1)
                {
                    TextPointer startPointer = _richTextBox.Document.ContentStart.GetPositionAtOffset(foundIndex);
                    TextPointer endPointer = startPointer.GetPositionAtOffset(searchText.Length);
                    _richTextBox.Selection.Select(startPointer, endPointer);  // Chọn từ khóa
                    _richTextBox.Selection.Text = replaceText;  // Thay thế từ khóa
                    startIndex = foundIndex + replaceText.Length;  // Cập nhật chỉ mục để tiếp tục thay thế
                }
                else
                {
                    MessageBox.Show("Wordpad has finished replacing the document.");
                    _currentFindIndex = 0;
                    break;
                }
            }
            if (startIndex >= text.Length)
            {
                MessageBox.Show("Wordpad has finished searching the document.");
                _currentFindIndex = 0; // Nếu đã tìm hết văn bản thì reset chỉ mục
            }
        }

        // Hàm tìm kiếm từ khóa trong văn bản
        private int FindNextMatch(string text, string searchText, int startIndex, StringComparison comparison, bool matchWholeWord)
        {
            int foundIndex = -1;

            for (int i = startIndex; i < text.Length - searchText.Length + 1; i++)
            {
                bool match = string.Compare(text.Substring(i, searchText.Length), searchText, comparison) == 0;

                if (match)
                {
                    if (matchWholeWord)
                    {
                        bool isWordBoundaryBefore = i == 0 || !char.IsLetterOrDigit(text[i - 1]);
                        bool isWordBoundaryAfter = i + searchText.Length == text.Length || !char.IsLetterOrDigit(text[i + searchText.Length]);

                        if (isWordBoundaryBefore && isWordBoundaryAfter)
                        {
                            foundIndex = i;
                            break;
                        }
                    }
                    else
                    {
                        foundIndex = i;
                        break;
                    }
                }
            }

            return foundIndex;
        }
    }
}
