using System;
using System.IO;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls; // Thư viện của WPF (MessageBox, v.v.)
using System.Windows.Documents;

namespace Wordpad.Files
{
    internal class SendEmailManager
    {
        private readonly RichTextBox richTextBox1;
        SaveManager saveManager;

        // Constructor để khởi tạo đối tượng với RichTextBox
        public SendEmailManager(RichTextBox rtx, SaveManager saveManager)
        {
            richTextBox1 = rtx; // Gán RichTextBox vào biến
            this.saveManager = saveManager;
        }

        // Phương thức gửi email
        public void SendEmail()
        {
            // Tạo một form để nhập thông tin gửi email
            EmailForm emailForm = new EmailForm(saveManager);

            // Kiểm tra nếu người dùng nhấn OK trong EmailForm
            if (emailForm.ShowDialog() == true) // ShowDialog() trong WPF trả về bool
            {
                // Lấy thông tin từ form email
                string fromEmail = emailForm.FromEmail;
                string toEmail = emailForm.ToEmail;
                string password = emailForm.Password;
                string filePath = emailForm.filePath;

                try
                {
                    // Tạo đối tượng MailMessage để cấu hình email
                    MailMessage mail = new MailMessage
                    {
                        From = new MailAddress(fromEmail), // Địa chỉ email người gửi
                        Subject = "Document from WordPad", // Tiêu đề email
                        Body = GetRichTextBoxContentAsPlainText() // Lấy nội dung RichTextBox
                    };
                    mail.To.Add(toEmail); // Thêm người nhận vào email

                    Attachment attachment = new Attachment(filePath);
                    mail.Attachments.Add(attachment);

                    // Cấu hình SMTP client để gửi email (Sử dụng Gmail trong ví dụ này)
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587, // Cổng sử dụng cho SMTP với SSL
                        Credentials = new System.Net.NetworkCredential(fromEmail, password), // Thông tin đăng nhập
                        EnableSsl = true, // Bật SSL để bảo mật kết nối
                    };

                    // Gửi email
                    smtpClient.Send(mail);

                    // Thông báo gửi email thành công
                    MessageBox.Show("Email has been sent successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    // Nếu có lỗi xảy ra, hiển thị thông báo lỗi
                    MessageBox.Show("An error occurred while sending the email: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Hàm hỗ trợ để lấy nội dung RichTextBox dưới dạng plain text
        private string GetRichTextBoxContentAsPlainText()
        {
            TextRange textRange = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd);
            return textRange.Text.Trim(); // Loại bỏ khoảng trắng không cần thiết
        }
    }
}
