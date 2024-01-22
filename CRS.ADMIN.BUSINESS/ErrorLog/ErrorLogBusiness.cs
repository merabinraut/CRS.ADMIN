using CRS.ADMIN.REPOSITORY.ErrorLog;
using CRS.ADMIN.SHARED.ErrorLog;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.ErrorLog
{
    public class ErrorLogBusiness : IErrorLogBusiness
    {
        private readonly IErrorLogRepository _repo;
        public ErrorLogBusiness(ErrorLogRepository repo)
        {
            _repo = repo;
        }

        public List<ErrorLogModelCommon> GetErrorLog(string searchFilter, string fromDate, string toDate)
        {
            return _repo.GetErrorLog(searchFilter, fromDate, toDate);
        }
    }
}
