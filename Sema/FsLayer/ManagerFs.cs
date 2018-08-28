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
            DirectoryInfo currentDir = new DirectoryInfo(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
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

        //static FileInfo GetExeFile(string dirPath)
        //{
        //    DirectoryInfo di = new DirectoryInfo(dirPath);
        //    FileInfo[] fArr = di.GetFiles("*sema*.exe");
        //    if (fArr.Length > 0)
        //    {
        //        return fArr[0];
        //    }
        //    return null;
        //}

        //public static long GetCurrentFileSize()
        //{
        //    try
        //    {
        //        return new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Length;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }            
        //}

        public static DateTime GetCurrentFileDate()
        {
            try
            {
                return File.GetLastWriteTime(System.Reflection.Assembly.GetEntryAssembly().Location);
            }
            catch (Exception)
            {
                throw;
            }

        }

        //public static long GetActualFileSize()
        //{
        //    try
        //    {
        //        return new FileInfo(@"x:\utils\Semaphore_new\0 Sema.exe").Length;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public static DateTime GetActualFileDate()
        {
            try
            {
                return File.GetLastWriteTime(@"x:\utils\Semaphore_new\0_Sema.exe");    //return GetExeFile(@"x:\utils\Semaphore_new").LastWriteTimeUtc;
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
                string path = System.Reflection.Assembly.GetEntryAssembly().Location;
                Process proc = new Process();
                string pathAsArg = path;
                if (path.Contains(" "))
                {
                    pathAsArg = "\"" + pathAsArg + "\"";
                }
                proc.StartInfo.Arguments = pathAsArg;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.FileName = @"x:\utils\Semaphore_updater\SemaUpd.exe";
                proc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }

        //internal static void IsUpdated()
        //{
        //    string[] argArr = Environment.GetCommandLineArgs();
        //    if (argArr.Length > 1)
        //    {
        //        MediatorSema.IsUpdated = true;                
        //    }
        //    else
        //    {
        //        MediatorSema.IsUpdated = false;
        //    }
        //}
    }
}
