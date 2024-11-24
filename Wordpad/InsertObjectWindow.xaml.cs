using Microsoft.Win32;
using System;
using System.Windows;
using WordPad;
using System.IO;

namespace Wordpad
{
    public partial class InsertObjectWindow : Window
    {
        private InsertManager _insertManager;
        private ClipboardManager _clipboardManager;
        private string directoryOfIcon = null;
        private string fileType = null;

        public InsertObjectWindow()
        {
            InitializeComponent();
        }

        public InsertObjectWindow(InsertManager insertManager, ClipboardManager clipboardManager)
        {
            InitializeComponent();

            _insertManager = insertManager; // Lưu trữ tham chiếu đến Insert Manager
            _clipboardManager = clipboardManager;

            _insertManager.AddObjectTypes(listOptions);

            listOptions.SelectionChanged += ListBoxOptions_SelectedIndexChanged;

        }

        private void ListBoxOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listOptions.SelectedItem != null)
            {
                string selectedOption = listOptions.SelectedItem.ToString();
                var (description, illustration, icon) = _clipboardManager.GetOptionDetails(selectedOption);
                
                txtDescription.Text = description;
                imgIllustration.Source = illustration;
                imgDisplayIcon.Source = icon;
            }
        }

        private void radCreateNew_Checked(object sender, RoutedEventArgs e)
        {
            if(panelCreateFromFile != null)
            {
                panelCreateFromFile.Visibility = Visibility.Collapsed;
                panelCreateNew.Visibility = Visibility.Visible;
            }                  
        }

        private void radCreateFromFile_Checked(object sender, RoutedEventArgs e)
        {
            if(panelCreateFromFile != null)
            {
                panelCreateFromFile.Visibility = Visibility.Visible;
                panelCreateNew.Visibility= Visibility.Collapsed;    

                txtDescription.Text = "Inserts the contents of the file as an object into your document so that you " +
                    "may active it using the program which create it.";
                string imagePath = Path.Combine(ClipboardManager.imageDirectory, "paste.png");
                imgIllustration.Source = _clipboardManager.LoadImage(imagePath);
            }    
           
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (radCreateNew.IsChecked == true)
            {
                if (listOptions.SelectedItem != null)
                {
                    string selectedOption = listOptions.SelectedItem.ToString();
                    // Chèn icon link để mở các app office có trong list box
                    _insertManager.InsertObjectAsIcon(selectedOption, null);
                }
            }
            else if (radCreateFromFile.IsChecked == true && directoryOfIcon != null) 
            {
                // Chèn icon link đến bất kì app nào muốn mở 
                _insertManager.InsertObjectAsIcon(fileType, directoryOfIcon);
            }   

            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            // Mở hộp thoại OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Chọn tệp",
                Filter = "All Files|*.*" // Lọc định dạng tệp
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Lưu thư mục của đường dẫn tệp vào Settings
                string selectedDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                Properties.Settings.Default.LastPath = selectedDirectory;
                Properties.Settings.Default.Save(); // Lưu lại setting

                // Đưa đường dẫn tệp vào TextBox
                txtPath.Text = openFileDialog.FileName;

                // Lấy phần mở rộng của tệp
                string extension = Path.GetExtension(openFileDialog.FileName).ToLower();

                // Xác định loại file dựa trên phần mở rộng
                fileType = "Không xác định";
                switch (extension)
                {
                    case ".doc":
                    case ".docx":
                        fileType = "Microsoft Word Document";
                        break;
                    case ".xls":
                    case ".xlsx":
                        fileType = "Microsoft Excel";
                        break;
                    case ".ppt":
                    case ".pptx":
                        fileType = "Microsoft PowerPoint Presentation";
                        break;
                    case ".pdf":
                        fileType = "Foxit PhantomPDF Document";
                        break;
                    default:
                        fileType = "HTML Document";
                        break;
                }

                // Hiển thị loại file trong lblFileType
                lblFileType.Content = "File: " + fileType;

                // Chèn icon link đến đối tượng file được chọn
                directoryOfIcon = openFileDialog.FileName;               
            }

        }


    }
}
