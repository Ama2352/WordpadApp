﻿using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        private List _formattedList;

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
                // Căn lề cho đoạn văn bản hiện tại nếu không có vùng chọn
                Paragraph para = selection.Start.Paragraph;
                if (para != null)
                    para.TextAlignment = TextAlignment.Left;
            }
            else
            {
                // Căn lề cho tất cả các đoạn văn bản trong vùng chọn
                TextPointer start = selection.Start;
                TextPointer end = selection.End;

                while (start != null && start.CompareTo(end) < 0)
                {
                    Paragraph para = start.Paragraph;
                    if (para != null)
                        para.TextAlignment = TextAlignment.Left;
                    start = start.GetNextContextPosition(LogicalDirection.Forward);
                }
            }
            _richTextBox.Focus();
        }


        // Căn lề phải
        public void AlignRight()
        {
            var selection = _richTextBox.Selection;
            if (selection.IsEmpty)
            {
                // Căn lề cho phần văn bản đã chọn
                Paragraph para = selection.Start.Paragraph;
                para.TextAlignment = TextAlignment.Right;
            }
            else
            {
                TextPointer start = selection.Start;
                TextPointer end = selection.End;
                while(start != null && start.CompareTo(end) < 0)
                {
                    Paragraph para = start.Paragraph;
                    if(para != null)
                        para.TextAlignment = TextAlignment.Right;
                    start = start.GetNextContextPosition(LogicalDirection.Forward);
                }    
            }
            _richTextBox.Focus();
        }

        // Căn giữa
        public void AlignCenter()
        {
            var selection = _richTextBox.Selection;
            if (selection.IsEmpty)
            {
                // Căn lề cho phần văn bản đã chọn
                Paragraph para = selection.Start.Paragraph;
                para.TextAlignment = TextAlignment.Center;
            }
            else
            {
                TextPointer start = selection.Start;
                TextPointer end = selection.End;
                while (start != null && start.CompareTo(end) < 0)
                {
                    Paragraph para = start.Paragraph;
                    if (para != null)
                        para.TextAlignment = TextAlignment.Center;
                    start = start.GetNextContextPosition(LogicalDirection.Forward);
                }
            }
            _richTextBox.Focus();
        }

        // Căn giữa
        public void AlignJustify()
        {
            var selection = _richTextBox.Selection;
            if (selection.IsEmpty)
            {
                // Căn lề cho phần văn bản đã chọn
                Paragraph para = selection.Start.Paragraph;
                para.TextAlignment = TextAlignment.Justify;
            }
            else
            {
                TextPointer start = selection.Start;
                TextPointer end = selection.End;
                while (start != null && start.CompareTo(end) < 0)
                {
                    Paragraph para = start.Paragraph;
                    if (para != null)
                        para.TextAlignment = TextAlignment.Justify;
                    start = start.GetNextContextPosition(LogicalDirection.Forward);
                }
            }
            _richTextBox.Focus();
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
                            //Length = 48 ~ 0,5 inch
                            ruler.IncreaseIndentSimplified(paragraph, 48);
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
                    ruler.IncreaseIndentSimplified(para, 48);
                }
            }
            _richTextBox.Focus();
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
                            && paragraph.ContentEnd.CompareTo(start) >= 0)
                        {
                            ruler.DecreaseIndentSimplified(paragraph, 48);
                        }
                    }
                }
            }
            else
            {
                Paragraph para = selection.Start.Paragraph;
                if (para != null)
                {
                    ruler.DecreaseIndentSimplified(para, 48);
                }
            }
            _richTextBox.Focus();
        }

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
            _richTextBox.Focus();
        }


        // Phương thức kiểm tra xem một đoạn văn có nằm trong vùng chọn hay không
        private bool IsParagraphInSelection(Paragraph paragraph, TextPointer startPosition, TextPointer endPosition)
        {
            // Kiểm tra nếu đoạn văn có chứa phần văn bản bắt đầu và kết thúc của vùng chọn
            var paragraphStart = paragraph.ElementStart;
            var paragraphEnd = paragraph.ElementEnd;

            return (startPosition.CompareTo(paragraphEnd) <= 0) && (endPosition.CompareTo(paragraphStart) >= 0);
        }

        //Các hàm truyền dữ liệu từ paragraph window

        public void SetIndentation(double leftIndent, double rightIndent, double firstLineIndent)
        {
            var selection = _richTextBox.Selection;
            if (!selection.IsEmpty)
            {
                foreach (var block in _richTextBox.Document.Blocks)
                {
                    if (block is Paragraph paragraph)
                    {
                        paragraph.Margin = new Thickness(leftIndent, 0, rightIndent, 0);
                        paragraph.TextIndent = firstLineIndent;
                    }
                }
            }
            else
            {
                Paragraph para = selection.Start.Paragraph;
                if (para != null)
                {
                    para.Margin = new Thickness(leftIndent,0, rightIndent, 0);
                    para.TextIndent = firstLineIndent;
                }
            }
        }

        public void SetAlignment(TextAlignment alignment)
        {
            var selection = _richTextBox.Selection;
            if (!selection.IsEmpty)
            {
                foreach (var block in _richTextBox.Document.Blocks)
                {
                    if (block is Paragraph paragraph)
                    {
                        paragraph.TextAlignment = alignment;
                    }
                }
            }
            else
            {
                Paragraph para = selection.Start.Paragraph;
                if (para != null)
                {
                    para.TextAlignment = alignment;
                }
            }
        }

        #region BulletStyles Function
        public void ApplyBulletStyles(string styleOption)
        {
            TextMarkerStyle bulletStyle = new TextMarkerStyle();
            switch (styleOption)
            {
                case "Bullet":
                    bulletStyle = TextMarkerStyle.Disc;
                    break;
                case "Numbering":
                    bulletStyle = TextMarkerStyle.Decimal;
                    break;
                case "Alphabet - Lower case":
                    bulletStyle = TextMarkerStyle.LowerLatin;
                    break;
                case "Alphabet - Upper case":
                    bulletStyle = TextMarkerStyle.UpperLatin;
                    break;
                case "Roman Numeral - Lower case":
                    bulletStyle = TextMarkerStyle.LowerRoman;
                    break;
                case "Roman Numeral - Upper case":
                    bulletStyle = TextMarkerStyle.UpperRoman;
                    break;
                default:
                    bulletStyle = TextMarkerStyle.None;
                    break;

            }

            var selection = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            if (selection.IsEmpty)
            {
                if (selection.Start.Paragraph != null)
                {
                    var selectedParagraph = selection.Start.Paragraph;
                    selection = new TextRange(selectedParagraph.ContentStart, selectedParagraph.ContentEnd);
                    if (selection.Text == "") { }
                }
                else if (selection.Start.Parent is ListItem selectedListItem)
                {
                    selection = new TextRange(selectedListItem.ContentStart, selectedListItem.ContentEnd);
                }
            }
            FormatBlocks(selection, bulletStyle);
        }

        private void FormatBlocks(TextRange formatRange, TextMarkerStyle bulletStyle)
        {
            CountParagraphsAndLists(formatRange, out int countParagraphs, out int countLists, out int countListItems, out TextMarkerStyle currentBulletStyle);
            if (countLists == 0 && countParagraphs != 0)
            {
                ApplyBulletStyleForOnlyParagraphsCase(formatRange, bulletStyle);
            }
            else if (countLists != 0 && countParagraphs != 0)
            {
                bool isReFormattedParagraphCase = false;
                ApplyBulletStyleForMixedParagraphsAndListsCase(formatRange, bulletStyle, isReFormattedParagraphCase);
            }
            else if (countLists != 0 && countParagraphs == 0)
            {
               FormatEachListsInSelectedRange(formatRange, bulletStyle);
            }
            _richTextBox.Focus();
        }


        private void CountParagraphsAndLists(TextRange formatRange, out int countParagraphs, out int countLists, out int countListItems, out TextMarkerStyle currentBulletStyle)
        {
            countParagraphs = 0;
            countLists = 0;
            countListItems = 0;
            currentBulletStyle = TextMarkerStyle.None;

            bool isListItemInList = false;

            foreach (Block block in _richTextBox.Document.Blocks)
            {
                if (block is Paragraph paragraph)
                {
                    // Xử lý với Paragraph
                    TextPointer paragraphStart = paragraph.ContentStart;
                    TextPointer paragraphEnd = paragraph.ContentEnd;

                    if (formatRange.Start.CompareTo(paragraphEnd) < 0 && formatRange.End.CompareTo(paragraphStart) > 0)
                    {
                        TextRange selectedInParagraph = new TextRange(paragraphStart, paragraphEnd);

                        if (selectedInParagraph.Text != "")
                            countParagraphs++;
                    }
                }
                else if (block is List list)
                {
                    isListItemInList = false;
                    // Xử lý với List
                    foreach (ListItem listItem in list.ListItems)
                    {
                        TextPointer listItemStart = listItem.ContentStart;
                        TextPointer listItemEnd = listItem.ContentEnd;

                        if (formatRange.Start.CompareTo(listItemEnd) < 0 && formatRange.End.CompareTo(listItemStart) > 0)
                        {
                            if (currentBulletStyle == TextMarkerStyle.None)
                                currentBulletStyle = list.MarkerStyle;

                            TextRange selectedInListItem = new TextRange(listItemStart, listItemEnd);

                            if (selectedInListItem.Text != "")
                            {
                                countListItems++;
                                isListItemInList = true;
                            }
                        }
                    }
                    if (isListItemInList)
                        countLists++;
                }
            }
        }

        private void ApplyBulletStyleForOnlyParagraphsCase(TextRange formatRange, TextMarkerStyle bulletStyle)
        {
            TextPointer formatRangeStart = null;
            bool isSetFormatRangeStart = false;

            List bulletStyleList = new List();
            bulletStyleList.MarkerStyle = bulletStyle;

            // Sao chép danh sách Blocks trước khi duyệt
            List<Block> copiedBlocks = _richTextBox.Document.Blocks.ToList();

            foreach (Block block in copiedBlocks)
            {
                if (block is Paragraph paragraph)
                {
                    // Xử lý với Paragraph
                    TextPointer paragraphStart = paragraph.ContentStart;
                    TextPointer paragraphEnd = paragraph.ContentEnd;

                    if (formatRange.Start.CompareTo(paragraphEnd) < 0 && formatRange.End.CompareTo(paragraphStart) > 0)
                    {
                        TextRange selectedInParagraph = new TextRange(paragraphStart, paragraphEnd);

                        if (selectedInParagraph.Text != "")
                        {
                            SettingBulletStyleForParagraph(paragraph, bulletStyleList);
                            if (!isSetFormatRangeStart)
                            {
                                formatRangeStart = paragraphStart;
                                isSetFormatRangeStart = true;
                            }
                        }
                    }
                }
            }

            _formattedList = bulletStyleList;

            bool isReFormattedParagraphCase = false;
            PasteCutListBlockAtTextPointer(formatRangeStart, isReFormattedParagraphCase);
            copiedBlocks = null;
        }

        private void SettingBulletStyleForParagraph(Block block, List bulletStyleList)
        {
            if (block is Paragraph para)
            {
                if (para.Parent is ListItem currentListItem)
                {
                    if (currentListItem.Parent is List currentList)
                    {
                        if (currentList.MarkerStyle.CompareTo(bulletStyleList.MarkerStyle) == 0)
                            return;
                        else
                        {
                            currentList.ListItems.Remove(currentListItem);
                            ListItem newItem = new ListItem(para);
                            bulletStyleList.ListItems.Add(newItem);
                        }
                    }

                }
                else
                {
                    ListItem newItem = new ListItem(para);
                    bulletStyleList.ListItems.Add(newItem);
                }
            }
        }

        private void ApplyBulletStyleForMixedParagraphsAndListsCase(TextRange formatRange, TextMarkerStyle bulletStyle, bool isReFormattedParagraphCase)
        {
            TextPointer formatRangeStart = null;
            bool isSetFormatRangeStart = false;

            List bulletStyleList = new List();
            bulletStyleList.MarkerStyle = bulletStyle;

            List<Paragraph> newParagraphs = new List<Paragraph>();

            // Khởi tạo một Dictionary để lưu trữ List cùng với các ListItem cần xóa
            Dictionary<List, List<ListItem>> listItemsToRemove = new Dictionary<List, List<ListItem>>();

            // Sao chép danh sách Blocks trước khi duyệt
            List<Block> copiedBlocks = _richTextBox.Document.Blocks.ToList();

            foreach (Block block in copiedBlocks)
            {
                if (block is Paragraph paragraph)
                {
                    // Xử lý với Paragraph
                    TextPointer paragraphStart = paragraph.ContentStart;
                    TextPointer paragraphEnd = paragraph.ContentEnd;

                    if (formatRange.Start.CompareTo(paragraphEnd) < 0 && formatRange.End.CompareTo(paragraphStart) > 0)
                    {
                        TextRange selectedInParagraph = new TextRange(paragraphStart, paragraphEnd);

                        if (selectedInParagraph.Text != "")
                        {
                            newParagraphs.Add(paragraph);

                            if (!isSetFormatRangeStart)
                            {
                                formatRangeStart = paragraphStart;
                                isSetFormatRangeStart = true;
                            }
                        }
                    }
                }
                else if (block is List list)
                {
                    // Khởi tạo một danh sách tạm để lưu các ListItem cần xóa cho List hiện tại
                    List<ListItem> itemsToRemove = new List<ListItem>();

                    // Xử lý với List
                    foreach (ListItem listItem in list.ListItems)
                    {
                        TextPointer listItemStart = listItem.ContentStart;
                        TextPointer listItemEnd = listItem.ContentEnd;

                        if (formatRange.Start.CompareTo(listItemEnd) < 0 && formatRange.End.CompareTo(listItemStart) > 0)
                        {
                            TextRange selectedInListItem = new TextRange(listItemStart, listItemEnd);

                            if (selectedInListItem.Text != "")
                            {
                                ExtractListItemsAsParagraphs(listItem, newParagraphs);
                                itemsToRemove.Add(listItem);  // Thêm vào danh sách tạm

                                if (!isSetFormatRangeStart)
                                {
                                    formatRangeStart = listItemStart;
                                    isSetFormatRangeStart = true;
                                }
                            }
                        }
                    }
                    // Nếu có ListItem cần xóa, thêm vào Dictionary
                    if (itemsToRemove.Count > 0)
                    {
                        listItemsToRemove[list] = itemsToRemove;
                    }
                }
            }

            // Sau khi vòng lặp kết thúc, xóa các ListItem từ các List trong Dictionary
            foreach (var entry in listItemsToRemove)
            {
                List list = entry.Key;
                List<ListItem> itemsToRemove = entry.Value;

                // Xóa các ListItem đã chọn khỏi List
                foreach (ListItem item in itemsToRemove)
                {
                    list.ListItems.Remove(item);
                }

                // Kiểm tra nếu ListItems trống, có thể xóa luôn List nếu cần thiết
                if (list.ListItems.Count == 0)
                {
                    _richTextBox.Document.Blocks.Remove(list);  // Xóa list khỏi danh sách copiedBlocks nếu không còn ListItem nào
                }
            }

            foreach (Paragraph para in newParagraphs)
            {
                ListItem listItem = new ListItem(para);
                bulletStyleList.ListItems.Add(listItem);
            }
            newParagraphs = null;

            _formattedList = bulletStyleList;

            PasteCutListBlockAtTextPointer(formatRangeStart, isReFormattedParagraphCase);
            copiedBlocks = null;
        }

        public void PasteCutListBlockAtTextPointer(TextPointer insertionPoint, bool isReFormattedParagraphCase)
        {
            if (_formattedList == null)
            {
                MessageBox.Show("No list block to paste!");
                return;
            }

            if (!isReFormattedParagraphCase)
            {
                bool isListCase = PasteMethodForParagraph_ListCase(insertionPoint);
                if (!isListCase)
                {
                    bool isParagraphCase = PasteMethodForParagraph_ParagraphCase(insertionPoint);
                }
            }
            else
            {
                PasteMethodForReApplyParagraphCase(insertionPoint);
            }

        }

        private bool PasteMethodForParagraph_ListCase(TextPointer insertionPoint)
        {
            TextPointer previousPointer = insertionPoint.GetNextContextPosition(LogicalDirection.Backward);
            if (previousPointer != null && (previousPointer.Parent is FlowDocument) == false)
            {
                List parentList = null;
                if (previousPointer.Parent is List)
                {
                    parentList = previousPointer.Parent as List;
                }
                else if (previousPointer.Parent is ListItem previousListItem)
                {
                    if (previousListItem.Parent is List)
                    {
                        parentList = previousListItem.Parent as List;
                    }
                }

                if (parentList != null)
                {
                    _formattedList.MarkerStyle = TextMarkerStyle.None;
                    foreach (ListItem item in _formattedList.ListItems.ToList())
                    {
                        ListItem clonedItem = RemoveTabForList(item);
                        parentList.ListItems.Add(item);
                    }

                    _formattedList = null;
                    CheckingAfterPasteForListCase(parentList.ContentEnd, parentList);
                    return true;
                }
            }
            else
            {
                TextPointer afterPointer = insertionPoint.GetNextContextPosition(LogicalDirection.Forward);
                if (afterPointer != null)
                {
                    List parentList = null;
                    if (afterPointer.Parent is List)
                    {
                        parentList = afterPointer.Parent as List;
                    }
                    else if (afterPointer.Parent is ListItem nextListItem)
                    {
                        if (nextListItem.Parent is List)
                        {
                            parentList = nextListItem.Parent as List;
                        }
                    }
                    else if (afterPointer.Parent is FlowDocument)
                    {
                        _richTextBox.Document.Blocks.Add(_formattedList);
                        _formattedList = null;
                        return true;
                    }

                    if (parentList != null)
                    {
                        _formattedList.MarkerStyle = TextMarkerStyle.None;
                        ListItem firstItem = parentList.ListItems.FirstListItem;
                        if (firstItem != null)
                        {
                            // Duyệt qua ListItems từ cuối lên đầu
                            for (int i = _formattedList.ListItems.Count - 1; i >= 0; i--)
                            {
                                ListItem item = _formattedList.ListItems.ElementAt(i);  // Lấy ListItem theo chỉ số bằng ElementAt()
                                ListItem clonedItem = RemoveTabForList(item);
                                parentList.ListItems.InsertBefore(firstItem, clonedItem);
                                firstItem = parentList.ListItems.FirstListItem;
                            }
                        }
                        else
                        {
                            foreach (ListItem item in _formattedList.ListItems.ToList())
                            {
                                ListItem clonedItem = RemoveTabForList(item);
                                parentList.ListItems.Add(clonedItem);
                            }
                        }

                        _formattedList = null;
                        return true;
                    }
                }

                if (previousPointer == null && afterPointer == null)
                {
                    _richTextBox.Document.Blocks.Add(_formattedList);
                    _formattedList = null;
                    return true;
                }
            }

            return false;
        }

        private void CheckingAfterPasteForListCase(TextPointer currentPointer, List previousList)
        {
            currentPointer = currentPointer.GetNextContextPosition(LogicalDirection.Forward);
            List listToRemove = new List();

            if (currentPointer.Parent is FlowDocument)
                currentPointer = currentPointer.GetNextContextPosition(LogicalDirection.Forward);

            if (currentPointer.Parent is ListItem || currentPointer.Parent is List) // Vị trí ContentStart của list khác nằm ngay sau đó
            {
                List nextList = new List();
                if (currentPointer.Parent is List)
                    nextList = currentPointer.Parent as List;
                else if (currentPointer.Parent is ListItem nextListItem)
                {
                    if (nextListItem.Parent is List)
                        nextList = nextListItem.Parent as List;
                }

                listToRemove = nextList;

                foreach (ListItem item in nextList.ListItems.ToList())
                {
                    nextList.ListItems.Remove(item);
                    previousList.ListItems.Add(item);
                }
            }

            _richTextBox.Document.Blocks.Remove(listToRemove);
        }

        private bool PasteMethodForParagraph_ParagraphCase(TextPointer insertionPoint)
        {
            TextPointer previousPointer = insertionPoint.GetNextContextPosition(LogicalDirection.Backward);
            Block currentBlock;

            if (previousPointer != null && previousPointer.Paragraph != null)
                currentBlock = previousPointer.Paragraph;
            else
                currentBlock = null;

            if (currentBlock != null)
            {
                // Insert the List block after the found block
                _richTextBox.Document.Blocks.InsertAfter(currentBlock, _formattedList);

                // Clear the stored block
                _formattedList = null;

                return true;
            }
            else
            {
                TextPointer afterPointer = insertionPoint.GetNextContextPosition(LogicalDirection.Forward);

                if (afterPointer != null && afterPointer.Paragraph != null)
                    currentBlock = afterPointer.Paragraph;
                else
                    currentBlock = null;

                if (currentBlock != null)
                {
                    _richTextBox.Document.Blocks.InsertBefore(currentBlock, _formattedList);
                    _formattedList = null;
                    return true;
                }

                if (previousPointer == null && afterPointer == null)
                {
                    _richTextBox.Document.Blocks.Add(_formattedList);
                    _formattedList = null;
                    return true;
                }
            }

            return false;
        }

        private void ExtractListItemsAsParagraphs(ListItem listItem, List<Paragraph> paragraphs)
        {
            // Duyệt qua tất cả các Block bên trong ListItem
            foreach (Block block in listItem.Blocks)
            {
                if (block is Paragraph para)
                {
                    // Thêm Paragraph vào danh sách
                    paragraphs.Add(para);
                }
            }
        }

        // Hàm di chuyển con trỏ tới Paragraph tiếp theo
        private TextPointer GetNextParagraph(TextPointer current)
        {
            // Lấy TextPointer của Paragraph tiếp theo
            TextPointer nextParagraph = current.GetNextContextPosition(LogicalDirection.Forward);

            // Duyệt qua các vị trí cho đến khi tìm thấy một Paragraph hợp lệ
            while (nextParagraph != null && nextParagraph.Paragraph == null)
            {
                nextParagraph = nextParagraph.GetNextContextPosition(LogicalDirection.Forward);
            }

            return nextParagraph;
        }

        private Dictionary<List, List<ListItem>> ExtractFormatRangeIntoLists_OnlyListsCase(TextRange formatRange)
        {
            Dictionary<List, List<ListItem>> listRanges = new Dictionary<List, List<ListItem>>();

            // Duyệt qua từng Paragraph trong TextRange
            TextPointer current = formatRange.Start;

            while (current != null && current.CompareTo(formatRange.End) < 0)
            {
                // Tìm Paragraph tại vị trí hiện tại
                Paragraph para = current.Paragraph;
                if (para != null)
                {
                    TextRange paraText = new TextRange(para.ContentStart, para.ContentEnd); 
                    if(paraText.Text != null) { }

                    // Tìm phần tử cha là List
                    List parentList = GetParentList(para);

                    if (parentList != null)
                    {
                        // Tạo ListItem từ Paragraph (đây là cách giả định về ListItem)
                        ListItem listItem = FindListItemFromParagraph(para, parentList);

                        // Nhóm các TextRange theo List
                        if (!listRanges.ContainsKey(parentList))
                        {
                            listRanges[parentList] = new List<ListItem>();
                        }
                        listRanges[parentList].Add(listItem);
                    }
                    current = para.ContentEnd;
                }

                // Di chuyển đến Paragraph tiếp theo
                current = GetNextParagraph(current);
            }
            return listRanges;
        }

        private List GetParentList(Paragraph para)
        {
            DependencyObject parent = para;
            while(parent != null && !(parent is List))
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }

            // Khi tìm thấy phần tử kiểu List, dừng vòng lặp và trả về phần tử đó.
            // Nếu không tìm thấy List, trả về null.
            return parent as List;         
        }
        private ListItem FindListItemFromParagraph(Paragraph para, List parentList)
        {
            // Lặp qua tất cả các ListItem trong List để tìm ListItem chứa Paragraph
            foreach (var listItem in parentList.ListItems)
            {
                // Kiểm tra xem listItem có chứa Paragraph hay không
                if (listItem.ContentStart.CompareTo(para.ContentStart) <= 0 &&
                    listItem.ContentEnd.CompareTo(para.ContentEnd) >= 0)
                {
                    return listItem;  // Trả về ListItem tìm thấy
                }
            }

            // Nếu không tìm thấy, trả về null
            return null;
        }

        private List<Paragraph> ExtractParagraphsFromList(List<ListItem> listItems)
        {
            List<Paragraph> paragraphs = new List<Paragraph>();
            foreach (ListItem item in listItems)
            {
                foreach(Block block in item.Blocks)
                {
                    if(block is Paragraph para)
                    {
                        paragraphs.Add(para);
                    }    
                }    
            } 
            return paragraphs;              
        }

        private List ReApplyingMarkerStyleForParagraph(List<Paragraph> paragraphs, TextMarkerStyle bulletStyle)
        {
            List newList = new List();
            newList.MarkerStyle = bulletStyle;
            foreach(var para in paragraphs)
            {
                ListItem item = new ListItem(para);
                newList.ListItems.Add(item);   
            }    
            return newList;
        }

        private void InsertListAfterReApplyingMarkerStyle(List newMarkerStyleList, List previousList)
        {
            _richTextBox.Document.Blocks.InsertAfter(previousList, newMarkerStyleList);
        }

        private void InsertParagraphAfterRemovingMarkerStyle(List<Paragraph> paragraphs, List previousList)
        {
            Block currentBlock = previousList;
            foreach (Paragraph para in paragraphs)
            {
                Paragraph clonedPara = CloneParagraph(para);
                _richTextBox.Document.Blocks.InsertAfter(currentBlock, clonedPara);
                currentBlock = clonedPara;
            }
            currentBlock = null;
        }
        private ListItem RemoveTabForList(ListItem item)
        {
            // Tạo TextRange từ item để lấy RTF
            TextRange range = new TextRange(item.ContentStart, item.ContentEnd);
            ListItem clonedItem = new ListItem();

            using (MemoryStream stream = new MemoryStream())
            {
                range.Save(stream, DataFormats.Rtf);
                stream.Position = 0;

                string rtfText = Encoding.UTF8.GetString(stream.ToArray());

                // Loại bỏ tab ở đầu chuỗi RTF
                if (rtfText.StartsWith(@"\t"))
                {
                    rtfText = rtfText.Remove(0, 2); // Xóa ký tự \t
                }

                stream.SetLength(0);  // Xóa nội dung cũ
                stream.Write(Encoding.UTF8.GetBytes(rtfText), 0, rtfText.Length);
                stream.Position = 0;

                range.Load(stream, DataFormats.Rtf);

                // Tạo ListItem và Paragraph
                ListItem listItem = new ListItem(new Paragraph());

                // Sao chép nội dung đã chỉnh sửa vào ListItem
                TextRange listItemRange = new TextRange(
                    listItem.Blocks.FirstBlock.ContentStart,
                    listItem.Blocks.FirstBlock.ContentEnd
                );
                listItemRange.Load(stream, DataFormats.Rtf);
                clonedItem = listItem;
            }

            return clonedItem;
        }

        private void RemoveFormattedListItems(Dictionary<List, List<ListItem>> listRanges)
        {
            // Sau khi vòng lặp kết thúc, xóa các ListItem từ các List trong Dictionary
            foreach (var entry in listRanges)
            {
                List list = entry.Key;
                List<ListItem> itemsToRemove = entry.Value;

                // Xóa các ListItem đã chọn khỏi List
                foreach (ListItem item in itemsToRemove)
                {
                    list.ListItems.Remove(item);
                }

                // Kiểm tra nếu ListItems trống, có thể xóa luôn List nếu cần thiết
                if (list.ListItems.Count == 0)
                {
                    _richTextBox.Document.Blocks.Remove(list);  // Xóa list khỏi danh sách copiedBlocks nếu không còn ListItem nào
                }
            }
        }

        private List InsertParagraphsIntoList(List<Paragraph> paragraphs, TextMarkerStyle bulletStyle)
        {
            List newMarkerStyleList = new List();
            newMarkerStyleList.MarkerStyle = bulletStyle;
            foreach(var para in paragraphs)
            {
                ListItem newItem = new ListItem(para);
                newMarkerStyleList.ListItems.Add(newItem);
            }
            return newMarkerStyleList;
        }

        private void FormatEachListsInSelectedRange(TextRange formatRange, TextMarkerStyle bulletStyle)
        {
            Dictionary<List, List<ListItem>> listRanges = ExtractFormatRangeIntoLists_OnlyListsCase(formatRange);
            List<Paragraph> paragraphs = new List<Paragraph>();
            List<ListItem> clonedListItems = new List<ListItem>();
            List firstList = new List();

            List firstKey = null;
            List<ListItem> firstValue = null;

            if (listRanges.Count > 0)
            {
                firstKey = listRanges.Keys.First();   // Gán key đầu tiên
                firstValue = listRanges[firstKey];    // Gán value tương ứng
            }

            firstList = firstKey;

            if (listRanges.Count == 1)
            {
                TextMarkerStyle currentMarkerStyle = firstList.MarkerStyle;

                firstList.MarkerStyle = TextMarkerStyle.None; // remember to return current markerstyle
                List<ListItem> listItems = firstValue;

                foreach (ListItem item in listItems)
                {
                    ListItem clonedItem = RemoveTabForList(item);
                    clonedListItems.Add(clonedItem);
                }
                paragraphs = ExtractParagraphsFromList(clonedListItems);

                if (currentMarkerStyle == bulletStyle)
                {
                    InsertParagraphAfterRemovingMarkerStyle(paragraphs, firstList);
                }
                else
                {
                    List newMarkerStyleList = InsertParagraphsIntoList(paragraphs, bulletStyle);
                    InsertListAfterReApplyingMarkerStyle(newMarkerStyleList, firstList);
                }
                firstKey.MarkerStyle = currentMarkerStyle;
            }
            else
            {
                foreach (var entry in listRanges)
                {
                    List list = entry.Key;

                    TextMarkerStyle currentMarkerStyle = list.MarkerStyle;

                    list.MarkerStyle = TextMarkerStyle.None; // remember to return current markerstyle
                    List<ListItem> listItems = entry.Value;

                    foreach (ListItem item in listItems)
                    {
                        ListItem clonedItem = RemoveTabForList(item);
                        clonedListItems.Add(clonedItem);
                    }

                    list.MarkerStyle = currentMarkerStyle;
                }

                paragraphs = ExtractParagraphsFromList(clonedListItems);
                List newMarkerStyleList = InsertParagraphsIntoList(paragraphs, bulletStyle);
                InsertListAfterReApplyingMarkerStyle(newMarkerStyleList, firstList);
            }
            RemoveFormattedListItems(listRanges);
        }

      
        private bool PasteMethodForReApplyParagraphCase(TextPointer insertionPoint)
        {
            TextPointer previousPointer = insertionPoint.GetNextContextPosition(LogicalDirection.Backward);
            Block block = null;
            if (previousPointer != null && (previousPointer.Parent is FlowDocument) == false)
            {
                if (previousPointer.Parent is List)
                {
                    block = previousPointer.Parent as List;
                }
                else if (previousPointer.Parent is ListItem previousListItem)
                {
                    if (previousListItem.Parent is List)
                    {
                        block = previousListItem.Parent as List;
                    }
                }
                else if (previousPointer.Paragraph != null)
                    block = previousPointer.Paragraph;

                if (block != null)
                {
                    _richTextBox.Document.Blocks.InsertAfter(block, _formattedList);
                    _formattedList = null;
                    return true;
                }
            }
            else
            {
                TextPointer afterPointer = insertionPoint.GetNextContextPosition(LogicalDirection.Forward);
                if (afterPointer != null)
                {
                    if (afterPointer.Parent is List)
                    {
                        block = afterPointer.Parent as List;
                    }
                    else if (afterPointer.Parent is ListItem nextListItem)
                    {
                        if (nextListItem.Parent is List)
                        {
                            block = nextListItem.Parent as List;
                        }
                    }
                    else if (afterPointer.Parent is FlowDocument)
                    {
                        _richTextBox.Document.Blocks.Add(_formattedList);
                        _formattedList = null;
                        return true;
                    }
                    else if (afterPointer.Paragraph != null)
                        block = afterPointer.Paragraph;

                    if (block != null)
                    {
                        _richTextBox.Document.Blocks.InsertBefore(block, _formattedList);
                        _formattedList = null;
                        return true;
                    }
                }

                if (previousPointer == null && afterPointer == null)
                {
                    _richTextBox.Document.Blocks.Add(_formattedList);
                    _formattedList = null;
                    return true;
                }
            }

            return false;
        }

        private void RemoveBulletStyle(TextRange formatRange, out TextPointer startFormatRangePointer, out TextPointer endFormatRangePointer)
        {
            TextPointer formatRangeStart = null;
            bool isSetFormatRangeStart = false;
            bool isSetFormatRangeEnd = false;
            startFormatRangePointer = null;
            endFormatRangePointer = null;

            List<Paragraph> newParagraphs = new List<Paragraph>();

            List<Paragraph> listItemsToSplit = new List<Paragraph>();
            List splitedList = new List();

            // Khởi tạo một Dictionary để lưu trữ List cùng với các ListItem cần xóa
            Dictionary<List, List<ListItem>> listItemsToRemove = new Dictionary<List, List<ListItem>>();

            // Sao chép danh sách Blocks trước khi duyệt
            List<Block> copiedBlocks = _richTextBox.Document.Blocks.ToList();

            foreach (Block block in copiedBlocks)
            {
                if (block is List list)
                {
                    // Khởi tạo một danh sách tạm để lưu các ListItem cần xóa cho List hiện tại
                    List<ListItem> itemsToRemove = new List<ListItem>();

                    splitedList.MarkerStyle = list.MarkerStyle;

                    // Xử lý với List
                    foreach (ListItem listItem in list.ListItems)
                    {
                        TextPointer listItemStart = listItem.ContentStart;
                        TextPointer listItemEnd = listItem.ContentEnd;

                        if (formatRange.Start.CompareTo(listItemEnd) < 0 && formatRange.End.CompareTo(listItemStart) > 0)
                        {
                            TextRange selectedInListItem = new TextRange(listItemStart, listItemEnd);

                            if (selectedInListItem.Text != "")
                            {
                                ExtractListItemsAsParagraphs(listItem, newParagraphs);
                                itemsToRemove.Add(listItem);  // Thêm vào danh sách tạm

                                if (!isSetFormatRangeStart)
                                {
                                    formatRangeStart = listItemStart;
                                    isSetFormatRangeStart = true;
                                }
                                isSetFormatRangeEnd = true;
                            }
                        }
                        else
                            isSetFormatRangeEnd = false;

                        if (isSetFormatRangeStart && !isSetFormatRangeEnd)
                        {
                            ExtractListItemsAsParagraphs(listItem, listItemsToSplit);
                            itemsToRemove.Add(listItem);
                        }
                    }
                    // Nếu có ListItem cần xóa, thêm vào Dictionary
                    if (itemsToRemove.Count > 0)
                    {
                        listItemsToRemove[list] = itemsToRemove;
                    }
                }
            }

            // Sau khi vòng lặp kết thúc, xóa các ListItem từ các List trong Dictionary
            foreach (var entry in listItemsToRemove)
            {
                List list = entry.Key;
                List<ListItem> itemsToRemove = entry.Value;

                // Xóa các ListItem đã chọn khỏi List
                foreach (ListItem item in itemsToRemove)
                {
                    list.ListItems.Remove(item);
                }

                // Kiểm tra nếu ListItems trống, có thể xóa luôn List nếu cần thiết
                if (list.ListItems.Count == 0)
                {
                    _richTextBox.Document.Blocks.Remove(list);  // Xóa list khỏi danh sách copiedBlocks nếu không còn ListItem nào
                }
            }

            foreach (var para in listItemsToSplit)
            {
                ListItem newItem = new ListItem(para);
                splitedList.ListItems.Add(newItem);
            }
            listItemsToSplit = null;

            PasteRemovedBulletStyleParagraph(formatRangeStart, newParagraphs, splitedList, out TextPointer startFormatPointer, out TextPointer endFormatPointer);
            startFormatRangePointer = startFormatPointer;
            endFormatRangePointer = endFormatPointer;
            copiedBlocks = null;
        }

        private void PasteRemovedBulletStyleParagraph(TextPointer insertionPoint, List<Paragraph> paragraphs, List splitedList, out TextPointer startFormatPointer, out TextPointer endFormatPointer)
        {
            TextPointer previousPointer = insertionPoint.GetNextContextPosition(LogicalDirection.Backward);
            Block currentBlock = null;
            startFormatPointer = null;
            endFormatPointer = null;

            bool isPasteSplitedList = false;
            if (splitedList.ListItems.Count != 0)
                isPasteSplitedList = true;

            if (previousPointer != null)
            {
                if (previousPointer.Paragraph != null)
                    currentBlock = previousPointer.Paragraph;
                else if (previousPointer.Parent is ListItem || previousPointer.Parent is List)
                {
                    if (previousPointer.Parent is ListItem listItem)
                    {
                        if (listItem.Parent is List list)
                            currentBlock = list;
                    }
                    else if (previousPointer.Parent is List list)
                        currentBlock = list;
                }
            }

            if (currentBlock != null)
            {
                bool isSetStartRange = false;
                foreach (Paragraph para in paragraphs.ToList())
                {
                    Paragraph clonedParagraph = CloneParagraph(para);
                    _richTextBox.Document.Blocks.InsertAfter(currentBlock, clonedParagraph);
                    currentBlock = clonedParagraph;

                    if (!isSetStartRange)
                    {
                        startFormatPointer = currentBlock.ContentStart;
                        isSetStartRange = true;
                    }
                }
                endFormatPointer = currentBlock.ContentEnd;

                if (isPasteSplitedList)
                    _richTextBox.Document.Blocks.InsertAfter(currentBlock, splitedList);
            }
            else
            {
                TextPointer afterPointer = insertionPoint.GetNextContextPosition(LogicalDirection.Forward);
                if (afterPointer != null)
                {
                    if (afterPointer.Paragraph != null)
                        currentBlock = afterPointer.Paragraph;
                    else if (afterPointer.Parent is ListItem || afterPointer.Parent is List)
                    {
                        if (afterPointer.Parent is ListItem listItem)
                        {
                            if (listItem.Parent is List list)
                                currentBlock = list;
                        }
                        else if (afterPointer.Parent is List list)
                            currentBlock = list;
                    }
                }

                if (currentBlock != null)
                {
                    if (isPasteSplitedList)
                    {
                        _richTextBox.Document.Blocks.InsertBefore(currentBlock, splitedList);
                        currentBlock = splitedList;
                    }

                    bool isSetEndRange = false;
                    for (int i = paragraphs.Count - 1; i >= 0; i--)
                    {
                        Paragraph clonedParagraph = CloneParagraph(paragraphs[i]);
                        if (isPasteSplitedList)
                            _richTextBox.Document.Blocks.InsertBefore(currentBlock, clonedParagraph);
                        currentBlock = clonedParagraph;

                        if (!isSetEndRange)
                        {
                            endFormatPointer = currentBlock.ContentEnd;
                            isSetEndRange = true;
                        }
                    }
                    startFormatPointer = currentBlock.ContentStart;
                }

                if (previousPointer == null && afterPointer == null)
                {
                    bool isSetStartRange = false;
                    foreach (Paragraph para in paragraphs.ToList())
                    {
                        Paragraph clonedParagraph = CloneParagraph(para);
                        _richTextBox.Document.Blocks.Add(clonedParagraph);

                        if (!isSetStartRange)
                        {
                            startFormatPointer = clonedParagraph.ContentStart;
                            isSetStartRange = true;
                        }
                    }
                    endFormatPointer = _richTextBox.Document.ContentEnd;
                    if (isPasteSplitedList)
                        _richTextBox.Document.Blocks.Add(splitedList);
                }
            }
        }

        private Paragraph CloneParagraph(Paragraph sourceParagraph)
        {
            // Tạo một MemoryStream để lưu trữ nội dung RTF tạm thời
            MemoryStream stream = new MemoryStream();

            // Sao chép nội dung và định dạng bằng TextRange
            TextRange sourceRange = new TextRange(sourceParagraph.ContentStart, sourceParagraph.ContentEnd);
            sourceRange.Save(stream, DataFormats.Xaml);

            // Đặt lại stream để đọc
            stream.Position = 0;

            // Tạo một Paragraph mới và sao chép dữ liệu từ stream
            Paragraph clonedParagraph = new Paragraph();
            TextRange clonedRange = new TextRange(clonedParagraph.ContentStart, clonedParagraph.ContentEnd);
            clonedRange.Load(stream, DataFormats.Xaml);

            return clonedParagraph;
        }
        #endregion
    }


}
