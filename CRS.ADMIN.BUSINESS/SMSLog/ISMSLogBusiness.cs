using CRS.ADMIN.SHARED.SMSLog;
using System;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.SMSLog
{
    public interface ISMSLogBusiness
    {
        List<SMSLogModelCommon> GetSmsLog(string searchFilter, string fromDate, string toDate);
    }
}
