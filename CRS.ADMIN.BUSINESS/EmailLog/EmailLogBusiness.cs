using CRS.ADMIN.REPOSITORY.EmailLog;
using CRS.ADMIN.SHARED.EmailLog;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.EmailLog
{
    public class EmailLogBusiness : IEmailLogBusiness
    {
        private readonly IEmailLogRepository _repo;
        public EmailLogBusiness(EmailLogRepository repo)
        {
            _repo = repo;
        }

        public List<EmailLogModelCommon> GetEmailLog(string searchFilter, string fromDate, string toDate)
        {
            return _repo.GetEmailLog(searchFilter, fromDate, toDate);
        }
    }
}
