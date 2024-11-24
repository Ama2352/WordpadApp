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
using Wordpad;

namespace Wordpad
{
    /// <summary>
    /// Interaction logic for DateAndTimeWindow.xaml
    /// </summary>
    public partial class DateAndTimeWindow : Window
    {
        public string chosenDateTime { get; private set; }
        public DateAndTimeWindow(InsertManager insertManager)
        {
            InitializeComponent();

            insertManager.DisplayDateTimeFormats(listBoxDateTime);
        }

        public DateAndTimeWindow()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxDateTime.SelectedIndex == -1) // Nếu không chọn định dạng nào
            {
                MessageBox.Show("Please select a format.", "Confirm Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                chosenDateTime = listBoxDateTime.SelectedItem.ToString();

                this.DialogResult = true;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
