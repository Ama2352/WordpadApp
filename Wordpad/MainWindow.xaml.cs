using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WordPad;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace Wordpad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ClipboardManager clipboardManager;
        FontManager fontManager;
        ParagraphManager paragraphManager;
        InsertManager insertManager;

        public MainWindow()
        {
            InitializeComponent();
            clipboardManager = new ClipboardManager(richTextBox);
            fontManager = new FontManager(richTextBox);
            paragraphManager = new ParagraphManager(richTextBox);
            insertManager = new InsertManager(richTextBox); 


            // Cài đặt kiểu chữ và kích cỡ mặc định
            SettingFontType(fontManager);
            SettingFontSize(fontManager);

        }

        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            clipboardManager.Cut();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            clipboardManager.Copy();
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            clipboardManager.Paste();   
        }

        private void btnGrowFont_Click(object sender, RoutedEventArgs e)
        {
            fontManager.GrowFont();
        }

        private void btnShrinkFont_Click(object sender, RoutedEventArgs e)
        {
            fontManager.ShrinkFont();
        }

        private void cbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fontManager.ChangeFontFamily(cbFontFamily.SelectedItem.ToString());
        }

        private void cbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (double.TryParse(cbFontSize.SelectedItem.ToString(), out double fontSize))
            {
                fontManager.ChangeFontSize(fontSize);
            }
        }

        private void SettingFontType(FontManager _fontManager)
        {
            // Liên kết danh sách font cho cbFontFamily
            cbFontFamily.ItemsSource = _fontManager.GetFontFamilies();

            // Đặt font mặc định cho richTextBox
            richTextBox.FontFamily = new FontFamily("Calibri");

            // Đặt font hiển thị mặc định cho cbFontFamily
            cbFontFamily.Text = "Calibri";
        }
        private void OnFontSizeChanged(double newSize)
        {
            // Cập nhật kích cỡ font đang hiển thị nếu có thay đổi về kích thước
            cbFontSize.Text = newSize.ToString();
        }
        private void OnFontFamilyChanged(string newFont)
        {
            cbFontFamily.Text = newFont;
        }

        private void SettingFontSize(FontManager _fontManager)
        {
            // Thêm các kích thước font vào cbFontSize
            cbFontSize.ItemsSource = new[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };

            // Đặt kích thước font mặc định trong cbFontSize và richtextbox
            cbFontSize.SelectedItem = 11;
            richTextBox.FontSize = 11;

            // Đăng ký sự kiện FontSizeChanged
            _fontManager.FontSizeChanged += OnFontSizeChanged;
        }

        private void btnBold_Click(object sender, RoutedEventArgs e)
        {
            fontManager.ToggleBold();
        }

        private void btnItalic_Click(object sender, RoutedEventArgs e)
        {
            fontManager.ToggleItalic();
        }

        private void btnUnderline_Click(object sender, RoutedEventArgs e)
        {
            fontManager.ToggleUnderline();
        }

        private void btnStrikethrough_Click(object sender, RoutedEventArgs e)
        {
            fontManager.ToggleStrikethrough();
        }

        private void btnSubscript_Click(object sender, RoutedEventArgs e)
        {
            fontManager.ToggleSubscript();
        }

        private void btnSuperscript_Click(object sender, RoutedEventArgs e)
        {
            fontManager.ToggleSuperscript();
        }

        private void richTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextSelection selectedText = richTextBox.Selection;
            if (selectedText != null)
            {
                // Kiểm tra xem font size và font family có giá trị hợp lệ không
                var fontSizeValue = selectedText.GetPropertyValue(TextElement.FontSizeProperty);
                var fontFamilyValue = selectedText.GetPropertyValue(TextElement.FontFamilyProperty);

                // Kiểm tra các giá trị trả về không phải là UnsetValue và phải là kiểu hợp lệ
                if (fontSizeValue != DependencyProperty.UnsetValue && fontSizeValue is double currentFontSize)
                {
                    OnFontSizeChanged(currentFontSize);
                }

                if (fontFamilyValue != DependencyProperty.UnsetValue && fontFamilyValue is FontFamily currentFontFamily)
                {
                    OnFontFamilyChanged(currentFontFamily.ToString());
                }
            }
                
        }

        private void btnColor_Click(object sender, RoutedEventArgs e)
        {
            // Lấy màu đã chọn từ ColorPicker
            var selectedColor = colorPicker.SelectedColor;
            if (selectedColor.HasValue)
            {
                // Gọi hàm thay đổi màu phông chữ
                fontManager.ChangeFontColor(selectedColor.Value);
            }
        }

        private void btnHighlight_Click(object sender, RoutedEventArgs e)
        {
            // Lấy màu đã chọn từ ColorPicker
            var selectedColor = colorPicker.SelectedColor;
            if (selectedColor.HasValue)
            {
                // Gọi hàm thay đổi màu highlight
                fontManager.ChangeHighlightColor(selectedColor.Value);
            }
        }

        private void btnDecreaseIndent_Click(object sender, RoutedEventArgs e)
        {
            paragraphManager.DecreaseIndent();
        }

        private void btnIncreaseIndent_Click(object sender, RoutedEventArgs e)
        {
            paragraphManager.IncreaseIndent();
        }

        private void btnLeftAlignment_Click(object sender, RoutedEventArgs e)
        {
            paragraphManager.AlignLeft();
        }

        private void btnCenterAlignment_Click(object sender, RoutedEventArgs e)
        {
            paragraphManager.AlignCenter();
        }

        private void btnRightAlignment_Click(object sender, RoutedEventArgs e)
        {
            paragraphManager.AlignRight();
        }

        private void btnJustifyAlignment_Click(object sender, RoutedEventArgs e)
        {
            paragraphManager.AlignJustify();
        }

        private void btnParagraph_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPicture_Click(object sender, RoutedEventArgs e)
        {
            insertManager.InsertImage();
        }
        private void btnChangePicture_Click(object sender, RoutedEventArgs e)
        {
            insertManager.ChangeImage();
        }

        private void btnResizePicture_Click(object sender, RoutedEventArgs e)
        {
            insertManager.ResizeImage();
        }

        

        private void btnDateAndTime_Click(object sender, RoutedEventArgs e)
        {
            DateAndTimeWindow dateAndTimeWindow = new DateAndTimeWindow(insertManager);
            if(dateAndTimeWindow.ShowDialog() == true)
            {
                // Get the chosen date and time from the dialog
                string chosenDateTime = dateAndTimeWindow.chosenDateTime;

                // Select the current text in the RichTextBox
                TextRange selection = new TextRange(richTextBox.Selection.Start, richTextBox.Selection.End);

                // Replace the selected text with the chosen date and time
                selection.Text = chosenDateTime;
            } 
                
        }

        private void btnInsertObject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void colorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (colorPicker.SelectedColor.HasValue)
            {
                // Gọi hàm để thay đổi màu văn bản
                fontManager.ChangeFontColor(colorPicker.SelectedColor.Value);
            }
        }

        private void highlightPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (highlightPicker.SelectedColor.HasValue)
            {
                // Gọi hàm để thay đổi màu nền (highlight) văn bản
                fontManager.ChangeHighlightColor(highlightPicker.SelectedColor.Value);
            }
        }

        private void cbLineSpacing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ép kiểu SelectedItem về ComboBoxItem
            ComboBoxItem selectedItem = (ComboBoxItem)cbLineSpacing.SelectedItem;

            // Lấy nội dung của ComboBoxItem
            string selectedValue = selectedItem.Content.ToString();

            // Kiểm tra trạng thái của checkbox (giả sử bạn có `chkAdd10pt`)
            bool checkAdd10pt = chkAdd10pt.IsChecked ?? false;

            MessageBox.Show(checkAdd10pt.ToString());

            float value = float.Parse(selectedValue);
            paragraphManager.SetLineSpacingWithSpacingAfterParagraphs(value, checkAdd10pt);
        }
    }
}
