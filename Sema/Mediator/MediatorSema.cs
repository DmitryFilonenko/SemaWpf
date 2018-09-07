using Sema.DbLayer.Manager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sema.Mediator
{
    static class MediatorSema
    {
        public static TableState UsingTable { get; set; }
        public static bool IsTableMy { get; set; }
        static List<FileInfo> _batFileList = new List<FileInfo>();
        public static List<FileInfo> BatFileList { get { return _batFileList; } set { _batFileList = value; } }
        public static string CurrentBat { get; set; }
        public static List<FileInfo> CtlFileList { get { return _batFileList; } set { _batFileList = value; } }
        public static string CurrentCtl { get; set; }
    }
}
