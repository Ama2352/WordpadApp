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
using System.Windows.Shapes;

namespace Wordpad
{
    /// <summary>
    /// Interaction logic for ParagraphWindow.xaml
    /// </summary>
    public partial class ParagraphWindow : Window
    {
        public double IndentLeft { get; private set; }
        public double IndentRight { get; private set; }
        public double FirstLineIndent { get; private set; }
        public float LineSpacing { get; private set; }
        public bool AddSpacingAfterParagraphs { get; private set; }
        public TextAlignment Alignment { get; private set; }
        public ParagraphWindow(double leftIndent, double rightIndent, double firstLineIndent, TextAlignment alignment,
            double lineSpacing, bool addSpacingAfterParagraphs)
        {
            InitializeComponent();

            // Gán các giá trị truyền vào cho các UI element
            LeftTextBox.Text = (leftIndent / 96).ToString(); // Chuyển đổi từ pixel sang inch
            RightTextBox.Text = (rightIndent / 96).ToString();
            FirstLineTextBox.Text = (firstLineIndent / 96).ToString();

            // Gán giá trị cho ComboBox Alignment
            switch (alignment)
            {
                case TextAlignment.Left:
                    cbAlignment.SelectedIndex = 0;
                    break;
                case TextAlignment.Right:
                    cbAlignment.SelectedIndex = 1;
                    break;
                case TextAlignment.Center:
                    cbAlignment.SelectedIndex = 2;
                    break;
                case TextAlignment.Justify:
                    cbAlignment.SelectedIndex = 3;
                    break;
            }

            // Thiết lập Add Spacing After Paragraphs
            SpacingCheckBox.IsChecked = addSpacingAfterParagraphs;

            // Thiết lập Alignment
            cbAlignment.SelectedItem = alignment.ToString();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            // Lấy giá trị từ các TextBox và ComboBox
            IndentLeft = double.Parse(LeftTextBox.Text) * 96;
            IndentRight = double.Parse(RightTextBox.Text) * 96;
            FirstLineIndent = double.Parse(FirstLineTextBox.Text) * 96;

            LineSpacing = float.Parse(cbLineSpacing.Text);
            AddSpacingAfterParagraphs = SpacingCheckBox.IsChecked == true;

            // Lấy giá trị Alignment
            switch (cbAlignment.SelectedIndex)
            {
                case 0:
                    Alignment = TextAlignment.Left;
                    break;
                case 1:
                    Alignment = TextAlignment.Right;
                    break;
                case 2:
                    Alignment = TextAlignment.Center;
                    break;
                case 3:
                    Alignment = TextAlignment.Justify;
                    break;
                default:
                    Alignment = TextAlignment.Left;
                    break;
            }


            // Đóng cửa sổ với kết quả OK
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Thoát bằng phím ESC
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
