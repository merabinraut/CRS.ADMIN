using CRS.ADMIN.SHARED.SMSLog;
using System;
using System.Collections.Generic;
using System.Data;

namespace CRS.ADMIN.REPOSITORY.SMSLog
{
    public class SMSRepository : ISMSRepository
    {
        private readonly RepositoryDao _dao;
        public SMSRepository()
        {
            _dao = new RepositoryDao();
        }

        public List<SMSLogModelCommon> GetSmsLog(string searchFilter, string fromDate, string toDate)
        {
            List<SMSLogModelCommon> responseInfo = new List<SMSLogModelCommon>();
            string sp_name = "EXEC sproc_admin_logreport @Flag='slr'";
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new SMSLogModelCommon()
                    {
                        DestinationNumber = row["DestinationNumber"].ToString(),
                        Message = row["Message"].ToString(),
                        Status = row["Status"].ToString(),
                        CreatedBy = row["CreatedBy"].ToString(),
                        CreatedDate = !string.IsNullOrEmpty(row["CreatedDate"].ToString()) ? DateTime.Parse(row["CreatedDate"].ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : row["CreatedDate"].ToString()
                    }); ;
                }
            }
            return responseInfo;
        }
    }
}
