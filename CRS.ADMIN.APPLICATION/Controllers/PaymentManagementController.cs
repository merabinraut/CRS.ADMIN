using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.PaymentManagement;
using CRS.ADMIN.BUSINESS.PaymentManagement;
using CRS.ADMIN.SHARED;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class PaymentManagementController : BaseController
    {
        private readonly IPaymentManagementBusiness _business;

        public PaymentManagementController(IPaymentManagementBusiness business) => this._business = business;

        public ActionResult Index(string SearchText = "", string ClubId = "", string Date = "")
        {
            Session["CurrentURL"] = "/PaymentManagement/Index";
            var cId = !string.IsNullOrEmpty(ClubId) ? ClubId.DecryptParameter() : null;
            var overViewCommon = _business.GetPaymentOverview();
            var paymentLogsCommon = _business.GetPaymentLogs(SearchText, cId, Date);
            paymentLogsCommon.ForEach(x => x.ClubId = x.ClubId.EncryptParameter());
            var paymentManagementModel = new PaymentManagementModel()
            {
                PaymentOverview = overViewCommon.MapObject<PaymentOverviewModel>(),
                PaymentLogs = paymentLogsCommon.MapObjects<PaymentLogsModel>()
            };

            ViewBag.ClubDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBLIST") as Dictionary<string, string>, null, "Select Club");

            return View(paymentManagementModel);
        }

        public ActionResult GetPaymentLedger(string clubId, string searchText = "", string Date = "")
        {
            var cId = !string.IsNullOrEmpty(clubId) ? clubId.DecryptParameter() : null;
            if (string.IsNullOrWhiteSpace(cId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    Title = NotificationMessage.INFORMATION.ToString(),
                    Message = "Club Id invalid",
                    NotificationType = NotificationMessage.INFORMATION
                });
                return RedirectToAction("Index", new { Date = Date });
            }

            var paymentLedgerCommon = _business.GetPaymentLedgerDetail(searchText, cId, Date);
            var paymentLedgerModel = paymentLedgerCommon.MapObjects<PaymentLedgerModel>();
            paymentLedgerModel.ForEach(x => x.ClubId = clubId.EncryptParameter());

            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/PaymentManagement/Index?Date=" + Date;
            return View(paymentLedgerModel);
        }

        public ActionResult GetPaymentLogs(string clubId, string searchText = "", string Date = "")
        {
            if (clubId != null)
                clubId = clubId.DecryptParameter();

            var paymentLogs = _business.GetPaymentLogs(searchText, clubId, Date);
            var paymentLogsModel = paymentLogs.MapObjects<PaymentLogsModel>();
            paymentLogsModel.ForEach(x => x.ClubId = x.ClubId.EncryptParameter());

            var paymentModel = new PaymentManagementModel()
            {
                PaymentLogs = paymentLogsModel,
                PaymentOverview = _business.GetPaymentOverview().MapObject<PaymentOverviewModel>()
            };
            ViewBag.ClubDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBLIST") as Dictionary<string, string>, null, "Select Club");

            return View("Index", paymentModel);

        }
    }
}