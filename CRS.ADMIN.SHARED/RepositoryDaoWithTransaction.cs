using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CRS.ADMIN.SHARED
{
    public class RepositoryDaoWithTransaction
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        public RepositoryDaoWithTransaction(SqlConnection connection = null, SqlTransaction transaction = null)
        {
            _connection = connection ?? new SqlConnection(GetConnectionString());
            _transaction = transaction;
        }

        private void OpenConnection()
        {
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
        }

        private void CloseConnection()
        {
            if (_connection.State == ConnectionState.Open)
                _connection.Close();
        }

        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConnString"]?.ConnectionString ?? string.Empty;
        }

        public void BeginTransaction()
        {
            OpenConnection();
            _transaction = _connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = null;
            }
            CloseConnection();
        }

        public void RollbackTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
                _transaction = null;
            }
            CloseConnection();
        }

        public DataTable ExecuteDataTable(string sql)
        {
            using (var ds = ExecuteDataset(sql))
            {
                if (ds == null || ds.Tables.Count == 0)
                    return null;

                if (ds.Tables[0].Rows.Count == 0)
                    return null;

                return ds.Tables[0];
            }
        }

        public DataRow ExecuteDataRow(string sql)
        {
            using (var ds = ExecuteDataset(sql))
            {
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    return null;

                return ds.Tables[0].Rows[0];
            }
        }

        public DataSet ExecuteDataset(string sql)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlCommand cmd = new SqlCommand(sql, _connection)
                {
                    CommandType = CommandType.Text,
                    Transaction = _transaction
                };

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing query: " + sql, ex);
            }
            return ds;
        }

        public CommonDbResponse ParseCommonDbResponse(string sql)
        {
            return ParseCommonDbResponse(ExecuteDataTable(sql));
        }

        public SqlTransaction GetCurrentTransaction() => _transaction;

        public SqlConnection GetCurrentConnection() => _connection;

        public CommonDbResponse ParseCommonDbResponse(System.Data.DataTable dt)
        {
            var res = new CommonDbResponse();
            if (dt != null)
                if (dt.Rows.Count > 0)
                {
                    string Code = dt.Rows[0][0].ToString();
                    string Message = dt.Rows[0][1].ToString();
                    string Extra1 = "", Extra2 = "", Extra3 = "", Extra4 = "", Extra5 = "";
                    if (dt.Columns.Count > 2)
                    {
                        Extra1 = dt.Rows[0][2].ToString();
                    }
                    if (dt.Columns.Count > 3)
                    {
                        Extra2 = dt.Rows[0][3].ToString();
                    }
                    if (dt.Columns.Count > 4)
                    {
                        Extra3 = dt.Rows[0][4].ToString();
                    }
                    if (dt.Columns.Count > 5)
                    {
                        Extra4 = dt.Rows[0][5].ToString();
                    }
                    if (dt.Columns.Count > 6)
                    {
                        Extra5 = dt.Rows[0][6].ToString();
                    }
                    res.SetMessage(Code, Message, Extra1, Extra2, Extra3, Extra4, Extra5, dt);
                    if (dt.Columns.Contains("id"))
                    {
                        res.Id = dt.Rows[0]["id"].ToString();
                    }
                }
            return res;
        }

        private String FilterQuote(string strVal)
        {
            if (string.IsNullOrEmpty(strVal))
            {
                strVal = "";
            }
            var str = strVal.Trim();

            if (!string.IsNullOrEmpty(str))
            {
                str = str.Replace(";", "");
                str = str.Replace("--", "");
                str = str.Replace("'", "''");
                str = str.Replace("/*", "");
                str = str.Replace("*/", "");
                str = str.Replace(" select ", "");
                str = str.Replace(" insert ", "");
                str = str.Replace(" update ", "");
                str = str.Replace(" delete ", "");
                str = str.Replace(" drop ", "");
                str = str.Replace(" truncate ", "");
                str = str.Replace(" create ", "");
                str = str.Replace(" begin ", "");
                str = str.Replace(" end ", "");
                str = str.Replace(" char(", "");
                str = str.Replace(" exec ", "");
                str = str.Replace(" xp_cmd ", "");
                str = str.Replace(" sp_help ", "");
                str = str.Replace(" sproc_ ", "");
                str = str.Replace(" apiproc_ ", "");
                str = str.Replace("'", "");
                str = str.Replace("<script", "");
            }
            else
                str = "";

            return str;
        }

        public String FilterString(string strVal)
        {
            var str = FilterQuote(strVal);

            if (str.ToLower() != "null")
                str = "'" + str + "'";

            return str.TrimEnd().TrimStart();
        }
    }
}
