using System.Windows;
using System.Windows.Controls;
using WordPadApp;

namespace WordPad
{
    public partial class MainWindow : Window
    {
        private ViewManagment viewManagment;

        public MainWindow()
        {
            InitializeComponent();

            // Khởi tạo ViewManagment
            viewManagment = new ViewManagment(rulerCanvas, statusBar, statusBarItem, textBox, unitComboBox);
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            viewManagment.ZoomIn();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            viewManagment.ZoomOut();
        }

        private void UnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewManagment.UpdateRuler();
        }

        private void ToggleRuler_Click(object sender, RoutedEventArgs e)
        {
            viewManagment.ShowRuler(rulerCanvas.Visibility != Visibility.Visible);
        }

        private void ToggleStatusBar_Click(object sender, RoutedEventArgs e)
        {
            viewManagment.ShowStatusBar(statusBar.Visibility != Visibility.Visible);
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (viewManagment != null)
            {
                viewManagment.SetZoom((int)((Slider)sender).Value);
            }
        }
    }
}
