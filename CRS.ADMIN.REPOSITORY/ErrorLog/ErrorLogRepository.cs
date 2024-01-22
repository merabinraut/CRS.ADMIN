using CRS.ADMIN.SHARED.ErrorLog;
using System.Collections.Generic;
using System.Data;

namespace CRS.ADMIN.REPOSITORY.ErrorLog
{
    public class ErrorLogRepository : IErrorLogRepository
    {
        private readonly RepositoryDao _dao;
        public ErrorLogRepository()
        {
            _dao = new RepositoryDao();
        }

        public List<ErrorLogModelCommon> GetErrorLog(string searchFilter, string fromDate, string toDate)
        {
            List<ErrorLogModelCommon> responswInfo = new List<ErrorLogModelCommon>();
            string sp_name = "EXEC sproc_admin_logreport @Flag='elr'";
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responswInfo.Add(new ErrorLogModelCommon()
                    {
                        ErrorId = row["ErrorId"].ToString(),
                        ErrorDescription = row["ErrorDescription"].ToString(),
                        ErrorCategory = row["ErrorCategory"].ToString(),
                        ErrorSource = row["ErrorSource"].ToString(),
                        ActionDate = row["ActionDate"].ToString()
                    });
                }
            }
            return responswInfo;
        }
    }
}
