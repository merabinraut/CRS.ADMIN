using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    [OverrideActionFilters]
    public class InquiriesController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}