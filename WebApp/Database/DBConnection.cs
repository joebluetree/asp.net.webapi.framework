using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Database
{
    public class DBConnection : IDisposable
    {
        public string ConnectionString = "";
        public SqlConnection Connection = null;
        public SqlTransaction Transaction = null;

        public DBConnection()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public SqlConnection OpenConnection()
        {
            if (Connection == null)
                Connection = new SqlConnection(ConnectionString);
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
            return Connection;
        }
        public void CloseConnection()
        {
            if (Connection != null)
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
        }
        public void BeginTransaction()
        {
            OpenConnection();
            Transaction = Connection.BeginTransaction();
        }
        public void CommitTransaction()
        {
            if (Transaction != null)
                Transaction.Commit();
        }
        public void RollbackTransaction()
        {
            if (Transaction != null)
                Transaction.Rollback();
        }

        public object ExecuteScalar(string sql)
        {
            try
            {
                OpenConnection();
                SqlCommand Cmd = Connection.CreateCommand();
                Cmd.CommandText = sql;
                Cmd.CommandType = CommandType.Text;
                if (Transaction != null)
                    Cmd.Transaction = Transaction;
                object mData = Cmd.ExecuteScalar();
                return mData;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public int ExecuteNonQuery(string sql)
        {
            try
            {
                OpenConnection();
                SqlCommand Cmd = Connection.CreateCommand();
                if (Transaction != null)
                    Cmd.Transaction = Transaction;
                Cmd.CommandText = sql;
                Cmd.CommandType = CommandType.Text;
                int mData = Cmd.ExecuteNonQuery();
                return mData;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable ExecuteQuery(string sql)
        {
            try
            {
                OpenConnection();
                DataTable DataTable = new DataTable();
                SqlCommand Cmd = new SqlCommand(sql, Connection);
                if (Transaction != null)
                    Cmd.Transaction = Transaction;
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = Cmd;
                dataAdapter.Fill(DataTable);
                return DataTable;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                CloseConnection();
                // free managed resources
            }
            // free native resources if there are any.
        }

    }

    


}
