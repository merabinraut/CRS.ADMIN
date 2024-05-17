using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ReservationLedger;
using CRS.ADMIN.BUSINESS.ReservationLedger;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PaymentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class ReservationLedgerController : BaseController
    {
        private readonly IReservationLedgerBusiness _business;
        public ReservationLedgerController(IReservationLedgerBusiness business) => _business = business;

        [HttpGet]
        public ActionResult ReservationLedgerList(string SearchText = "", string ClubId = "", string Date = "", string FromDate = "", string ToDate = "", int StartIndex = 0, int PageSize = 10)
        {
            Session["CurrentURL"] = "/ReservationLedger/ReservationLedgerList";
            var cId = !string.IsNullOrEmpty(ClubId) ? ClubId.DecryptParameter() : null;
            var dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = SearchText,
                FromDate = FromDate,
                ToDate = ToDate
            };
            var dbResponse = _business.GetReservationLedgerList(dbRequest, cId, Date);
            var responseInfo = dbResponse.MapObjects<ReservationLedgerModel>();
            responseInfo.ForEach(x =>
                {
                    x.ClubId = x.ClubId.EncryptParameter();
                    x.ClubLogo = ImageHelper.ProcessedImage(x.ClubLogo);
                }
            );
            ViewBag.ClubDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBLIST") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.ClubIdKey = ClubId;
            ViewBag.LedgerList = responseInfo;
            ViewBag.SearchText = SearchText;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            return View();
        }
        [HttpGet]
        public ActionResult ReservationLedgerDetail(string ClubId, string Date, string SearchText = "", int StartIndex = 0, int PageSize = 10, string FromDate = "", string ToDate = "")
        {
            Session["CurrentURL"] = "/ReservationLedger/ReservationLedgerList";
            var cId = !string.IsNullOrEmpty(ClubId) ? ClubId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(cId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    Title = NotificationMessage.INFORMATION.ToString(),
                    Message = "Invalid request",
                    NotificationType = NotificationMessage.INFORMATION
                });
                return RedirectToAction("ReservationLedgerList", new { Date = Date });
            }
            var Response = new List<ReservationLedgerDetailModel>();
            var dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = SearchText,
                FromDate = FromDate,
                ToDate = ToDate
            };
            var dbResponse = _business.GetReservationLedgerDetail(dbRequest, cId, Date);
            Response = dbResponse.MapObjects<ReservationLedgerDetailModel>();
            Response.ForEach(x =>
                {
                    x.ClubId = x.ClubId.EncryptParameter();
                    x.CustomerImage = ImageHelper.ProcessedImage(x.CustomerImage);
                }
            );
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/ReservationLedger/ReservationLedgerList";

            ViewBag.ClubId = ClubId;
            ViewBag.SearchText = SearchText;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            return View(Response);
        }
    }
}