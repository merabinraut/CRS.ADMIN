using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.CustomerManagement;
using CRS.ADMIN.APPLICATION.Models.StaffManagement;
using CRS.ADMIN.BUSINESS.StaffManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.StaffManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class StaffManagementController : BaseController
    {
        private readonly IStaffManagementBusiness _business;
        public StaffManagementController(IStaffManagementBusiness business)
        {
            _business = business;
        }
        [HttpGet]
        public ActionResult StaffList(CustomerListCommonModel Request, int StartIndex = 0, int PageSize = 10)
        {
            Session["CurrentUrl"] = "/StaffManagement/StaffList";
            string RenderId = "";
            StaffManagementCommonModel responseInfo = new StaffManagementCommonModel();
            if (TempData.ContainsKey("ManageStaffModel")) responseInfo.ManageStaffModel = TempData["ManageStaffModel"] as ManageStaff;
            else responseInfo.ManageStaffModel = new ManageStaff();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(Request.SearchFilter) ? Request.SearchFilter : null
            };
            var dbResponseInfo = _business.GetStaffList(dbRequest);
            responseInfo.StaffListModel = dbResponseInfo.MapObjects<StaffManagementModel>();
            foreach (var item in responseInfo.StaffListModel)
            {
                item.ActionDate = DateTime.Parse(item.ActionDate).ToString("MMM d, yyyy HH:mm:ss");
                item.ProfileImage = ImageHelper.ProcessedImage(item.ProfileImage);
            }
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.RoleDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("ROLEDDL", "2", "") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.RoleIdKey = responseInfo.ManageStaffModel.RoleId.EncryptParameter();
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponseInfo != null && dbResponseInfo.Any() ? dbResponseInfo[0].TotalRecords : 0;
            responseInfo.SearchFilter = !string.IsNullOrEmpty(Request.SearchFilter) ? Request.SearchFilter : null;
            return View(responseInfo);
        }
        [HttpGet]
        public ActionResult ManageStaff(string id = "")
        {
            ManageStaff model = new ManageStaff();
            if (!string.IsNullOrEmpty(id))
            {
                if (string.IsNullOrEmpty(id))
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid staff details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("StaffList", "StaffManagement");
                }
                var dbResponseInfo = _business.GetStaffDetails(id);
                model = dbResponseInfo.MapObject<ManageStaff>();
                model.Id = model.Id.EncryptParameter();
            }
            TempData["ManageStaffModel"] = model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("StaffList");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageStaff(ManageStaff Model, string roleIdSelect = "")
        {
            if (ModelState.IsValid)
            {
                ManagerStaffCommon commonModel = Model.MapObject<ManagerStaffCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                if (!string.IsNullOrEmpty(roleIdSelect?.DecryptParameter())) ModelState.Remove("RoleId");
                commonModel.RoleId = roleIdSelect?.DecryptParameter();
                if (commonModel.Id != null)
                {
                    commonModel.Id = commonModel.Id.DecryptParameter();
                }
                var dbResponseInfo = _business.ManageStaff(commonModel);
                if (dbResponseInfo == null)
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.WARNING,
                        Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                        Title = NotificationMessage.WARNING.ToString()
                    });
                    return RedirectToAction("StaffList");
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = dbResponseInfo.Message ?? "Staff Added Successfully",
                        Title = NotificationMessage.SUCCESS.ToString()
                    });
                    return RedirectToAction("StaffList");
                }
            }

            TempData["ManageStaffModel"] = Model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("StaffList");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteStaff(string id = "")
        {
            var responseInfo = new CommonDbResponse();
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
            };
            var dbResponseInfo = _business.DeleteStaff(id, commonRequest);
            responseInfo = dbResponseInfo;
            if (dbResponseInfo == null)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.WARNING,
                    Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                    Title = NotificationMessage.WARNING.ToString()
                });
                return Json(responseInfo.SetMessageInTempData(this));
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponseInfo.Message ?? "Staff has been deleted successfully",
                    Title = NotificationMessage.SUCCESS.ToString()
                });

            }
            return Json(responseInfo.SetMessageInTempData(this));
        }
    }
}