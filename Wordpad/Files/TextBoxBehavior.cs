using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Wordpad
{
    internal class TextBoxBehavior
    {
        private RichTextBox _richTextBox;
        //private ScrollBar _customScrollBar;
        private DockPanel _dockPanel;

        public TextBoxBehavior(RichTextBox richTextBox, DockPanel DP)
        {
            _richTextBox = richTextBox;
            //_customScrollBar = customScrollBar;
            _dockPanel = DP;

            // Gắn sự kiện cho RichTextBox và thanh cuộn tùy chỉnh
            _richTextBox.TextChanged += RichTextBox_TextChanged;
            _richTextBox.LayoutUpdated += RichTextBox_LayoutUpdated;

        }

        // Khi nội dung RichTextBox thay đổi, cập nhật phạm vi và thumb của thanh cuộn tùy chỉnh
        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AdjustRichTextBoxHeight();
        }
        //Sự kiện xử lý khi chèn ảnh hoặc văn bản tràn kích thước RTB
        private void RichTextBox_LayoutUpdated(object sender, EventArgs e)
        {
            double contentHeight = GetRichTextBoxContentHeight();
            double actualHeight = _richTextBox.ActualHeight - _richTextBox.Padding.Top - _richTextBox.Padding.Bottom;
            if (contentHeight > actualHeight)
            {
                AdjustRichTextBoxHeight(); // Đảm bảo DockPanel mở rộng khi nội dung tràn
            }
        }

        // Kéo dài vùng soạn thảo vô tận
        private void AdjustRichTextBoxHeight()
        {
            double contentHeight = GetRichTextBoxContentHeight();
            double actualHeight = _richTextBox.ActualHeight - _richTextBox.Padding.Top - _richTextBox.Padding.Bottom;
            if (contentHeight > actualHeight)
            {
                //1 lần mở rộng nhiều để giảm số lần phải mở rộng
                _dockPanel.Height += 1000; // Kéo dài DockPanel
            }
            //MessageBox.Show($"content height: {contentHeight}\n dock panel height = {_dockPanel.Height}\n RTB height: {actualHeight}");
        }


        private double GetRichTextBoxContentHeight()
        {
            // Tính toán chiều cao nội dung thực tế
            var scrollViewer = GetScrollViewer(_richTextBox);
            if (scrollViewer != null)
            {
                return scrollViewer.ExtentHeight; // Chiều cao nội dung thực tế
            }
            return 0;
        }

        // Trợ giúp: Lấy ScrollViewer từ RichTextBox
        private ScrollViewer GetScrollViewer(DependencyObject element)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                if (child is ScrollViewer viewer)
                    return viewer;
                else
                {
                    var result = GetScrollViewer(child);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }
    }
}
