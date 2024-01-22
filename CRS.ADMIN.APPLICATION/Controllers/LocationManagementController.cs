using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.LocationManagement;
using CRS.ADMIN.BUSINESS.LocationManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.LocationManagement;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class LocationManagementController : BaseController
    {
        private readonly ILocationManagementBusiness _business;

        public LocationManagementController(ILocationManagementBusiness business) => this._business = business;

        [HttpGet]
        public ActionResult LocationList(LocationManagementModel Model = null)
        {
            Session["CurrentURL"] = "/LocationManagemen/LocationList";
            LocationManagementModel ResponseModel = new LocationManagementModel();
            var locationLists = _business.GetLocations(Model.SearchFilter);
            ResponseModel.LocationListModel = locationLists.MapObjects<LocationListModel>();
            ResponseModel.LocationListModel.ForEach(x => x.LocationId = x.LocationId.EncryptParameter());

            string RenderId = "";
            if (TempData.ContainsKey("LocationModel")) ResponseModel.LocationModel = TempData["LocationModel"] as LocationModel;
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ResponseModel.SearchFilter = !string.IsNullOrEmpty(Model.SearchFilter) ? Model.SearchFilter : null;
            return View(ResponseModel);
        }

        public ActionResult LocationDetail(string id = "")
        {
            var viewModel = new LocationModel();

            if (!string.IsNullOrWhiteSpace(id))
            {
                var common = new LocationCommon()
                {
                    LocationId = id.DecryptParameter(),
                    ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
                    ActionIP = ApplicationUtilities.GetIP(),
                    ActionPlatform = "Admin"
                };

                var serviceResp = _business.GetLocation(common);
                viewModel = serviceResp.MapObject<LocationModel>();
            }

            return View(viewModel);
        }

        public ActionResult ManageLocation(string LocationId = "")
        {
            var lId = !string.IsNullOrEmpty(LocationId) ? LocationId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(lId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("LocationList", "LocationManagement");
            }
            var dbRequest = new LocationCommon()
            {
                LocationId = LocationId.DecryptParameter(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
                ActionIP = ApplicationUtilities.GetIP()
            };
            var dbResponse = _business.GetLocation(dbRequest);
            dbResponse.LocationId = dbResponse.LocationId.EncryptParameter();
            TempData["LocationModel"] = dbResponse.MapObject<LocationModel>();
            TempData["RenderId"] = "ManageLocation";
            return RedirectToAction("LocationList", "LocationManagement");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageLocation(LocationModel Model, HttpPostedFileBase LocationImageFile)
        {
            if (!ModelState.IsValid)
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Please fill all required fields",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                TempData["LocationModel"] = Model;
                TempData["RenderId"] = "ManageLocation";
                return RedirectToAction("LocationList", "LocationManagement");
            }
            var locationId = !string.IsNullOrEmpty(Model.LocationId) ? Model.LocationId.DecryptParameter() : null;
            if (!string.IsNullOrEmpty(Model.LocationId) && string.IsNullOrEmpty(locationId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid location details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("LocationList", "LocationManagement");
            }
            if (LocationImageFile == null)
            {
                if (string.IsNullOrEmpty(Model.LocationImage))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Image required",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["LocationModel"] = Model;
                    TempData["RenderId"] = "ManageLocation";
                    return RedirectToAction("LocationList", "LocationManagement");
                }
            }

            var common = Model?.MapObject<LocationCommon>();
            string imgPath = "";
            var allowedContentType = AllowedImageContentType();
            if (LocationImageFile != null)
            {
                var contentType = LocationImageFile.ContentType;
                var ext = Path.GetExtension(LocationImageFile.FileName);
                if (allowedContentType.Contains(contentType.ToLower()))
                {
                    string datet = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                    string myfilename = "LocationImg_" + datet + ext.ToLower();
                    imgPath = Path.Combine(Server.MapPath("~/Content/userupload/LocationImages/"), myfilename);
                    common.LocationImage = "/Content/userupload/LocationImages/" + myfilename;
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "File Must be .jpg, .png, .jpeg, .heif",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["LocationModel"] = Model;
                    TempData["RenderId"] = "ManageLocation";
                    return RedirectToAction("LocationList", "LocationManagement");
                }
            }

            common.LocationId = locationId;
            common.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
            common.ActionIP = ApplicationUtilities.GetIP();
            var serviceResp = _business.ManageLocation(common);
            if (serviceResp.Code == ResponseCode.Success)
            {
                if (LocationImageFile != null) ApplicationUtilities.ResizeImage(LocationImageFile, imgPath);
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = serviceResp.Message ?? "Success",
                    Title = NotificationMessage.SUCCESS.ToString(),
                });
                return RedirectToAction("LocationList", "LocationManagement");
            }
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = NotificationMessage.INFORMATION,
                Message = serviceResp.Message ?? "Failed",
                Title = NotificationMessage.INFORMATION.ToString()
            });
            TempData["LocationModel"] = Model;
            TempData["RenderId"] = "ManageLocation";
            return RedirectToAction("LocationList", "LocationManagement");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult EnableDisableLocation(string Id)
        {
            var i = !string.IsNullOrEmpty(Id) ? Id.DecryptParameter() : null;
            if (string.IsNullOrEmpty(i))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return Json(new { Code = 1, Message = "Invalid Request." });
            }
            var common = new LocationCommon()
            {
                LocationId = i,
                ActionUser = ApplicationUtilities.GetSessionValue("Username")?.ToString(),
                ActionIP = ApplicationUtilities.GetIP()
            };
            var dbResponse = _business.EnableDisableLocation(common);
            if (dbResponse.Code == ResponseCode.Success)
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponse.Message ?? "Success",
                    Title = NotificationMessage.SUCCESS.ToString(),
                });
            }
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = dbResponse.Message ?? "Failed",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
            }
            return Json(dbResponse.Message);
        }
    }
}