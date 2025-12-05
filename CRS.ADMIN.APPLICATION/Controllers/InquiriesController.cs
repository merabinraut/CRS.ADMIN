using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.Inquiries;
using CRS.ADMIN.BUSINESS.HostManagement;
using CsvHelper;
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
        public ActionResult Index(string inquiryId = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            ViewBag.SearchFilter = SearchFilter;
            Session["CurrentURL"] = "/Inquiries/Index";
            if (!string.IsNullOrEmpty(inquiryId))
            {
                var inquiriesDetails = _buss.GetInquiryDetailsAsync(inquiryId);
                var response = inquiriesDetails.MapObject<InquiriesModel>();
                ViewBag.InqueryId = response.InquiryId;
                TempData["DetailsModel"] = response;
                return PartialView("_GetInquiriesDetails", response);
            }
            var listInquiry = _buss.GetInquiryListAsync(SearchFilter, StartIndex, PageSize);
            var mappedResponse = listInquiry.MapObjects<InquiryListModel>();
            TempData["ListModel"] = mappedResponse;
            ViewBag.StartIndex = 1;
            ViewBag.PageSize = 10;
            ViewBag.TotalData = 10;
            return View();
        }

        public ActionResult GetInquiriesDetailsView(string inquiryId = "")
        {
            var inquiriesDetails = _buss.GetInquiryDetailsAsync(inquiryId);
            var response = inquiriesDetails.MapObject<InquiriesModel>();
            ViewBag.imagename = !string.IsNullOrEmpty(response.Attachments) ? response.Attachments : response.FullName;
            response.Attachments = ImageHelper.ProcessedImage(response.Attachments);
            ViewBag.InqueryId = response.InquiryId;
            TempData["DetailsModel"] = response;
            return PartialView("_GetInquiriesDetails", response);
        }
    }
}