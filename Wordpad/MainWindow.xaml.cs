using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using Wordpad.Files;
using WordPad;

namespace Wordpad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NewManager _NewManager;
        OpenManager _OpenManager;
        PrintManager _PrintManager;
        SaveManager _SaveManager;
        SendEmailManager _SendEmailManager;
        TextBoxBehavior _TextBoxBehavior;
        public static bool IsTextChanged;
        public static float lineSpacing = 1.0f;
        public MainWindow()
        {
            InitializeComponent();
            _NewManager = new NewManager(richTextBox, this);
            _OpenManager = new OpenManager(richTextBox);
            _PrintManager = new PrintManager(RTBContainer, richTextBox);
            _SaveManager = new SaveManager(richTextBox);
            _SendEmailManager = new SendEmailManager(richTextBox);
            _TextBoxBehavior = new TextBoxBehavior(richTextBox, customScrollBar, editorArea, RTBContainer);

        }
        #region SetLineSpacing
        // Hàm Set Line Spacing
        public void SetLineSpacingWithSpacingAfterParagraphs(float lineSpacing, bool addSpacingAfterParagraphs = false)
        {
            // Kiểm tra giá trị lineSpacing có hợp lệ không (chỉ cho phép các giá trị 1.0, 1.25, 1.5, 2.0)
            if (lineSpacing != 1.0f && lineSpacing != 1.25f && lineSpacing != 1.5f && lineSpacing != 2.0f)
            {
                throw new ArgumentException("Invalid line spacing value. Valid values are 1.0, 1.25, 1.5, and 2.0.");
            }

            // Lấy đối tượng Selection từ RichTextBox để xem có đoạn văn nào được chọn không
            var selection = richTextBox.Selection;

            // Kiểm tra nếu không có đoạn văn nào được chọn (selection.IsEmpty)
            if (selection.IsEmpty)
            {
                // Nếu không có phần văn bản nào được chọn, chúng ta chỉ xử lý đoạn văn tại vị trí con trỏ
                Paragraph para = selection.Start.Paragraph;

                // Thiết lập khoảng cách dòng cho đoạn văn tại vị trí con trỏ
                para.LineHeight = para.FontSize * lineSpacing;

                // Nếu addSpacingAfterParagraphs == true, thêm 10pt khoảng cách dưới đoạn văn
                if (addSpacingAfterParagraphs)
                {
                    // Đặt Margin.Bottom (khoảng cách dưới đoạn văn) thêm 10pt
                    para.Margin = new Thickness(para.Margin.Left, para.Margin.Top, para.Margin.Right, para.Margin.Bottom + 10);
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
                foreach (var block in richTextBox.Document.Blocks)
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
                    selectedParagraph.LineHeight = selectedParagraph.FontSize * lineSpacing;

                    // Nếu addSpacingAfterParagraphs == true, thêm 10pt khoảng cách dưới đoạn văn
                    if (addSpacingAfterParagraphs)
                    {
                        selectedParagraph.Margin = new Thickness(selectedParagraph.Margin.Left, selectedParagraph.Margin.Top, selectedParagraph.Margin.Right, selectedParagraph.Margin.Bottom + 10);
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
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lấy giá trị khoảng cách dòng từ ComboBox
            var selectedItem = LineSpacingComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                // Lấy giá trị khoảng cách dòng từ nội dung của ComboBoxItem
                string lineSpacingValue = selectedItem.Content.ToString();

                // Chuyển đổi giá trị chọn thành float
                lineSpacing = float.Parse(lineSpacingValue);

                // Gọi hàm để thiết lập khoảng cách dòng cho RichTextBox
                SetLineSpacingWithSpacingAfterParagraphs(lineSpacing, true); // Thêm khoảng cách dưới đoạn văn
            }
        }
        #endregion

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsTextChanged = true;


        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //_PrintManager.AdjustDockPanelToPageSetup();
        }

        #region
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
            _PrintManager.PrintDoc();
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

        #endregion


    }
}
