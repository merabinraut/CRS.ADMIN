using CRS.ADMIN.SHARED.ErrorLog;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.ErrorLog
{
    public interface IErrorLogRepository
    {
        List<ErrorLogModelCommon> GetErrorLog(string searchFilter,string fromDate,string toDate);
    }
}
