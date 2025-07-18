﻿using CRS.ADMIN.APPLICATION.CustomHelpers;
using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models;
using CRS.ADMIN.APPLICATION.Models.HostManagement;
using CRS.ADMIN.BUSINESS.HostManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.HostManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using CsvHelper;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static CRS.ADMIN.APPLICATION.Controllers.HostManagementController;
using static Google.Apis.Requests.BatchRequest;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class HostManagementController : BaseController
    {
        private readonly IHostManagementBusiness _buss;
        private readonly CsvService _csvService = new CsvService();
        public HostManagementController(IHostManagementBusiness buss)
        {
            _buss = buss;
        }
        [HttpGet]
        public ActionResult HostList(string AgentId, string SearchFilter = "", int StartIndex = 0, int PageSize = 10, string clubCategory = "")
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ViewBag.AgentId = AgentId;
            ViewBag.SearchFilter = SearchFilter;
            string RenderId = "";
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            ActionResult redirectresult = null;
            if (!string.IsNullOrEmpty(clubCategory) && clubCategory.ToUpper() == "BASIC")
            {
                redirectresult = RedirectToAction("BasicClubManagementList", "BasicClubManagement");

            }
            else
            {
                redirectresult = RedirectToAction("ClubList", "ClubManagement");
            }

            if (string.IsNullOrEmpty(aId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return redirectresult;
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
            else
            {
                var respDb = _buss.GetHostIdentityDetail(ResponseModel.ManageHostModel.AgentId.DecryptParameter(), ResponseModel.ManageHostModel.HostId?.DecryptParameter());
                ResponseModel.ManageHostModel.HostIdentityDataModel = respDb.MapObjects<HostIdentityDataModel>();// model = dbResponse.MapObject<ManageHostModel>();
                ResponseModel.ManageHostModel.HostIdentityDataModel.ForEach(x => x.IdentityLabel = (!string.IsNullOrEmpty(culture) && culture == "en") ? x.IdentityLabelEnglish : x.IdentityLabelJapanese);
                ResponseModel.ManageHostModel.HostIdentityDataModel.ForEach(x => x.IdentityType = x.IdentityType.EncryptParameter());
                ResponseModel.ManageHostModel.HostIdentityDataModel.ForEach(x => x.IdentityValue = x.IdentityValue.EncryptParameter());
            }
            ViewBag.ZodiacSignsDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("ZODIACSIGNSDDL", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.ZodiacSignsDDLKey = ResponseModel.ManageHostModel.ConstellationGroup;
            ViewBag.BloodGroupDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("BLOODGROUPDDL", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.BloodGroupDDLKey = ResponseModel.ManageHostModel.BloodType;
            //ViewBag.OccupationDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("OCCUPATIONDDL", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            //ViewBag.OccupationDDLKey = ResponseModel.ManageHostModel.PreviousOccupation;
            ViewBag.LiquorStrengthDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("LIQUORSTRENGTHDDL", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.LiquorStrengthDDLKey = ResponseModel.ManageHostModel.LiquorStrength;
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.IsBackAllowed = true;
            if (!string.IsNullOrEmpty(clubCategory) && clubCategory.ToUpper() == "BASIC")
            {
                ViewBag.BackButtonURL = "/BasicClubManagement/BasicClubManagementList";
            }
            else
            {
                ViewBag.BackButtonURL = "/ClubManagement/ClubList";
            }

            ViewBag.RankDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("RANKDDL", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.RankDDLKey = ResponseModel.ManageHostModel.Rank;
            ViewBag.SkillDDL = ApplicationUtilities.SetDDLValue(CustomLoadDropdownList("SKILLDDL") as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.BirthPlaceDdl = ApplicationUtilities.SetDDLValue(DDLHelper.LoadDropdownList("BIRTHPLACE", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            //ViewBag.heightlistddl = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("HEIGHTLIST", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            //ViewBag.positionddl = ApplicationUtilities.SetDDLValue(DDLHelper.LoadDropdownList("POSITIONLIST", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.BirthPlacekey = ResponseModel.ManageHostModel.Address;
            //ViewBag.heightlistkey = ResponseModel.ManageHostModel.Height;
            //ViewBag.postitionkey = ResponseModel.ManageHostModel.Position;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ResponseModel.ManageHostModel.clubCategory = clubCategory;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            return View(ResponseModel);
        }

        [HttpGet]
        public ActionResult ManageHost(string AgentId, string HostId = null, string clubCategory = null, string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ActionResult redirectresult = null;
            if (!string.IsNullOrEmpty(clubCategory) && clubCategory.ToUpper() == "BASIC")
            {
                redirectresult = RedirectToAction("BasicClubManagementList", "BasicClubManagement");

            }
            else
            {
                redirectresult = RedirectToAction("ClubList", "ClubManagement");
            }
            if (string.IsNullOrEmpty(AgentId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return redirectresult;
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
                    return redirectresult;
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
                    return RedirectToAction("HostList", "HostManagement", new
                    {
                        AgentId,
                        clubCategory = clubCategory,
                        SearchFilter = SearchFilter,
                        StartIndex = StartIndex,
                        PageSize = PageSize
                    });
                }
                var dbResponse = _buss.GetHostDetail(aId, hId);
                model = dbResponse.MapObject<ManageHostModel>();
                //if (!string.IsNullOrEmpty(model.DOB))
                //{
                //    DateTime parsedDate;
                //    if (DateTime.TryParse(model.DOB, out parsedDate))
                //    {
                //        model.DOB = parsedDate.ToString("yyyy-MM-dd");
                //    }
                //}
                //    model.DOB = DateTime.Parse(model.DOB).ToString("yyyy-MM-dd");

                model.AgentId = AgentId;
                model.HostId = HostId;
                //if (!string.IsNullOrEmpty(model.DOB))
                //{
                //    DateTime dob;
                //    if (DateTime.TryParseExact(model.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dob))
                //    {
                //        model.BirthYear = dob.Year.ToString();
                //        model.BirthMonth = dob.Month.ToString("00");
                //        model.BirthDate = dob.Day.ToString("00");
                //    }
                //}

                if (!string.IsNullOrEmpty(model.DOB))
                {
                    if (model.DOB != "--")
                    {
                        var Dateparts1 = model.DOB.Split('-');
                        if (model.DOB.Count(c => c == '-') == 2)
                        {
                            // Case: 1995--10 => year and day
                            var parts = model.DOB.Split('-');
                            if (parts.Length == 3 && !string.IsNullOrEmpty(parts[0]) && string.IsNullOrEmpty(parts[1]) && !string.IsNullOrEmpty(parts[2]))
                            {
                                model.BirthYear = parts[0]; // Extract year
                                model.BirthDate = parts[2]; // Extract day

                            }
                            else
                            {
                                // Case: 1995-09-11 => yyyy-mm-dd
                                DateTime dob;
                                if (DateTime.TryParseExact(model.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dob))
                                {
                                    model.BirthYear = dob.Year.ToString();
                                    model.BirthMonth = dob.Month.ToString("00");
                                    model.BirthDate = dob.Day.ToString("00");

                                }
                            }
                        }
                        if (model.DOB.StartsWith("--"))
                        {
                            // Case: --11 => day
                            model.BirthDate = model.DOB.Substring(2, 2); // Extract day

                        }
                        else if (model.DOB.EndsWith("--") && model.DOB.Count(c => c == '-') == 2)
                        {
                            // Case: 1995-- => year
                            model.BirthYear = model.DOB.Substring(0, 4); // Extract year

                        }
                        else if (model.DOB.StartsWith("-") && model.DOB.Count(c => c == '-') == 2 && string.IsNullOrEmpty(Dateparts1[2]))
                        {
                            // Case: -12- => month
                            model.BirthMonth = model.DOB.Substring(1, 2); // Extract month

                        }
                        else if (model.DOB.StartsWith("-") && model.DOB.Count(c => c == '-') == 2)
                        {
                            // Case: -12-10 => mm-dd
                            model.BirthMonth = model.DOB.Substring(1, 2); // Extract month
                            model.BirthDate = model.DOB.Substring(4, 2);  // Extract day

                        }



                        else if (model.DOB.EndsWith("-") && model.DOB.Count(c => c == '-') == 2)
                        {
                            // Case: 1995-09- => year and month
                            var parts = model.DOB.Split('-');
                            model.BirthYear = parts[0]; // Extract year
                            model.BirthMonth = parts[1]; // Extract month                       
                        }

                    }
                }

                model.ConstellationGroup = model.ConstellationGroup?.EncryptParameter();
                model.BloodType = model.BloodType?.EncryptParameter();
                //model.PreviousOccupation = model.PreviousOccupation?.EncryptParameter();
                model.LiquorStrength = model.LiquorStrength?.EncryptParameter();
                model.HostLogo = dbResponse.ImagePath;
                model.HostIconImage = dbResponse.IconImagePath;
                model.Rank = dbResponse.Rank.EncryptParameter();
                model.Address = dbResponse.Address.EncryptParameter();
                //model.Height = dbResponse.Height.EncryptParameter();
                //model.Position = dbResponse.Position.EncryptParameter();
                model.HostIdentityDataModel.ForEach(x => x.IdentityLabel = (!string.IsNullOrEmpty(culture) && culture == "en") ? x.IdentityLabelEnglish : x.IdentityLabelJapanese);
                model.HostIdentityDataModel.ForEach(x => x.IdentityType = x.IdentityType.EncryptParameter());
                model.HostIdentityDataModel.ForEach(x => x.IdentityValue = x.IdentityValue.EncryptParameter());
                model.HostIdentityDataModel.ForEach(x => x.IdentityDDLType = !string.IsNullOrEmpty(x.IdentityDDLType) ? x.IdentityDDLType.EncryptParameter() : null);
                TempData["RenderId"] = "ManageHost";
                TempData["ManageHostModel"] = model;
                return RedirectToAction("HostList", "HostManagement", new
                {
                    AgentId,
                    clubCategory = clubCategory,
                    SearchFilter = SearchFilter,
                    StartIndex = StartIndex,
                    PageSize = PageSize
                });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageHost(ManageHostModel Model, string ZodiacSignsDDLKey, string BloodGroupDDLKey,
            string LiquorStrengthDDLKey, string BirthYearKey, string BirthMonthKey, string BirthDayKey, HttpPostedFileBase HostLogoFile, HttpPostedFileBase HostIconImageFile)
        {
            //Model.BirthYear = BirthYearKey;
            //Model.BirthMonth = BirthMonthKey;
            //Model.BirthDate = BirthDayKey;
            Model.DOB = string.Concat(Model.BirthYear, '-', Model.BirthMonth, '-', Model.BirthDate);
            Model.ConstellationGroup = ZodiacSignsDDLKey;
            Model.BloodType = BloodGroupDDLKey;
            //Model.PreviousOccupation = OccupationDDLKey;
            Model.LiquorStrength = LiquorStrengthDDLKey;
            //if (!string.IsNullOrEmpty(RankDDLKey?.DecryptParameter())) ModelState.Remove("Rank");
            if (!string.IsNullOrEmpty(ZodiacSignsDDLKey?.DecryptParameter())) ModelState.Remove("ConstellationGroup");
            if (!string.IsNullOrEmpty(BloodGroupDDLKey?.DecryptParameter())) ModelState.Remove("BloodType");
            //if (!string.IsNullOrEmpty(OccupationDDLKey?.DecryptParameter())) ModelState.Remove("PreviousOccupation");
            if (!string.IsNullOrEmpty(LiquorStrengthDDLKey?.DecryptParameter())) ModelState.Remove("LiquorStrength");
            if (!string.IsNullOrEmpty(Model.DOB))
            {
                ModelState.Remove("DOB");
            }

            //if (HostLogoFile == null)
            //{
            //    if (string.IsNullOrEmpty(Model.HostLogo))
            //    {
            //        bool allowRedirect = false;
            //        var ErrorMessage = string.Empty;
            //        if (HostLogoFile == null && string.IsNullOrEmpty(Model.HostLogo))
            //        {
            //            ErrorMessage = "Image required";
            //            allowRedirect = true;
            //        }
            //        if (allowRedirect)
            //        {
            //            this.AddNotificationMessage(new NotificationModel()
            //            {
            //                NotificationType = NotificationMessage.INFORMATION,
            //                Message = ErrorMessage ?? "Something went wrong. Please try again later.",
            //                Title = NotificationMessage.INFORMATION.ToString(),
            //            });
            //            TempData["RenderId"] = "ManageHost";
            //            TempData["ManageHostModel"] = Model;
            //            return RedirectToAction("HostList", "HostManagement", new { AgentId = Model.AgentId });
            //        }
            //    }
            //}

            //if (HostIconImageFile == null)
            //{
            //    if (string.IsNullOrEmpty(Model.HostIconImage))
            //    {
            //        bool allowRedirect = false;
            //        var ErrorMessage = string.Empty;
            //        //if (HostIconImageFile == null && string.IsNullOrEmpty(Model.HostIconImage))
            //        //{
            //        //    ErrorMessage = "Image required";
            //        //    allowRedirect = true;
            //        //}
            //        if (allowRedirect)
            //        {
            //            this.AddNotificationMessage(new NotificationModel()
            //            {
            //                NotificationType = NotificationMessage.INFORMATION,
            //                Message = ErrorMessage ?? "Something went wrong. Please try again later.",
            //                Title = NotificationMessage.INFORMATION.ToString(),
            //            });
            //            TempData["RenderId"] = "ManageHost";
            //            TempData["ManageHostModel"] = Model;
            //            return RedirectToAction("HostList", "HostManagement", new { AgentId = Model.AgentId });
            //        }
            //    }
            //}

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
                    return RedirectToAction("HostList", "HostManagement", new { AgentId = Model.AgentId, clubCategory = Model.clubCategory });
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
                    return RedirectToAction("HostList", "HostManagement", new { AgentId = Model.AgentId, clubCategory = Model.clubCategory });
                }
            }
            if (ModelState.IsValid)
            {
                var requestCommon = Model.MapObject<ManageHostCommon>();
                requestCommon.AgentId = !string.IsNullOrEmpty(Model.AgentId) ? Model.AgentId.DecryptParameter() : null;
                ActionResult redirectresult = null;
                if (!string.IsNullOrEmpty(Model.clubCategory) && Model.clubCategory.ToUpper() == "BASIC")
                {
                    redirectresult = RedirectToAction("BasicClubManagementList", "BasicClubManagement");

                }
                else
                {
                    redirectresult = RedirectToAction("ClubList", "ClubManagement");
                }
                if (string.IsNullOrEmpty(requestCommon.AgentId))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid club details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return redirectresult;
                }
                if (!string.IsNullOrEmpty(requestCommon.HostId)) requestCommon.HostId = !string.IsNullOrEmpty(Model.HostId) ? Model.HostId.DecryptParameter() : null;
                if (!string.IsNullOrEmpty(requestCommon.Rank))
                    requestCommon.Rank = requestCommon.Rank.DecryptParameter();
                if (!string.IsNullOrEmpty(requestCommon.ConstellationGroup))
                    requestCommon.ConstellationGroup = ZodiacSignsDDLKey?.DecryptParameter();
                if (!string.IsNullOrEmpty(requestCommon.BloodType))
                    requestCommon.BloodType = BloodGroupDDLKey?.DecryptParameter();
                //if (!string.IsNullOrEmpty(requestCommon.PreviousOccupation))
                //    requestCommon.PreviousOccupation = OccupationDDLKey?.DecryptParameter();
                if (!string.IsNullOrEmpty(requestCommon.LiquorStrength))
                    requestCommon.LiquorStrength = LiquorStrengthDDLKey?.DecryptParameter();
                if (!string.IsNullOrEmpty(requestCommon.Address))
                    requestCommon.Address = Model.Address?.DecryptParameter();
                //if (!string.IsNullOrEmpty(requestCommon.Height))
                //    requestCommon.Height = Model.Height?.DecryptParameter();
                //if (!string.IsNullOrEmpty(requestCommon.Position))
                //    requestCommon.Position = Model.Position?.DecryptParameter();
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
                    string apiUrl = ConfigurationManager.AppSettings["RevalidateApiUrl"];
                    string apiResponse = ExternalApiCallHelpers.CallApi(apiUrl, HttpMethod.Get);
                    return RedirectToAction("HostList", "HostManagement", new { AgentId = Model.AgentId, clubCategory = Model.clubCategory });
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
                    return RedirectToAction("HostList", "HostManagement", new { AgentId = Model.AgentId, clubCategory = Model.clubCategory });
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
            return RedirectToAction("HostList", "HostManagement", new { AgentId = Model.AgentId, clubCategory = Model.clubCategory });
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
        public JsonResult BlockHost(string AgentId, string HostId, string clubCategory, string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
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
            string apiUrl = ConfigurationManager.AppSettings["RevalidateApiUrl"];
            string apiResponse = ExternalApiCallHelpers.CallApi(apiUrl, HttpMethod.Get);
            return Json(response.Message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UnBlockHost(string AgentId, string HostId, string clubCategory, string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
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
            string apiUrl = ConfigurationManager.AppSettings["RevalidateApiUrl"];
            string apiResponse = ExternalApiCallHelpers.CallApi(apiUrl, HttpMethod.Get);
            return Json(response.Message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken, OverrideActionFilters]
        public JsonResult DeleteHostAsync(string AgentId, string HostId, string clubCategory, string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
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
            string status = "D";
            var dbResponse = _buss.ManageHostStatus(aId, hId, status, commonRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Failed",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            string apiUrl = ConfigurationManager.AppSettings["RevalidateApiUrl"];
            string apiResponse = ExternalApiCallHelpers.CallApi(apiUrl, HttpMethod.Get);
            return Json(response.Message, JsonRequestBehavior.AllowGet);
        }



        #region Gallery Management
        [HttpGet]
        public ActionResult GalleryManagement(string AgentId, string HostId, string SearchFilter = "",string clubCategory="", int StartIndexBck = 0, int PageSizeBck = 10,string SearchFilterBck = "")
        {
            ViewBag.AgentId = AgentId;
            ViewBag.HostId = HostId;
            ViewBag.SearchFilter = SearchFilter;
            var aId = AgentId?.DecryptParameter();
            var hId = HostId?.DecryptParameter();
            ActionResult redirectresult = null;
            if (!string.IsNullOrEmpty(clubCategory) && clubCategory.ToUpper() == "BASIC")
            {
                redirectresult = RedirectToAction("BasicClubManagementList", "BasicClubManagement");

            }
            else
            {
                redirectresult = RedirectToAction("ClubList", "ClubManagement");
            }
            if (string.IsNullOrEmpty(aId) || string.IsNullOrEmpty(hId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid club details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return redirectresult;
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
                item.CreatedDate = !string.IsNullOrEmpty(item.CreatedDate) ? DateTime.Parse(item.CreatedDate).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : item.CreatedDate;
                item.UpdatedDate = !string.IsNullOrEmpty(item.UpdatedDate) ? DateTime.Parse(item.UpdatedDate).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : item.UpdatedDate;

            }
            ReturnModel.HostManageGalleryImageModel.AgentId = AgentId;
            ReturnModel.HostManageGalleryImageModel.HostId = HostId;
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.IsBackAllowed = true;
            if (!string.IsNullOrEmpty(clubCategory) && clubCategory.ToUpper() == "BASIC")
            {

                ViewBag.BackButtonURL = "/HostManagement/HostList?AgentId=" + AgentId+ "&clubCategory=" + clubCategory + "&StartIndex=" + StartIndexBck + "&PageSize=" + PageSizeBck+ "&SearchFilter=" + SearchFilterBck;

            }
            else
            {
                ViewBag.BackButtonURL = "/HostManagement/HostList?AgentId=" + AgentId + "&StartIndex=" + StartIndexBck + "&PageSize=" + PageSizeBck + "&SearchFilter=" + SearchFilterBck; ;
            }
            ReturnModel.HostManageGalleryImageModel.clubCategory = clubCategory;
            ViewBag.SearchFilterBck = SearchFilterBck;
            ViewBag.StartIndexBck = StartIndexBck;
            ViewBag.PageSizeBck = PageSizeBck;
            return View(ReturnModel);
        }

        [HttpGet]
        public ActionResult ManageGallery(string AgentId, string HostId, string GalleryId,string clubCategory, int StartIndexBck = 0, int PageSizeBck = 10, string SearchFilterBck = "")
        {
            HostManageGalleryImageModel model = new HostManageGalleryImageModel();
            var aId = AgentId?.DecryptParameter();
            var hId = HostId?.DecryptParameter();
            var gId = GalleryId?.DecryptParameter();
            ActionResult redirectresult = null;
            if (!string.IsNullOrEmpty(clubCategory) && clubCategory.ToUpper() == "BASIC")
            {
                redirectresult = RedirectToAction("BasicClubManagementList", "BasicClubManagement");

            }
            else
            {
                redirectresult = RedirectToAction("ClubList", "ClubManagement");
            }
            if (string.IsNullOrEmpty(aId) || string.IsNullOrEmpty(hId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return redirectresult;
            }
            if (string.IsNullOrEmpty(gId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid gallery details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("GalleryManagement", "HostManagement", new { AgentId, HostId, clubCategory, StartIndexBck, PageSizeBck, SearchFilterBck });
            }
            var dbResponse = _buss.GetGalleryImage(aId, hId, gId);
            if (dbResponse.Count > 0) model = dbResponse[0].MapObject<HostManageGalleryImageModel>();
            model.AgentId = model.AgentId.EncryptParameter();
            model.HostId = model.HostId.EncryptParameter();
            model.GalleryId = model.GalleryId.EncryptParameter();
            TempData["GalleryManagementModel"] = model;
            TempData["RenderId"] = "ManageHostGallery";
            return RedirectToAction("GalleryManagement", "HostManagement", new { AgentId, HostId, clubCategory, StartIndexBck, PageSizeBck, SearchFilterBck });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageGallery(HostManageGalleryImageModel Model, HttpPostedFileBase Image_Path, int StartIndexBck = 0, int PageSizeBck = 10, string SearchFilterBck = "")
        {
            var aId = Model.AgentId?.DecryptParameter();
            var hId = Model.HostId?.DecryptParameter();
            ActionResult redirectresult = null;
            if (!string.IsNullOrEmpty(Model.clubCategory) && Model.clubCategory.ToUpper() == "BASIC")
            {
                redirectresult = RedirectToAction("BasicClubManagementList", "BasicClubManagement");

            }
            else
            {
                redirectresult = RedirectToAction("ClubList", "ClubManagement");
            }
            if (string.IsNullOrEmpty(aId) || string.IsNullOrEmpty(hId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return redirectresult;
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

                    return RedirectToAction("GalleryManagement", "HostManagement", new { AgentId = Model.AgentId, HostId = Model.HostId, clubCategory =Model.clubCategory, StartIndexBck = StartIndexBck, PageSizeBck = PageSizeBck, SearchFilterBck = SearchFilterBck });
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
                        return RedirectToAction("GalleryManagement", "HostManagement", new { AgentId = Model.AgentId, HostId = Model.HostId, clubCategory = Model.clubCategory ,StartIndexBck = StartIndexBck, PageSizeBck = PageSizeBck, SearchFilterBck = SearchFilterBck });
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
                    return RedirectToAction("GalleryManagement", "HostManagement", new { AgentId = Model.AgentId, HostId = Model.HostId, clubCategory = Model.clubCategory , StartIndexBck = StartIndexBck, PageSizeBck = PageSizeBck, SearchFilterBck = SearchFilterBck });
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
                return RedirectToAction("GalleryManagement", "HostManagement", new { AgentId = Model.AgentId, HostId = Model.HostId, clubCategory = Model.clubCategory,StartIndexBck = StartIndexBck, PageSizeBck = PageSizeBck, SearchFilterBck = SearchFilterBck });
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
                return RedirectToAction("GalleryManagement", "HostManagement", new { AgentId = Model.AgentId, HostId = Model.HostId, clubCategory = Model.clubCategory , StartIndexBck = StartIndexBck, PageSizeBck = PageSizeBck, SearchFilterBck = SearchFilterBck });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ManageGalleryStatus(string AgentId, string HostId, string GalleryId,string clubCategory, int StartIndexBck = 0, int PageSizeBck = 10, string SearchFilterBck = "")
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

        [HttpGet, AllowAnonymous, OverrideActionFilters]
        public ActionResult UploadBulkImage()
        {
            return View();
        }

        [HttpPost, AllowAnonymous, OverrideActionFilters, ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadBulkImage(HttpPostedFileBase CSVFileUpload)
        {
            if (CSVFileUpload == null || CSVFileUpload.ContentLength == 0)
            {
                //ViewBag.Message = "Please select a valid CSV file.";


                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.ERROR,
                    Message = "Please select a valid CSV file",
                    Title = NotificationMessage.ERROR.ToString()
                });
                return View();
            }

            var tempFilePath = Path.GetTempFileName();

            var filePath = Path.Combine(Server.MapPath("~/App_Data/"), CSVFileUpload.FileName);
            CSVFileUpload.SaveAs(filePath);

            var people = _csvService.ReadCsv(filePath);
            var allowedContentType = AllowedImageContentType();
            foreach (var item in people)
            {
                HttpPostedFileBase imageFile = GetLocalImageAsPostedFile(item.host_image_path);
                if (imageFile != null || imageFile.ContentLength != 0)
                {

                    var contentType = imageFile.ContentType;
                    var ext = Path.GetExtension(imageFile.FileName);
                    var HostIconImageFileName = "";
                    var HostIconImage = "";
                    if (allowedContentType.Contains(contentType.ToLower()))
                    {
                        HostIconImageFileName = $"{AWSBucketFolderNameModel.HOST}/IconImage_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                        HostIconImage = $"/{HostIconImageFileName}";

                    }
                    await ImageHelper.ImageUpload(HostIconImageFileName, imageFile);

                    _buss.UploadHostImage(item.Club_name, item.LOCATION_ID, item.Host_name, HostIconImage);

                }

            }
            //ProcessCsvAndUploadImagesToS3(tempFilePath);
            //CSVFileUpload.SaveAs(tempFilePath);

            System.IO.File.Delete(tempFilePath);
            System.IO.File.Delete(filePath);

            return View();
        }

        public class HostRecord
        {
            public string Club_name { get; set; }
            public string LOCATION_ID { get; set; }
            public string Host_name { get; set; }
            public string host_image_path { get; set; }
            //public string ClubName { get; set; }
            //public string HostName { get; set; }
            //public string ImagePath { get; set; }

        }

        //private async Task<List<HostRecord>> ProcessCsvAndUploadImagesToS3(string csvFilePath)
        //{
        //    var hosts = new List<HostRecord>();

        //    using (var reader = new StreamReader(csvFilePath))
        //    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        //    {
        //        var records = csv.GetRecords<HostRecord>().ToList();

        //        foreach (var record in records)
        //        {
        //            var localImagePath = record.host_image_path; // Path from the CSV file

        //            // Create a unique S3 key (organized by club and host name)
        //            //var s3Key = $"{record.ClubName}/{record.HostName}/host_photo.jpg";

        //            // Upload the image to S3 and get the S3 URL
        //            //var s3Url = await UploadImageToS3(localImagePath, s3Key);

        //            // Set the S3 URL in the record
        //            //record.HostImageUrl = s3Url;



        //            hosts.Add(record);
        //        }
        //    }

        //    return hosts;
        //}

        public HttpPostedFileBase GetLocalImageAsPostedFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            // Get the file stream
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            // Infer the content type (e.g., "image/jpeg" or "image/png") based on the file extension
            string contentType = MimeMapping.GetMimeMapping(filePath);
            string fileName = Path.GetFileName(filePath);

            // Create and return the custom HttpPostedFileBase object
            return new CustomPostedFile(fileStream, fileName, contentType);
        }

    }

    public class CsvService
    {
        public List<HostRecord> ReadCsv(string filePath)
        {
            List<HostRecord> records = new List<HostRecord>();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<HostRecord>().ToList();
            }

            return records;
        }
    }

    public class CustomPostedFile : HttpPostedFileBase
    {
        private Stream _fileStream;
        private string _fileName;
        private string _contentType;

        public CustomPostedFile(Stream fileStream, string fileName, string contentType)
        {
            _fileStream = fileStream;
            _fileName = fileName;
            _contentType = contentType;
        }

        public override int ContentLength => (int)_fileStream.Length;
        public override string FileName => _fileName;
        public override string ContentType => _contentType;
        public override Stream InputStream => _fileStream;
    }
}