using System.Windows;

namespace Wordpad.Files
{
    public partial class PrintPageNumberWindow : Window
    {
        public bool PrintPageNumberSelected => chkPrintPageNumber.IsChecked == true;
        public PrintPageNumberWindow()
        {
            InitializeComponent();
        }
        // Sự kiện khi nhấn nút OK
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            // Đóng hộp thoại với kết quả OK
            this.DialogResult = true;
        }

        // Sự kiện khi nhấn nút Cancel
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Đóng hộp thoại với kết quả Cancel
            this.DialogResult = false;
        }
    }
}
