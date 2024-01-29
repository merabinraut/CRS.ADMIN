using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.PaymentManagement;
using CRS.ADMIN.BUSINESS.PaymentManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class PaymentManagementController : BaseController
    {
        private readonly IPaymentManagementBusiness _business;

        public PaymentManagementController(IPaymentManagementBusiness business) => this._business = business;

        public ActionResult Index(string SearchText = "", string ClubId = "", string LocationId = "", string FromDate = "", string ToDate = "", int StartIndex = 0, int PageSize = 10, string Date = "")
        {
            Session["CurrentURL"] = "/PaymentManagement/Index";
            var cId = !string.IsNullOrEmpty(ClubId) ? ClubId.DecryptParameter() : null;
            var lId = !string.IsNullOrEmpty(LocationId) ? LocationId.DecryptParameter() : null;
            var overViewCommon = _business.GetPaymentOverview();
            var dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = SearchText
            };
            var paymentLogsCommon = _business.GetPaymentLogs(cId, Date, dbRequest, lId, FromDate, ToDate);
            paymentLogsCommon.ForEach(x => x.ClubId = x.ClubId.EncryptParameter());
            var paymentManagementModel = new PaymentManagementModel()
            {
                PaymentOverview = overViewCommon.MapObject<PaymentOverviewModel>(),
                PaymentLogs = paymentLogsCommon.MapObjects<PaymentLogsModel>()
            };
            paymentManagementModel.PaymentLogs.ForEach(x => x.ClubId.EncryptParameter());
            ViewBag.SearchFilter = SearchText;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = paymentLogsCommon != null && paymentLogsCommon.Any() ? paymentLogsCommon[0].TotalRecords : 0;
            ViewBag.ClubDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBLIST") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.ClubIdKey = ClubId;
            ViewBag.LocationDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("LOCATIONDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.LocationIdKey = LocationId;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            return View(paymentManagementModel);
        }

        public ActionResult GetPaymentLedger(string clubId, string searchText = "", string Date = "", int StartIndex = 0, int PageSize = 10, string FromDate = "", string ToDate = "")
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
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(searchText) ? searchText : null
            };
            var paymentLedgerCommon = _business.GetPaymentLedgerDetail(cId, Date, dbRequest, FromDate, ToDate);
            var paymentLedgerModel = paymentLedgerCommon.MapObjects<PaymentLedgerModel>();
            paymentLedgerModel.ForEach(x => x.ClubId = clubId.EncryptParameter());
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/PaymentManagement/Index";
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.ClubId = clubId;
            ViewBag.TotalData = paymentLedgerCommon != null && paymentLedgerCommon.Any() ? paymentLedgerCommon[0].TotalRecords : 0;
            return View(paymentLedgerModel);
        }

        //public ActionResult GetPaymentLogs(string clubId, string searchText = "", string Date = "")
        //{
        //    if (clubId != null)
        //        clubId = clubId.DecryptParameter();

        //    var paymentLogs = _business.GetPaymentLogs(searchText, clubId, Date);
        //    var paymentLogsModel = paymentLogs.MapObjects<PaymentLogsModel>();
        //    paymentLogsModel.ForEach(x => x.ClubId = x.ClubId.EncryptParameter());

        //    var paymentModel = new PaymentManagementModel()
        //    {
        //        PaymentLogs = paymentLogsModel,
        //        PaymentOverview = _business.GetPaymentOverview().MapObject<PaymentOverviewModel>()
        //    };
        //    ViewBag.ClubDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBLIST") as Dictionary<string, string>, null, "Select Club");

        //    return View("Index", paymentModel);

        //}
    }
}