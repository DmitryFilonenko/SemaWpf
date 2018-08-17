using Sema.DbLayer.Manager;
using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sema.DbLayer
{
    static class ManagerDb
    {
        #region Support
        static OracleConnection _con;
        static OracleCommand _cmd;

        static ManagerDb()
        {
            try
            {
                _con = new OracleConnection("Data Source = CD_WORK; User ID = IMPORT_USER; Password = sT7hk9Lm; Unicode = True");
                _con.Open();
            }
            catch (Exception)
            {
                throw;
            }
        }

        static void ExecCommand(string query)
        {
            try
            {
                _cmd = new OracleCommand(query, _con);
                _cmd.ExecuteNonQuery();
                _cmd.Dispose();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static OracleDataReader GetReader(string query)
        {
            try
            {
                _cmd = new OracleCommand(query, _con);
                OracleDataReader reader = _cmd.ExecuteReader();
                _cmd.Dispose();
                return reader;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        static TableState GetTableState(string tableName)
        {            
            throw new NotImplementedException();
        }

        public static void SetTableState(TableState tableState)
        {            
            throw new NotImplementedException();
        }

        internal static bool IsTableFree(string tableName)
        {
            //TableState ts = ManagerDb.GetTableState(tableName);
            throw new NotImplementedException();
        }
    }
}
