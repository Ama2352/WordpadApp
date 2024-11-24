using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Wordpad
{
    public partial class ResizeWindow : Window
    {
        private Image resizedImage;

        private bool isUpdating = false; // Biến để ngăn vòng lặp

        // Kích thước gốc của ảnh (Pixel)
        private int originalWidth;
        private int originalHeight;

        // Kích thước điều chỉnh
        public double newWidth { get; private set; }
        public double newHeight { get; private set; }

        // Dictionary lưu trữ tỉ lệ điều chỉnh của từng khung ảnh
        public static Dictionary<Image, KeyValuePair<int, int>> aspectRatios { get; private set; }
           = new Dictionary<Image, KeyValuePair<int, int>>();

        // Tỉ lệ khung hình gốc giữa rtb với ảnh
        private double originalAspectRatio;

        // Dictionary đánh dấu ảnh đã được điều chỉnh lần nào chưa
        public static Dictionary<Image, bool> isFirstTimes = new Dictionary<Image, bool>();

        public ResizeWindow()
        {
            InitializeComponent();
        }

        public ResizeWindow(Image image, double rtbWidth, double rtbHeight, double aspectRatio)
        {
            InitializeComponent();

            resizedImage = image;
            originalAspectRatio = aspectRatio;
            GetOriginalSize();

            DisplayCurrentSize();

            numericUpDownHorizontal.ValueChanged += NumericUpDown_ValueChanged;
            numericUpDownVertical.ValueChanged += NumericUpDown_ValueChanged;
        }

        private void GetOriginalSize()
        {
            BitmapImage originalSize = resizedImage.Source as BitmapImage;
            originalWidth = originalSize.PixelWidth;
            originalHeight = originalSize.PixelHeight;
        }

        private void NumericUpDown_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (isUpdating) return; // Kiểm tra nếu đang cập nhật thì return

            isUpdating = true; // Đánh dấu là đang cập nhật giá trị

            bool aspectRatioLocked = chkLockAspectRatio.IsChecked ?? false;

            if (aspectRatioLocked)
            {
                if (sender == numericUpDownHorizontal)
                {
                    numericUpDownVertical.Value = numericUpDownHorizontal.Value;
                }
                else if (sender == numericUpDownVertical)
                {
                    numericUpDownHorizontal.Value = numericUpDownVertical.Value;
                }
            }

            isUpdating = false; // Đánh dấu kết thúc phiên cập nhật
        }

        // Phương thức kiểm tra khung ảnh có được resize lần nào chưa
        private bool isFirstTimeResized(Image image)
        {
            // Nếu chưa tồn tại thì tạo mới
            if (!isFirstTimes.ContainsKey(image))
            {
                isFirstTimes[image] = true;
                return true;
            }

            // Nếu tồn tại và chưa resize lần nào
            if (!isFirstTimes[image])
            {
                isFirstTimes[image] = true;
                return true;
            }

            // Nếu đã resize 
            return false;
        }

        // Phương thức xác định tỉ lệ hiện tại của khung ảnh so với rtb
        private void DisplayCurrentSize()
        {
            bool checkIfFirstTimeResized = isFirstTimeResized(resizedImage);

            if (checkIfFirstTimeResized && resizedImage != null)
            {
                numericUpDownHorizontal.Value = (int)Math.Round(originalAspectRatio * 100);
                numericUpDownVertical.Value = (int)Math.Round(originalAspectRatio * 100);

                aspectRatios[resizedImage] =
                    new KeyValuePair<int, int>(
                        (int)numericUpDownHorizontal.Value,
                        (int)numericUpDownVertical.Value);
            }
            else if (!checkIfFirstTimeResized && resizedImage != null)
            {
                if (aspectRatios.ContainsKey(resizedImage))
                {
                    int horizontalValue = aspectRatios[resizedImage].Key;
                    int verticalValue = aspectRatios[resizedImage].Value;

                    numericUpDownHorizontal.Value = horizontalValue;
                    numericUpDownVertical.Value = verticalValue;
                }
                else
                {
                    MessageBox.Show("Please choose a valid image.", "Resize Image", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            newWidth = ((double)numericUpDownHorizontal.Value / 100) * originalWidth;
            newHeight = ((double)numericUpDownVertical.Value / 100) * originalHeight;

            aspectRatios[resizedImage] =
                new KeyValuePair<int, int>(
                    (int)numericUpDownHorizontal.Value,
                   (int)numericUpDownVertical.Value);

            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
