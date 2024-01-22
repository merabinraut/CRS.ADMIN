using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.RoleManagement;
using CRS.ADMIN.BUSINESS.RoleManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.RoleManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class RoleManagementController : BaseController
    {
        private readonly IRoleManagementBusiness _BUSS;
        public RoleManagementController(IRoleManagementBusiness BUSS)
        {
            _BUSS = BUSS;
        }
        [HttpGet]
        public ActionResult RoleList()
        {
            Session["CurrentUrl"] = "/RoleManagement/RoleList";
            string RenderId = "";
            RoleManagementCommonModel ResponseModel = new RoleManagementCommonModel();
            if (TempData.ContainsKey("ManageAgentRoleModel")) ResponseModel.ManageAgentRoleModel = TempData["ManageAgentRoleModel"] as ManageAgentRoleModel;
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            var dbResponse = _BUSS.GetRoleList();
            ResponseModel.RoleListModel = dbResponse.MapObjects<RoleListModel>();
            foreach (var item in ResponseModel.RoleListModel)
            {
                item.RoleId = item.RoleId.EncryptParameter();
            }
            ViewBag.UserTypeDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("USERTYPEDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.UserListViaTypeDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("USERLISTVIATYPEDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.RoleDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("ROLEDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            return View(ResponseModel);
        }

        [HttpGet]
        public ActionResult RoleTypeList(string RoleId, string RoleName, string SearchFilter = "")
        {
            Session["CurrentUrl"] = "/RoleManagement/RoleList";
            string RenderId = "";
            var ResponseModel = new RoleTypeManagementCommonModel();
            var roleType = !string.IsNullOrEmpty(RoleId) ? RoleId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(roleType))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("RoleList", "RoleManagement");
            }
            var dbResponse = _BUSS.GetRoleTypeList(roleType, SearchFilter);
            ResponseModel.RoleTypeListModel = dbResponse.MapObjects<RoleTypeListModel>();
            ResponseModel.RoleTypeListModel.ForEach(x => x.RoleId = x.RoleId.EncryptParameter());
            ResponseModel.RoleTypeListModel.ForEach(x => x.RoleType = x.RoleType.EncryptParameter());
            ResponseModel.ManageRole.RoleType = RoleId;
            ResponseModel.ManageRole.RoleTypeName = RoleName;
            if (TempData.ContainsKey("ManageRole")) ResponseModel.ManageRole = TempData["ManageRole"] as ManageRole;
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/RoleManagement/RoleList";
            ViewBag.RoleId = RoleId;
            ViewBag.RoleName = RoleName;
            ViewBag.SearchFilter = SearchFilter;
            return View(ResponseModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddRoleType(ManageRole Model)
        {
            var roleType = !string.IsNullOrEmpty(Model.RoleType) ? Model.RoleType.DecryptParameter() : null;
            if (string.IsNullOrEmpty(roleType))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("RoleList", "RoleManagement");
            }
            if (ModelState.IsValid)
            {
                var request = Model.MapObject<ManageRoleCommon>();
                request.RoleType = request.RoleType.DecryptParameter();
                request.ActionUser = ApplicationUtilities.GetSessionValue("UserName").ToString();
                request.ActionIP = ApplicationUtilities.GetIP();
                var response = _BUSS.AddRoleType(request);
                if (response.Code == ResponseCode.Success)
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = response.Message ?? "Success",
                        Title = NotificationMessage.SUCCESS.ToString(),
                    });
                    return RedirectToAction("RoleTypeList", "RoleManagement", new { RoleId = Model.RoleType, RoleName = Model.RoleTypeName });
                }
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = response.Message ?? "Failed",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                TempData["ManageRole"] = Model;
                TempData["RenderId"] = "Manage";
                return RedirectToAction("RoleTypeList", "RoleManagement", new { RoleId = Model.RoleType, RoleName = Model.RoleTypeName });
            }
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Please fill all required fields",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                TempData["ManageRole"] = Model;
                TempData["RenderId"] = "Manage";
                return RedirectToAction("RoleTypeList", "RoleManagement", new { RoleId = Model.RoleType, RoleName = Model.RoleTypeName });
            }
        }

        [HttpGet]
        public ActionResult Menus(string RoleId, string Name, string RoleTypeId)
        {
            var id = !string.IsNullOrEmpty(RoleId) ? RoleId.DecryptParameter() : null;
            var rId = !string.IsNullOrEmpty(RoleTypeId) ? RoleTypeId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(rId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return RedirectToAction("RoleList", "RoleManagement");
            }
            var Response = new List<MenuManagementListModel>();
            var dbResponse = _BUSS.GetMenus(id);
            if (dbResponse.Count > 0)
            {
                Response = dbResponse.MapObjects<MenuManagementListModel>();
                Response.ForEach(x => x.MenuId = x.MenuId.EncryptParameter());
            }
            ViewBag.RoleName = Name;
            ViewBag.RoleId = RoleId;
            ViewBag.RoleTypeId = RoleTypeId;
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/RoleManagement/RoleTypeList?RoleId=" + RoleTypeId + "&&RoleName=" + Name;
            return View(Response);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Menus(string RoleId, string[] RoleList, string RoleName, string RoleTypeId)
        {
            var redirectToUrl = string.Empty;
            var id = !string.IsNullOrEmpty(RoleId) ? RoleId.DecryptParameter() : null;
            var rId = !string.IsNullOrEmpty(RoleTypeId) ? RoleTypeId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(rId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                redirectToUrl = Url.Action("RoleList", "RoleManagement");
                return Json(new { redirectToUrl });
            }
            var rList = RoleList.Select(x => x.DecryptParameter()).ToList();
            var roles = RoleList != null ? string.Join(",", rList.ToArray()) : null;
            var ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
            var ActionIP = ApplicationUtilities.GetIP();
            var dbResponse = _BUSS.AssignMenus(id, roles, ActionUser, ActionIP);
            if (dbResponse.Code == ResponseCode.Success)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponse.Message ?? "Success",
                    Title = NotificationMessage.SUCCESS.ToString()
                });
                redirectToUrl = Url.Action("RoleTypeList", "RoleManagement", new { RoleId = RoleTypeId, RoleName = RoleName });
                return Json(new { redirectToUrl });
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = dbResponse.Message ?? "Failed",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return Json(new { redirectToUrl });
            }
        }

        [HttpGet]
        public ActionResult Functions(string RoleId, string Name, string RoleTypeId)
        {
            var id = !string.IsNullOrEmpty(RoleId) ? RoleId.DecryptParameter() : null;
            var rId = !string.IsNullOrEmpty(RoleTypeId) ? RoleTypeId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(rId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return RedirectToAction("RoleList", "RoleManagement");
            }
            var ResponseModel = new List<FunctionManagementListModel>();
            var dbResponse = _BUSS.GetFunctions(id);
            if (dbResponse.Count > 0)
            {
                ResponseModel = dbResponse.MapObjects<FunctionManagementListModel>();
                ResponseModel.ForEach(x => x.FunctionId = x.FunctionId.EncryptParameter());
            }
            ViewBag.RoleName = Name;
            ViewBag.RoleId = RoleId;
            ViewBag.RoleTypeId = RoleTypeId;
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/RoleManagement/RoleTypeList?RoleId=" + RoleTypeId + "&&RoleName=" + Name;
            return View(ResponseModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Functions(string RoleId, string[] FunctionList, string RoleName, string RoleTypeId)
        {
            var redirectToUrl = string.Empty;
            var id = !string.IsNullOrEmpty(RoleId) ? RoleId.DecryptParameter() : null;
            var rId = !string.IsNullOrEmpty(RoleTypeId) ? RoleTypeId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(rId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                redirectToUrl = Url.Action("RoleList", "RoleManagement");
                return Json(new { redirectToUrl });
            }
            var fList = FunctionList.Select(x => x.DecryptParameter()).ToList();
            var functions = fList != null ? string.Join(",", fList.ToArray()) : null;
            var actionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
            var ActionIP = ApplicationUtilities.GetIP();
            var dbResponse = _BUSS.AssignFunctions(id, functions, actionUser, ActionIP);
            if (dbResponse.Code == ResponseCode.Success)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponse.Message ?? "Success",
                    Title = NotificationMessage.SUCCESS.ToString()
                });
                redirectToUrl = Url.Action("RoleTypeList", "RoleManagement", new { RoleId = RoleTypeId, RoleName = RoleName });
                return Json(new { redirectToUrl });
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = dbResponse.Message ?? "Failed",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return Json(new { redirectToUrl });
            }
        }

        #region Assign Role Management
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AssignRole(string UserTypeDDL, string RoleDDL, string UserListViaTypeDDL)
        {
            var FailedReturnModel = new ManageAgentRoleModel()
            {
                RoleId = RoleDDL,
                AgentType = UserTypeDDL,
                AgentId = UserListViaTypeDDL,
            };
            var AgentType = !string.IsNullOrEmpty(UserTypeDDL) ? UserTypeDDL.DecryptParameter() : null;
            var RoleId = !string.IsNullOrEmpty(RoleDDL) ? RoleDDL.DecryptParameter() : null;
            var AgentId = !string.IsNullOrEmpty(UserListViaTypeDDL) ? UserListViaTypeDDL.DecryptParameter() : null;
            if (string.IsNullOrEmpty(AgentType) || string.IsNullOrEmpty(RoleId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                TempData["RenderId"] = "AssignRole";
                TempData["ManageAgentRoleModel"] = FailedReturnModel;
                return RedirectToAction("RoleList", "RoleManagement");
            }
            if (!string.IsNullOrEmpty(UserListViaTypeDDL) && string.IsNullOrEmpty(AgentId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid agent detail",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                TempData["RenderId"] = "AssignRole";
                TempData["ManageAgentRoleModel"] = FailedReturnModel;
                return RedirectToAction("RoleList", "RoleManagement");
            }

            var dbRequest = new ManageAgentRoleCommon()
            {
                RoleId = RoleId,
                AgentType = AgentType,
                AgentId = AgentId,
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
                ActionIP = ApplicationUtilities.GetIP()
            };
            var dbResponse = _BUSS.ManageAgentRole(dbRequest);
            if (dbResponse != null && dbResponse.Code == ResponseCode.Success)
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponse.Message ?? "Success",
                    Title = NotificationMessage.SUCCESS.ToString(),
                });
                return RedirectToAction("RoleList", "RoleManagement");
            }
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = NotificationMessage.INFORMATION,
                Message = dbResponse.Message ?? "Failed",
                Title = NotificationMessage.INFORMATION.ToString(),
            });
            TempData["RenderId"] = "AssignRole";
            TempData["ManageAgentRoleModel"] = FailedReturnModel;
            return RedirectToAction("RoleList", "RoleManagement");
        }
        [HttpPost, OverrideActionFilters, ValidateAntiForgeryToken]
        public async Task<JsonResult> GetAgentListAndRoleByAgentType(string AgentType)
        {
            var aId = !string.IsNullOrEmpty(AgentType) ? AgentType.DecryptParameter() : null;
            List<SelectListItem> UserListViaTypeDDL = new List<SelectListItem>();
            List<SelectListItem> RoleDDL = new List<SelectListItem>();
            if (string.IsNullOrEmpty(aId)) { return null; }
            UserListViaTypeDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("USERLISTVIATYPEDDL", aId) as Dictionary<string, string>, null);
            RoleDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("ROLEDDL", aId) as Dictionary<string, string>, null);
            return Json(new { UserListViaTypeDDL, RoleDDL }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, OverrideActionFilters, ValidateAntiForgeryToken]
        public JsonResult GetAgentCurrentRole(string AgentId, string AgentType)
        {
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            var aType = !string.IsNullOrEmpty(AgentType) ? AgentType.DecryptParameter() : null;
            if (!string.IsNullOrEmpty(aId) || !string.IsNullOrEmpty(aType))
            {
                var dbResponse = _BUSS.GetCurrentAssignedRole(aId, aType);
                if (dbResponse.Code == SHARED.ResponseCode.Success)
                {
                    return Json(new
                    {
                        RoleId = dbResponse.RoleId.EncryptParameter(),
                        RoleName = dbResponse.RoleName
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }
        #endregion
    }
}