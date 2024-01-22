using CRS.ADMIN.SHARED.EmailLog;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.EmailLog
{
    public interface IEmailLogBusiness
    {
        List<EmailLogModelCommon> GetEmailLog(string searchFilter, string fromDate, string toDate);
    }
}
