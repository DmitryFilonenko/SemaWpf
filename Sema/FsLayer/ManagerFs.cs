using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
