using Sema.DbLayer;
using Sema.DbLayer.Manager;
using Sema.FsLayer;
using Sema.Mediator;
using Sema.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Deployment.Application;
using System.Reflection;

namespace Sema
{
    enum FileType { Bat, Ctl }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool _firstInit = true;
        bool _isOwnerChange = false;
        int _countToExit = 0;
        bool _isUpdate = false;
        //FileType _fileType = FileType.Bat;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                this.Title += (" v." + version);
            }
            catch (Exception)
            {
                throw;
            }
            


            if (!isActualVersion())
            {
                //MessageBoxResult result = MessageBox.Show("Я устарел, обновитьcя?", "Старый Сема", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                //if (result == MessageBoxResult.OK)
                //{
                    _isUpdate = true;
                    this.Close();
                //}                
            }
            Subsribe();
        }

        private bool isActualVersion()
        {
            bool isActual = true;
            try
            {
                DateTime currentFileDate = ManagerFs.GetCurrentFileDate();
                DateTime actualFileDate = ManagerFs.GetActualFileDate();
                isActual = (currentFileDate >= actualFileDate);                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "isActualVersion() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return isActual;
        }

        private void Subsribe()
        {
            try
            {
                ManagerDb.EventOwnerChanged += ManagerDb_EventOwnerChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Subsribe() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }

        private void ManagerDb_EventOwnerChanged(object sender, EventArgs e)
        {
            _isOwnerChange = true;
        }

        private void CheckAndUpdateState()
        {
            try
            {
                string tableName = ManagerFs.GetTableNameFromCtl();
                if (tableName == "Не найден файл контрола")
                {
                    MessageBox.Show(tableName);
                    this.Close();
                    return;
                }

                string curDir = ManagerFs.GetCurrentDir().FullName;
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                this.Title = curDir.Substring(curDir.IndexOf('\\') + 1) + "   (" + tableName + ")    v." + version;
                bool isTableFree = ManagerDb.IsTableFree(tableName);
                if (isTableFree)
                {
                    ManagerDb.InsertToTable(tableName);
                    MediatorSema.CurrentFileType = FileType.Bat;
                    GetFiles();
                }
                else
                {
                    AskWindow askWin = new AskWindow();
                    askWin.Title = tableName;
                    askWin.label_ask.Content = String.Format("Таблица занята пользователем {0} c {1}.", MediatorSema.UsingTable.UserName, MediatorSema.UsingTable.StartTime);
                    askWin.EventExit += AskWin_EventExit;
                    askWin.EventPickUpTable += AskWin_EventPickUpTable;
                    askWin.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CheckAndUpdateState() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void AskWin_EventPickUpTable(object sender, EventArgs e)
        {
            try
            {
                TableState tableState = new TableState();
                tableState.TableName = MediatorSema.UsingTable.TableName;
                tableState.UserName = Environment.UserName;
                tableState.StartTime = String.Format("{0:G}", DateTime.Now);
                tableState.Path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                MediatorSema.UsingTable = tableState;
                ManagerDb.UpdateTableState();
                MediatorSema.IsTableMy = true;
                MediatorSema.CurrentFileType = FileType.Bat;
                GetFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "AskWin_EventPickUpTable() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }

        private void AskWin_EventExit(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GetFiles()
        {
            try
            {
                MediatorSema.BatFileList.Clear();
                MediatorSema.CtlFileList.Clear();
                DirectoryInfo di = new DirectoryInfo(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));

                //MessageBox.Show(di.FullName);

                FileInfo[] files = MediatorSema.CurrentFileType == FileType.Bat? di.GetFiles("*import*.bat") : di.GetFiles("*.ctl");
                if (files.Length == 1)
                {
                    if (MediatorSema.CurrentFileType == FileType.Bat) StartBatFile(files[0].Name);
                    else RunTxtFile(files[0].Name);
                }
                else
                {
                    foreach (var item in files)
                    {
                        MessageBox.Show(item.FullName);
                        if (MediatorSema.CurrentFileType == FileType.Bat) MediatorSema.BatFileList.Add(item);
                        else MediatorSema.CtlFileList.Add(item);
                    }
                    RunSelectWindow();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "GetBatFile() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }

        private void RunTxtFile(string fileName)
        {
            try
            {
                if (!fileName.Contains(":")) fileName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), fileName);
                //MessageBox.Show(fileName);
                Process proc = new Process();              
                proc.StartInfo.CreateNoWindow = false;
                proc.StartInfo.FileName = System.IO.Path.Combine(MediatorSema.UsingTable.Path, fileName);                
                proc.Start();

                MediatorSema.ProcIdList.Add(proc.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "RunTxtFile() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RunSelectWindow()
        {
            try
            {
                SelectBatWindow win = new SelectBatWindow();
                win.EventSelected += Win_EventSelected;
                win.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "RunSelectWindow() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }

        private void Win_EventSelected(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                string name = btn.Content.ToString();
                string fullName = GetFileFullPath(name);
                StartBatFile(fullName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Win_EventBatSelected() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }

        private string GetFileFullPath(string name)
        {
            string str = "";
            try
            {
                foreach (var item in MediatorSema.BatFileList)
                {
                    if (item.Name == name)
                    {
                        str = name;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "GetBatFileFullPath() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return str;
        }

        void StartBatFile(string fileName)
        {
            if (!fileName.Contains(":")) fileName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), fileName);
            //MessageBox.Show(fileName);
            try
            {
                MediatorSema.CurrentBat = fileName;
                Process proc = new Process();
                proc.StartInfo.CreateNoWindow = true;
                proc.Exited += Proc_Exited;
                proc.StartInfo.FileName = fileName;
                proc.EnableRaisingEvents = true;
                proc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "StartBatFile() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Proc_Exited(object sender, EventArgs e)
        {
            try
            {
                if (MediatorSema.CurrentFileType == FileType.Bat)
                {
                    string ctlName = ManagerFs.GetCtlFromBat(MediatorSema.CurrentBat);
                    string logName = ManagerFs.GetLogName(ctlName);
                    RunTxtFile(logName);
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Proc_Exited() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }
        
        private void Window_Activated(object sender, EventArgs e)
        {
            try
            {
                if (_firstInit)
                {
                    _firstInit = false;
                    CheckAndUpdateState();
                }
                else
                {
                    ManagerDb.IsOwnerChanged();
                    if (_isOwnerChange && _countToExit == 0)
                    {
                        MessageBox.Show("Таблицу занял другой пользователь.", MediatorSema.UsingTable.TableName, MessageBoxButton.OK, MessageBoxImage.Warning);
                        _countToExit++;
                    }
                }
                GetDataFromDb();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Window_Activated() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }        

        private void GetDataFromDb()
        {
            try
            {
                Sema.DataSet1 dataSet1 = ((Sema.DataSet1)(this.FindResource("dataSet1")));                
                Sema.DataSet1TableAdapters.SEMAPHORETableAdapter dataSet1SEMAPHORETableAdapter = new Sema.DataSet1TableAdapters.SEMAPHORETableAdapter();
                dataSet1SEMAPHORETableAdapter.Fill(dataSet1.SEMAPHORE);
                System.Windows.Data.CollectionViewSource sEMAPHOREViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("sEMAPHOREViewSource")));
                sEMAPHOREViewSource.View.MoveCurrentToFirst();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "GetDataFromDb() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }

        #region Closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                SetTableFree();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Window_Closing() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetTableFree();
                CloseLogFiles();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Button_Click() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseLogFiles()
        {
            try
            {
                Process[] processes = Process.GetProcesses();

                foreach (var item in MediatorSema.ProcIdList)
                {
                    foreach (var i in processes)
                    {
                        if (i.Id == item)
                        {
                            i.Kill();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CloseLofFiles() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetTableFree()
        {
            try
            {
                if (MediatorSema.IsTableMy)
                {
                    ManagerDb.DeleteFromTable(MediatorSema.UsingTable);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Button_Click() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }                        
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                if (_isUpdate)
                {
                    ManagerFs.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Button_Click() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }
        #endregion

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                MediatorSema.CurrentFileType = FileType.Bat;
                if (MediatorSema.BatFileList.Count == 0)
                {
                    StartBatFile(MediatorSema.CurrentBat);//////////////////////////////////////////////////////////
                }
                else
                {
                    RunSelectWindow();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "GetDataFromDb() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }                        
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                MediatorSema.CurrentFileType = FileType.Ctl;
                GetFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "GetDataFromDb() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
