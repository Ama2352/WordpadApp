using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Wordpad
{
    internal class TextBoxBehavior
    {
        private RichTextBox _richTextBox;
        //private ScrollBar _customScrollBar;
        private Grid editorArea;
        private DockPanel _dockPanel;

        public TextBoxBehavior(RichTextBox richTextBox, Grid grid, DockPanel DP)
        {
            _richTextBox = richTextBox;
            //_customScrollBar = customScrollBar;
            editorArea = grid;
            _dockPanel = DP;

            // Gắn sự kiện cho RichTextBox và thanh cuộn tùy chỉnh
            _richTextBox.TextChanged += RichTextBox_TextChanged;
            //_customScrollBar.ValueChanged += CustomScrollBar_ValueChanged;
            //_richTextBox.TextChanged += (s, e) => ScrollToCaret();
            _richTextBox.LayoutUpdated += RichTextBox_LayoutUpdated;

            // Xử lý sự kiện PreviewMouseWheel trên vùng soạn thảo
            //editorArea.PreviewMouseWheel += EditorArea_PreviewMouseWheel;
        }

        // Khi nội dung RichTextBox thay đổi, cập nhật phạm vi và thumb của thanh cuộn tùy chỉnh
        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AdjustRichTextBoxHeight();
            //UpdateCustomScrollBar();
        }
        //Sự kiện xử lý khi chèn ảnh hoặc văn bản tràn kích thước RTB
        private void RichTextBox_LayoutUpdated(object sender, EventArgs e)
        {
            double contentHeight = GetRichTextBoxContentHeight();
            double actualHeight = _richTextBox.ActualHeight - _richTextBox.Padding.Top - _richTextBox.Padding.Bottom;
            if (contentHeight > actualHeight)
            {
                AdjustRichTextBoxHeight(); // Đảm bảo DockPanel mở rộng khi nội dung tràn
                //UpdateCustomScrollBar(); // Cập nhật thanh cuộn tùy chỉnh
            }
        }
        // Khi người dùng thay đổi giá trị trên thanh cuộn tùy chỉnh
        //private void CustomScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    // Điều chỉnh vị trí DockPanel dựa trên giá trị thanh cuộn
        //    double offset = _customScrollBar.Value;
        //    _dockPanel.Margin = new Thickness(_dockPanel.Margin.Left, -offset, _dockPanel.Margin.Right, 0); // Di chuyển DockPanel lên/xuống
        //}

        // Cập nhật phạm vi và thumb cho thanh cuộn tùy chỉnh
        //public void UpdateCustomScrollBar()
        //{
        //    // Chiều cao hiển thị của vùng editor
        //    double viewportHeight = editorArea.ActualHeight;

        //    // Tổng chiều cao nội dung của DockPanel
        //    double contentHeight = _dockPanel.Height;

        //    // Cập nhật phạm vi cuộn cho thanh cuộn
        //    _customScrollBar.Maximum = Math.Max(0, contentHeight - viewportHeight);
        //    _customScrollBar.Value = Math.Min(_customScrollBar.Value, _customScrollBar.Maximum);

        //    // Tính toán thumb (kích thước của thanh cuộn) dựa trên tỷ lệ
        //    if (_customScrollBar.Maximum > 0)
        //    {
        //        double ratio = viewportHeight / contentHeight;
        //        _customScrollBar.ViewportSize = ratio * _customScrollBar.Maximum;
        //    }
        //}

        // Sự kiện PreviewMouseWheel trên vùng soạn thảo
        //private void EditorArea_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    // Cuộn nội dung theo hướng của chuột
        //    double newValue = _customScrollBar.Value - (e.Delta / 3.0); // Điều chỉnh tốc độ cuộn
        //    _customScrollBar.Value = Math.Max(0, Math.Min(_customScrollBar.Maximum, newValue));

        //    e.Handled = true; // Ngăn xử lý mặc định
        //}

        // Kéo dài vùng soạn thảo vô tận
        private void AdjustRichTextBoxHeight()
        {
            double contentHeight = GetRichTextBoxContentHeight();
            double actualHeight = _richTextBox.ActualHeight - _richTextBox.Padding.Top - _richTextBox.Padding.Bottom;
            if (contentHeight > actualHeight)
            {
                //1 lần mở rộng nhiều để giảm số lần phải mở rộng
                _dockPanel.Height += 1000; // Kéo dài DockPanel
                //UpdateCustomScrollBar(); // Cập nhật thanh cuộn
            }
            // Cuộn để hiển thị con trỏ khi mở rộng
            //ScrollToCaret();
            //MessageBox.Show($"content height: {contentHeight}\n dock panel height = {_dockPanel.Height}\n RTB height: {actualHeight}");
        }

        //Lăn chuột đến vị trí con trỏ
        //private void ScrollToCaret()
        //{
        //    // Lấy vị trí con trỏ tính theo tọa độ RichTextBox

        //    Rect caretRect = _richTextBox.CaretPosition.GetCharacterRect(LogicalDirection.Forward);

        //    // Vị trí con trỏ tính trong DockPanel
        //    double caretPositionInDockPanel = caretRect.Top + _richTextBox.Margin.Top;

        //    // Xác định chiều cao hiển thị của DockPanel
        //    double viewportHeight = editorArea.ActualHeight;

        //    //Tỷ lệ % con trỏ đã "chiếm" được bao nhieu $ của panel.
        //    //Lấy % đó nhân với scrollbar max để tìm được vị trí phù hợp
        //    int cursorToSBValue = (int)(((float)caretPositionInDockPanel / _dockPanel.ActualHeight) * _customScrollBar.Maximum);
        //    // Kiểm tra nếu con trỏ nằm ngoài vùng hiển thị
        //    _customScrollBar.Value = Math.Max(0, Math.Min(cursorToSBValue, _customScrollBar.Maximum));
        //}

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
