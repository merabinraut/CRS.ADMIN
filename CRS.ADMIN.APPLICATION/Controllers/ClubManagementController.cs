using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ClubManagement;
using CRS.ADMIN.APPLICATION.Models.HostManagement;
using CRS.ADMIN.APPLICATION.Models.TagManagement;
using CRS.ADMIN.BUSINESS.ClubManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.Home;
using CRS.ADMIN.SHARED.PaginationManagement;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using static Google.Apis.Requests.BatchRequest;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class ClubManagementController : BaseController
    {
        private readonly IClubManagementBusiness _BUSS;
        public ClubManagementController(IClubManagementBusiness BUSS)
        {
            _BUSS = BUSS;
        }
        [HttpGet]
        public ActionResult ClubList(string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            ViewBag.SearchFilter = SearchFilter;
            Session["CurrentURL"] = "/ClubManagement/ClubList";
            string RenderId = "";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ViewBag.Pref = DDLHelper.LoadDropdownList("PREF") as Dictionary<string, string>;
            ViewBag.PlansList = ApplicationUtilities.LoadDropdownList("CLUBPLANS") as Dictionary<string, string>;
            var response = new ClubManagementCommonModel();
            if (TempData.ContainsKey("ManageClubModel")) response.ManageClubModel = TempData["ManageClubModel"] as ManageClubModel;
            else response.ManageClubModel = new ManageClubModel();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            if (TempData.ContainsKey("ManageTagModel")) response.ManageTag = TempData["ManageTagModel"] as ManageTag;
            else response.ManageTag = new ManageTag();
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            var dbResponse = _BUSS.GetClubList(dbRequest);
            List<PlanListCommon> planlist = _BUSS.GetClubPlanIdentityList(culture);
            response.ManageClubModel.PlanDetailList = planlist.MapObjects<PlanList>();
            List<planIdentityDataCommon> addablerow = _BUSS.GetClubPlanIdentityListAddable(culture);
            response.ManageClubModel.PlanDetailList = planlist.MapObjects<PlanList>();
            ////response.ManageClubModel.PlanDetailList.ForEach(x => x.IdentityLabel = (!string.IsNullOrEmpty(culture) && culture == "en") ? x.English : x.japanese);
            response.ClubListModel = dbResponse.MapObjects<ClubListModel>();
            foreach (var item in response.ClubListModel)
            {
                item.AgentId = item.AgentId?.EncryptParameter();
            }


            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.LocationDDLList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("LOCATIONDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.BusinessTypeDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("BUSINESSTYPEDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.RankDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("RANKDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.ClubStoreDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBSTOREDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.ClubCategoryDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBCATEGORYDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.RankDDLKey = response.ManageTag.Tag2RankName;
            ViewBag.ClubStoreDDLKey = response.ManageTag.Tag5StoreName;
            ViewBag.ClubCategoryDDLKey = response.ManageTag.Tag3CategoryName;
            ViewBag.LocationIdKey = response.ManageClubModel.LocationId ?? response.ManageTag.Tag1Location;
            ViewBag.BusinessTypeKey = response.ManageClubModel.BusinessType;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            response.SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null;
            return View(response);
        }

        [HttpGet]
        public ActionResult ManageClub(string AgentId = "")
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ManageClubModel model = new ManageClubModel();
            if (!string.IsNullOrEmpty(AgentId))
            {
                var id = AgentId.DecryptParameter();
                if (string.IsNullOrEmpty(id))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid club details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("ClubList", "ClubManagement");
                }
                var dbResponse = _BUSS.GetClubDetails(id);
                List<PlanListCommon> planlist = _BUSS.GetClubPlanIdentityList(culture);
                model.PlanDetailList = planlist.MapObjects<PlanList>();
                model = dbResponse.MapObject<ManageClubModel>();
                model.AgentId = model.AgentId.EncryptParameter();
                model.LocationId = !string.IsNullOrEmpty(model.LocationId) ? model.LocationId.EncryptParameter() : null;
                model.BusinessType = !string.IsNullOrEmpty(model.BusinessType) ? model.BusinessType.EncryptParameter() : null;
            }
            TempData["ManageClubModel"] = model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("ClubList", "ClubManagement");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageClub(ManageClubModel Model, HttpPostedFileBase Business_Certificate, HttpPostedFileBase Logo_Certificate, HttpPostedFileBase CoverPhoto_Certificate, string LocationDDL, string BusinessTypeDDL)
        {
            string ErrorMessage = string.Empty;
            if
            (
               string.IsNullOrEmpty(Model.BusinessCertificate) ||
               string.IsNullOrEmpty(Model.Logo) ||
               string.IsNullOrEmpty(Model.CoverPhoto) ||
               string.IsNullOrEmpty(Model.Gallery)
            )
            {
                if (Business_Certificate == null || Logo_Certificate == null || CoverPhoto_Certificate == null)
                {
                    bool allowRedirect = false;

                    if (Business_Certificate == null && string.IsNullOrEmpty(Model.BusinessCertificate))
                    {
                        ErrorMessage = "Business certificate required";
                        allowRedirect = true;
                    }
                    else if (Logo_Certificate == null && string.IsNullOrEmpty(Model.Logo))
                    {
                        ErrorMessage = "Logo required";
                        allowRedirect = true;
                    }
                    else if (CoverPhoto_Certificate == null && string.IsNullOrEmpty(Model.CoverPhoto))
                    {
                        ErrorMessage = "Cover photo required";
                        allowRedirect = true;
                    }
                    if (allowRedirect)
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManageClubModel"] = Model;
                        TempData["RenderId"] = "Manage";
                        return RedirectToAction("ClubList", "ClubManagement");
                    }
                }
            }
            if (!string.IsNullOrEmpty(BusinessTypeDDL?.DecryptParameter())) ModelState.Remove("BusinessType");
            if (!string.IsNullOrEmpty(LocationDDL?.DecryptParameter())) ModelState.Remove("LocationId");
            ModelState.Remove("LocationURL");
            if (ModelState.IsValid)
            {
                string businessCertificatePath = "";
                string LogoPath = "";
                string coverPhotoPath = "";
                var allowedContentType = AllowedImageContentType();
                string dateTime = "";
                if (Business_Certificate != null)
                {
                    var contentType = Business_Certificate.ContentType;
                    var ext = Path.GetExtension(Business_Certificate.FileName);
                    if (allowedContentType.Contains(contentType.ToLower()))
                    {
                        dateTime = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                        string fileName = "BusinessCertificate_" + dateTime + ext.ToLower();
                        businessCertificatePath = Path.Combine(Server.MapPath("~/Content/UserUpload/ClubManagement"), fileName);
                        Model.BusinessCertificate = "/Content/UserUpload/ClubManagement/" + fileName;
                    }
                    else
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid image format.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManageClubModel"] = Model;
                        TempData["RenderId"] = "Manage";
                        return RedirectToAction("ClubList", "ClubManagement");
                    }
                }

                if (Logo_Certificate != null)
                {
                    var contentType = Logo_Certificate.ContentType;
                    var ext = Path.GetExtension(Logo_Certificate.FileName);
                    if (allowedContentType.Contains(contentType.ToLower()))
                    {
                        dateTime = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                        string fileName = "Logo_" + dateTime + ext.ToLower();
                        LogoPath = Path.Combine(Server.MapPath("~/Content/UserUpload/ClubManagement"), fileName);
                        Model.Logo = "/Content/UserUpload/ClubManagement/" + fileName;
                    }
                    else
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid image format.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManageClubModel"] = Model;
                        TempData["RenderId"] = "Manage";
                        return RedirectToAction("ClubList", "ClubManagement");
                    }
                }

                if (CoverPhoto_Certificate != null)
                {
                    var contentType = CoverPhoto_Certificate.ContentType;
                    var ext = Path.GetExtension(CoverPhoto_Certificate.FileName);
                    if (allowedContentType.Contains(contentType.ToLower()))
                    {
                        dateTime = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                        string fileName = "CoverPhoto_" + dateTime + ext.ToLower();
                        coverPhotoPath = Path.Combine(Server.MapPath("~/Content/UserUpload/ClubManagement"), fileName);
                        Model.CoverPhoto = "/Content/UserUpload/ClubManagement/" + fileName;
                    }
                    else
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid image format.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManageClubModel"] = Model;
                        TempData["RenderId"] = "Manage";
                        return RedirectToAction("ClubList", "ClubManagement");
                    }
                }
                ManageClubCommon commonModel = Model.MapObject<ManageClubCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                if (!string.IsNullOrEmpty(commonModel.AgentId))
                {
                    commonModel.AgentId = commonModel.AgentId.DecryptParameter();
                    if (string.IsNullOrEmpty(commonModel.AgentId))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid club details.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManageClubModel"] = Model;
                        TempData["RenderId"] = "Manage";
                        return RedirectToAction("ClubList", "ClubManagement");
                    }
                }
                commonModel.LocationId = LocationDDL?.DecryptParameter();
                commonModel.BusinessType = BusinessTypeDDL?.DecryptParameter();
                var dbResponse = _BUSS.ManageClub(commonModel);
                if (dbResponse != null && dbResponse.Code == 0)
                {
                    if (Business_Certificate != null) ApplicationUtilities.ResizeImage(Business_Certificate, businessCertificatePath);
                    if (Logo_Certificate != null) ApplicationUtilities.ResizeImage(Logo_Certificate, LogoPath);
                    if (CoverPhoto_Certificate != null) ApplicationUtilities.ResizeImage(CoverPhoto_Certificate, coverPhotoPath);
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });
                    return RedirectToAction("ClubList", "ClubManagement");
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });
                    TempData["ManageClubModel"] = Model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("ClubList", "ClubManagement");
                }
            }
            var errorMessages = ModelState.Where(x => x.Value.Errors.Count > 0)
                                  .SelectMany(x => x.Value.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                                  .ToList();

            var notificationModels = errorMessages.Select(errorMessage => new NotificationModel
            {
                NotificationType = NotificationMessage.INFORMATION,
                Message = errorMessage,
                Title = NotificationMessage.INFORMATION.ToString(),
            }).ToArray();
            AddNotificationMessage(notificationModels);
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key }).ToList();
            TempData["ManageClubModel"] = Model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("ClubList", "ClubManagement");
        }

        [HttpGet]
        public ActionResult ClubDetails(string AgentId)
        {
            ClubDetailModel response = new ClubDetailModel();
            var id = AgentId.DecryptParameter();
            if (string.IsNullOrEmpty(id))
            {
                this.ShowPopup(1, "Invalid club details");
                return RedirectToAction("ClubList");
            }
            var dbResponse = _BUSS.GetClubDetails(id);
            response = dbResponse.MapObject<ClubDetailModel>();
            response.AgentId = null;
            response.UserId = null;
            return View(response);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult BlockClub(string AgentId)
        {
            var response = new CommonDbResponse();
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
            }
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            string status = "D";
            var dbResponse = _BUSS.ManageClubStatus(aId, status, commonRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return Json(dbResponse.Message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UnBlockClub(string AgentId)
        {
            var response = new CommonDbResponse();
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId)) response = new CommonDbResponse { ErrorCode = 1, Message = "Invalid details" };
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            string status = "A";
            var dbResponse = _BUSS.ManageClubStatus(aId, status, commonRequest);
            response = dbResponse;
            return Json(response.SetMessageInTempData(this));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ResetClubPassword(string AgentId)
        {
            var response = new CommonDbResponse();
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId)) response = new CommonDbResponse { ErrorCode = 1, Message = "Invalid details" };
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            var dbResponse = _BUSS.ResetClubUserPassword(aId, "", commonRequest);
            response = dbResponse;
            return Json(response.SetMessageInTempData(this));
        }

        #region "Tag Management"
        [HttpGet]
        public ActionResult ManageTag(string ClubId = "")
        {
            var ResponseModel = new ManageTag();
            var cId = ClubId?.DecryptParameter();
            if (string.IsNullOrEmpty(cId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ClubList", "ClubManagement");
            }
            else
            {
                var dbResponseInfo = _BUSS.GetTagDetails(cId);
                if (dbResponseInfo == null || dbResponseInfo.Code != "0")
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponseInfo.Message ?? "Invalid details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("ClubList", "ClubManagement");
                }
                ResponseModel = dbResponseInfo.MapObject<ManageTag>();
                ResponseModel.ClubId = ClubId;
                ResponseModel.TagId = ResponseModel.TagId.EncryptParameter();
                ResponseModel.Tag1Location = ResponseModel.Tag1Location?.EncryptParameter();
                ResponseModel.Tag2RankName = ResponseModel.Tag2RankName?.EncryptParameter();
                ResponseModel.Tag3CategoryName = ResponseModel.Tag3CategoryName?.EncryptParameter();
                ResponseModel.Tag5StoreName = ResponseModel.Tag5StoreName?.EncryptParameter();
                TempData["ManageTagModel"] = ResponseModel;
                TempData["RenderId"] = "ManageTag";
                return RedirectToAction("ClubList", "ClubManagement");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageTag(ManageTag Model, string tag1Select, string tag2Select, string tag3Select, string tag5Select)
        {
            var ClubId = !string.IsNullOrEmpty(Model.ClubId) ? Model.ClubId.DecryptParameter() : null;
            var TagId = !string.IsNullOrEmpty(Model.TagId) ? Model.TagId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(ClubId) || string.IsNullOrEmpty(TagId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ClubList", "ClubManagement");
            }
            if (ModelState.IsValid)
            {
                var dbRequest = Model.MapObject<ManageTagCommon>();
                dbRequest.ClubId = ClubId;
                dbRequest.TagId = TagId;
                dbRequest.Tag1Status = string.IsNullOrEmpty(dbRequest.Tag1Status) ? "B" : dbRequest.Tag1Status;
                dbRequest.Tag1Location = !string.IsNullOrEmpty(dbRequest.Tag1Status) ? tag1Select?.DecryptParameter() : null;
                dbRequest.Tag2Status = string.IsNullOrEmpty(dbRequest.Tag2Status) ? "B" : dbRequest.Tag2Status;
                dbRequest.Tag2RankName = !string.IsNullOrEmpty(dbRequest.Tag2Status) ? tag2Select?.DecryptParameter() : null;
                dbRequest.Tag3Status = string.IsNullOrEmpty(dbRequest.Tag3Status) ? "B" : dbRequest.Tag3Status;
                dbRequest.Tag3CategoryName = !string.IsNullOrEmpty(dbRequest.Tag3Status) ? tag3Select?.DecryptParameter() : null;
                dbRequest.Tag4Status = string.IsNullOrEmpty(dbRequest.Tag4Status) ? "B" : dbRequest.Tag4Status;
                dbRequest.Tag5Status = string.IsNullOrEmpty(dbRequest.Tag5Status) ? "B" : dbRequest.Tag5Status;
                dbRequest.Tag5StoreName = !string.IsNullOrEmpty(dbRequest.Tag5Status) ? tag5Select?.DecryptParameter() : null;
                dbRequest.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                dbRequest.ActionIP = ApplicationUtilities.GetIP();
                var dbResponse = _BUSS.ManageTag(dbRequest);
                if (dbResponse != null && dbResponse.Code == 0)
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = dbResponse.Message ?? "Success",
                        Title = NotificationMessage.SUCCESS.ToString(),
                    });
                    return RedirectToAction("ClubList", "ClubManagement");
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["ManageTagModel"] = Model;
                    TempData["RenderId"] = "ManageTag";
                    return RedirectToAction("ClubList", "ClubManagement");
                }
            }
            var errorMessages = ModelState.Where(x => x.Value.Errors.Count > 0)
                                 .SelectMany(x => x.Value.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                                 .ToList();

            var notificationModels = errorMessages.Select(errorMessage => new NotificationModel
            {
                NotificationType = NotificationMessage.INFORMATION,
                Message = errorMessage,
                Title = NotificationMessage.INFORMATION.ToString(),
            }).ToArray();
            AddNotificationMessage(notificationModels);
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key }).ToList();
            TempData["ManageTagModel"] = Model;
            TempData["RenderId"] = "ManageTag";
            return RedirectToAction("ClubList", "ClubManagement");
        }

        #endregion

        #region Gallery Management
        [HttpGet]
        public ActionResult GalleryManagement(string ClubId, string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            PaginationFilterCommon request = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = SearchFilter
            };
            var CID = ClubId?.DecryptParameter();
            if (string.IsNullOrEmpty(CID))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid club details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ClubList", "ClubManagement");
            }
            string RenderId = "";
            var ReturnModel = new CommonGalleryManagementModel();
            if (TempData.ContainsKey("GalleryManagementModel")) ReturnModel.ManageGalleryImageModel = TempData["GalleryManagementModel"] as ManageGalleryImageModel;
            else ReturnModel.ManageGalleryImageModel = new ManageGalleryImageModel();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            var dbResponse = _BUSS.GetGalleryImage(CID, request, "");
            ReturnModel.GalleryManagementModel = dbResponse.MapObjects<GalleryManagementModel>();
            foreach (var item in ReturnModel.GalleryManagementModel)
            {
                item.AgentId = item.AgentId?.EncryptParameter();
                item.GalleryId = item.GalleryId?.EncryptParameter();
            }
            ReturnModel.ManageGalleryImageModel.AgentId = ClubId;
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/ClubManagement/ClubList";
            ViewBag.SearchFilter = SearchFilter;
            ViewBag.ClubId = ClubId;

            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            return View(ReturnModel);
        }

        [HttpGet]
        public ActionResult ManageGallery(string AgentId, string GalleryId)
        {
            ManageGalleryImageModel model = new ManageGalleryImageModel();
            PaginationFilterCommon request = new PaginationFilterCommon();
            var aId = AgentId?.DecryptParameter();
            var gId = GalleryId?.DecryptParameter();
            if (string.IsNullOrEmpty(aId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid club details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ClubList", "ClubManagement");
            }
            if (string.IsNullOrEmpty(gId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid gallery details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("GalleryManagement", "ClubManagement");
            }
            var dbResponse = _BUSS.GetGalleryImage(aId, request, gId);
            model = dbResponse[0].MapObject<ManageGalleryImageModel>();
            model.AgentId = model.AgentId.EncryptParameter();
            model.GalleryId = model.GalleryId.EncryptParameter();
            TempData["GalleryManagementModel"] = model;
            TempData["RenderId"] = "ManageClubGallery";
            return RedirectToAction("GalleryManagement", "ClubManagement", new { ClubId = AgentId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageGallery(ManageGalleryImageModel Model, HttpPostedFileBase Image_Path)
        {
            var aId = Model.AgentId?.DecryptParameter();

            if (string.IsNullOrEmpty(aId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid club details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ClubList", "ClubManagement");
            }
            if (!string.IsNullOrEmpty(Model.GalleryId))
            {
                var gId = Model.GalleryId?.DecryptParameter();
                if (string.IsNullOrEmpty(gId))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid gallery details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["GalleryManagementModel"] = Model;
                    TempData["RenderId"] = "ManageClubGallery";
                    return RedirectToAction("GalleryManagement", "ClubManagement", new { ClubId = Model.AgentId });
                }
            }
            if (Image_Path == null)
            {
                if (string.IsNullOrEmpty(Model.ImagePath))
                {
                    bool allowRedirect = false;
                    var ErrorMessage = string.Empty;
                    if (Image_Path == null && string.IsNullOrEmpty(Model.ImagePath))
                    {
                        ErrorMessage = "Image required";
                        allowRedirect = true;
                    }
                    if (allowRedirect)
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["GalleryManagementModel"] = Model;
                        TempData["RenderId"] = "ManageClubGallery";
                        return RedirectToAction("GalleryManagement", "ClubManagement", new { ClubId = Model.AgentId });
                    }
                }
            }
            string ImagePath = "";
            var allowedContentType = AllowedImageContentType();
            if (Image_Path != null)
            {
                var contentType = Image_Path.ContentType;
                var ext = Path.GetExtension(Image_Path.FileName);
                if (allowedContentType.Contains(contentType.ToLower()))
                {
                    var dateTime = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                    string fileName = "GalleryImage_" + dateTime + ext.ToLower();
                    ImagePath = Path.Combine(Server.MapPath("~/Content/UserUpload/ClubManagement/Gallery"), fileName);
                    Model.ImagePath = "/Content/UserUpload/ClubManagement/Gallery/" + fileName;
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid image format.",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["GalleryManagementModel"] = Model;
                    TempData["RenderId"] = "ManageClubGallery";
                    return RedirectToAction("GalleryManagement", "ClubManagement", new { ClubId = Model.AgentId });
                }
            }
            var dbRequest = Model.MapObject<ManageGalleryImageCommon>();
            dbRequest.AgentId = Model.AgentId?.DecryptParameter();
            dbRequest.GalleryId = Model.GalleryId?.DecryptParameter();
            dbRequest.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
            dbRequest.ActionIP = ApplicationUtilities.GetIP();
            var dbResponse = _BUSS.ManageGalleryImage(dbRequest);
            if (dbResponse != null && dbResponse.Code == 0)
            {
                if (Image_Path != null) ApplicationUtilities.ResizeImage(Image_Path, ImagePath);
                //if (Image_Path != null) Image_Path.SaveAs(ImagePath);
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                    Message = dbResponse.Message ?? "Failed",
                    Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                });
                return RedirectToAction("GalleryManagement", "ClubManagement", new { ClubId = Model.AgentId });
            }
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = dbResponse.Message ?? "Failed",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                TempData["GalleryManagementModel"] = Model;
                TempData["RenderId"] = "ManageClubGallery";
                return RedirectToAction("GalleryManagement", "ClubManagement", new { ClubId = Model.AgentId });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ManageGalleryStatus(string AgentId, string GalleryId)
        {
            var response = new CommonDbResponse();
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            var gId = !string.IsNullOrEmpty(GalleryId) ? GalleryId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId) || string.IsNullOrEmpty(gId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return Json(response.Message, JsonRequestBehavior.AllowGet);
            }
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            var dbResponse = _BUSS.ManageGalleryImageStatus(aId, gId, commonRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return Json(response.Message, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Event 



        [HttpGet]
        public ActionResult EventList(string ClubId = "",string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            ViewBag.SearchFilter = null;
            Session["CurrentURL"] = "/ClubManagement/EventList";
            string RenderId = "";
            var response = new EventManagementCommonModel();
            if (TempData.ContainsKey("ManageEventModel")) response.ManageEventModel = TempData["ManageEventModel"] as EventModel;
            else response.ManageEventModel = new EventModel();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            var dbResponse = _BUSS.GetEventList(dbRequest, ClubId.DecryptParameter());
            response.EventListModel = dbResponse.MapObjects<EventListModel>();
            foreach (var item in response.EventListModel)
            {
                item.AgentId = item.AgentId?.EncryptParameter();
                item.EventId = item.EventId?.EncryptParameter();
           
            }
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.EventType = ApplicationUtilities.LoadDropdownValuesList("EVENTTYPE", "", "", culture);
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            response.SearchFilter = null;
            response.ManageEventModel.AgentId = ClubId;
           
            return View(response);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageEvent(EventModel Model, HttpPostedFileBase Image)
        {
            var agentid = string.Empty;
            
                string ImagePath = "";
                string ErrorMessage = string.Empty;
                 agentid = Model.AgentId;
                if (string.IsNullOrEmpty(Model.EventType))
                {
                    ErrorMessage = "Event Type is Required.";
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("EventList", "ClubManagement", new
                    {
                        ClubId = agentid
                    });
                }
                Model.EventType = Model.EventType.DecryptParameter();
                if (Model.EventType.ToUpper() == "1") //check for notice event type
                {
                    Model.Title = "";
                    Model.Image = "";
                   ModelState.Remove("Title");
                if (Model.EventDate == null || Model.Description == null)
                    {
                        bool allowRedirect = false;

                        if (string.IsNullOrEmpty(Model.EventDate))
                        {
                            ErrorMessage = "Event Date required";
                            allowRedirect = true;
                        }
                        else if (string.IsNullOrEmpty(Model.Description))
                        {
                            ErrorMessage = "Event Description required";
                            allowRedirect = true;
                        }

                        if (allowRedirect)
                        {
                            this.AddNotificationMessage(new NotificationModel()
                            {
                                NotificationType = NotificationMessage.INFORMATION,
                                Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                                Title = NotificationMessage.INFORMATION.ToString(),
                            });
                            TempData["ManageEventModel"] = Model;
                            TempData["RenderId"] = "Manage";
                            return RedirectToAction("EventList", "ClubManagement", new
                            {
                                ClubId = agentid
                            });
                        }
                    }
                }
                else if (Model.EventType.ToUpper() == "2")     //check for schedule event type
                {
                    if (Model.EventDate == null || Model.Description == null || Model.Title == null)
                    {
                        bool allowRedirect = false;

                        if (string.IsNullOrEmpty(Model.EventDate))
                        {
                            ErrorMessage = "Event Date required";
                            allowRedirect = true;
                        }
                        else if (string.IsNullOrEmpty(Model.Description))
                        {
                            ErrorMessage = "Event Description required";
                            allowRedirect = true;
                        }
                        else if (string.IsNullOrEmpty(Model.Description))
                        {
                            ErrorMessage = "Event Title required";
                            allowRedirect = true;
                        }

                        if (allowRedirect)
                        {
                            this.AddNotificationMessage(new NotificationModel()
                            {
                                NotificationType = NotificationMessage.INFORMATION,
                                Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                                Title = NotificationMessage.INFORMATION.ToString(),
                            });
                            TempData["ManageEventModel"] = Model;
                            TempData["RenderId"] = "Manage";
                            return RedirectToAction("EventList", "ClubManagement", new
                            {
                                ClubId = agentid
                            });
                        }
                    }
                    var allowedContentType = AllowedImageContentType();


                    if (Image == null)
                    {
                        if (string.IsNullOrEmpty(Model.Image))
                        {
                            bool allowRedirect = false;

                            if (Image == null && string.IsNullOrEmpty(Model.Image))
                            {
                                ErrorMessage = "Image required";
                                allowRedirect = true;
                            }
                            if (allowRedirect)
                            {
                                this.AddNotificationMessage(new NotificationModel()
                                {
                                    NotificationType = NotificationMessage.INFORMATION,
                                    Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                                    Title = NotificationMessage.INFORMATION.ToString(),
                                });
                                TempData["ManageEventModel"] = Model;
                                TempData["RenderId"] = "ManageClubGallery";
                                return RedirectToAction("EventList", "ClubManagement", new { ClubId = agentid });
                            }
                        }
                    }  //for update  of image
                    if (Image != null)
                    {
                        var contentType = Image.ContentType;
                        var ext = Path.GetExtension(Image.FileName);
                        if (allowedContentType.Contains(contentType.ToLower()))
                        {
                            var dateTime = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                            string fileName = "EventImage_" + dateTime + ext.ToLower();
                            ImagePath = Path.Combine(Server.MapPath("~/Content/UserUpload/ClubManagement/Event"), fileName);
                            Model.Image = "/Content/UserUpload/ClubManagement/Event/" + fileName;
                        }
                        else
                        {
                            this.AddNotificationMessage(new NotificationModel()
                            {
                                NotificationType = NotificationMessage.INFORMATION,
                                Message = "Invalid image format.",
                                Title = NotificationMessage.INFORMATION.ToString(),
                            });
                            TempData["ManageEventModel"] = Model;
                            TempData["RenderId"] = "ManageClubGallery";
                            return RedirectToAction("EventList", "ClubManagement", new { ClubId = agentid });
                        }
                    }
                }

            if (ModelState.IsValid)
            {
                EventCommon commonModel = Model.MapObject<EventCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                if (!string.IsNullOrEmpty(commonModel.AgentId))
                {
                    commonModel.AgentId = commonModel.AgentId.DecryptParameter();
                    if (string.IsNullOrEmpty(commonModel.AgentId))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid event details.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManageEventModel"] = Model;
                        TempData["RenderId"] = "Manage";
                        return RedirectToAction("EventList", "ClubManagement", new
                        {
                            ClubId = agentid
                        });
                    }
                }
                if (Model.EventId!=null)
                {
                    commonModel.EventId = Model.EventId.DecryptParameter();
                }
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                var dbResponse = _BUSS.ManageEvent(commonModel);
                if (dbResponse != null && dbResponse.Code == 0)
                {
                    if (Image != null) ApplicationUtilities.ResizeImage(Image, ImagePath);

                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });
                    return RedirectToAction("EventList", "ClubManagement", new
                    {
                        ClubId = agentid
                    });
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });
                    TempData["ManageEventModel"] = Model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("EventList", "ClubManagement", new
                    {
                        ClubId = agentid
                    });
                }
            }
            var errorMessages = ModelState.Where(x => x.Value.Errors.Count > 0)
                                  .SelectMany(x => x.Value.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                                  .ToList();

            var notificationModels = errorMessages.Select(errorMessage => new NotificationModel
            {
                NotificationType = NotificationMessage.INFORMATION,
                Message = errorMessage,
                Title = NotificationMessage.INFORMATION.ToString(),
            }).ToArray();
            AddNotificationMessage(notificationModels);
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key }).ToList();
            TempData["ManageEventModel"] = Model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("EventList", "ClubManagement", new
            {
                ClubId = agentid
            });
        }


        [HttpGet]
        public ActionResult ManageEvent(string AgentId = "",string EventId="")
        {
            EventModel model = new EventModel();
            var agentid = AgentId;
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            ViewBag.EventType = ApplicationUtilities.LoadDropdownValuesList("EVENTTYPE", "", "", culture);
            if (!string.IsNullOrEmpty(AgentId)  )
            {
                if (!string.IsNullOrEmpty(EventId))
                {
                    var id = AgentId.DecryptParameter();
                    var Eventid = EventId.DecryptParameter();
                    if (string.IsNullOrEmpty(id))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid event details",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        return RedirectToAction("EventList", "ClubManagement", new
                        {
                            ClubId =agentid
                        });
                    }
                    if (string.IsNullOrEmpty(Eventid))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid event details",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        return RedirectToAction("EventList", "ClubManagement", new
                        {
                            ClubId = agentid
                        });
                    }
                    var dbResponse = _BUSS.GetEventDetails(id,Eventid);
                    model = dbResponse.MapObject<EventModel>();
                    model.AgentId = model.AgentId.EncryptParameter();
                    model.EventId = model.EventId.EncryptParameter();
                }
            }
            TempData["ManageEventModel"] = model;
           TempData["RenderId"] = "Manage";

            return RedirectToAction("EventList", "ClubManagement", new
            {
                ClubId = model.AgentId
            });
        }

        [HttpGet]
        public ActionResult DeleteEvent(string AgentId = "", string EventId = "")
        {
            var response = new CommonDbResponse();
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
            }
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            EventCommon commonModel =new EventCommon();
            commonModel.AgentId = aId;
            commonModel.flag = "del";
            commonModel.EventId = EventId.DecryptParameter();
            var dbResponse = _BUSS.ManageEvent(commonModel);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });

            return RedirectToAction("EventList", "ClubManagement", new
            {
                ClubId = AgentId
            });
            
        }


        #endregion
    }
}