using Sema.Mediator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Sema.FsLayer
{
    static class ManagerFs
    {
        #region GetTableNameFromCtl
        public static string GetTableNameFromCtl()
        {
            string tableName = "Не найден файл контрола";
            DirectoryInfo currentDir = new DirectoryInfo(Environment.CurrentDirectory);
            FileInfo[] pathArr = currentDir.GetFiles("*.ctl");
            if (pathArr.Length > 0)
            {
                string pathToCtl = pathArr[0].FullName;
                tableName = GetTableName(pathToCtl);
            }
            return tableName;
        }

        private static string GetTableName(string path)
        {
            string str = File.ReadAllText(path);
            string startText = "INTO TABLE";
            string endText = "FIELDS TERMINATED BY";
            int indexStart = str.IndexOf(startText);
            str = str.Substring(indexStart + startText.Length);
            int indexEnd = str.IndexOf(endText);
            str = str.Substring(0, indexEnd);
            str = str.Replace("\"", "").Trim();
            return str;
        }
        #endregion

        static FileInfo GetExeFile(string dirPath)
        {
            DirectoryInfo di = new DirectoryInfo(dirPath);
            FileInfo[] fArr = di.GetFiles("*sema*.exe");
            if (fArr.Length > 0)
            {
                return fArr[0];
            }
            return null;
        }

        public static long GetCurrentFileSize()
        {
            try
            {
                return GetExeFile(MediatorSema.IsUpdated? Environment.GetCommandLineArgs()[1] : Environment.CurrentDirectory).Length;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public static long GetActualFileSize()
        {
            try
            {
                return GetExeFile(@"x:\utils\Semaphore_new").Length;
            }
            catch (Exception)
            {
                throw;
            }            
        }

        public static void Update()
        {
            try
            {
                //  Where run other app wich will copy files after close this app
                //  Something like this:
                //  FileInfo source = GetExeFile(@"x:\utils\Semaphore_new");
                //  FileInfo dest = GetExeFile(Environment.CurrentDirectory);
                //  File.Copy(source.FullName, dest.FullName, true);

                string path = (MediatorSema.IsUpdated ? Environment.GetCommandLineArgs()[1] : GetExeFile(Environment.CurrentDirectory).FullName);
                Process proc = new Process();
                string pathAsArg = path;
                if (path.Contains(" "))
                {
                    pathAsArg = "\"" + pathAsArg + "\"";
                }
                proc.StartInfo.Arguments = pathAsArg;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.FileName = @"d:\Dima\Programming\git\semophore_updater\bin\Debug\SemaUpd.exe";
                proc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }

        internal static void IsUpdated()
        {
            string[] argArr = Environment.GetCommandLineArgs();
            if (argArr.Length > 1)
            {
                MediatorSema.IsUpdated = true;                
            }
            else
            {
                MediatorSema.IsUpdated = false;
            }

        }
    }
}
