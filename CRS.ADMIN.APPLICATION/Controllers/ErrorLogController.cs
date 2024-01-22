using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ErrorLog;
using CRS.ADMIN.BUSINESS.ErrorLog;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    [OverrideActionFilters]
    public class ErrorLogController : BaseController
    {
        private readonly IErrorLogBusiness _buss;
        public ErrorLogController(IErrorLogBusiness buss)
        {
            _buss = buss;
        }
        public ActionResult Index(string SearchFilter = "", string FromDate = "", string ToDate = "")
        {
            List<ErrorLogModel> responseInfo = new List<ErrorLogModel>();
            var dbResponseInfo = _buss.GetErrorLog(SearchFilter, FromDate, ToDate);
            responseInfo = dbResponseInfo.MapObjects<ErrorLogModel>();
            return View(responseInfo);
        }
    }
}