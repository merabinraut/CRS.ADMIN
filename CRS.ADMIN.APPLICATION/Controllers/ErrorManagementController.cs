using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    [OverrideActionFilters]
    public class ErrorManagementController : BaseController
    {
        public ActionResult Index(string Id = "")
        {
            if (string.IsNullOrEmpty(Id)) return RedirectToAction("Dashboard", "Home");
            ViewBag.ErrorId = Id;
            return View();
        }
        public ActionResult Error_403()
        {
            return View();
        }
        public ActionResult Error_404()
        {
            return View();
        }
    }
}