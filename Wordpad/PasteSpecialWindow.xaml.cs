using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Wordpad
{
    public partial class PasteSpecialWindow : Window
    {
        ClipboardManager clipboardManager;
        private DispatcherTimer clipboardCheckTimer;
        public PasteSpecialWindow(ClipboardManager clipboardManager)
        {
            InitializeComponent();
            this.clipboardManager = clipboardManager;

            LoadPasteOptions();

            InitializeTimer(); // Gọi hàm khởi tạo timer
        }

        public void InitializeTimer()
        {
            // Khởi tạo và cấu hình Timer
            clipboardCheckTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // Kiểm tra mỗi giây
            };

            clipboardCheckTimer.Tick += ClipboardCheckTimer_Tick;
            clipboardCheckTimer.Start();
        }

        private void ClipboardCheckTimer_Tick(object sender, EventArgs e)
        {
            LoadPasteOptions(); // Cập nhật lại các tùy chọn dán
        }

        private void LoadPasteOptions()
        {
            // Lấy các tùy chọn dán từ clipboard
            var options = clipboardManager.GetPasteOptions();

            // So sánh danh sách mới với danh sách hiện tại để tránh làm mới nếu không cần thiết
            if (!listOptions.Items.Cast<string>().SequenceEqual(options))
            {
                listOptions.Items.Clear(); // Xóa các mục cũ
                foreach (var option in options)
                {
                    listOptions.Items.Add(option);
                }
            }
        }

        private void listOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listOptions.SelectedItem != null)
            {
                string selectedOption = listOptions.SelectedItem.ToString();
                var (description, illustration, displayIcon) = clipboardManager.GetOptionDetails(selectedOption);
                txtDescription.Text = description;
                imgIllustration.Source = illustration;
            }
        }
        
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (listOptions.IsEnabled == true && listOptions.SelectedItem == null)
            {
                MessageBox.Show("Please select a paste option.", "Paste Special", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string selectedOption = listOptions.SelectedItem?.ToString();
            clipboardManager.PasteSpecial(selectedOption);

            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Dừng Timer khi hộp thoại đóng
            clipboardCheckTimer.Stop();
            this.DialogResult = false;
            this.Close();
        }
    }
}
