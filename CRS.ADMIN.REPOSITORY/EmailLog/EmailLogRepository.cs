using CRS.ADMIN.SHARED.EmailLog;
using System;
using System.Collections.Generic;
using System.Data;

namespace CRS.ADMIN.REPOSITORY.EmailLog
{
    public class EmailLogRepository : IEmailLogRepository
    {
        private readonly RepositoryDao _dao;
        public EmailLogRepository()
        {
            _dao = new RepositoryDao();
        }

        public List<EmailLogModelCommon> GetEmailLog(string searchFilter, string fromDate, string toDate)
        {
            List<EmailLogModelCommon> responseInfo = new List<EmailLogModelCommon>();
            string sp_name = "EXEC sproc_admin_logreport @Flag='emlr'";
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new EmailLogModelCommon()
                    {
                        EmailRequestId = row["EmailRequestId"].ToString(),
                        EmailText = row["EmailText"].ToString(),
                        EmailSendTo = row["EmailSendTo"].ToString(),
                        EmailSendBy = row["EmailSendBy"].ToString(),
                        EmailSendStatus = row["EmailSendStatus"].ToString(),
                        EmailSendToCC = row["EmailSendToCC"].ToString(),
                        CreatedBy = row["CreatedBy"].ToString(),
                        CreatedDate = !string.IsNullOrEmpty(row["CreatedDate"].ToString()) ? DateTime.Parse(row["CreatedDate"].ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : row["CreatedDate"].ToString() 
                    });
                }
            }
            return responseInfo;
        }
    }
}
