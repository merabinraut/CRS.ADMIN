using CRS.ADMIN.APPLICATION.Library;
using System;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class LanguageManagementController : BaseController
    {
        public ActionResult Index()
        {
            Session["CurrentURL"] = "/LanguageManagement/Index";
            return View();
        }

        [HttpPost]
        public JsonResult Index(string lang)
        {
            try
            {
                new ManageLanguage().SetLanguage(lang);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}