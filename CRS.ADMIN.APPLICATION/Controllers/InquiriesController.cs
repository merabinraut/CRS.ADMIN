using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    [OverrideActionFilters]
    public class InquiriesController : BaseController
    {
        public ActionResult Index(string SearchFilter = "",  int StartIndex = 0, int PageSize = 10)
        {
            ViewBag.SearchFilter = SearchFilter;
            Session["CurrentURL"] = "/Inquiries/Index";

            ViewBag.StartIndex = 1;
            ViewBag.PageSize = 10;
            ViewBag.TotalData = 10;
            return View();
        }
    }
}