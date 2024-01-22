using CRS.ADMIN.SHARED.ErrorLog;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.ErrorLog
{
    public interface IErrorLogBusiness
    {
        List<ErrorLogModelCommon> GetErrorLog(string searchFilter, string fromDate, string toDate);
    }
}
