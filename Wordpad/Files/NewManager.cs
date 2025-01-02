using System.Windows; // Thay thế MessageBox của Windows Forms
using System.Windows.Controls; // Thay thế RichTextBox của WPF

namespace Wordpad
{
    internal class NewManager
    {
        private RichTextBox richTextBox1; // RichTextBox của WPF
        private SaveManager saveManager;

        public NewManager(RichTextBox RTX)
        {
            richTextBox1 = RTX;
            saveManager = new SaveManager(RTX);
        }

        public void CreateNew()
        {
            // Kiểm tra xem có cần lưu thay đổi trước khi tạo tài liệu mới
            if (ConfirmSaveChanges())
            {
                richTextBox1.Document.Blocks.Clear(); // Xóa nội dung hiện tại trong RichTextBox (WPF)
                SaveManager.CurrentFilePath = string.Empty; // Đặt lại đường dẫn file hiện tại
                MainWindow.IsTextChanged = false; // Đặt lại trạng thái thay đổi
            }
        }

        // Xác nhận người dùng có muốn lưu thay đổi trước khi thực hiện thao tác
        public bool ConfirmSaveChanges()
        {
            // Nếu có thay đổi chưa lưu
            if (MainWindow.IsTextChanged)
            {
                // Hiển thị hộp thoại xác nhận
                MessageBoxResult result = MessageBox.Show(
                    "Do you want to save changes to Document?", // Thông điệp hiển thị
                    "Save Changes", // Tiêu đề hộp thoại
                    MessageBoxButton.YesNoCancel, // Các nút lựa chọn
                    MessageBoxImage.Warning // Biểu tượng cảnh báo
                );

                // Nếu người dùng chọn "Yes", gọi phương thức lưu file
                if (result == MessageBoxResult.Yes)
                {
                    saveManager.Save();
                }
                // Nếu chọn "Cancel", hủy thao tác
                else if (result == MessageBoxResult.Cancel)
                {
                    return false; // Không thực hiện thao tác
                }
            }
            return true; // Nếu không có thay đổi hoặc người dùng chọn "No"
        }
    }
}
