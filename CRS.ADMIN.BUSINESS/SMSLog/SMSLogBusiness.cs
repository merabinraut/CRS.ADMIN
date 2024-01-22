using CRS.ADMIN.REPOSITORY.SMSLog;
using CRS.ADMIN.SHARED.SMSLog;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.SMSLog
{
    public class SMSLogBusiness : ISMSLogBusiness
    {
        private readonly ISMSRepository _repo;
        public SMSLogBusiness(SMSRepository repo)
        {
            _repo = repo;
        }

        public List<SMSLogModelCommon> GetSmsLog(string searchFilter, string fromDate, string toDate)
        {
            return _repo.GetSmsLog(searchFilter, fromDate, toDate);
        }
    }
}
