using Sema.DbLayer.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sema.Mediator
{
    static class MediatorSema
    {
        public static TableState UsingTable { get; set; }
        public static string TableName { get; set; }
    }
}
