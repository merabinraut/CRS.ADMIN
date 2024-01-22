using CRS.ADMIN.SHARED.SMSLog;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.SMSLog
{
    public interface ISMSRepository
    {
        List<SMSLogModelCommon> GetSmsLog(string searchFilter, string fromDate, string toDate);
    }
}
