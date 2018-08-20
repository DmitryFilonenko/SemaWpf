using Sema.DbLayer;
using Sema.DbLayer.Manager;
using Sema.FsLayer;
using Sema.Mediator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sema
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool _firstInit = true;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void CheckAndUpdateState()
        {
            string tableName = ManagerFs.GetTableNameFromCtl();
            this.Title = tableName;
            bool isTableFree = ManagerDb.IsTableFree(tableName);
            if (isTableFree)
            {
                ManagerDb.InsertToTable(tableName);
                GetBatFile();
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

        private void GetBatFile()
        {
            DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);
            FileInfo[] files = di.GetFiles("*.bat");
            if (files.Length == 1)
            {
                StartBatFile(files[0].FullName);
            }           
        }

        void StartBatFile(string fileName)
        {
                Process proc = new Process();
                //proc.Exited += new EventHandler(FinishHandler);
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.FileName = fileName;
                proc.EnableRaisingEvents = true;
                proc.Start();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (_firstInit)
            {
                _firstInit = false;
                CheckAndUpdateState();
            }
            
            GetDataFromDb();
        }

        private void GetDataFromDb()
        {
            Sema.DataSet1 dataSet1 = ((Sema.DataSet1)(this.FindResource("dataSet1")));
            // Load data into the table SEMAPHORE. You can modify this code as needed.
            Sema.DataSet1TableAdapters.SEMAPHORETableAdapter dataSet1SEMAPHORETableAdapter = new Sema.DataSet1TableAdapters.SEMAPHORETableAdapter();
            dataSet1SEMAPHORETableAdapter.Fill(dataSet1.SEMAPHORE);
            System.Windows.Data.CollectionViewSource sEMAPHOREViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("sEMAPHOREViewSource")));
            sEMAPHOREViewSource.View.MoveCurrentToFirst();
        }

        #region Closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SetTableFree();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetTableFree();
            this.Close();
        }

        private void SetTableFree()
        {
            ManagerDb.DeleteFromTable(MediatorSema.UsingTable);
        }
        #endregion
    }
}
