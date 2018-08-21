using Sema.DbLayer;
using Sema.DbLayer.Manager;
using Sema.FsLayer;
using Sema.Mediator;
using Sema.Windows;
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
                //MessageBox.Show(String.Format("Таблица занята пользователем {0} c {1}.", MediatorSema.UsingTable.UserName, MediatorSema.UsingTable.StartTime));
                AskWindow askWin = new AskWindow();
                askWin.Title = tableName;
                askWin.label_ask.Content = String.Format("Таблица {0} занята пользователем {1} c {2}.", tableName, MediatorSema.UsingTable.UserName, MediatorSema.UsingTable.StartTime);
                askWin.EventExit += AskWin_EventExit;
                askWin.EventPickUpTable += AskWin_EventPickUpTable;
                askWin.ShowDialog();
            }
        }

        private void AskWin_EventPickUpTable(object sender, EventArgs e)
        {
            MessageBox.Show("Pick Up");
        }

        private void AskWin_EventExit(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GetBatFile()
        {
            DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);
            FileInfo[] files = di.GetFiles("*.bat");
            if (files.Length == 1)
            {
                StartBatFile(files[0].Name);
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
            if(MediatorSema.IsTableMy)
            {
                ManagerDb.DeleteFromTable(MediatorSema.UsingTable);
            }            
        }
        #endregion
    }
}
