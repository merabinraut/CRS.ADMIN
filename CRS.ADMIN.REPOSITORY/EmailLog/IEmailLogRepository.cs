using CRS.ADMIN.SHARED.EmailLog;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.EmailLog
{
    public interface IEmailLogRepository
    {
        List<EmailLogModelCommon> GetEmailLog(string searchFilter, string fromDate, string toDate);
    }
}
