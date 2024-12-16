using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace Wordpad
{
    internal class ViewManagment
    {
        private readonly StatusBar statusBar;
        private readonly StatusBarItem statusBarItem;
        private readonly RichTextBox richTextBox;
        private readonly ComboBox unitComboBox;
        private DockPanel dockPanel;
        private Slider zoomSlider;
        private double zoomScale = 1.0; // Giá trị ban đầu (100%)
        //Kích thước ban đầu cảu dockpanel
        public static double DPWidthOri;
        private Ruler ruler;

        public ViewManagment(StatusBar statusBar, StatusBarItem statusBarItem, 
            RichTextBox richTextBox, ComboBox unitComboBox, DockPanel DP, Slider slider, Ruler ruler)
        {
            this.statusBar = statusBar;
            this.statusBarItem = statusBarItem;
            this.richTextBox = richTextBox;
            this.unitComboBox = unitComboBox;
            this.dockPanel = DP;
            this.zoomSlider = slider;

            // Đăng ký sự kiện TextChanged cho RichTextBox
            richTextBox.TextChanged += RichTextBox_TextChanged;
            this.ruler = ruler;
        }


        public void ShowRuler(bool show)
        {
            ruler.IsVisible(show);
        }

        public void ShowStatusBar(bool show)
        {
            statusBar.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        public void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int charCount = GetCharacterCount();
            int lineCount = GetLineCount();

            statusBarItem.Content = $"Line: {lineCount} | Characters: {charCount}";
        }

        #region Zoom
        public void ZoomIn()
        {
            if (zoomScale < 5) // Đảm bảo mức zoom không xuống dưới 10%
            {
                zoomScale += 0.1; // Giảm 10% mức zoom
                ApplyZoom();
            }
        }

        public void ZoomOut()
        {
            if (zoomScale > 0.1) // Đảm bảo mức zoom không xuống dưới 10%
            {
                zoomScale -= 0.1; // Giảm 10% mức zoom
                ApplyZoom();
            }
        }

        public void Set100()
        {
            zoomScale = 1;
            ApplyZoom();
        }

        public void SetZoom(double zoomLevel)
        {
            if (zoomLevel < 0.1 || zoomLevel > 5)
                return;

            zoomScale = zoomLevel;
            ApplyZoom();
        }

        private void ApplyZoom()
        {
            double preDPWidth = dockPanel.Width;
            // Phóng to cả RichTextBox bằng cách thay đổi kích thước.
            dockPanel.Width = DPWidthOri * zoomScale; // Kích thước mặc định là 743.
            // Áp dụng zoom cho RichTextBox qua LayoutTransform
            richTextBox.LayoutTransform = new ScaleTransform(zoomScale, zoomScale);
            //Scale ruler theo zoom
            ruler.ScaleRuler(zoomScale, preDPWidth);

            // Cập nhật giá trị trên thanh trạng thái
            zoomSlider.Value = zoomScale * 100;
        }
        #endregion

        #region StatusBarItems
        private int GetCharacterCount()
        {
            // Tính tổng số ký tự trong RichTextBox
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            return textRange.Text.TrimEnd('\r', '\n').Length; // Loại bỏ ký tự xuống dòng cuối cùng
        }

        private int GetLineCount()
        {
            // Dựa vào số dòng logic trong FlowDocument
            return richTextBox.Document.Blocks.Count;
        }
        #endregion

    }
}
