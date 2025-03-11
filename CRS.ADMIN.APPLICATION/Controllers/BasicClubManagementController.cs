using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Middleware;
using CRS.ADMIN.APPLICATION.Models;
using CRS.ADMIN.APPLICATION.Models.BasicClubManagement;
using CRS.ADMIN.BUSINESS.BasicClubManagement;
using CRS.ADMIN.BUSINESS.ClubManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.BasicClubManagement;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class BasicClubManagementController : BaseController
    {
        private readonly IBasicClubManagementBusiness _buss;
        private readonly AmazonCognitoMiddleware _amazonCognitoMiddleware;
        private readonly IClubManagementBusiness _clubManagementBusiness;
        public BasicClubManagementController(IBasicClubManagementBusiness buss, AmazonCognitoMiddleware amazonCognitoMiddleware, IClubManagementBusiness clubManagementBusiness)
        {
            _buss = buss;
            _amazonCognitoMiddleware = amazonCognitoMiddleware;
            _clubManagementBusiness = clubManagementBusiness;
            _amazonCognitoMiddleware.SetConfigNameViaUserType("club");
        }
        [HttpGet]
        public ActionResult BasicClubManagementList(string AgentId, string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            BasicClubManagementModel objBasicClubManagementModel = new BasicClubManagementModel();
            ViewBag.SearchFilter = null;
            Session["CurrentURL"] = "/BasicClubManagement/BasicClubManagementList";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            string RenderId = "";
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            if (TempData.ContainsKey("ManageBasicClub")) objBasicClubManagementModel.ManageBasicClub = TempData["ManageBasicClub"] as ManageBasicClubModel;
            else objBasicClubManagementModel.ManageBasicClub = new ManageBasicClubModel();
            if (TempData.ContainsKey("ManageClubModel")) objBasicClubManagementModel.ManagePremiumClub = TempData["ManageClubModel"] as ManagePremiumClubModel;
            else objBasicClubManagementModel.ManagePremiumClub = new ManagePremiumClubModel();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            var dbResponse = _buss.GetBasicClubList(dbRequest);
            objBasicClubManagementModel.BasicClubList = dbResponse.MapObjects<BasicClubListModel>();
            foreach (var item in objBasicClubManagementModel.BasicClubList)
            {
                item.AgentId = item.AgentId?.EncryptParameter();
                item.ClubLogo = ImageHelper.ProcessedImage(item.ClubLogo);
            }
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            ViewBag.ClosingDate = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLOSINGDATE", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.ClosingDateIdKeyBasic = objBasicClubManagementModel.ManageBasicClub.ClosingDate;
            ViewBag.Holiday = ApplicationUtilities.SetDDLValue(DDLHelper.LoadDropdownList("Holiday", "", culture) as Dictionary<string, string>, null, "");
            ViewBag.HolidayIdKeyBasic = !string.IsNullOrEmpty(objBasicClubManagementModel.ManageBasicClub.Holiday) ? objBasicClubManagementModel.ManageBasicClub.Holiday : null;
            ViewBag.LocationDDLList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("LOCATIONDDLPREFECTURE", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.LocationIdKeyBasic = objBasicClubManagementModel.ManageBasicClub.LocationDDL;
            objBasicClubManagementModel.SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null;


            ViewBag.Pref = DDLHelper.LoadDropdownList("PREF") as Dictionary<string, string>;
            ViewBag.Holiday = ApplicationUtilities.SetDDLValue(DDLHelper.LoadDropdownList("Holiday", "", culture) as Dictionary<string, string>, null, "");
            ViewBag.IdentificationType = DDLHelper.LoadDropdownList("DOCUMENTTYPE") as Dictionary<string, string>;
            ViewBag.IdentificationTypeIdKey = objBasicClubManagementModel.ManagePremiumClub.IdentificationType;
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.LocationDDLList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("LOCATIONDDLPREFECTURE", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.LocationDDLListTag = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("LOCATIONTAG", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.BusinessTypeDDL = ApplicationUtilities.SetDDLValue(DDLHelper.LoadDropdownList("BUSINESSTYPEDDL", "", culture) as Dictionary<string, string>, null, "");
            ViewBag.RankDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("RANKDDL", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.ClubStoreDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBSTOREDDL", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.ClubCategoryDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBCATEGORYDDL", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.LocationIdKey = objBasicClubManagementModel.ManagePremiumClub.LocationDDL;
            ViewBag.PrefIdKey = !string.IsNullOrEmpty(objBasicClubManagementModel.ManagePremiumClub.Prefecture) ? ViewBag.Pref[objBasicClubManagementModel.ManagePremiumClub.Prefecture] : null;
            ViewBag.HolidayIdKey = !string.IsNullOrEmpty(objBasicClubManagementModel.ManagePremiumClub.Holiday) ? objBasicClubManagementModel.ManagePremiumClub.Holiday : null;
            ViewBag.BusinessTypeKey = objBasicClubManagementModel.ManagePremiumClub.BusinessTypeDDL;
            ViewBag.ClosingDate = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLOSINGDATE", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.ClosingDateIdKey = objBasicClubManagementModel.ManagePremiumClub.ClosingDate;
            ViewBag.OthersHoliday = ApplicationUtilities.SetDDLValue(DDLHelper.LoadDropdownList("OTHERSHOLIDAY", "", culture) as Dictionary<string, string>, null, "");
            ViewBag.OthersHolidayIdKey = !string.IsNullOrEmpty(objBasicClubManagementModel.ManageBasicClub.OthersHoliday) ? objBasicClubManagementModel.ManageBasicClub.OthersHoliday : null;
            ViewBag.OthersHolidayIdConversion = !string.IsNullOrEmpty(objBasicClubManagementModel.ManagePremiumClub.OthersHoliday) ? objBasicClubManagementModel.ManagePremiumClub.OthersHoliday : null;
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            return View(objBasicClubManagementModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageBasicClub(ManageBasicClubModel Model, HttpPostedFileBase Logo_Certificate, HttpPostedFileBase CoverPhoto_Certificate, string LocationDDL, string BusinessTypeDDL)
        {
            string holidays = "";
            string othersHoliday = "";
            string[] array = Model.HolidayStr;
            string commaSeparatedString = string.Join(", ", array);
            List<string> holidayList = commaSeparatedString.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            string[] otherArray = Model.OthersHolidayStr;
            string otherCommaSeparatedString = string.Join(", ", otherArray);
            List<string> othersHolidayList = otherCommaSeparatedString.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            ActionResult redirectresult = null;
            redirectresult = RedirectToAction("BasicClubManagementList", "BasicClubManagement", new
            {
                SearchFilter = Model.SearchFilter,
                StartIndex = Model.StartIndex,
                PageSize = Model.PageSize == 0 ? 10 : Model.PageSize
            });

            foreach (var holiday in holidayList)
            {
                var item = holiday.DecryptParameter();
                if (string.IsNullOrEmpty(holidays))
                {
                    holidays = item.Trim();
                }
                else
                {
                    holidays = holidays + "," + item.Trim();
                }

            }
            Model.Holiday = holidays;
            foreach (var otherholiday in othersHolidayList)
            {
                var item = otherholiday.DecryptParameter();
                if (string.IsNullOrEmpty(othersHoliday))
                {
                    othersHoliday = item.Trim();
                }
                else
                {
                    othersHoliday = othersHoliday + "," + item.Trim();
                }

            }
            Model.OthersHoliday = othersHoliday;
            string ErrorMessage = string.Empty;
            string concatenateplanvalue = string.Empty;
            string LogoPath = "";
            string coverPhotoPath = "";
            var allowedContentType = AllowedImageContentType();
            string dateTime = "";
            if (ModelState.IsValid)
            {
                if
           (
              string.IsNullOrEmpty(Model.Logo) ||
              string.IsNullOrEmpty(Model.CoverPhoto)
           )
                {
                    bool allowRedirect = false;
                    if (Logo_Certificate == null || CoverPhoto_Certificate == null)
                    {
                        if (Logo_Certificate == null && string.IsNullOrEmpty(Model.Logo))
                        {
                            ErrorMessage = "Logo required";
                            allowRedirect = true;
                        }


                    }

                    if (allowRedirect)
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManageBasicClub"] = Model;
                        TempData["RenderId"] = "Manage";
                        return redirectresult;
                    }
                }

                bool allowRedirectfile = false;

                string LogoFileName = string.Empty;
                if (Logo_Certificate != null)
                {
                    var contentType = Logo_Certificate.ContentType;
                    var ext = Path.GetExtension(Logo_Certificate.FileName);
                    if (allowedContentType.Contains(contentType.ToLower()))
                    {
                        LogoFileName = $"{AWSBucketFolderNameModel.CLUB}/Logo_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                        Model.Logo = $"/{LogoFileName}";
                    }
                    else
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid image format.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        allowRedirectfile = true;
                    }
                }

                string CoverPhotoFileName = string.Empty;
                if (CoverPhoto_Certificate != null)
                {
                    var contentType = CoverPhoto_Certificate.ContentType;
                    var ext = Path.GetExtension(CoverPhoto_Certificate.FileName);
                    if (allowedContentType.Contains(contentType.ToLower()))
                    {
                        CoverPhotoFileName = $"{AWSBucketFolderNameModel.CLUB}/CoverPhoto_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                        Model.CoverPhoto = $"/{CoverPhotoFileName}";
                    }
                    else
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid image format.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        allowRedirectfile = true;

                    }
                }

                if (allowRedirectfile == true)
                {
                    TempData["ManageBasicClub"] = Model;
                    TempData["RenderId"] = "Manage";
                    return redirectresult;
                }
                ManageBasicClubCommon commonModel = Model.MapObject<ManageBasicClubCommon>();
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
                        TempData["ManageBasicClub"] = Model;
                        TempData["RenderId"] = "Manage";
                        return redirectresult;
                    }
                }
                commonModel.LocationId = LocationDDL?.DecryptParameter();
                commonModel.Prefecture = commonModel.Prefecture?.DecryptParameter();
                commonModel.ClosingDate = commonModel.ClosingDate?.DecryptParameter();
                var returntype = string.Empty;
                var dbResponse = _buss.ManageBasicClub(commonModel);
                if (dbResponse != null && dbResponse.Code == 0)
                {

                    if (Logo_Certificate != null) await ImageHelper.ImageUpload(LogoFileName, Logo_Certificate);
                    if (CoverPhoto_Certificate != null) await ImageHelper.ImageUpload(CoverPhotoFileName, CoverPhoto_Certificate);
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == CRS.ADMIN.SHARED.ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == CRS.ADMIN.SHARED.ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });
                    return redirectresult;
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });

                    TempData["ManageBasicClub"] = Model;
                    TempData["RenderId"] = "Manage";
                    return redirectresult;
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
            TempData["ManageBasicClub"] = Model;
            TempData["RenderId"] = "Manage";
            return redirectresult;
        }

        [HttpGet]
        public ActionResult ManageBasicClub(string AgentId = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;

            ManageBasicClubModel model = new ManageBasicClubModel();
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
                    return RedirectToAction("BasicClubManagementList", "BasicClubManagement", new
                    {
                        SearchFilter = SearchFilter,
                        StartIndex = StartIndex,
                        PageSize = PageSize
                    });
                }
                var dbResponse = _buss.GetBasicClubDetails(id, culture);
                model = dbResponse.MapObject<ManageBasicClubModel>();
                model.AgentId = model.AgentId.EncryptParameter();
                model.LocationDDL = !string.IsNullOrEmpty(model.LocationId) ? model.LocationId.EncryptParameter() : null;
                model.Prefecture = !string.IsNullOrEmpty(model.Prefecture) ? model.Prefecture.EncryptParameter() : null;
                string holidays = "";
                string[] array = model.Holiday.Split(','); ;
                string commaSeparatedString = string.Join(", ", array);
                List<string> holidayList = commaSeparatedString.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var holiday in holidayList)
                {
                    var item = holiday.EncryptParameter();
                    if (string.IsNullOrEmpty(holidays))
                    {
                        holidays = item.Trim();
                    }
                    else
                    {
                        holidays = holidays + "," + item.Trim();
                    }

                }
                model.Holiday = !string.IsNullOrEmpty(holidays) ? holidays : null;
                string otherHolidays = "";
                string[] otherarray = model.OthersHoliday.Split(','); ;
                string othercommaSeparatedString = string.Join(", ", otherarray);
                List<string> otherholidayList = othercommaSeparatedString.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var holiday in otherholidayList)
                {
                    var item = holiday.EncryptParameter();
                    if (string.IsNullOrEmpty(otherHolidays))
                    {
                        otherHolidays = item.Trim();
                    }
                    else
                    {
                        otherHolidays = otherHolidays + "," + item.Trim();
                    }

                }
                model.OthersHoliday = !string.IsNullOrEmpty(otherHolidays) ? otherHolidays : null;
                model.ClosingDate = !string.IsNullOrEmpty(model.ClosingDate) ? model.ClosingDate.EncryptParameter() : null;
            }
            TempData["RenderId"] = "Manage";
            TempData["ManageBasicClub"] = model;

            return RedirectToAction("BasicClubManagementList", "BasicClubManagement", new
            {
                SearchFilter = SearchFilter,
                StartIndex = StartIndex,
                PageSize = PageSize
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DeleteBasicClub(string AgentId, string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
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
            var dbResponse = _buss.DeleteBasicClubStatus(aId, commonRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == CRS.ADMIN.SHARED.ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == CRS.ADMIN.SHARED.ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return Json(dbResponse.Message, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult BlockBasicClub(string AgentId, string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
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
            var dbResponse = _buss.BlockBasicClubStatus(aId, commonRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == CRS.ADMIN.SHARED.ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == CRS.ADMIN.SHARED.ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return Json(dbResponse.Message, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UnBlockBasicClub(string AgentId, string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            var response = new CommonDbResponse();
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId)) response = new CommonDbResponse { ErrorCode = 1, Message = "Invalid details" };
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            var dbResponse = _buss.UnBlockBasicClubStatus(aId, commonRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == CRS.ADMIN.SHARED.ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == CRS.ADMIN.SHARED.ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return Json(response.SetMessageInTempData(this));
        }

        [HttpGet]
        public ActionResult ManageConversionBasicClub(string AgentId = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ManagePremiumClubModel model = new ManagePremiumClubModel();
            if (!string.IsNullOrEmpty(AgentId))
            {
                var id = AgentId.DecryptParameter();
                if (string.IsNullOrEmpty(id))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid basic club details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("BasicClubManagementList", "BasicClubManagement", new { SearchFilter = SearchFilter, StartIndex = StartIndex, PageSize = PageSize });
                }
                model.SearchFilter = SearchFilter;
                model.StartIndex = StartIndex;
                model.PageSize = PageSize;
                var dbResponse = _buss.GetBasicConversionClubDetails(id, culture);
                model = dbResponse.MapObject<ManagePremiumClubModel>();
                model.AgentId = model.AgentId.EncryptParameter();
                model.LocationDDL = !string.IsNullOrEmpty(model.LocationId) ? model.LocationId.EncryptParameter() : null;
                model.BusinessTypeDDL = !string.IsNullOrEmpty(model.BusinessType) ? model.BusinessType.EncryptParameter() : null;
                model.Prefecture = !string.IsNullOrEmpty(model.Prefecture) ? model.Prefecture.EncryptParameter() : null;
                model.IdentificationType = !string.IsNullOrEmpty(model.IdentificationType) ? model.IdentificationType.EncryptParameter() : null;
                string holidays = "";
                string[] array = model.Holiday.Split(','); ;
                string commaSeparatedString = string.Join(", ", array);
                List<string> holidayList = commaSeparatedString.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var holiday in holidayList)
                {
                    var item = holiday.EncryptParameter();
                    if (string.IsNullOrEmpty(holidays))
                    {
                        holidays = item.Trim();
                    }
                    else
                    {
                        holidays = holidays + "," + item.Trim();
                    }

                }
                model.Holiday = !string.IsNullOrEmpty(holidays) ? holidays : null;
                string otherHolidays = "";
                string[] otherarray = model.OthersHoliday.Split(','); ;
                string othercommaSeparatedString = string.Join(", ", otherarray);
                List<string> otherholidayList = othercommaSeparatedString.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var holiday in otherholidayList)
                {
                    var item = holiday.EncryptParameter();
                    if (string.IsNullOrEmpty(otherHolidays))
                    {
                        otherHolidays = item.Trim();
                    }
                    else
                    {
                        otherHolidays = otherHolidays + "," + item.Trim();
                    }

                }
                model.OthersHoliday = !string.IsNullOrEmpty(otherHolidays) ? otherHolidays : null;
                model.ClosingDate = !string.IsNullOrEmpty(model.ClosingDate) ? model.ClosingDate.EncryptParameter() : null;
            }

            TempData["ManageClubModel"] = model;
            TempData["RenderId"] = "ManagePremium";
            return RedirectToAction("BasicClubManagementList", "BasicClubManagement", new { SearchFilter = SearchFilter, StartIndex = StartIndex, PageSize = PageSize });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageConversionBasicClub(ManagePremiumClubModel Model, HttpPostedFileBase Business_Certificate, HttpPostedFileBase Logo_Certificate, HttpPostedFileBase CoverPhoto_Certificate, HttpPostedFileBase KYCDocument_Certificate, HttpPostedFileBase KYCDocumentBack_Certificate, HttpPostedFileBase PassportPhotot_Certificate, HttpPostedFileBase InsurancePhoto_Certificate, HttpPostedFileBase CorporateRegistry_Certificate, string LocationDDL, string BusinessTypeDDL)
        {
            string holidays = "";
            string othersHoliday = "";
            ActionResult redirectresult = null;
            if (!string.IsNullOrEmpty(Model.holdId))
            {
                redirectresult = RedirectToAction("BasicClubManagementList", "BasicClubManagement", new
                {
                    TabValue = "02",
                    SearchFilter = Model.SearchFilter,
                    StartIndex2 = Model.StartIndex,
                    PageSize2 = Model.PageSize
                });
            }
            else
            {
                redirectresult = RedirectToAction("BasicClubManagementList", "BasicClubManagement", new
                {
                    SearchFilter = Model.SearchFilter,
                    StartIndex = Model.StartIndex,
                    PageSize = Model.PageSize
                });
            }
            string[] array = Model.HolidayStr;
            if (Model.HolidayStr != null && Model.HolidayStr.Any(str => !string.IsNullOrEmpty(str)))
            {
                string commaSeparatedString = string.Join(", ", array);
                List<string> holidayList = commaSeparatedString.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var holiday in holidayList)
                {
                    var item = holiday.DecryptParameter();
                    if (string.IsNullOrEmpty(holidays))
                    {
                        holidays = item.Trim();
                    }
                    else
                    {
                        holidays = holidays + "," + item.Trim();
                    }

                }
                Model.Holiday = holidays;
            }
            string[] otherarray = Model.OthersHolidayStr;
            if (Model.OthersHolidayStr != null && Model.OthersHolidayStr.Any(str => !string.IsNullOrEmpty(str)))
            {
                string othercommaSeparatedString = string.Join(", ", otherarray);
                List<string> otherholidayList = othercommaSeparatedString.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var holiday in otherholidayList)
                {
                    var item = holiday.DecryptParameter();
                    if (string.IsNullOrEmpty(othersHoliday))
                    {
                        othersHoliday = item.Trim();
                    }
                    else
                    {
                        othersHoliday = othersHoliday + "," + item.Trim();
                    }

                }
                Model.OthersHoliday = othersHoliday;
            }
            string ErrorMessage = string.Empty;
            if (!string.IsNullOrEmpty(BusinessTypeDDL?.DecryptParameter())) ModelState.Remove("BusinessType");
            ViewBag.PlansList = ApplicationUtilities.LoadDropdownList("CLUBPLANS") as Dictionary<string, string>;
            ModelState.Remove("LocationURL");
            string concatenateplanvalue = string.Empty;
            string businessCertificatePath = "";
            string kycDocumentPath = "";
            string LogoPath = "";
            string coverPhotoPath = "";
            var allowedContentType = AllowedImageContentType();
            var allowedContentTypeWithPDF = AllowedImageContentTypePdf();
            string dateTime = "";
            if (Model.BusinessTypeDDL.DecryptParameter() == "1")
            {
                if (string.IsNullOrEmpty(Model.Representative1_ContactName))
                {
                    ModelState.AddModelError("Representative1_ContactName", "Required");
                }
                if (string.IsNullOrEmpty(Model.Representative1_MobileNo))
                {
                    ModelState.AddModelError("Representative1_MobileNo", "Required");
                }
                if (string.IsNullOrEmpty(Model.Representative1_Email))
                {
                    ModelState.AddModelError("Representative1_Email", "Required");
                }
                if (string.IsNullOrEmpty(Model.Representative1_Furigana))
                {
                    ModelState.AddModelError("Representative1_Furigana", "Required");
                }
                if (string.IsNullOrEmpty(Model.CompanyName))
                {
                    ModelState.AddModelError("CompanyName", "Required");
                }

            }
            else
            {
                ModelState.Remove("CompanyName");
                ModelState.Remove("Representative1_ContactName");
                ModelState.Remove("Representative1_MobileNo");
                ModelState.Remove("Representative1_Email");
                ModelState.Remove("Representative1_Furigana");
            }
            ModelState.Remove("LoginId");
            if (ModelState.IsValid)
            {
                if
           (
              string.IsNullOrEmpty(Model.BusinessCertificate) ||
              //string.IsNullOrEmpty(Model.KYCDocument) ||
              string.IsNullOrEmpty(Model.Logo) ||
              string.IsNullOrEmpty(Model.CoverPhoto) ||
              string.IsNullOrEmpty(Model.Gallery) 
              //string.IsNullOrEmpty(Model.KYCDocumentBack) ||
              //string.IsNullOrEmpty(Model.PassportPhoto) ||
              //string.IsNullOrEmpty(Model.InsurancePhoto)
           )
                {
                    bool allowRedirect = false;
                    if (Business_Certificate == null || Logo_Certificate == null || CoverPhoto_Certificate == null)
                    {


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
                        //else if (CoverPhoto_Certificate == null && string.IsNullOrEmpty(Model.CoverPhoto))
                        //{
                        //    ErrorMessage = "Cover photo required";
                        //    allowRedirect = true;
                        //}


                        //if (Model.BusinessTypeDDL.DecryptParameter() == "1")
                        //{
                        //    if (CorporateRegistry_Certificate == null && string.IsNullOrEmpty(Model.CorporateRegistryDocument))
                        //    {
                        //        ErrorMessage = "Corporate registry required";
                        //        allowRedirect = true;
                        //    }
                        //}

                    }
                    //if (Model.IdentificationType.DecryptParameter() == "2")
                    //{
                    //    if (PassportPhotot_Certificate == null && string.IsNullOrEmpty(Model.PassportPhoto))
                    //    {
                    //        ErrorMessage = "Passport photo required";
                    //        allowRedirect = true;
                    //    }
                    //    else if (InsurancePhoto_Certificate == null && string.IsNullOrEmpty(Model.InsurancePhoto))
                    //    {
                    //        ErrorMessage = "Insurance card required";
                    //        allowRedirect = true;
                    //    }
                    //}
                    //else
                    //{
                    //    if (KYCDocument_Certificate == null && string.IsNullOrEmpty(Model.KYCDocument))
                    //    {
                    //        ErrorMessage = "KYC front document required";
                    //        allowRedirect = true;
                    //    }
                    //    else if (KYCDocumentBack_Certificate == null && string.IsNullOrEmpty(Model.KYCDocumentBack))
                    //    {
                    //        ErrorMessage = "KYC back document required";
                    //        allowRedirect = true;
                    //    }

                    //}
                    if (allowRedirect)
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManageClubModel"] = Model;
                        TempData["RenderId"] = "ManagePremium";
                        return redirectresult;
                    }
                }

                bool allowRedirectfile = false;
                string businessCertificateFileName = string.Empty;
                if (Business_Certificate != null)
                {
                    var contentType = Business_Certificate.ContentType;
                    var ext = Path.GetExtension(Business_Certificate.FileName);
                    if (allowedContentTypeWithPDF.Contains(contentType.ToLower()))
                    {
                        businessCertificateFileName = $"{AWSBucketFolderNameModel.CLUB}/BusinessCertificate_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                        Model.BusinessCertificate = $"/{businessCertificateFileName}";
                    }
                    else
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid image format.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        allowRedirectfile = true;

                    }
                }

                string LogoFileName = string.Empty;
                if (Logo_Certificate != null)
                {
                    var contentType = Logo_Certificate.ContentType;
                    var ext = Path.GetExtension(Logo_Certificate.FileName);
                    if (allowedContentType.Contains(contentType.ToLower()))
                    {
                        LogoFileName = $"{AWSBucketFolderNameModel.CLUB}/Logo_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                        Model.Logo = $"/{LogoFileName}";
                    }
                    else
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid image format.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        allowRedirectfile = true;
                    }
                }

                string CoverPhotoFileName = string.Empty;
                if (CoverPhoto_Certificate != null)
                {
                    var contentType = CoverPhoto_Certificate.ContentType;
                    var ext = Path.GetExtension(CoverPhoto_Certificate.FileName);
                    if (allowedContentType.Contains(contentType.ToLower()))
                    {
                        CoverPhotoFileName = $"{AWSBucketFolderNameModel.CLUB}/CoverPhoto_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                        Model.CoverPhoto = $"/{CoverPhotoFileName}";
                    }
                    else
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid image format.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        allowRedirectfile = true;

                    }
                }
                string CorporateRegistryFileName = string.Empty;
                if (CorporateRegistry_Certificate != null)
                {
                    var contentType = CorporateRegistry_Certificate.ContentType;
                    var ext = Path.GetExtension(CorporateRegistry_Certificate.FileName);
                    if (allowedContentTypeWithPDF.Contains(contentType.ToLower()))
                    {
                        CorporateRegistryFileName = $"{AWSBucketFolderNameModel.CLUB}/CompanyRegistry_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                        Model.CorporateRegistryDocument = $"/{CorporateRegistryFileName}";
                    }
                    else
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid image format.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        allowRedirectfile = true;

                    }
                }

                string KYCDocumentFileName = string.Empty;
                string KYCDocumentBackFileName = string.Empty;
                string PassportFileName = string.Empty;
                string InsuranceFileName = string.Empty;

                if (Model.IdentificationType.DecryptParameter() == "2")
                {
                    if (PassportPhotot_Certificate != null)
                    {
                        var contentType = PassportPhotot_Certificate.ContentType;
                        var ext = Path.GetExtension(PassportPhotot_Certificate.FileName);
                        if (allowedContentTypeWithPDF.Contains(contentType.ToLower()))
                        {
                            PassportFileName = $"{AWSBucketFolderNameModel.CLUB}/PassportPhotot_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                            Model.PassportPhoto = $"/{PassportFileName}";
                        }
                        else
                        {
                            this.AddNotificationMessage(new NotificationModel()
                            {
                                NotificationType = NotificationMessage.INFORMATION,
                                Message = "Invalid image format.",
                                Title = NotificationMessage.INFORMATION.ToString(),
                            });
                            allowRedirectfile = true;

                        }
                    }

                    if (InsurancePhoto_Certificate != null)
                    {
                        var contentType = InsurancePhoto_Certificate.ContentType;
                        var ext = Path.GetExtension(InsurancePhoto_Certificate.FileName);
                        if (allowedContentTypeWithPDF.Contains(contentType.ToLower()))
                        {
                            InsuranceFileName = $"{AWSBucketFolderNameModel.CLUB}/InsuranceCard_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                            Model.InsurancePhoto = $"/{InsuranceFileName}";
                        }
                        else
                        {
                            this.AddNotificationMessage(new NotificationModel()
                            {
                                NotificationType = NotificationMessage.INFORMATION,
                                Message = "Invalid image format.",
                                Title = NotificationMessage.INFORMATION.ToString(),
                            });
                            allowRedirectfile = true;

                        }
                    }

                }
                else
                {
                    if (KYCDocument_Certificate != null)
                    {
                        var contentType = KYCDocument_Certificate.ContentType;
                        var ext = Path.GetExtension(KYCDocument_Certificate.FileName);
                        if (allowedContentTypeWithPDF.Contains(contentType.ToLower()))
                        {
                            KYCDocumentFileName = $"{AWSBucketFolderNameModel.CLUB}/KYCDocument_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                            Model.KYCDocument = $"/{KYCDocumentFileName}";
                        }
                        else
                        {
                            this.AddNotificationMessage(new NotificationModel()
                            {
                                NotificationType = NotificationMessage.INFORMATION,
                                Message = "Invalid image format.",
                                Title = NotificationMessage.INFORMATION.ToString(),
                            });
                            allowRedirectfile = true;
                        }
                    }

                    if (KYCDocumentBack_Certificate != null)
                    {
                        var contentType = KYCDocumentBack_Certificate.ContentType;
                        var ext = Path.GetExtension(KYCDocumentBack_Certificate.FileName);
                        if (allowedContentTypeWithPDF.Contains(contentType.ToLower()))
                        {
                            KYCDocumentBackFileName = $"{AWSBucketFolderNameModel.CLUB}/KYCDocumentBack_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                            Model.KYCDocumentBack = $"/{KYCDocumentBackFileName}";
                        }
                        else
                        {
                            this.AddNotificationMessage(new NotificationModel()
                            {
                                NotificationType = NotificationMessage.INFORMATION,
                                Message = "Invalid image format.",
                                Title = NotificationMessage.INFORMATION.ToString(),
                            });
                            allowRedirectfile = true;

                        }
                    }
                }
                if (allowRedirectfile == true)
                {
                    TempData["ManageClubModel"] = Model;
                    TempData["RenderId"] = "ManagePremium";
                    return redirectresult;
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
                        TempData["RenderId"] = "ManagePremium";
                        return redirectresult;
                    }
                }
                if (!string.IsNullOrEmpty(commonModel.holdId))
                {
                    commonModel.holdId = commonModel.holdId.DecryptParameter();
                    if (string.IsNullOrEmpty(commonModel.holdId))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid club details.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });

                        TempData["ManageClubModel"] = Model;
                        TempData["RenderId"] = "ManagePremium";
                        return redirectresult;
                    }
                }
                commonModel.LocationId = LocationDDL?.DecryptParameter();
                commonModel.BusinessType = BusinessTypeDDL?.DecryptParameter();
                commonModel.Prefecture = commonModel.Prefecture?.DecryptParameter();
                commonModel.ClosingDate = commonModel.ClosingDate?.DecryptParameter();
                commonModel.IdentificationType = commonModel.IdentificationType?.DecryptParameter();
                Model.BusinessType = BusinessTypeDDL;
                var returntype = string.Empty;
                var _sqlTransactionHandler = new RepositoryDaoWithTransaction(null, null);
                _sqlTransactionHandler.BeginTransaction();
                try
                {
                    var dbResponse = _buss.ManageConversionClub(commonModel, _sqlTransactionHandler.GetCurrentConnection(), _sqlTransactionHandler.GetCurrentTransaction());
                    if (dbResponse == null || dbResponse.Code != 0
                        || string.IsNullOrEmpty(dbResponse?.Extra1)
                        || string.IsNullOrEmpty(dbResponse?.Extra2)
                        || string.IsNullOrEmpty(dbResponse?.Extra3)
                        || string.IsNullOrEmpty(dbResponse?.Extra4)
                        || string.IsNullOrEmpty(dbResponse?.Extra5)
                    )
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Failed",
                            Title = NotificationMessage.INFORMATION.ToString()
                        });
                        TempData["ManageClubModel"] = Model;
                        TempData["RenderId"] = "ManagePremium";
                        _sqlTransactionHandler.RollbackTransaction();
                        return redirectresult;
                    }

                    var countryCode = ConfigurationManager.AppSettings["CountryCode"];
                    var signUpResponse = await _amazonCognitoMiddleware.AdminCreateUserAsync(new CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.SignUp.SignUpModel.Request
                    {
                        Username = dbResponse?.Extra5,
                        Password = dbResponse.Extra2,
                        AttributeType = new List<CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.SignUp.SignUpModel.AttributeType>
                        {
                            new CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.SignUp.SignUpModel.AttributeType
                            {
                                Name = AttributeTypeName.PhoneNumber,
                                Value = ApplicationUtilities.FormatPhoneNumber(dbResponse?.Extra3, countryCode)
                            },
                            new CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.SignUp.SignUpModel.AttributeType
                            {
                                Name = AttributeTypeName.Email,
                                Value =dbResponse?.Extra4
                            }
                        }
                    });

                    if (signUpResponse?.Code != CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.ResponseCode.Success)
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Something went wrong. Please try again later",
                            Title = NotificationMessage.INFORMATION.ToString()
                        });
                        TempData["ManageClubModel"] = Model;
                        TempData["RenderId"] = "ManagePremium";
                        _sqlTransactionHandler.RollbackTransaction();
                        return redirectresult;
                    }

                    var cognitoUserId = signUpResponse.Data.MapObjects<CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.SignUp.SignUpModel.AdminCreateUserResponse>().FirstOrDefault(x => x.Name == CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.AttributeTypeName.Sub)?.Value;
                    var manageClubCognitoDetailResponse = _clubManagementBusiness.ManageClubCognitoDetail(dbResponse.Extra1, dbResponse.Extra5, cognitoUserId, _sqlTransactionHandler.GetCurrentConnection(), _sqlTransactionHandler.GetCurrentTransaction());
                    if (manageClubCognitoDetailResponse?.Code != CRS.ADMIN.SHARED.ResponseCode.Success)
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Something went wrong. Please try again later",
                            Title = NotificationMessage.INFORMATION.ToString()
                        });
                        TempData["ManageClubModel"] = Model;
                        TempData["RenderId"] = "ManagePremium";
                        _sqlTransactionHandler.RollbackTransaction();
                        return redirectresult;
                    }

                    if (Business_Certificate != null) await ImageHelper.ImageUpload(businessCertificateFileName, Business_Certificate);
                    if (Logo_Certificate != null) await ImageHelper.ImageUpload(LogoFileName, Logo_Certificate);
                    if (CoverPhoto_Certificate != null) await ImageHelper.ImageUpload(CoverPhotoFileName, CoverPhoto_Certificate);
                    if (commonModel.IdentificationType == "2")
                    {
                        if (PassportPhotot_Certificate != null) await ImageHelper.ImageUpload(PassportFileName, PassportPhotot_Certificate);
                        if (InsurancePhoto_Certificate != null) await ImageHelper.ImageUpload(InsuranceFileName, InsurancePhoto_Certificate);
                    }
                    else
                    {
                        if (KYCDocument_Certificate != null) await ImageHelper.ImageUpload(KYCDocumentFileName, KYCDocument_Certificate);
                        if (KYCDocumentBack_Certificate != null) await ImageHelper.ImageUpload(KYCDocumentBackFileName, KYCDocumentBack_Certificate);
                    }

                    if (commonModel.BusinessType == "1")
                    {
                        if (CorporateRegistry_Certificate != null) await ImageHelper.ImageUpload(CorporateRegistryFileName, CorporateRegistry_Certificate);
                    }
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == CRS.ADMIN.SHARED.ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == CRS.ADMIN.SHARED.ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });
                    _sqlTransactionHandler.CommitTransaction();
                    return redirectresult;
                }
                catch (Exception ex)
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.WARNING,
                        Message = $"Something went wrong. Please try again later {ex.Message}",
                        Title = "Exception"
                    });
                    _sqlTransactionHandler.RollbackTransaction();
                    TempData["ManageClubModel"] = Model;
                    TempData["RenderId"] = "ManagePremium";
                    return redirectresult;
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
            TempData["RenderId"] = "ManagePremium";
            return redirectresult;
        }


    }
}
