using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models;
using CRS.ADMIN.APPLICATION.Models.HostManagement;
using CRS.ADMIN.BUSINESS.HostManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.HostManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Google.Apis.Requests.BatchRequest;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class HostManagementController : BaseController
    {
        private readonly IHostManagementBusiness _buss;
        public HostManagementController(IHostManagementBusiness buss)
        {
            _buss = buss;
        }
        [HttpGet]
        public ActionResult HostList(string AgentId, string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ViewBag.AgentId = AgentId;
            ViewBag.SearchFilter = SearchFilter;
            string RenderId = "";
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ClubList", "ClubManagement");
            }
            var ResponseModel = new ManageHostCommonModel();
            if (TempData.ContainsKey("ManageHostModel")) ResponseModel.ManageHostModel = TempData["ManageHostModel"] as ManageHostModel;
            else ResponseModel.ManageHostModel = new ManageHostModel();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            var dbResponse = _buss.GetHostList(aId, dbRequest);
            ResponseModel.HostListModel = dbResponse.MapObjects<HostListModel>();
            foreach (var item in ResponseModel.HostListModel)
            {
                item.HostId = item.HostId?.EncryptParameter();
                item.AgentId = item.AgentId?.EncryptParameter();
                item.HostImage = ImageHelper.ProcessedImage(item.HostImage);
            }

            if (string.IsNullOrEmpty(ResponseModel.ManageHostModel.AgentId) && string.IsNullOrEmpty(ResponseModel.ManageHostModel.HostId))
            {
                var dbHostIdentityResponse = _buss.GetHostIdentityDetail();
                ResponseModel.ManageHostModel.HostIdentityDataModel = dbHostIdentityResponse.MapObjects<HostIdentityDataModel>();
                ResponseModel.ManageHostModel.HostIdentityDataModel.ForEach(x => x.IdentityLabel = (!string.IsNullOrEmpty(culture) && culture == "en") ? x.IdentityLabelEnglish : x.IdentityLabelJapanese);
                ResponseModel.ManageHostModel.HostIdentityDataModel.ForEach(x => x.IdentityType = x.IdentityType.EncryptParameter());
                ResponseModel.ManageHostModel.HostIdentityDataModel.ForEach(x => x.IdentityValue = x.IdentityValue.EncryptParameter());
            }
            ViewBag.ZodiacSignsDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("ZODIACSIGNSDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.ZodiacSignsDDLKey = ResponseModel.ManageHostModel.ConstellationGroup;
            ViewBag.BloodGroupDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("BLOODGROUPDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.BloodGroupDDLKey = ResponseModel.ManageHostModel.BloodType;
            ViewBag.OccupationDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("OCCUPATIONDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.OccupationDDLKey = ResponseModel.ManageHostModel.PreviousOccupation;
            ViewBag.LiquorStrengthDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("LIQUORSTRENGTHDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.LiquorStrengthDDLKey = ResponseModel.ManageHostModel.LiquorStrength;
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/ClubManagement/ClubList";
            ViewBag.RankDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("RANKDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.RankDDLKey = ResponseModel.ManageHostModel.Rank;
            ViewBag.SkillDDL = ApplicationUtilities.SetDDLValue(CustomLoadDropdownList("SKILLDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.BirthPlaceDdl = ApplicationUtilities.SetDDLValue(DDLHelper.LoadDropdownList("BIRTHPLACE") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.heightlistddl = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("HEIGHTLIST") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.BirthPlacekey = ResponseModel.ManageHostModel.Address;
            ViewBag.heightlistkey = ResponseModel.ManageHostModel.Height;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            return View(ResponseModel);
        }

        [HttpGet]
        public ActionResult ManageHost(string AgentId, string HostId = null)
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            if (string.IsNullOrEmpty(AgentId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ClubList", "ClubManagement");
            }
            ManageHostModel model = new ManageHostModel();
            if (string.IsNullOrEmpty(HostId)) return View(model);
            else
            {
                var aId = AgentId.DecryptParameter();
                var hId = HostId.DecryptParameter();
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
                if (string.IsNullOrEmpty(hId))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid host details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["RenderId"] = "ManageHost";
                    return RedirectToAction("HostList", "HostManagement", new { AgentId });
                }
                var dbResponse = _buss.GetHostDetail(aId, hId);
                model = dbResponse.MapObject<ManageHostModel>();
                model.AgentId = AgentId;
                model.HostId = HostId;
                if (!string.IsNullOrEmpty(dbResponse.DOB))
                {
                    DateTime dob = DateTime.ParseExact(dbResponse.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    model.BirthYear = dob.Year.ToString();
                    model.BirthMonth = dob.Month.ToString().PadLeft(2, '0');
                    model.BirthDate = dob.Day.ToString().PadLeft(2, '0');
                }
                model.ConstellationGroup = model.ConstellationGroup?.EncryptParameter();
                model.BloodType = model.BloodType?.EncryptParameter();
                model.PreviousOccupation = model.PreviousOccupation?.EncryptParameter();
                model.LiquorStrength = model.LiquorStrength?.EncryptParameter();
                model.HostLogo = dbResponse.ImagePath;
                model.HostIconImage = dbResponse.IconImagePath;
                model.Rank = dbResponse.Rank.EncryptParameter();
                model.Address = dbResponse.Address.EncryptParameter();
                model.Height = dbResponse.Height.EncryptParameter();
                model.HostIdentityDataModel.ForEach(x => x.IdentityLabel = (!string.IsNullOrEmpty(culture) && culture == "en") ? x.IdentityLabelEnglish : x.IdentityLabelJapanese);
                model.HostIdentityDataModel.ForEach(x => x.IdentityType = x.IdentityType.EncryptParameter());
                model.HostIdentityDataModel.ForEach(x => x.IdentityValue = x.IdentityValue.EncryptParameter());
                model.HostIdentityDataModel.ForEach(x => x.IdentityDDLType = !string.IsNullOrEmpty(x.IdentityDDLType) ? x.IdentityDDLType.EncryptParameter() : null);
                TempData["RenderId"] = "ManageHost";
                TempData["ManageHostModel"] = model;
                return RedirectToAction("HostList", "HostManagement", new { AgentId });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageHost(ManageHostModel Model, string ZodiacSignsDDLKey, string BloodGroupDDLKey,
            string OccupationDDLKey, string LiquorStrengthDDLKey, string BirthYearKey, string BirthMonthKey, string BirthDayKey, HttpPostedFileBase HostLogoFile, HttpPostedFileBase HostIconImageFile)
        {
            //Model.BirthYear = BirthYearKey;
            //Model.BirthMonth = BirthMonthKey;
            //Model.BirthDate = BirthDayKey;
            Model.DOB = string.Concat(Model.BirthYear, '-', Model.BirthMonth, '-', Model.BirthDate);
            Model.ConstellationGroup = ZodiacSignsDDLKey;
            Model.BloodType = BloodGroupDDLKey;
            Model.PreviousOccupation = OccupationDDLKey;
            Model.LiquorStrength = LiquorStrengthDDLKey;
            //if (!string.IsNullOrEmpty(RankDDLKey?.DecryptParameter())) ModelState.Remove("Rank");
            if (!string.IsNullOrEmpty(ZodiacSignsDDLKey?.DecryptParameter())) ModelState.Remove("ConstellationGroup");
            if (!string.IsNullOrEmpty(BloodGroupDDLKey?.DecryptParameter())) ModelState.Remove("BloodType");
            if (!string.IsNullOrEmpty(OccupationDDLKey?.DecryptParameter())) ModelState.Remove("PreviousOccupation");
            if (!string.IsNullOrEmpty(LiquorStrengthDDLKey?.DecryptParameter())) ModelState.Remove("LiquorStrength");
            if (!string.IsNullOrEmpty(Model.DOB))
            {
                ModelState.Remove("DOB");
            }
            if (HostLogoFile == null)
            {
                if (string.IsNullOrEmpty(Model.HostLogo))
                {
                    bool allowRedirect = false;
                    var ErrorMessage = string.Empty;
                    if (HostLogoFile == null && string.IsNullOrEmpty(Model.HostLogo))
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
                        TempData["RenderId"] = "ManageHost";
                        TempData["ManageHostModel"] = Model;
                        return RedirectToAction("HostList", "HostManagement", new { AgentId = Model.AgentId });
                    }
                }
            }
            if (HostIconImageFile == null)
            {
                if (string.IsNullOrEmpty(Model.HostIconImage))
                {
                    bool allowRedirect = false;
                    var ErrorMessage = string.Empty;
                    if (HostIconImageFile == null && string.IsNullOrEmpty(Model.HostIconImage))
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
                        TempData["RenderId"] = "ManageHost";
                        TempData["ManageHostModel"] = Model;
                        return RedirectToAction("HostList", "HostManagement", new { AgentId = Model.AgentId });
                    }
                }
            }

            string HostLogoFileName = string.Empty;
            var allowedContentType = AllowedImageContentType();
            if (HostLogoFile != null)
            {
                var contentType = HostLogoFile.ContentType;
                var ext = Path.GetExtension(HostLogoFile.FileName);
                if (allowedContentType.Contains(contentType.ToLower()))
                {
                    HostLogoFileName = $"{AWSBucketFolderNameModel.HOST}/Logo_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                    Model.HostLogo = $"/{HostLogoFileName}";
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid image format.",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["RenderId"] = "ManageHost";
                    TempData["ManageHostModel"] = Model;
                    return RedirectToAction("HostList", "HostManagement", new { AgentId = Model.AgentId });
                }
            }
            string HostIconImageFileName = string.Empty;
            if (HostIconImageFile != null)
            {
                var contentType = HostIconImageFile.ContentType;
                var ext = Path.GetExtension(HostIconImageFile.FileName);
                if (allowedContentType.Contains(contentType.ToLower()))
                {
                    HostIconImageFileName = $"{AWSBucketFolderNameModel.HOST}/IconImage_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                    Model.HostIconImage = $"/{HostIconImageFileName}";
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid image format.",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["RenderId"] = "ManageHost";
                    TempData["ManageHostModel"] = Model;
                    return RedirectToAction("HostList", "HostManagement", new { AgentId = Model.AgentId });
                }
            }
            if (ModelState.IsValid)
            {
                var requestCommon = Model.MapObject<ManageHostCommon>();
                requestCommon.AgentId = !string.IsNullOrEmpty(Model.AgentId) ? Model.AgentId.DecryptParameter() : null;
                if (string.IsNullOrEmpty(requestCommon.AgentId))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid club details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("ClubList", "ClubManagement");
                }
                if (!string.IsNullOrEmpty(requestCommon.HostId)) requestCommon.HostId = !string.IsNullOrEmpty(Model.HostId) ? Model.HostId.DecryptParameter() : null;
                requestCommon.Rank = requestCommon.Rank.DecryptParameter();
                requestCommon.ConstellationGroup = ZodiacSignsDDLKey?.DecryptParameter();
                requestCommon.BloodType = BloodGroupDDLKey?.DecryptParameter();
                requestCommon.PreviousOccupation = OccupationDDLKey?.DecryptParameter();
                requestCommon.LiquorStrength = LiquorStrengthDDLKey?.DecryptParameter();
                requestCommon.Address = Model.Address?.DecryptParameter();
                requestCommon.Height = Model.Height?.DecryptParameter();
                requestCommon.DOB = Model.DOB;
                requestCommon.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                requestCommon.ActionIP = ApplicationUtilities.GetIP();
                requestCommon.ImagePath = Model.HostLogo;
                requestCommon.IconImagePath = Model.HostIconImage;
                requestCommon.HostIdentityDataModel.ForEach(x => x.IdentityType = x.IdentityType.DecryptParameter());
                requestCommon.HostIdentityDataModel.ForEach(x => x.IdentityValue = x.IdentityValue.DecryptParameter());
                requestCommon.HostIdentityDataModel.ForEach(x => x.IdentityDDLType = !string.IsNullOrEmpty(x.IdentityDDLType) ? x.IdentityDDLType.DecryptParameter() : null);
                var dbResponse = _buss.ManageHost(requestCommon);
                if (dbResponse != null && dbResponse.Code == 0)
                {
                    if (HostLogoFile != null) await ImageHelper.ImageUpload(HostLogoFileName, HostLogoFile);
                    if (HostIconImageFile != null) await ImageHelper.ImageUpload(HostIconImageFileName, HostIconImageFile);
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = dbResponse.Message ?? "Success",
                        Title = NotificationMessage.SUCCESS.ToString()
                    });
                    return RedirectToAction("HostList", "HostManagement", new { AgentId = Model.AgentId });
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });
                    TempData["RenderId"] = "ManageHost";
                    TempData["ManageHostModel"] = Model;
                    return RedirectToAction("HostList", "HostManagement", new { AgentId = Model.AgentId });
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
            TempData["RenderId"] = "ManageHost";
            TempData["ManageHostModel"] = Model;
            return RedirectToAction("HostList", "HostManagement", new { AgentId = Model.AgentId });
        }

        [HttpGet]
        public ActionResult HostDetail(string AgentId, string HostId)
        {
            var aId = AgentId.DecryptParameter();
            var hId = !string.IsNullOrEmpty(HostId) ? HostId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId))
            {
                this.ShowPopup(1, "Invalid details");
                return RedirectToAction("ClubList", "ClubManagement");
            }
            if (string.IsNullOrEmpty(hId))
            {
                this.ShowPopup(1, "Invalid details");
                return RedirectToAction("HostList", "HostManagement", new { AgentId });
            }
            var dbResponse = _buss.GetHostDetail(aId, hId);
            var response = dbResponse.MapObject<ManageHostModel>();
            response.AgentId = null;
            response.HostId = null;
            return View(response);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult BlockHost(string AgentId, string HostId)
        {
            var response = new CommonDbResponse();
            var aId = AgentId.DecryptParameter();
            var hId = !string.IsNullOrEmpty(HostId) ? HostId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId) || string.IsNullOrEmpty(hId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return Json(response.Message, JsonRequestBehavior.AllowGet);
            }
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            string status = "B";
            var dbResponse = _buss.ManageHostStatus(aId, hId, status, commonRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Failed",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return Json(response.Message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UnBlockHost(string AgentId, string HostId)
        {
            var response = new CommonDbResponse();
            var aId = AgentId.DecryptParameter();
            var hId = !string.IsNullOrEmpty(HostId) ? HostId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId) || string.IsNullOrEmpty(hId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return Json(response.Message, JsonRequestBehavior.AllowGet);
            }
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            string status = "A";
            var dbResponse = _buss.ManageHostStatus(aId, hId, status, commonRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Failed",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return Json(response.Message, JsonRequestBehavior.AllowGet);
        }

        #region Gallery Management
        [HttpGet]
        public ActionResult GalleryManagement(string AgentId, string HostId, string SearchFilter = "")
        {
            ViewBag.AgentId = AgentId;
            ViewBag.HostId = HostId;
            ViewBag.SearchFilter = SearchFilter;

            var aId = AgentId?.DecryptParameter();
            var hId = HostId?.DecryptParameter();
            if (string.IsNullOrEmpty(aId) || string.IsNullOrEmpty(hId))
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
            var ReturnModel = new CommonHostGalleryManagementModel();
            if (TempData.ContainsKey("GalleryManagementModel")) ReturnModel.HostManageGalleryImageModel = TempData["GalleryManagementModel"] as HostManageGalleryImageModel;
            else ReturnModel.HostManageGalleryImageModel = new HostManageGalleryImageModel();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            var dbResponse = _buss.GetGalleryImage(aId, hId, "", SearchFilter);
            ReturnModel.HostGalleryManagementModel = dbResponse.MapObjects<HostGalleryManagementModel>();
            foreach (var item in ReturnModel.HostGalleryManagementModel)
            {
                item.AgentId = AgentId;
                item.HostId = HostId;
                item.GalleryId = item.GalleryId?.EncryptParameter();
                item.ImagePath = ImageHelper.ProcessedImage(item.ImagePath);
            }
            ReturnModel.HostManageGalleryImageModel.AgentId = AgentId;
            ReturnModel.HostManageGalleryImageModel.HostId = HostId;
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/HostManagement/HostList?AgentId=" + AgentId;
            return View(ReturnModel);
        }

        [HttpGet]
        public ActionResult ManageGallery(string AgentId, string HostId, string GalleryId)
        {
            HostManageGalleryImageModel model = new HostManageGalleryImageModel();
            var aId = AgentId?.DecryptParameter();
            var hId = HostId?.DecryptParameter();
            var gId = GalleryId?.DecryptParameter();
            if (string.IsNullOrEmpty(aId) || string.IsNullOrEmpty(hId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
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
                return RedirectToAction("GalleryManagement", "HostManagement");
            }
            var dbResponse = _buss.GetGalleryImage(aId, hId, gId);
            if (dbResponse.Count > 0) model = dbResponse[0].MapObject<HostManageGalleryImageModel>();
            model.AgentId = model.AgentId.EncryptParameter();
            model.HostId = model.HostId.EncryptParameter();
            model.GalleryId = model.GalleryId.EncryptParameter();
            TempData["GalleryManagementModel"] = model;
            TempData["RenderId"] = "ManageHostGallery";
            return RedirectToAction("GalleryManagement", "HostManagement", new { AgentId, HostId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageGallery(HostManageGalleryImageModel Model, HttpPostedFileBase Image_Path)
        {
            var aId = Model.AgentId?.DecryptParameter();
            var hId = Model.HostId?.DecryptParameter();
            if (string.IsNullOrEmpty(aId) || string.IsNullOrEmpty(hId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
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
                    TempData["RenderId"] = "ManageHostGallery";
                    return RedirectToAction("GalleryManagement", "HostManagement", new { AgentId = Model.AgentId, HostId = Model.HostId });
                }
            }
            if (string.IsNullOrEmpty(Model.ImagePath))
            {
                if (Image_Path == null)
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
                        TempData["RenderId"] = "ManageHostGallery";
                        return RedirectToAction("GalleryManagement", "HostManagement", new { AgentId = Model.AgentId, HostId = Model.HostId });
                    }
                }
            }
            string fileName = string.Empty;
            var allowedContentType = AllowedImageContentType();
            if (Image_Path != null)
            {
                var contentType = Image_Path.ContentType;
                var ext = Path.GetExtension(Image_Path.FileName);
                if (allowedContentType.Contains(contentType.ToLower()))
                {
                    fileName = $"{AWSBucketFolderNameModel.HOST}/GalleryImage_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                    Model.ImagePath = $"/{fileName}";
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
                    TempData["RenderId"] = "ManageHostGallery";
                    return RedirectToAction("GalleryManagement", "HostManagement", new { AgentId = Model.AgentId, HostId = Model.HostId });
                }
            }
            var dbRequest = Model.MapObject<HostManageGalleryImageCommon>();
            dbRequest.AgentId = Model.AgentId?.DecryptParameter();
            dbRequest.HostId = Model.HostId?.DecryptParameter();
            dbRequest.GalleryId = Model.GalleryId?.DecryptParameter();
            dbRequest.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
            dbRequest.ActionIP = ApplicationUtilities.GetIP();
            var dbResponse = _buss.ManageGalleryImage(dbRequest);
            if (dbResponse != null && dbResponse.Code == 0)
            {
                if (Image_Path != null) await ImageHelper.ImageUpload(fileName, Image_Path);
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                    Message = dbResponse.Message ?? "Failed",
                    Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                });
                return RedirectToAction("GalleryManagement", "HostManagement", new { AgentId = Model.AgentId, HostId = Model.HostId });
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
                TempData["RenderId"] = "ManageHostGallery";
                return RedirectToAction("GalleryManagement", "HostManagement", new { AgentId = Model.AgentId, HostId = Model.HostId });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ManageGalleryStatus(string AgentId, string HostId, string GalleryId)
        {
            var response = new CommonDbResponse();
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            var hId = !string.IsNullOrEmpty(HostId) ? HostId.DecryptParameter() : null;
            var gId = !string.IsNullOrEmpty(GalleryId) ? GalleryId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId) || string.IsNullOrEmpty(hId) || string.IsNullOrEmpty(gId))
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
            var dbResponse = _buss.ManageGalleryImageStatus(aId, hId, gId, commonRequest);
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
        #region
        public object CustomLoadDropdownList(string ForMethod)
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            var dbResponse = new List<StaticDataCommon>();
            var response = new Dictionary<string, string>();
            switch (ForMethod.ToUpper())
            {
                case "SKILLDDL":
                    dbResponse = _buss.GetSkillsDLL();
                    dbResponse.ForEach(x => x.StaticLabel = (!string.IsNullOrEmpty(culture) && culture == "en") ? x.StaticLabelEnglish : x.StaticLabelJapanese);
                    dbResponse.ForEach(item => { response.Add(item.StaticValue.EncryptParameter(), item.StaticLabel); });
                    return response;
                default: return response;
            }
        }
        #endregion
    }
}