using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using Wordpad.Files;
using Wordpad;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace Wordpad
{
    public partial class MainWindow : Window
    {
        NewManager _NewManager;
        OpenManager _OpenManager;
        PrintManager _PrintManager;
        SaveManager _SaveManager;
        SendEmailManager _SendEmailManager;
        TextBoxBehavior _TextBoxBehavior;
        public static bool IsTextChanged;
        private ViewManagment viewManagment;
        ClipboardManager clipboardManager;
        FontManager fontManager;
        ParagraphManager paragraphManager;
        InsertManager insertManager;
        EditingManager editingManager;

        public MainWindow()
        {
            InitializeComponent();
            _NewManager = new NewManager(richTextBox, this);
            _OpenManager = new OpenManager(richTextBox);
            _PrintManager = new PrintManager(RTBContainer, richTextBox);
            _SaveManager = new SaveManager(richTextBox);
            _SendEmailManager = new SendEmailManager(richTextBox);
            _TextBoxBehavior = new TextBoxBehavior(richTextBox, editorArea, RTBContainer);
            // Khởi tạo ViewManagment
            viewManagment = new ViewManagment(rulerCanvas, statusBar, statusBarItem, richTextBox, unitComboBox, RTBContainer, zoomSlider);
            //Home
            clipboardManager = new ClipboardManager(richTextBox);
            fontManager = new FontManager(richTextBox);
            paragraphManager = new ParagraphManager(richTextBox);
            insertManager = new InsertManager(richTextBox); 
            editingManager = new EditingManager(richTextBox);


            // Cài đặt kiểu chữ và kích cỡ mặc định
            SettingFontType(fontManager);
            SettingFontSize(fontManager);
        }
        #region Events
        private void UnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewManagment.UpdateRuler();
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //if (zoomSlider.IsLoaded)
            //{
            //    double newValue = Math.Round(e.NewValue / 10) * 10; // Làm tròn đến bội số của 10
            //    zoomSlider.Value = newValue;
            //}
            if (viewManagment != null)
            {
                viewManagment.SetZoom(((Slider)sender).Value / 100.0);
                zoomPercent.Content = zoomSlider.Value.ToString() + "%";
            }
        }

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsTextChanged = true;


        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _PrintManager.AdjustDockPanelToPageSetup();
          
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //_TextBoxBehavior.UpdateCustomScrollBar();
        }

        private void cbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fontManager.ChangeFontFamily(cbFontFamily.SelectedItem.ToString());
        }

        private void cbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (double.TryParse(cbFontSize.SelectedItem.ToString(), out double fontSize))
                {
                    fontManager.ChangeFontSize(fontSize);
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());
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
            // Tạm thời gỡ kết nối sự kiện cập nhật font size
            cbFontSize.SelectionChanged -= cbFontSize_SelectionChanged;

            // Cập nhật kích cỡ font đang hiển thị nếu có thay đổi về kích thước (Chỉ thay đổi text của size đang hiển thị)
            cbFontSize.Text = newSize.ToString();

            // Đăng ký lại sự kiện cập nhật font size
            cbFontSize.SelectionChanged += cbFontSize_SelectionChanged;
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
        private void richTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            insertManager.OpenDocumentIfLinkIconClicked(sender, e);
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

            ParagraphManager.lineSpacing = float.Parse(selectedValue);
            paragraphManager.SetLineSpacingWithSpacingAfterParagraphs(ParagraphManager.lineSpacing, checkAdd10pt);
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            paragraphManager.SetLineSpacingWithSpacingAfterParagraphs(ParagraphManager.lineSpacing, true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            paragraphManager.SetLineSpacingWithSpacingAfterParagraphs(ParagraphManager.lineSpacing, false);
        }

        private void richTextBox_LayoutUpdated(object sender, EventArgs e)
        {

        }

        #endregion

        #region ShortCuts
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //Save
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _SaveManager.Save();
                e.Handled = true;
                //System.Windows.MessageBox.Show("Ctrl+S shortcut triggered!", "Shortcut Example", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _NewManager.CreateNew();
                e.Handled = true;
                //System.Windows.MessageBox.Show("Ctrl+N shortcut triggered!", "Shortcut Example", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _OpenManager.Open();
                e.Handled = true;
                //System.Windows.MessageBox.Show("Ctrl+O shortcut triggered!", "Shortcut Example", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            if (e.Key == Key.P && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _PrintManager.PrintRichTextBoxContent();
                e.Handled = true;
                //System.Windows.MessageBox.Show("Ctrl+P shortcut triggered!", "Shortcut Example", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        #endregion

        #region ClickEvent
        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _NewManager.CreateNew();
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _OpenManager.Open();
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _SaveManager.Save();
        }

        private void SaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _SaveManager.SaveAs();
        }

        private void PrintMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _PrintManager.PrintRichTextBoxContent();
        }
        private void QuickPrintMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _PrintManager.QuickPrint();
        }

        private void PrintPreviewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _PrintManager.ShowPrintPreview();
        }

        private void PageSetupMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _PrintManager.ShowPageSetupDialog();
        }

        private void SendEmailMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _SendEmailManager.SendEmail();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.MessageBox.Show("WordPad Clone\nPhiên bản 1.0\nĐược phát triển bởi Nhóm 7",
                            "Giới thiệu",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra có cần lưu thay đổi trước khi thoát
            if (_NewManager.ConfirmSaveChanges())
            {
                System.Windows.Application.Current.Shutdown(); // Thoát ứng dụng
            }
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            viewManagment.ZoomIn();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            viewManagment.ZoomOut();
        }

        private void btnResetZoom_Click(object sender, RoutedEventArgs e)
        {
            viewManagment.Set100();
        }

        private void btnDecreaseZoom_Click(object sender, RoutedEventArgs e)
        {
            viewManagment.ZoomOut();
        }

        private void btnIncreaseZoom_Click(object sender, RoutedEventArgs e)
        {
            viewManagment.ZoomIn();
        }

        private void ToggleRuler_Click(object sender, RoutedEventArgs e)
        {
            viewManagment.ShowRuler(rulerCanvas.Visibility != Visibility.Visible);
        }

        private void ToggleStatusBar_Click(object sender, RoutedEventArgs e)
        {
            viewManagment.ShowStatusBar(statusBar.Visibility != Visibility.Visible);
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
                insertManager.InsertDateTime(dateAndTimeWindow.chosenDateTime);       
        }

        private void btnInsertObject_Click(object sender, RoutedEventArgs e)
        {
            InsertObjectWindow insertObjectWindow = new InsertObjectWindow(insertManager,clipboardManager);
            if(insertObjectWindow.ShowDialog() == true) { }
                
        }
        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            editingManager.SelectAllText();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            FindWindow findWindow = new FindWindow(editingManager);
            findWindow.Show();
        }

        private void btnReplace_Click(object sender, RoutedEventArgs e)
        {
            ReplaceWindow replaceWindow = new ReplaceWindow(editingManager);
            replaceWindow.Show();
        }
        #endregion


    }
}
