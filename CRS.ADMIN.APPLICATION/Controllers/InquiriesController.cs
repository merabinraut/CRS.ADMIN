using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.Inquiries;
using CRS.ADMIN.BUSINESS.HostManagement;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    [OverrideActionFilters]
    public class InquiriesController : BaseController
    {
        private readonly IHostManagementBusiness _buss;
        public InquiriesController(IHostManagementBusiness buss)
        {
            _buss = buss;
        }
        public ActionResult Index(string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            ViewBag.SearchFilter = SearchFilter;
            Session["CurrentURL"] = "/Inquiries/Index";

            var listInquiry = _buss.GetInquiryListAsync(SearchFilter,StartIndex,PageSize);
            var mappedResponse = listInquiry.MapObjects<InquiryListModel>();
            TempData["ListModel"] = mappedResponse;
            ViewBag.StartIndex = 1;
            ViewBag.PageSize = 10;
            ViewBag.TotalData = 10;
            return View();
        }
    }
}