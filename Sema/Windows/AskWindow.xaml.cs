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

namespace Sema.Windows
{
    /// <summary>
    /// Interaction logic for AskWindow.xaml
    /// </summary>
    public partial class AskWindow : Window
    {
        public AskWindow()
        {
            InitializeComponent();
        }

        bool _isCloseParrent = false;
        public event EventHandler EventExit;
        public event EventHandler EventPickUpTable;

        private void button_ok_Click(object sender, RoutedEventArgs e)
        {
            _isCloseParrent = true;
            this.Close();            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (this.EventExit != null && _isCloseParrent)
            {
                this.EventExit(this, EventArgs.Empty);
            }
        }

        private void button_update_Click(object sender, RoutedEventArgs e)
        {
            if (this.EventPickUpTable != null)
            {
                this.EventPickUpTable(this, EventArgs.Empty);
            }
            this.Close();
        }
    }
}
