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
    public partial class FindWindow : Window
    {
        private EditingManager editingManager;
        public FindWindow(EditingManager editingManager)
        {
            InitializeComponent();
            this.editingManager = editingManager;
        }

        private void btnFindNext_Click(object sender, RoutedEventArgs e)
        {
            // Lấy giá trị từ các điều khiển trong FindForm
            string searchText = txtSearchText.Text;

            // False nếu IsCheck là null hoặc false
            bool matchCase = chkMatchCase.IsChecked == true; 
            bool matchWholeWord = chkMatchWholeWordOnly.IsChecked == true;

            // Gọi hàm FindText trong EditingManager để thực hiện tìm kiếm
            editingManager.FindNext(searchText, matchCase, matchWholeWord);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();  // Đóng form FindForm
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                this.Close();  // Đóng form FindForm
            }        
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
