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
using Wordpad.View;
using System.Windows.Controls.Primitives;

namespace Wordpad
{
    public partial class MainWindow : Window
    {
        //File
        NewManager _NewManager;
        OpenManager _OpenManager;
        PrintManager _PrintManager;
        SaveManager _SaveManager;
        SendEmailManager _SendEmailManager;
        TextBoxBehavior _TextBoxBehavior;
        public static bool IsTextChanged;
        //View
        ViewManagment viewManagment;
        Ruler ruler;
        //Home
        ClipboardManager clipboardManager;
        FontManager fontManager;
        ParagraphManager paragraphManager;
        InsertManager insertManager;
        EditingManager editingManager;
        //private GlobalDashedLineAdorner _adorner;   //Biến dùng để vẽ gạch đứt global cho thumb ruler

        public MainWindow()
        {
            InitializeComponent();
            // Khởi tạo ViewManagment
            ruler = new Ruler(marginCanvas, tickCanvas, thumbCanvas, rulerCanvas, RTBContainer, null);
            viewManagment = new ViewManagment(statusBar, statusBarItem, richTextBox, unitComboBox, RTBContainer, zoomSlider, ruler);
            //Home
            _NewManager = new NewManager(richTextBox, this);
            _OpenManager = new OpenManager(richTextBox);
            _PrintManager = new PrintManager(RTBContainer, richTextBox, ruler);
            _SaveManager = new SaveManager(richTextBox);
            _SendEmailManager = new SendEmailManager(richTextBox);
            _TextBoxBehavior = new TextBoxBehavior(richTextBox, RTBContainer);

            //Home
            clipboardManager = new ClipboardManager(richTextBox);
            fontManager = new FontManager(richTextBox);
            paragraphManager = new ParagraphManager(richTextBox, ruler);
            insertManager = new InsertManager(richTextBox);
            editingManager = new EditingManager(richTextBox);


            // Cài đặt kiểu chữ và kích cỡ mặc định
            SettingFontType(fontManager);
            SettingFontSize(fontManager);
        }
        #region Events
        private void UnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lấy đơn vị được chọn từ ComboBox
            if (unitComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedUnit = selectedItem.Content.ToString();
                ruler.UpdateUnit(selectedUnit);
            }
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
            //MessageBox.Show($"Dock width: {RTBContainer.Width}");
            //Vẽ ruler lần đầu
            ruler.DrawRuler();
            //ruler.InitializeThumbs();
            //// Trì hoãn việc khởi tạo dashed lines để giao diện hoàn tất
            //Dispatcher.BeginInvoke(new Action(ruler.InitializeDashLines), System.Windows.Threading.DispatcherPriority.Render);
            // Scale toàn bộ rulerCanvas cho bằng dock panel
            rulerCanvas.Width = RTBContainer.Width;
            // Lấy AdornerLayer của toàn bộ cửa sổ
            //Hoãn thời điểm lấy adorner để nó được tạo trước.
            this.Dispatcher.InvokeAsync(() =>
            {
                //    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(textEditor);
                //    if (adornerLayer == null)
                //    {
                //        MessageBox.Show("AdornerLayer is null. Ensure that the AdornerLayer exists in the visual tree.");
                //        return;
                //    }

                //    _adorner = new GlobalDashedLineAdorner(textEditor);
                //    adornerLayer.Add(_adorner);

                //    ruler.SetAdorner(_adorner);
                ruler.SetViewManager(viewManagment);
            });
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //_TextBoxBehavior.UpdateCustomScrollBar();
        }

        #region FontFamilly
        private bool _isTyping = false;
        private void cbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Nếu người dùng đang gõ, không xử lý sự kiện SelectionChanged
                if (_isTyping)
                    return;

