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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wordpad.Files;
using WordPad;

namespace Wordpad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NewManager _NewManager;
        OpenManager _OpenManager;
        PrintManager _PrintManager;
        SaveManager _SaveManager;
        SendEmailManager _SendEmailManager;
        TextBoxBehavior _TextBoxBehavior;
        public static bool IsTextChanged;
        public MainWindow()
        {
            InitializeComponent();
            _NewManager = new NewManager(richTextBox, this);
            _OpenManager = new OpenManager(richTextBox);
            _PrintManager = new PrintManager(richTextBox);
            _SaveManager = new SaveManager(richTextBox);
            _SendEmailManager = new SendEmailManager(richTextBox);
            _TextBoxBehavior = new TextBoxBehavior(richTextBox, customScrollBar, editorArea, RTBContainer);

        }
        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsTextChanged = true;
        }

        #region
        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _NewManager.CreateNew();
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _OpenManager.Open();
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _SaveManager.Save();
        }

        private void SaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _SaveManager.SaveAs();
        }

        private void PrintMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _PrintManager.PrintWithMargins();
        }

        private void PageSetupMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SendEmailMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _SendEmailManager.SendEmail();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {

            MessageBox.Show("WordPad Clone\nPhiên bản 1.0\nĐược phát triển bởi Nhóm 7",
                            "Giới thiệu",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra có cần lưu thay đổi trước khi thoát
            if (_NewManager.ConfirmSaveChanges())
            {
                System.Windows.Application.Current.Shutdown(); // Thoát ứng dụng
            }
        }
        #endregion

    }
}
