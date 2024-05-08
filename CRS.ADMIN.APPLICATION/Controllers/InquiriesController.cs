using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    [OverrideActionFilters]
    public class InquiriesController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.StartIndex = 1;
            ViewBag.PageSize = 10;
            ViewBag.TotalData = 100;
            return View();
        }
    }
}