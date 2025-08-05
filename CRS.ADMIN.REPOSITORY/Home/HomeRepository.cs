using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.Home;
using CRS.ADMIN.SHARED.NotificationManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CRS.ADMIN.REPOSITORY.Home
{
    public class HomeRepository : IHomeRepository
    {
        RepositoryDao _DAO;
        public HomeRepository()
        {
            _DAO = new RepositoryDao();
        }


        #region "Dashboard Information"
        public DashboardInfoModelCommon GetDashboardAnalytic()
        {
            string sp_name = "sproc_superadmin_getdashboardannlytics @Flag='gda'";
            var dbResponseInfo = _DAO.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null && dbResponseInfo.Rows.Count > 0)
            {
                return new DashboardInfoModelCommon()
                {
                    TotalBasicClubs = Convert.ToInt32(dbResponseInfo.Rows[0]["TotalBasicClubs"]),
                    TotalClubs = Convert.ToInt32(dbResponseInfo.Rows[0]["TotalClubs"]),
                    TotalVisitors = Convert.ToInt32(dbResponseInfo.Rows[0]["TotalVisitors"]),
                    TotalSales = Convert.ToDecimal(dbResponseInfo.Rows[0]["TotalSales"]),

                };
            }
            return new DashboardInfoModelCommon();
        }

        public List<HostListModelCommon> GetHostList()
        {
            List<HostListModelCommon> responseInfo = new List<HostListModelCommon>();
            string sp_name = "sproc_superadmin_getdashboardannlytics @Flag='ghl'";
            var dbResponseInfo = _DAO.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new HostListModelCommon()
                    {
                        HostName = row["HostName"].ToString(),
                        HostAddress = row["HostAddress"].ToString(),
                        HostCharge = Convert.ToDecimal(row["HostCharge"]),
                        HostImage = row["HostImage"].ToString(),
                        EmailAddress = row["EmailAddress"].ToString(),
                    });
                }
            }
            return responseInfo;
        }
        public List<ChartInfoCommon> GetChartInfoList()
        {
            List<ChartInfoCommon> responseInfo = new List<ChartInfoCommon>();
            string sp_name = "sproc_superadmin_getdashboardannlytics @Flag='gcd'";
            var dbResponseInfo = _DAO.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new ChartInfoCommon()
                    {
                        Month = row["Month"].ToString(),
                        TotalSales = row["TotalSales"].ToString(),
                    });
                }
            }
            return responseInfo;
        }
        public List<TopBookedHostRankingModelCommon> GetTopBookedHostList()
        {
            List<TopBookedHostRankingModelCommon> responseInfo = new List<TopBookedHostRankingModelCommon>();
            string sp_name = "sproc_superadmin_getdashboardannlytics @Flag='tbrh'";
            var dbResponseInfo = _DAO.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new TopBookedHostRankingModelCommon()
                    {
                        HostName = row["HostName"].ToString(),
                        HostImage = row["HostImage"].ToString(),
                        HostCount = row["HostCount"].ToString(),
                        HostUsername = row["HostUsername"].ToString(),
                        ClubName = row["ClubName"].ToString(),
                        ClubImage = row["ClubImage"].ToString(),
                        Price = row["Price"].ToString(),
                        PlanName = row["PlanName"].ToString(),
                    });
                }
            }
            return responseInfo;
        }
        public List<ReceivedAmountModelCommon> GetReceivedAmount()
        {
            List<ReceivedAmountModelCommon> responseInfo = new List<ReceivedAmountModelCommon>();
            string sp_name = "sproc_superadmin_getdashboardannlytics @Flag='gpm'";
            var dbResponseInfo = _DAO.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new ReceivedAmountModelCommon()
                    {
                        PaymentMethod = row["PaymentMethod"].ToString(),
                        TotalAmount = row["TotalAmount"].ToString(),
                        TodayDate = row["TodayDate"].ToString(),
                    });
                }
            }
            return responseInfo;
        }

        #endregion
        #region Login
        public CommonDbResponse Login(LoginRequestCommon Request)
        {
            string SQL = "EXEC sproc_admin_login_management @flag='Login'";
            SQL += ",@Username=" + _DAO.FilterString(Request.Username);
            SQL += ",@Password=" + _DAO.FilterString(Request.Password);
            SQL += ",@Session=" + _DAO.FilterString(Request.SessionId);
            var dbResponse = _DAO.ExecuteDataRow(SQL);
            if (dbResponse != null)
            {
                string code = _DAO.ParseColumnValue(dbResponse, "Code").ToString();
                string message = _DAO.ParseColumnValue(dbResponse, "Message").ToString();
                if (string.IsNullOrEmpty(code) || code.Trim() != "0")
                {
                    return new CommonDbResponse()
                    {
                        Code = ResponseCode.Failed,
                        Message = string.IsNullOrEmpty(message) ? "Failed" : message
                    };
                }
                else
                {
                    LoginResponseCommon loginResponse = new LoginResponseCommon()
                    {
                        RoleId = _DAO.ParseColumnValue(dbResponse, "RoleId").ToString(),
                        UserId = _DAO.ParseColumnValue(dbResponse, "UserId").ToString(),
                        RoleName = _DAO.ParseColumnValue(dbResponse, "RoleName").ToString(),
                        Username = _DAO.ParseColumnValue(dbResponse, "Username").ToString(),
                        FullName = _DAO.ParseColumnValue(dbResponse, "FullName").ToString(),
                        ProfileImage = _DAO.ParseColumnValue(dbResponse, "ProfileImage").ToString(),
                        Session = _DAO.ParseColumnValue(dbResponse, "Session").ToString(),
                        IsPasswordForceful = _DAO.ParseColumnValue(dbResponse, "IsPasswordForceful").ToString(),
                        LastPasswordChangedDate = _DAO.ParseColumnValue(dbResponse, "LastPasswordChangedDate").ToString(),
                    };

                    string menuSQL = "EXEC sproc_get_menus @Flag='gam'";
                    menuSQL += ",@Username=" + _DAO.FilterString(Request.Username);
                    var menuDBResponse = _DAO.ExecuteDataTable(menuSQL);
                    if (menuDBResponse != null)
                        loginResponse.Menus = _DAO.DataTableToListObject<MenuCommon>(menuDBResponse).ToList();
                    else loginResponse.Menus = null;

                    string functionSQL = "EXEC sproc_get_function @Flag='gaf'";
                    functionSQL += ",@RoleId=" + _DAO.FilterString(loginResponse.RoleId);
                    var functionDBResponse = _DAO.ExecuteDataTable(functionSQL);
                    loginResponse.Functions = new List<string>();
                    if (functionDBResponse != null)
                    {
                        foreach (DataRow item in functionDBResponse.Rows)
                        {
                            loginResponse.Functions.Add(item["FunctionURL"].ToString());
                        }
                    }

                    var notificationSQL = "sproc_admin_notification_management @Flag='s'";
                    notificationSQL += ",@AdminId=" + _DAO.FilterString(loginResponse.UserId);
                    var notiDBResp = _DAO.ExecuteDataTable(notificationSQL);
                    if (notiDBResp != null)
                    {
                        loginResponse.Notifications = _DAO.DataTableToListObject<NotificationDetailCommon>(notiDBResp).ToList();
                    }
                    return new CommonDbResponse()
                    {
                        Code = ResponseCode.Success,
                        Message = "Success",
                        Data = loginResponse
                    };
                }
            }
            return new CommonDbResponse()
            {
                Code = ResponseCode.Failed,
                Message = "Login failed"
            };
        }
        #endregion

        #region get admin balance
        public string GetAdminBalance()
        {
            string spName = "sproc_get_admin_balance"; // Stored procedure name
            var dbResponseInfo = _DAO.ExecuteDataTable(spName);

            // Check if the response is valid and contains rows
            if (dbResponseInfo != null && dbResponseInfo.Rows.Count > 0)
            {
                // Retrieve and convert the value to string
                var points = dbResponseInfo.Rows[0]["Points"];
                return points != DBNull.Value ? points.ToString() : string.Empty;
            }

            // Return a default or empty string if no rows are returned
            return string.Empty;
        }

        #endregion
    }
}
