using Sema.Mediator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public SelectBatWindow()
        {
            InitializeComponent();
            CreateButtons();
        }

        private void CreateButtons()
        {
            for (int i = 0; i < MediatorSema.BatCounter; i++)
            {
                Button newBtn = new Button();
                newBtn.Content = i.ToString();
                newBtn.Name = "Button" + i.ToString();
                stackPanel.Children.Add(newBtn);
            }
        }

        private readonly ObservableCollection<SomeDataModel> _MyData = new ObservableCollection<SomeDataModel>();
        public ObservableCollection<SomeDataModel> MyData { get { return _MyData; } }



    }
}
