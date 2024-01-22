using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ReservationLedger;
using CRS.ADMIN.BUSINESS.ReservationLedger;
using CRS.ADMIN.SHARED;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class ReservationLedgerController : BaseController
    {
        private readonly IReservationLedgerBusiness _business;
        public ReservationLedgerController(IReservationLedgerBusiness business) => _business = business;

        [HttpGet]
        public ActionResult ReservationLedgerList(string SearchText = "", string ClubId = "", string Date = "")
        {
            Session["CurrentURL"] = "/ReservationLedger/ReservationLedgerList";
            var cId = !string.IsNullOrEmpty(ClubId) ? ClubId.DecryptParameter() : null;
            var dbResponse = _business.GetReservationLedgerList(SearchText, cId, Date);
            var responseInfo = dbResponse.MapObjects<ReservationLedgerModel>();
            responseInfo.ForEach(x => x.ClubId = x.ClubId.EncryptParameter());
            ViewBag.LedgerList = responseInfo;
            ViewBag.SearchText = SearchText;
            return View();
        }
        [HttpGet]
        public ActionResult ReservationLedgerDetail(string ClubId, string Date, string SearchText = "")
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
            var dbResponse = _business.GetReservationLedgerDetail(cId, Date, SearchText);
            Response = dbResponse.MapObjects<ReservationLedgerDetailModel>();
            Response.ForEach(x => x.ClubId = x.ClubId.EncryptParameter());
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/ReservationLedger/ReservationLedgerList";
            return View(Response);
        }
    }
}