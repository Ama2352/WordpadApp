﻿using System.Windows;
using System.Windows.Controls;

namespace Wordpad.Files
{
    public partial class EmailForm : Window
    {
        public string FromEmail { get; private set; }
        public string ToEmail { get; private set; }
        public string Password { get; private set; }

        public EmailForm()
        {
            InitializeComponent();
        }

        private void SendEmail()
        {
            // Validate that both fields are filled in
            if (string.IsNullOrEmpty(txtFrom.Text) || string.IsNullOrEmpty(txtTo.Text))
            {
                MessageBox.Show("Please enter both the sender and recipient email addresses.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Assign the input values to the properties
            FromEmail = txtFrom.Text;
            ToEmail = txtTo.Text;
            Password = txtPassword.Password; // Get the password from PasswordBox

            // Close the form and indicate success
            this.DialogResult = true;
            this.Close();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            SendEmail();
        }

        private void EmailForm_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SendEmail();
            }
        }

        private void chkShowPassword_CheckedChanged(object sender, RoutedEventArgs e)
        {
            // Kiểm tra trạng thái của checkbox
            if (chkShowPassword.IsChecked == true)
            {
                // Ẩn PasswordBox và hiển thị TextBox với mật khẩu
                txtPassword.Visibility = Visibility.Collapsed;
                var txtVisiblePassword = new TextBox
                {
                    Text = txtPassword.Password, // Hiển thị mật khẩu trong TextBox
                    Margin = txtPassword.Margin,
                    VerticalAlignment = txtPassword.VerticalAlignment,
                    HorizontalAlignment = txtPassword.HorizontalAlignment,
                    Width = txtPassword.Width,
                };

                // Đồng bộ hóa mật khẩu khi TextBox mất focus
                txtVisiblePassword.LostFocus += (s, args) =>
                {
                    txtPassword.Password = txtVisiblePassword.Text;
                    txtPassword.Visibility = Visibility.Visible;
                    ((Grid)txtPassword.Parent).Children.Remove(txtVisiblePassword); // Xóa TextBox tạm thời
                };

                // Thêm TextBox vào Grid và focus
                ((Grid)txtPassword.Parent).Children.Add(txtVisiblePassword);
                txtVisiblePassword.Focus();
            }
            else
            {
                // Trả về trạng thái ẩn mật khẩu mặc định
                foreach (var child in ((Grid)txtPassword.Parent).Children)
                {
                    if (child is TextBox txtVisiblePassword)
                    {
                        txtPassword.Password = txtVisiblePassword.Text;
                        txtPassword.Visibility = Visibility.Visible;
                        ((Grid)txtPassword.Parent).Children.Remove(txtVisiblePassword);
                        break;
                    }
                }
            }
        }

    }
}
