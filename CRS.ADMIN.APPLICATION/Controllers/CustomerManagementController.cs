using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.CustomerManagement;
using CRS.ADMIN.BUSINESS.CustomerManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.CustomerManagement;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class CustomerManagementController : BaseController
    {
        private readonly ICustomerManagementBusiness _BUSS;
        public CustomerManagementController(ICustomerManagementBusiness BUSS)
        {
            _BUSS = BUSS;
        }
        [HttpGet]
        public ActionResult CustomerList(CustomerListCommonModel Request, int StartIndex = 0, int PageSize = 10)
        {
            Session["CurrentURL"] = "/CustomerManagement/CustomerList";
            ViewBag.UserStatusKey = Request.Status;
            ViewBag.UserStatusDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("USERSTATUSDDL") as Dictionary<string, string>, null, "--- Select ---");
            var Response = Request.MapObject<CustomerListCommonModel>();
            var dbRequest = Request.MapObject<CustomerSearchFilterCommon>();
            dbRequest.Status = !string.IsNullOrEmpty(dbRequest.Status) ? dbRequest.Status.DecryptParameter() : null;
            dbRequest.Skip = StartIndex;
            dbRequest.Take = PageSize;
            var dbResponse = _BUSS.GetCustomerList(dbRequest);
            Response.CustomerListModel = dbResponse.MapObjects<CustomerListModel>();
            Response.CustomerListModel.ForEach(x => x.AgentId = x.AgentId.EncryptParameter());
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            return View(Response);
        }

        [HttpGet]
        public ActionResult ManageCustomer(string AgentId)
        {
            var model = new ManageCustomerModel();
            if (string.IsNullOrEmpty(AgentId))
            {
                return View(model);
            }
            else
            {
                var id = AgentId.DecryptParameter();
                if (string.IsNullOrEmpty(id))
                {
                    this.ShowPopup(1, "Invalid customer details");
                    return RedirectToAction("CustomerList", "CustomerManagement");
                }
                var dbResponse = _BUSS.GetCustomerDetail(id);
                model = dbResponse.MapObject<ManageCustomerModel>();
                model.AgentId = AgentId;
                return View(model);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageCustomer(ManageCustomerModel Request)
        {
            if (ModelState.IsValid)
            {
                var commonRequest = Request.MapObject<ManageCustomerCommon>();
                if (!string.IsNullOrEmpty(commonRequest.AgentId))
                {
                    commonRequest.AgentId = commonRequest.AgentId.DecryptParameter();
                    if (string.IsNullOrEmpty(commonRequest.AgentId))
                    {
                        this.ShowPopup(1, "Invalid customer details");
                        return RedirectToAction("CustomerList", "CustomerManagement");
                    }
                }
                commonRequest.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonRequest.ActionPlatform = "AdminWeb";
                commonRequest.ActionIP = ApplicationUtilities.GetIP();
                var dbResponse = _BUSS.ManageCustomer(commonRequest);
                if (dbResponse != null && dbResponse.Code == 0)
                {
                    this.ShowPopup((int)dbResponse.Code, dbResponse.Message);
                    return RedirectToAction("CustomerList", "CustomerManagement");
                }
                else
                {
                    this.ShowPopup((int)dbResponse.Code, dbResponse.Message);
                    return View(Request);
                }
            }
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key }).ToList();
            this.ShowPopup(2, "Required fields are missing");
            return View(Request);
        }

        [HttpGet]
        public ActionResult CustomerDetail(string AgentId)
        {
            var model = new ManageCustomerModel();
            var id = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(id))
            {
                this.ShowPopup(1, "Invalid customer details");
                return RedirectToAction("CustomerList", "CustomerManagement");
            }
            var dbResponse = _BUSS.GetCustomerDetail(id);
            model = dbResponse.MapObject<ManageCustomerModel>();
            model.AgentId = null;
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult BlockCustomer(string AgentId)
        {
            var response = new CommonDbResponse();
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return Json(JsonRequestBehavior.AllowGet);
            }
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            string status = "B";
            var dbResponse = _BUSS.ManageCustomerStatus(aId, status, commonRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UnBlockCustomer(string AgentId)
        {
            var response = new CommonDbResponse();
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return Json(JsonRequestBehavior.AllowGet);
            }

            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            string status = "A";
            var dbResponse = _BUSS.ManageCustomerStatus(aId, status, commonRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ResetCustomerPassword(string AgentId)
        {
            var response = new CommonDbResponse();
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId)) response = new CommonDbResponse { ErrorCode = 1, Message = "Invalid details" };
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            var dbResponse = _BUSS.ResetCustomerPassword(aId, commonRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return Json(JsonRequestBehavior.AllowGet);
        }


    }
}