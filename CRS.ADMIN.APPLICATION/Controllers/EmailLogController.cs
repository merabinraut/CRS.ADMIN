using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.EmailLog;
using CRS.ADMIN.BUSINESS.EmailLog;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    [OverrideActionFilters]
    public class EmailLogController : BaseController
    {
        private readonly IEmailLogBusiness _buss;
        public EmailLogController(IEmailLogBusiness buss)
        {
            _buss = buss;
        }
        public ActionResult Index(string SearchFilter = "", string FromDate = "", string ToDate = "")
        {
            Session["CurrentURL"] = "/EmailLog/Index";
            List<EmailLogModel> responseInfo = new List<EmailLogModel>();
            var dbResponseInfo = _buss.GetEmailLog(SearchFilter, FromDate, ToDate);
            responseInfo = dbResponseInfo.MapObjects<EmailLogModel>();
            return View(responseInfo);
        }
    }
}