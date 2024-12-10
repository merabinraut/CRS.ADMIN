using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.PaymentManagement;
using CRS.ADMIN.BUSINESS.PaymentManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class PaymentManagementController : BaseController
    {
        private readonly IPaymentManagementBusiness _business;

        public PaymentManagementController(IPaymentManagementBusiness business) => this._business = business;

        public ActionResult Index(string SearchText = "", string ClubId = "", string LocationId = "", string FromDate = "", string ToDate = "", int StartIndex = 0, int PageSize = 10, string Date = "",string Value="")
        {
            Session["CurrentURL"] = "/PaymentManagement/Index";
            var cId = !string.IsNullOrEmpty(ClubId) ? ClubId.DecryptParameter() : null;
            var lId = !string.IsNullOrEmpty(LocationId) ? LocationId.DecryptParameter() : null;
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            var overViewCommon = _business.GetPaymentOverview();
            var dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = SearchText
            };
            var paymentLogsCommon = _business.GetPaymentLogs(cId, Date, dbRequest, lId, FromDate, ToDate);
            var paymentManagementModel = new PaymentManagementModel()
            {
                PaymentOverview = overViewCommon.MapObject<PaymentOverviewModel>(),
                PaymentLogs = paymentLogsCommon.MapObjects<PaymentLogsModel>()
            };
            paymentManagementModel.PaymentLogs.ForEach(x =>
            {
                x.ClubId = x.ClubId.EncryptParameter();
                x.ClubLogo = ImageHelper.ProcessedImage(x.ClubLogo);
            });
            ViewBag.SearchFilter = SearchText;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = paymentLogsCommon != null && paymentLogsCommon.Any() ? paymentLogsCommon[0].TotalRecords : 0;
            ViewBag.ClubDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBLIST") as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.ClubIdKey = ClubId;
            ViewBag.LocationDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("LOCATIONDDL") as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.LocationIdKey = LocationId;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.TabValue = Value;
            return View(paymentManagementModel);
        }

        public ActionResult GetPaymentLedger(string clubId, string clubIdFilter, string LocationId,string searchText = "", string Date = "", int StartIndex = 0, int PageSize = 10, string FromDate = "", string ToDate = "")
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
            paymentLedgerModel.ForEach(x =>
            {
                x.ClubId = clubId.EncryptParameter();
                x.CustomerImage = ImageHelper.ProcessedImage(x.CustomerImage);
            });
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/PaymentManagement/Index?Value=PL&searchText="+ searchText + "&ClubId=" + clubIdFilter + "&LocationId=" + LocationId + "&StartIndex=" + StartIndex  + "&PageSize=" + PageSize + "&FromDate=" + FromDate+ "&ToDate=" + ToDate;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.ClubId = clubId;
            ViewBag.TotalData = paymentLedgerCommon != null && paymentLedgerCommon.Any() ? paymentLedgerCommon[0].TotalRecords : 0;
            return View(paymentLedgerModel);
        }
    }
}