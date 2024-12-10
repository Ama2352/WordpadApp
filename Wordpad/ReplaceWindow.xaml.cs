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
    public partial class ReplaceWindow : Window
    {
        private EditingManager editingManager;
        public ReplaceWindow(EditingManager editingManager)
        {
            InitializeComponent();
            this.editingManager = editingManager;
        }

        private void btnFindNext_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txtFindText.Text;

            // False nếu IsCheck là null hoặc false
            bool matchCase = chkMatchCase.IsChecked == true;
            bool matchWholeWord = chkMatchWholeWordOnly.IsChecked == true;

            // Gọi hàm FindText trong EditingManager để thực hiện tìm kiếm
            editingManager.FindNext(searchText, matchCase, matchWholeWord);
        }

        private void btnReplace_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txtFindText.Text;
            string replaceText = txtReplaceText.Text;

            // False nếu IsCheck là null hoặc false
            bool matchCase = chkMatchCase.IsChecked == true;
            bool matchWholeWord = chkMatchWholeWordOnly.IsChecked == true;

            // Gọi hàm FindText trong EditingManager để thực hiện tìm kiếm
            editingManager.Replace(searchText, replaceText, matchCase, matchWholeWord);
        }

        private void btnReplaceAll_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txtFindText.Text;
            string replaceText = txtReplaceText.Text;

            // False nếu IsCheck là null hoặc false
            bool matchCase = chkMatchCase.IsChecked == true;
            bool matchWholeWord = chkMatchWholeWordOnly.IsChecked == true;

            // Gọi hàm FindText trong EditingManager để thực hiện tìm kiếm
            editingManager.ReplaceAll(searchText, replaceText, matchCase, matchWholeWord);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
