using CRS.ADMIN.APPLICATION.Models.ChargeManagement;
using CRS.ADMIN.BUSINESS.ChargeManagement;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class ChargeManagementController : Controller
    {
        private readonly IChargeManagementBusiness _chargeManagementBusiness;
        public ChargeManagementController(IChargeManagementBusiness chargeManagementBusiness)
        {
            _chargeManagementBusiness = chargeManagementBusiness;
        }

        #region Charge Category Management
        public ActionResult Index()
        {
            Session["CurrentURL"] = "/ChargeManagement/Index";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            var response = new ChargeCategoryManagementModel();
            var dbResponseModel = _chargeManagementBusiness.GetChargeCategory(null, null);
            return View(response);
        }
        #endregion
    }
}