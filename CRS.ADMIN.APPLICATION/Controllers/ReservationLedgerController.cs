﻿using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ReservationLedger;
using CRS.ADMIN.BUSINESS.ReservationLedger;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PaymentManagement;
using System;
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
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
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
                    x.AdminPayment = Convert.ToInt64(x.AdminPayment).ToString("N0");
                }
            );
            ViewBag.ClubDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBLIST","",culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
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
                    x.InvoiceId = x.InvoiceId;
                    x.Id = x.Id.EncryptParameter();                   
                    x.CustomerImage = ImageHelper.ProcessedImage(x.CustomerImage);
                    x.VisitDate = !string.IsNullOrEmpty(x.VisitDate) ? DateTime.Parse(x.VisitDate).ToString("yyyy'年'MM'月'dd'日'"):x.VisitDate;
                    x.CreatedDate = !string.IsNullOrEmpty(x.CreatedDate) ? DateTime.Parse(x.CreatedDate).ToString("yyyy'年'MM'月'dd'日'") : x.CreatedDate;
                    x.PlanAmount = Convert.ToInt64(x.PlanAmount).ToString("N0");
                    x.TotalPlanAmount = Convert.ToInt64(x.TotalPlanAmount).ToString("N0");
                    x.TotalClubPlanAmount = Convert.ToInt64(x.TotalClubPlanAmount).ToString("N0");
                    x.AdminPlanCommissionAmount = Convert.ToInt64(x.AdminPlanCommissionAmount).ToString("N0");
                    x.TotalAdminPlanCommissionAmount = Convert.ToInt64(x.TotalAdminPlanCommissionAmount).ToString("N0");
                    x.AdminCommissionAmount = Convert.ToInt64(x.AdminCommissionAmount).ToString("N0");
                    x.TotalAdminCommissionAmount = Convert.ToInt64(x.TotalAdminCommissionAmount).ToString("N0");
                    x.TotalAdminPayableAmount = Convert.ToInt64(x.TotalAdminPayableAmount).ToString("N0");
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
            ViewBag.Date = Date;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            return View(Response);
        }

        [HttpPost]
        public JsonResult VerifyOTPClubCode(string reservationId, string agentId, string code)
        {
            var response = new CommonDbResponse();
            var aId = !string.IsNullOrEmpty(agentId) ? agentId.DecryptParameter() : null;
            var rId = !string.IsNullOrEmpty(reservationId) ? reservationId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
            }
            if (string.IsNullOrEmpty(rId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Reservation Id is required",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
            }
            if (string.IsNullOrEmpty(code))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Confirmation code is required",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
            }
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };        
            var dbResponse = _business.VerifyCode(rId, aId, code, commonRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == CRS.ADMIN.SHARED.ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == CRS.ADMIN.SHARED.ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return Json(dbResponse.Message, JsonRequestBehavior.AllowGet);
        }
    }
}