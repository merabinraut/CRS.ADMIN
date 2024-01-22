using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.SMSLog;
using CRS.ADMIN.BUSINESS.SMSLog;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class SMSLogController : BaseController
    {
        private readonly ISMSLogBusiness _buss;
        public SMSLogController(ISMSLogBusiness buss)
        {
            _buss = buss;
        }
        public ActionResult Index(string SearchFilter = "", string FromDate = "", string ToDate = "")
        {
            Session["CurrentURL"] = "/SMSLog/Index";
            List<SMSLogModel> responseInfo = new List<SMSLogModel>();
            var dbResponseInfo = _buss.GetSmsLog(SearchFilter, FromDate, ToDate);
            responseInfo = dbResponseInfo.MapObjects<SMSLogModel>();
            return View(responseInfo);
        }
    }
}