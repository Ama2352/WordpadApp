using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using Wordpad.Files;
using Wordpad;
//using WordpadApp;

namespace Wordpad
{
    public partial class MainWindow : Window
    {
        NewManager _NewManager;
        OpenManager _OpenManager;
        PrintManager _PrintManager;
        SaveManager _SaveManager;
        SendEmailManager _SendEmailManager;
        TextBoxBehavior _TextBoxBehavior;
        public static bool IsTextChanged;
        private ViewManagment viewManagment;

        public MainWindow()
        {
            InitializeComponent();
            _NewManager = new NewManager(richTextBox, this);
            _OpenManager = new OpenManager(richTextBox);
            _PrintManager = new PrintManager(RTBContainer, richTextBox);
            _SaveManager = new SaveManager(richTextBox);
            _SendEmailManager = new SendEmailManager(richTextBox);
            _TextBoxBehavior = new TextBoxBehavior(richTextBox, customScrollBar, editorArea, RTBContainer);
            // Khởi tạo ViewManagment
            viewManagment = new ViewManagment(rulerCanvas, statusBar, statusBarItem, richTextBox, unitComboBox);
        }

        private void UnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewManagment.UpdateRuler();
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (viewManagment != null)
            {
                viewManagment.SetZoom((int)((Slider)sender).Value);
            }
        }

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsTextChanged = true;


        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _PrintManager.AdjustDockPanelToPageSetup();
          
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _TextBoxBehavior.UpdateCustomScrollBar();
        }
        #region ShortCuts
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //Save
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _SaveManager.Save();
                e.Handled = true;
                //System.Windows.MessageBox.Show("Ctrl+S shortcut triggered!", "Shortcut Example", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _NewManager.CreateNew();
                e.Handled = true;
                //System.Windows.MessageBox.Show("Ctrl+N shortcut triggered!", "Shortcut Example", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _OpenManager.Open();
                e.Handled = true;
                //System.Windows.MessageBox.Show("Ctrl+O shortcut triggered!", "Shortcut Example", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            if (e.Key == Key.P && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _PrintManager.PrintDoc();
                e.Handled = true;
                //System.Windows.MessageBox.Show("Ctrl+P shortcut triggered!", "Shortcut Example", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        #endregion
        #region ClickEvent
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
            _PrintManager.PrintDoc();
        }
        private void QuickPrintMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _PrintManager.QuickPrint();
        }

        private void PrintPreviewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _PrintManager.ShowPrintPreview();
        }

        private void PageSetupMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _PrintManager.ShowPageSetupDialog();
        }

        private void SendEmailMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _SendEmailManager.SendEmail();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.MessageBox.Show("WordPad Clone\nPhiên bản 1.0\nĐược phát triển bởi Nhóm 7",
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

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            viewManagment.ZoomIn();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            viewManagment.ZoomOut();
        }

        private void ToggleRuler_Click(object sender, RoutedEventArgs e)
        {
            viewManagment.ShowRuler(rulerCanvas.Visibility != Visibility.Visible);
        }

        private void ToggleStatusBar_Click(object sender, RoutedEventArgs e)
        {
            viewManagment.ShowStatusBar(statusBar.Visibility != Visibility.Visible);
        }

        #endregion


    }
}
