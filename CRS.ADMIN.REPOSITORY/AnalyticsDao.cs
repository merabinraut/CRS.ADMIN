namespace CRS.ADMIN.REPOSITORY
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;

    public class AnalyticsDao
    {
        private SqlConnection _analyticsConnection;

        public AnalyticsDao()
        {
            _analyticsConnection = new SqlConnection(
                ConfigurationManager
                    .ConnectionStrings["DbConnString1"].ConnectionString
            );
        }
        public new DataSet ExecuteDataset(string sql)
        {
            var ds = new DataSet();
            using (var cmd = new SqlCommand(sql, _analyticsConnection))
            {
                cmd.CommandTimeout = 120;
                var da = new SqlDataAdapter(cmd);
                try
                {
                    _analyticsConnection.Open();
                    da.Fill(ds);
                }
                finally
                {
                    if (_analyticsConnection.State == ConnectionState.Open)
                        _analyticsConnection.Close();
                    da.Dispose();
                }
            }
            return ds;
        }
        public new DataRow ExecuteDataRow(string sql)
        {
            using (var ds = ExecuteDataset(sql))
            {
                if (ds == null || ds.Tables.Count == 0) return null;
                if (ds.Tables[0].Rows.Count == 0) return null;
                return ds.Tables[0].Rows[0];
            }
        }
        private void OpenConnection()
        {
            if (_analyticsConnection.State == ConnectionState.Open)
                _analyticsConnection.Close();
            _analyticsConnection.Open();
        }

        private void CloseConnection()
        {
            if (_analyticsConnection.State == ConnectionState.Open)
                _analyticsConnection.Close();
        }

        public DataTable ExecuteDataTable(string sql)
        {
            var ds = new DataSet();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(sql, _analyticsConnection);
                cmd.CommandTimeout = 120;
                var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                da.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;

            return ds.Tables[0];
        }


        public string DBNullToValue(object obj)
        {
            return obj != DBNull.Value ? obj.ToString() : null;
        }
        public string FilterString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            return input.Replace("'", "''").Trim();
        }
        public object ParseColumnValue(DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
            {
                return string.Empty;
            }
            return row[columnName];
        }
        public DataSet ExecuteDataSet(string sql)
        {
            var ds = new DataSet();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(sql, _analyticsConnection);
                cmd.CommandTimeout = 120;
                var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                da.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }

            return ds;
        }
    }
}