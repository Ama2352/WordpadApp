using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Wordpad
{
    internal class ParagraphManager
    {
        private RichTextBox _richTextBox;
        private Ruler ruler;
        public static float lineSpacing = 1.0f;
        //Biến đề đồng bộ line height để bằng với wordpad(đoán mò ~~)
        private float LineHeightMultiplier = 1.3f;
        //Biến lưu giá trị của margin.bottom của paragraph khi check add 10pt
        private float marginBot = 17;


        // Khởi tạo với tham chiếu tới RichTextBox
        public ParagraphManager(RichTextBox richTextBox, Ruler ruler)
        {
            _richTextBox = richTextBox;
            this.ruler = ruler;
        }

        // Căn lề trái
        public void AlignLeft()
        {
            var selection = _richTextBox.Selection;
            if (selection.IsEmpty)
            {
                // Căn lề cho toàn bộ văn bản nếu không có phần chọn
                foreach (var block in _richTextBox.Document.Blocks)
                {
                    if (block is Paragraph paragraph)
                    {
                        paragraph.TextAlignment = TextAlignment.Left;
                    }
                }
            }
            else
            {
                // Căn lề cho phần văn bản đã chọn
                Paragraph para = selection.Start.Paragraph;
                para.TextAlignment = TextAlignment.Left;
            }
        }

        // Căn lề phải
        public void AlignRight()
        {
            var selection = _richTextBox.Selection;
            if (selection.IsEmpty)
            {
                // Căn lề cho toàn bộ văn bản nếu không có phần chọn
                foreach (var block in _richTextBox.Document.Blocks)
                {
                    if (block is Paragraph paragraph)
                    {
                        paragraph.TextAlignment = TextAlignment.Right;
                    }
                }
            }
            else
            {
                // Căn lề cho phần văn bản đã chọn
                Paragraph para = selection.Start.Paragraph;
                para.TextAlignment = TextAlignment.Right;
            }
        }

        // Căn giữa
        public void AlignCenter()
        {
            var selection = _richTextBox.Selection;
            if (selection.IsEmpty)
            {
                // Căn lề cho toàn bộ văn bản nếu không có phần chọn
                foreach (var block in _richTextBox.Document.Blocks)
                {
                    if (block is Paragraph paragraph)
                    {
                        paragraph.TextAlignment = TextAlignment.Center;
                    }
                }
            }
            else
            {
                // Căn lề cho phần văn bản đã chọn
                Paragraph para = selection.Start.Paragraph;
                para.TextAlignment = TextAlignment.Center;
            }
        }

        // Căn giữa
        public void AlignJustify()
        {
            var selection = _richTextBox.Selection;
            if (selection.IsEmpty)
            {
                // Căn lề cho toàn bộ văn bản nếu không có phần chọn
                foreach (var block in _richTextBox.Document.Blocks)
                {
                    if (block is Paragraph paragraph)
                    {
                        paragraph.TextAlignment = TextAlignment.Justify;
                    }
                }
            }
            else
            {
                // Căn lề cho phần văn bản đã chọn
                Paragraph para = selection.Start.Paragraph;
                para.TextAlignment = TextAlignment.Justify;
            }
        }

        // Tăng thụt lề
        public void IncreaseIndent()
        {
            var selection = _richTextBox.Selection;

            // Nếu có văn bản được chọn
            if (!selection.IsEmpty)
            {
                var start = selection.Start;
                var end = selection.End;

                // Duyệt qua tất cả các khối trong tài liệu
                foreach (var block in _richTextBox.Document.Blocks)
                {
                    if (block is Paragraph paragraph)
                    {
                        // Kiểm tra xem Paragraph có nằm trong phạm vi lựa chọn hay không
                        if (paragraph.ContentStart.CompareTo(end) <= 0
                            && paragraph.ContentEnd.CompareTo(start) >= 0)
                        {
                            ruler.isParagraphChanged = true;
                            ruler.IncreaseIndent(48);   //Tăng 0,5 inch
                            ruler.isParagraphChanged = false;
                        }
                    }
                }
            }
            else
            {
                // Lấy đoạn văn bản tại vị trí con trỏ
                Paragraph para = selection.Start.Paragraph;
                if (para != null)
                {
                    ruler.isParagraphChanged = true;
                    ruler.IncreaseIndent(48);   //Tăng 0,5 inch
                    ruler.isParagraphChanged = false;
                }
            }

        }


        // Giảm thụt lề
        public void DecreaseIndent()
        {
            var selection = _richTextBox.Selection;
            if (!selection.IsEmpty)
            {
                var start = selection.Start;
                var end = selection.End;

                foreach (Block block in _richTextBox.Document.Blocks)
                {
                    if (block is Paragraph paragraph)
                    {
                        if (paragraph.ContentStart.CompareTo(end) <= 0
                            && paragraph.ContentEnd.CompareTo(start) >= 0
                            && paragraph.TextIndent >= 20)
                        {
                            ruler.isParagraphChanged = true;
                            ruler.DecreaseIndent(48);   //Tăng 0,5 inch
                            ruler.isParagraphChanged = false;
                        }
                    }
                }
            }
            else
            {
                Paragraph para = selection.Start.Paragraph;
                if (para != null)
                {
                    ruler.isParagraphChanged = true;
                    ruler.DecreaseIndent(48);   //Tăng 0,5 inch
                    ruler.isParagraphChanged = false;
                }
            }
        }


        // Hàm Set Line Spacing
        // Hàm Set Line Spacing
        public void SetLineSpacingWithSpacingAfterParagraphs(float lineSpacing, bool addSpacingAfterParagraphs = false)
        {
            // Kiểm tra giá trị lineSpacing có hợp lệ không (chỉ cho phép các giá trị 1.0, 1.25, 1.5, 2.0)
            if (lineSpacing != 1.0f && lineSpacing != 1.25f && lineSpacing != 1.5f && lineSpacing != 2.0f)
            {
                throw new ArgumentException("Invalid line spacing value. Valid values are 1.0, 1.25, 1.5, and 2.0.");
            }

            // Lấy đối tượng Selection từ RichTextBox để xem có đoạn văn nào được chọn không
            var selection = _richTextBox.Selection;

            // Kiểm tra nếu không có đoạn văn nào được chọn (selection.IsEmpty)
            if (selection.IsEmpty)
            {
                // Nếu không có phần văn bản nào được chọn, chúng ta chỉ xử lý đoạn văn tại vị trí con trỏ
                Paragraph para = selection.Start.Paragraph;

                // Thiết lập khoảng cách dòng cho đoạn văn tại vị trí con trỏ
                para.LineHeight = para.FontSize * lineSpacing * LineHeightMultiplier;

                // Nếu addSpacingAfterParagraphs == true, thêm 10pt khoảng cách dưới đoạn văn
                if (addSpacingAfterParagraphs)
                {
                    // Đặt Margin.Bottom (khoảng cách dưới đoạn văn) thêm 10pt
                    //magin.Bottom = marginBot + lineheight --> khi điều chỉnh lineheight thì margin cũng sẽ bị điều chỉnh --> spacing đúng
                    para.Margin = new Thickness(para.Margin.Left, para.Margin.Top, para.Margin.Right,
                        marginBot + para.LineHeight * 0.05);
                }
                else
                {
                    //Nếu ko check (hoặc unchecked) thì trả lại margin.bottom = 1
                    para.Margin = new Thickness(para.Margin.Left, para.Margin.Top, para.Margin.Right, 1);
                }
            }
            else
            {
                // Nếu có phần văn bản được chọn, ta sẽ lấy tất cả các đoạn văn trong vùng chọn
                var startPosition = selection.Start;
                var endPosition = selection.End;

                // Tạo danh sách để chứa các đoạn văn trong vùng chọn
                var selectedParagraphs = new List<Paragraph>();

                // Lặp qua các đoạn văn trong tài liệu
                foreach (var block in _richTextBox.Document.Blocks)
                {
                    if (block is Paragraph paragraph)
                    {
                        // Kiểm tra nếu đoạn văn nằm trong vùng chọn
                        if (IsParagraphInSelection(paragraph, startPosition, endPosition))
                        {
                            selectedParagraphs.Add(paragraph);
                        }
                    }
                }

                // Áp dụng line spacing cho các đoạn văn trong vùng chọn
                foreach (var selectedParagraph in selectedParagraphs)
                {
                    selectedParagraph.LineHeight = selectedParagraph.FontSize * lineSpacing * LineHeightMultiplier;

                    // Nếu addSpacingAfterParagraphs == true, thêm 10pt khoảng cách dưới đoạn văn
                    if (addSpacingAfterParagraphs)
                    {
                        selectedParagraph.Margin = new Thickness(selectedParagraph.Margin.Left,
                            selectedParagraph.Margin.Top, selectedParagraph.Margin.Right, marginBot + selectedParagraph.LineHeight * 0.05);
                    }
                    else
                    {
                        selectedParagraph.Margin = new Thickness(selectedParagraph.Margin.Left,
                            selectedParagraph.Margin.Top, selectedParagraph.Margin.Right, 1);
                    }
                }
            }

        }


        // Phương thức kiểm tra xem một đoạn văn có nằm trong vùng chọn hay không
        private bool IsParagraphInSelection(Paragraph paragraph, TextPointer startPosition, TextPointer endPosition)
        {
            // Kiểm tra nếu đoạn văn có chứa phần văn bản bắt đầu và kết thúc của vùng chọn
            var paragraphStart = paragraph.ElementStart;
            var paragraphEnd = paragraph.ElementEnd;

            return (startPosition.CompareTo(paragraphEnd) <= 0) && (endPosition.CompareTo(paragraphStart) >= 0);
        }
    }

}
