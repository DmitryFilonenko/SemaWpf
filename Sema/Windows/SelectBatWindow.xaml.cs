using Sema.Mediator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    /// Interaction logic for SelectBatWindow.xaml
    /// </summary>
    public partial class SelectBatWindow : Window
    {
        public event EventHandler EventSelected;

        const int _cButtonHeight = 70;
        const int _cButtonWidth = 300;

       // bool _isSelected = false;

        public SelectBatWindow()
        {
            InitializeComponent();
           // this.Title = MediatorSema.UsingTable.TableName;
            CreateButtons();
        }

        private void CreateButtons()
        {
            int count = 0;
            
            List<FileInfo> list = MediatorSema.CurrentFileType == FileType.Bat? MediatorSema.BatFileList : MediatorSema.CtlFileList;

            SetWindowSize(list.Count);

            foreach (var item in list)
            {
                Button newBtn = new Button();
                newBtn.Content = item.Name;
                newBtn.Name = "button_" + count++;
                newBtn.Width = _cButtonWidth;
                newBtn.Height = _cButtonHeight;

                Thickness margin = newBtn.Margin;
                margin.Top = list.Count * 10;
                newBtn.Margin = margin;

                newBtn.Click += NewBtn_Click;

                stackPanel.Children.Add(newBtn);
            }
        }

        private void NewBtn_Click(object sender, RoutedEventArgs e)
        {
           // _isSelected = true;
            if (EventSelected != null)
            {
               EventSelected(sender, EventArgs.Empty);
            }            
            this.Close();
        }

        private void SetWindowSize(int count)
        {
            this.Height = (_cButtonHeight * count) + (50 * count);
            this.Width = _cButtonWidth + 70;
        }

        //private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    if(!_isSelected)
        //    {
        //        e.Cancel = true;
        //    }           
        //}
    }
}
