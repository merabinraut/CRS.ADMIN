using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ScheduleManagement;
using CRS.ADMIN.BUSINESS.ScheduleManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ScheduleManagement;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class ScheduleManagementController : BaseController
    {
        private readonly IScheduleManagementBusiness _scheduleBuss;
        public ScheduleManagementController(IScheduleManagementBusiness scheduleBuss)
        {
            _scheduleBuss = scheduleBuss;
        }
        [HttpGet]
        public ActionResult ClubScheduleManagement(string ClubId)
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ViewBag.CultureLang = culture;
            var Response = new List<ClubScheduleModel>();
            var CId = !string.IsNullOrEmpty(ClubId) ? ClubId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(CId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return RedirectToAction("ClubList", "ClubManagement");
            }
            var scheduleDBResponse = _scheduleBuss.GetClubSchedule(CId);
            if (scheduleDBResponse != null && scheduleDBResponse.Count > 0) Response = scheduleDBResponse.MapObjects<ClubScheduleModel>();
            Response.ForEach(x => x.ScheduleId = x.ScheduleId.EncryptParameter());
            string scheduleJsonData = JsonConvert.SerializeObject(Response);
            ViewBag.ClubSchedulesJson = scheduleJsonData;
            ViewBag.ClubScheduleDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBSCHEDULE") as Dictionary<string, string>, "", "--- Select ---");
            ViewBag.ClubId = ClubId;
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/ClubManagement/ClubList";
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken, OverrideActionFilters]
        public JsonResult ManageSchedule(ClubScheduleModel Request)
        {
            var redirectToUrl = string.Empty;
            var ClubId = !string.IsNullOrEmpty(Request.ClubId) ? Request.ClubId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(ClubId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return Json(new { redirectToUrl });
            }
            var ScheduleId = !string.IsNullOrEmpty(Request.ScheduleId) ? Request.ScheduleId.DecryptParameter() : null;
            var ClubSchedule = !string.IsNullOrEmpty(Request.ClubSchedule) ? Request.ClubSchedule.DecryptParameter() : null;
            if (!string.IsNullOrEmpty(Request.ScheduleId) && string.IsNullOrEmpty(ScheduleId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return Json(new { redirectToUrl });
            }
            if (string.IsNullOrEmpty(ClubSchedule))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return Json(new { redirectToUrl });
            }
            var dbRequest = Request.MapObject<ManageScheduleCommon>();
            dbRequest.ClubId = ClubId;
            dbRequest.ScheduleId = ScheduleId;
            dbRequest.ClubSchedule = ClubSchedule;
            dbRequest.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
            dbRequest.ActionIP = ApplicationUtilities.GetIP();
            var dbResponse = _scheduleBuss.ManageSchedule(dbRequest);
            if (dbResponse != null && dbResponse.Code == ResponseCode.Success)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponse.Message ?? "Success",
                    Title = NotificationMessage.SUCCESS.ToString()
                });
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = dbResponse.Message ?? "Failed",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
            }
            return Json(new { redirectToUrl });
        }
    }
}