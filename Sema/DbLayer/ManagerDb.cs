﻿
using Sema.DbLayer.Manager;
using Sema.Mediator;
using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sema.DbLayer
{    static class ManagerDb
    {
        public static event EventHandler EventOwnerChanged;

        #region Support
        static OracleConnection _con;
        static OracleCommand _cmd;

        static ManagerDb()
        {
            try
            {
                string connectStr = Properties.Settings.Default.ConnectionString;
                _con = new OracleConnection(connectStr);
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
            TableState ts = null;
            string query = "select t.table_name, t.user_name, t.start_time from SEMAPHORE t where t.table_name = '" + tableName + "'";
            OracleDataReader reader = GetReader(query);
            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    ts = new TableState();
                    ts.TableName = reader[0].ToString();
                    ts.UserName = reader[1].ToString();
                    ts.StartTime = reader[2].ToString();
                }                
            }            
            return ts;
        }

        public static void InsertToTable(string tableName)
        {
            TableState tableState = new TableState() { UserName = Environment.UserName, TableName = tableName, StartTime = String.Format("{0:G}", DateTime.Now) };
            MediatorSema.UsingTable = tableState;
            string query = String.Format("insert into semaphore(table_name, user_name, start_time) values ('{0}', '{1}', '{2}')", tableState.TableName, tableState.UserName, tableState.StartTime);
            ExecCommand(query);
        }

        public static void DeleteFromTable(TableState tableState)
        {
            string query = String.Format("delete from SEMAPHORE t where t.table_name = '{0}'", tableState.TableName);
            ExecCommand(query);
        }

        public static void UpdateTableState()
        {
            string query = String.Format("update SEMAPHORE t set t.user_name = '{0}', t.start_time = '{1}' where t.table_name = '{2}'", MediatorSema.UsingTable.UserName, MediatorSema.UsingTable.StartTime, MediatorSema.UsingTable.TableName);
            ExecCommand(query);
        }

        internal static bool IsTableFree(string tableName)
        {
            bool isFree = true;
            TableState tableState = GetTableState(tableName);
            if (tableState != null)
            {
                isFree = false;                
            }
            MediatorSema.UsingTable = tableState;
            MediatorSema.IsTableMy = isFree;
            return isFree;
        }

        public static void IsOwnerChanged()
        {            
            string query = "select count(*) from SEMAPHORE t where t.table_name = '" + MediatorSema.UsingTable.TableName + "' and t.user_name = '" + Environment.UserName + "'";
            OracleDataReader reader = GetReader(query);
            int count = 0;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    count = Convert.ToInt32(reader[0]);
                }
            }
            if (count == 0)
            {
                MediatorSema.IsTableMy = false;
                if (EventOwnerChanged != null)
                {
                    EventOwnerChanged(count, EventArgs.Empty);
                }
            }
        }
    }
}
