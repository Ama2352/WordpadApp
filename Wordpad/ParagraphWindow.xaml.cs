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
        public ParagraphWindow()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            // Lấy giá trị từ các TextBox và ComboBox
            IndentLeft = double.Parse(LeftTextBox.Text) * 96;
            IndentRight = double.Parse(RightTextBox.Text) * 96;
            FirstLineIndent = double.Parse(FirstLineTextBox.Text) * 96;

            LineSpacing = float.Parse(cbLineSpacing.Text);
            AddSpacingAfterParagraphs = SpacingCheckBox.IsChecked == true;

            string alignment = (cbAlignment.SelectedItem as ComboBoxItem)?.Content.ToString()?.Trim();

            if (string.Equals(alignment, "Left", StringComparison.OrdinalIgnoreCase))
            {
                Alignment = TextAlignment.Left;
            }
            else if (string.Equals(alignment, "Right", StringComparison.OrdinalIgnoreCase))
            {
                Alignment = TextAlignment.Right;
            }
            else if (string.Equals(alignment, "Center", StringComparison.OrdinalIgnoreCase))
            {
                Alignment = TextAlignment.Center;
            }
            else if (string.Equals(alignment, "Justified", StringComparison.OrdinalIgnoreCase))
            {
                Alignment = TextAlignment.Justify;
            }
            else
            {
                Alignment = TextAlignment.Left; // Mặc định
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
    }
}
