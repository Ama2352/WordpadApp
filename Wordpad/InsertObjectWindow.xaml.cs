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
using WordPad;

namespace Wordpad
{
    public partial class InsertObjectWindow : Window
    {
        private InsertManager _insertManager;
        private ClipboardManager _clipboardManager;
        private string iconWithDirectory = null;

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



            

        }
    }
}