                // Tiến hành thay đổi phông chữ khi lựa chọn được thay đổi
                if (cbFontFamily.SelectedItem != null)
                {
                    string selectedFontFamily = cbFontFamily.SelectedItem.ToString();
                    fontManager.ChangeFontFamily(selectedFontFamily);
                }
            }
            catch (Exception ex)
            {
                // Log hoặc xử lý ngoại lệ
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void cbFontFamily_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Khi người dùng gõ văn bản, tạm thời ngừng xử lý SelectionChanged
            _isTyping = true;
        }

        private void cbFontFamily_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Kiểm tra khi người dùng nhấn Enter
            if (e.Key == Key.Enter)
            {
                // Đảm bảo xử lý lựa chọn khi nhấn Enter
                _isTyping = false;

                if (cbFontFamily.SelectedItem != null)
                {
                    string selectedFontFamily = cbFontFamily.SelectedItem.ToString();
                    fontManager.ChangeFontFamily(selectedFontFamily);
                }
            }
        }

        private void cbFontFamily_LostFocus(object sender, RoutedEventArgs e)
        {
            // Xử lý khi mất tiêu điểm (focus)
            _isTyping = false;

            if (cbFontFamily.SelectedItem != null)
            {
                string selectedFontFamily = cbFontFamily.SelectedItem.ToString();
                fontManager.ChangeFontFamily(selectedFontFamily);
            }
        }

        private void cbFontFamily_DropDownOpened(object sender, EventArgs e)
        {
            // Khi dropdown được mở, đảm bảo rằng việc nhập liệu không bị ngừng
            _isTyping = false;
        }
        #endregion

        #region FontSize
        private void cbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Kiểm tra nếu có giá trị được chọn và giá trị có thể chuyển đổi thành double
                if (cbFontSize.SelectedItem != null && double.TryParse(cbFontSize.SelectedItem.ToString(), out double fontSize))
                {
                    fontManager.ChangeFontSize(fontSize); // Thay đổi kích thước font
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và thông báo cho người dùng
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbFontSize_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Kiểm tra nếu phím được nhấn là Enter
            if (e.Key == Key.Enter)
            {
                fontManager.ProcessFontSizeInput(cbFontSize.Text);
            }
        }
        #endregion


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
            cbFontSize.Text = ((int)newSize).ToString();

            // Đăng ký lại sự kiện cập nhật font size
            cbFontSize.SelectionChanged += cbFontSize_SelectionChanged;
        }
        private void OnFontFamilyChanged(string newFont)
        {
            cbFontFamily.SelectionChanged -= cbFontFamily_SelectionChanged;
            cbFontFamily.Text = newFont;
            cbFontFamily.SelectionChanged += cbFontFamily_SelectionChanged;
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

        private void chkAdd10pt_Checked(object sender, RoutedEventArgs e)
        {
            paragraphManager.SetLineSpacingWithSpacingAfterParagraphs(ParagraphManager.lineSpacing, true);
        }

        private void chkAdd10pt_Unchecked(object sender, RoutedEventArgs e)
        {
            paragraphManager.SetLineSpacingWithSpacingAfterParagraphs(ParagraphManager.lineSpacing, false);
        }

        private void richTextBox_LayoutUpdated(object sender, EventArgs e)
        {

        }
        //Điều khiển thanh ruler khi lân chuột ngang
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Đồng bộ HorizontalOffset của ScrollViewer chứa rulerCanvas
            if (e.HorizontalChange != 0)
            {
                rulerScrollViewer.ScrollToHorizontalOffset(RTBSCrollViewer.HorizontalOffset);
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            // Đảm bảo chỉ một ToggleButton được chọn
            if (sender is ToggleButton selectedButton)
            {
                if (!IsLoaded) return;
                btnSpacing1.IsChecked = selectedButton == btnSpacing1;
                btnSpacing1_25.IsChecked = selectedButton == btnSpacing1_25;
                btnSpacing1_5.IsChecked = selectedButton == btnSpacing1_5;
                btnSpacing2.IsChecked = selectedButton == btnSpacing2;
                // Lấy giá trị khoảng cách dòng từ nút được chọn
                if (float.TryParse(selectedButton.Content.ToString(), out float lineSpacing))
                {
                    // Lấy trạng thái của CheckBox "Add 10pt Space"
                    bool add10pt = chkAdd10pt.IsChecked ?? false;

                    // Cập nhật vào ParagraphManager
                    ParagraphManager.lineSpacing = lineSpacing;
                    paragraphManager.SetLineSpacingWithSpacingAfterParagraphs(lineSpacing, add10pt);
                }
            }
        }
        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            if (rulerCanvas != null)
                viewManagment.ShowRuler(rulerCanvas.Visibility != Visibility.Visible);
        }

        private void CheckBox_Checked_2(object sender, RoutedEventArgs e)
        {
            if (rulerCanvas != null)
                viewManagment.ShowStatusBar(statusBar.Visibility != Visibility.Visible);
        }
        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            viewManagment.ShowRuler(rulerCanvas.Visibility != Visibility.Visible);
        }

        private void CheckBox_Unchecked_2(object sender, RoutedEventArgs e)
        {
            viewManagment.ShowStatusBar(statusBar.Visibility != Visibility.Visible);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Kiểm tra xem người dùng có cần lưu hay không
            if (MainWindow.IsTextChanged) // Hàm kiểm tra tài liệu đã được sửa đổi
            {
                // Hiển thị hộp thoại Save
                MessageBoxResult result = MessageBox.Show(
                    "Do you want to save changes to Document?",
                    "Save Changes",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning
                );

                // Xử lý theo lựa chọn của người dùng
                if (result == MessageBoxResult.Yes)
                {
                    _SaveManager.Save(); // Gọi hàm Save
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    // Hủy đóng cửa sổ
                    e.Cancel = true;
                }
                // Nếu chọn No, không làm gì và tiếp tục đóng
            }
        }

        #endregion

        #region ShortCuts
        private bool isCtrlPressed = false;

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Kiểm tra xem phím Ctrl có được nhấn hay không
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                isCtrlPressed = true;  // Đánh dấu phím Ctrl đã được nhấn
                e.Handled = true;  // Ngừng sự kiện tại đây, không cho các đối tượng khác xử lý
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Kiểm tra nếu phím Ctrl được nhấn và xem phím kết hợp với Ctrl
            if (isCtrlPressed)
            {
                // Kiểm tra các phím nhấn kết hợp với Ctrl
                if (e.Key == Key.S)
                {
                    _SaveManager.Save();
                }
                else if (e.Key == Key.N)
                {
                    _NewManager.CreateNew();
                }
                else if (e.Key == Key.O)
                {
                    _OpenManager.Open();                 
                }
                else if (e.Key == Key.P)
                {
                    _PrintManager.PrintRichTextBoxContent();
                }
                else if (e.Key == Key.C)
                {
                    clipboardManager.Copy();
                }
                else if (e.Key == Key.X)
                {
                    clipboardManager.Cut();
                }
                else if (e.Key == Key.V)
                {
                    clipboardManager.Paste();
                }
                isCtrlPressed = false;
                e.Handled = true;
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

            System.Windows.MessageBox.Show("WordPad\nVersion 1.0\nDeveloped by Group 2",
                            "About",
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
            if (dateAndTimeWindow.ShowDialog() == true)
                insertManager.InsertDateTime(dateAndTimeWindow.chosenDateTime);
        }

        private void btnInsertObject_Click(object sender, RoutedEventArgs e)
        {
            InsertObjectWindow insertObjectWindow = new InsertObjectWindow(insertManager, clipboardManager);
            if (insertObjectWindow.ShowDialog() == true) { }

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

        private void btnParagraph_Click_1(object sender, RoutedEventArgs e)
        {
            // Lấy đoạn văn tại vị trí con trỏ hoặc đoạn văn được chọn
            TextPointer caretPosition = richTextBox.CaretPosition;
            Paragraph paragraphAtCaret = caretPosition.Paragraph;

            // Nếu có text được chọn
            TextSelection selection = richTextBox.Selection;
            Paragraph selectedParagraph = selection?.Start.Paragraph ?? paragraphAtCaret;

            if (selectedParagraph != null)
            {
                // Lấy thông tin paragraph
                double leftIndent = selectedParagraph.Margin.Left;
                double rightIndent = selectedParagraph.Margin.Right;
                double firstLineIndent = selectedParagraph.TextIndent;
                TextAlignment alignment = selectedParagraph.TextAlignment;

                // Lấy Line Spacing và kiểm tra Add Spacing After Paragraphs
                double lineSpacing = selectedParagraph.LineHeight > 0
                    ? selectedParagraph.LineHeight / selectedParagraph.FontSize / ParagraphManager.LineHeightMultiplierPublic
                    : 1.0; // Mặc định là 1.0 nếu LineHeight không được đặt.

                bool addSpacingAfterParagraphs = selectedParagraph.Margin.Bottom > 10;

                // Mở ParagraphWindow
                ParagraphWindow paragraphWindow = new ParagraphWindow(
                    leftIndent,
                    rightIndent,
                    firstLineIndent,
                    alignment,
                    lineSpacing,
                    addSpacingAfterParagraphs);

                if (paragraphWindow.ShowDialog() == true) // Người dùng nhấn OK
                {
                    // Lấy giá trị từ ParagraphWindow
                    double leftIndentInput = paragraphWindow.IndentLeft;
                    double rightIndentInput = paragraphWindow.IndentRight;
                    double firstLineIndentInput = paragraphWindow.FirstLineIndent;
                    float lineSpacingInput = paragraphWindow.LineSpacing;
                    bool addSpacingInput = paragraphWindow.AddSpacingAfterParagraphs;
                    TextAlignment alignmentInput = paragraphWindow.Alignment;

                    // Truyền các giá trị này vào ParagraphManager
                    paragraphManager.SetIndentation(leftIndentInput, rightIndentInput, firstLineIndentInput);
                    paragraphManager.SetLineSpacingWithSpacingAfterParagraphs(lineSpacingInput, addSpacingInput);
                    paragraphManager.SetAlignment(alignmentInput);
                }

            }
            //Trường hợp lấy thông tin bị thất bại
            else
            {
                MessageBox.Show("No paragraph available.");
            }

        }
        private void btnPasteSpecial_Click(object sender, RoutedEventArgs e)
        {
            PasteSpecialWindow pasteSpecialWindow = new PasteSpecialWindow(clipboardManager);
            if (pasteSpecialWindow.ShowDialog() == true) { }
        }

        private void QuickUndo_Click(object sender, RoutedEventArgs e)
        {
            if (richTextBox.CanUndo)
                richTextBox.Undo();
        }

        private void QuickRedo_Click(object sender, RoutedEventArgs e)
        {
            if (richTextBox.CanRedo)
                richTextBox.Redo();
        }
        private void QuickSave_Click(object sender, RoutedEventArgs e)
        {
            SaveMenuItem_Click(sender, e);
        }

        #region StartAListClick
        private void btnBullet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                paragraphManager.ApplyBulletStyles("Bullet");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi tại đây, ví dụ log lỗi hoặc hiển thị thông báo lỗi cho người dùng
                MessageBox.Show($"Error applying bullet style: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNumbering_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                paragraphManager.ApplyBulletStyles("Numbering");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying numbering style: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLowerAlphabet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                paragraphManager.ApplyBulletStyles("Alphabet - Lower case");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying lower alphabet style: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUpperAlphabet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                paragraphManager.ApplyBulletStyles("Alphabet - Upper case");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying upper alphabet style: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLowerRoman_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                paragraphManager.ApplyBulletStyles("Roman Numeral - Lower case");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying lower roman numeral style: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUpperRoman_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                paragraphManager.ApplyBulletStyles("Roman Numeral - Upper case");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying upper roman numeral style: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSquare_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                paragraphManager.ApplyBulletStyles("Square");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying square bullet style: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                paragraphManager.ApplyBulletStyles("Circle");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying dot bullet style: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                paragraphManager.ApplyBulletStyles("Box");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying box bullet style: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }






        #endregion

        #endregion


    }
}
