using Sema.DbLayer;
using Sema.DbLayer.Manager;
using Sema.FsLayer;
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

namespace Sema
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();            
        }

        private void CheckAndUpdateState()
        {
            string tableName = ManagerFs.GetTableNameFromCtl();
            bool isTableFree = ManagerDb.IsTableFree(tableName);
            if (isTableFree)
            {
                ManagerDb.InsertToTable(tableName);
            }
            else
            {
                //
                //if(отбираем)
                //{
                //    update
                //}
                //else
                //{
                //    отбой
                //}
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckAndUpdateState();

            Sema.DataSet1 dataSet1 = ((Sema.DataSet1)(this.FindResource("dataSet1")));
            // Load data into the table SEMAPHORE. You can modify this code as needed.
            Sema.DataSet1TableAdapters.SEMAPHORETableAdapter dataSet1SEMAPHORETableAdapter = new Sema.DataSet1TableAdapters.SEMAPHORETableAdapter();
            dataSet1SEMAPHORETableAdapter.Fill(dataSet1.SEMAPHORE);
            System.Windows.Data.CollectionViewSource sEMAPHOREViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("sEMAPHOREViewSource")));
            sEMAPHOREViewSource.View.MoveCurrentToFirst();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ManagerDb.DeleteFromTable();
        }
    }
}
