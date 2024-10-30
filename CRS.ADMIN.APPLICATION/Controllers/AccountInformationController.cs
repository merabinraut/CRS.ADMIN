using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.AccountInformation;
using CRS.ADMIN.BUSINESS.AccountInformation;
using CRS.ADMIN.SHARED.AccountInformation;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class AccountInformationController : BaseController
    {
        private readonly IAccountInformationBusiness _accountInformationBusiness;
        public AccountInformationController(IAccountInformationBusiness accountInformationBusiness)
        {
            _accountInformationBusiness = accountInformationBusiness;
        }
        public ActionResult GetAccountInformation(string SearchFilter = "", int StartIndex = 0, int PageSize = 10, string fromDate = null, string toDate = null)
        {
            ViewBag.SearchFilter = null;
            Session["CurrentURL"] = "/AccountInformation/GetAccountInformation";
            var request = new AccountInformationFilterModel()
            {
                fromDate=fromDate,
                toDate=toDate,
            };
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            var requestMapped = request.MapObject<AccountInformationFilterCommon>();
            var response = _accountInformationBusiness.GetAccountInformationAsync(requestMapped, dbRequest);
            var responseObj = response.MapObjects<AccountInformationResponse>();
            request.fromDate = fromDate;
            request.toDate = toDate;
            request.searchFilter = SearchFilter;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewData["grid"] = responseObj;
            return View(request);
        }
    }
}