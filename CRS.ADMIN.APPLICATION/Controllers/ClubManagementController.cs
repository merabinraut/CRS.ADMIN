using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models;
using CRS.ADMIN.APPLICATION.Models.ClubManagement;
using CRS.ADMIN.APPLICATION.Models.ClubManagerModel;
using CRS.ADMIN.APPLICATION.Models.TagManagement;
using CRS.ADMIN.BUSINESS.ClubManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using DocumentFormat.OpenXml.Office2019.Excel.RichData2;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Google.Apis.Requests.BatchRequest;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class ClubManagementController : BaseController
    {
        private readonly IClubManagementBusiness _BUSS;
        private readonly HttpClient _httpClient;
        public ClubManagementController(IClubManagementBusiness BUSS, HttpClient httpClient)
        {
            _BUSS = BUSS;
            _httpClient = httpClient;
        }
        [HttpGet]
        public ActionResult ClubList( string TabValue = "",string SearchFilter = "", int StartIndex = 0, int PageSize = 10, int StartIndex2 = 0, int PageSize2 = 10, int StartIndex3 = 0, int PageSize3 = 10)
        {
            ViewBag.SearchFilter = SearchFilter;
            Session["CurrentURL"] = "/ClubManagement/ClubList";
            string RenderId = "";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            var response = new ClubManagementCommonModel();
            if (TempData.ContainsKey("ManageClubModel")) response.ManageClubModel = TempData["ManageClubModel"] as ManageClubModel;
            else response.ManageClubModel = new ManageClubModel();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            if (TempData.ContainsKey("ManageTagModel")) response.ManageTag = TempData["ManageTagModel"] as ManageTag;
            else response.ManageTag = new ManageTag();
            if (TempData.ContainsKey("ManageManagerModel")) response.ManageManager = TempData["ManageManagerModel"] as ManageManagerModel;
            else response.ManageManager = new ManageManagerModel();
            if (TempData.ContainsKey("ClubHoldDetails")) response.ClubHoldModel = TempData["ClubHoldDetails"] as ManageClubModel;
            else response.ClubHoldModel = new ManageClubModel();


            if (TempData.ContainsKey("AvailabilityModel")) response.GetAvailabilityList = TempData["AvailabilityModel"] as List<AvailabilityTagModel>;
            else response.ManageTag.GetAvailabilityTagModel = new List<AvailabilityTagModel>();
            
            //****************************  Start Approved List  **************************************//

            if(TabValue == "")
            {
                PaginationFilterCommon dbRequestall = new PaginationFilterCommon()
                {
                    Skip = TabValue == "" ? StartIndex : 0,
                    Take = TabValue == "" ? PageSize : 10,
                    SearchFilter = TabValue == "" ? !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null : null
                };
                var dbResponse = _BUSS.GetClubList(dbRequestall);
                response.ClubListModel = dbResponse.MapObjects<ClubListModel>();
                foreach (var item in response.ClubListModel)
                {
                    item.AgentId = item.AgentId?.EncryptParameter();
                    item.ClubLogo = ImageHelper.ProcessedImage(item.ClubLogo);
                }
              ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            }
            else
            {
                ViewBag.TotalData =  0;
            }


            //****************************   END Approved List   **************************************//

            //****************************   Start Pending List   **************************************//
            if (TabValue == "02")
            {
                PaginationFilterCommon dbRequestpending = new PaginationFilterCommon()
                {
                    Skip = StartIndex2,
                    Take = PageSize2,
                    SearchFilter = TabValue == "02" ? !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null : null
                };

                var dbResponsePending = _BUSS.GetClubPendingList(dbRequestpending);
                response.ClubPendingListModel = dbResponsePending.MapObjects<ClubListModel>();
                response.ClubPendingListModel.ForEach(item =>
                {
                    item.SNO = item.SNO?.EncryptParameter();
                    item.AgentId = item.AgentId?.EncryptParameter();
                    item.ClubLogo = ImageHelper.ProcessedImage(item.ClubLogo);
                });
                ViewBag.TotalData2 = dbResponsePending != null && dbResponsePending.Any() ? dbResponsePending[0].TotalRecords : 0;
            }
            else
            {
                ViewBag.TotalData2 = 0;
            }
            //************************************   END Pending List     **************************************//

            //***********************************   START Rejected List   **************************************//
            if (TabValue == "03")
            {
                PaginationFilterCommon dbRequestreject = new PaginationFilterCommon()
                {
                    Skip = StartIndex3,
                    Take = PageSize3,
                    SearchFilter = TabValue == "03" ? !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null : null
                };
                var dbResponseRejected = _BUSS.GetClubRejectedList(dbRequestreject);
                response.ClubRejectedListModel = dbResponseRejected.MapObjects<ClubListModel>();
                response.ClubRejectedListModel.ForEach(item =>
                {
                    item.SNO = item.SNO?.EncryptParameter();
                    item.ClubLogo = ImageHelper.ProcessedImage(item.ClubLogo);
                });
                ViewBag.TotalData3 = dbResponseRejected != null && dbResponseRejected.Any() ? dbResponseRejected[0].TotalRecords : 0;
            }
            else
            {
                ViewBag.TotalData3 =  0;
            }
            //*************************************   END Rejected List  **************************************//

            ViewBag.StartIndex2 = StartIndex2;
            ViewBag.PageSize2 = PageSize2;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.StartIndex3 = StartIndex3;
            ViewBag.PageSize3 = PageSize3;                                
            ViewBag.Pref = DDLHelper.LoadDropdownList("PREF") as Dictionary<string, string>;          
            ViewBag.Holiday = ApplicationUtilities.SetDDLValue(DDLHelper.LoadDropdownList("Holiday", "", culture) as Dictionary<string, string>, null, "");
            ViewBag.IdentificationType = DDLHelper.LoadDropdownList("DOCUMENTTYPE") as Dictionary<string, string>;
            ViewBag.IdentificationTypeIdKey = response.ManageClubModel.IdentificationType;
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.LocationDDLList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("LOCATIONDDLPREFECTURE", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.LocationDDLListTag = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("LOCATIONTAG", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.BusinessTypeDDL = ApplicationUtilities.SetDDLValue(DDLHelper.LoadDropdownList("BUSINESSTYPEDDL", "", culture) as Dictionary<string, string>, null, "");
            ViewBag.RankDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("RANKDDL", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.ClubStoreDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBSTOREDDL", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.ClubCategoryDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBCATEGORYDDL", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.RankDDLKey = response.ManageTag.Tag2RankName;
            ViewBag.ClubStoreDDLKey = response.ManageTag.Tag5StoreName;
            ViewBag.ClubCategoryDDLKey = response.ManageTag.Tag3CategoryName;
            ViewBag.LocationIdKey = response.ManageClubModel.LocationDDL;
            ViewBag.LocationIdtagKey = response.ManageTag.Tag1Location;
            ViewBag.PrefIdKey = !string.IsNullOrEmpty(response.ManageClubModel.Prefecture) ? ViewBag.Pref[response.ManageClubModel.Prefecture] : null;
            ViewBag.HolidayIdKey = !string.IsNullOrEmpty(response.ManageClubModel.Holiday) ? response.ManageClubModel.Holiday : null;
            ViewBag.BusinessTypeKey = response.ManageClubModel.BusinessTypeDDL;
            ViewBag.ClosingDate = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLOSINGDATE", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.ClosingDateIdKey = response.ManageClubModel.ClosingDate;           
            response.ListType = TabValue;
            response.TabValue = TabValue;          
            response.SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null;
            return View(response);
        }
       

        [HttpGet]
        public ActionResult ManageClub(string AgentId = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ManageClubModel model = new ManageClubModel();
            //ViewBag.CountryCodeDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("COUNTRYCODE") as Dictionary<string, string>, null);
           
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
                    return RedirectToAction("ClubList", "ClubManagement", new { SearchFilter = SearchFilter, StartIndex = StartIndex, PageSize = PageSize });
                }
                model.SearchFilter = SearchFilter;
                model.StartIndex = StartIndex;
                model.PageSize = PageSize;
                var dbResponse = _BUSS.GetClubDetails(id, culture);
                model = dbResponse.MapObject<ManageClubModel>();
                //ViewBag.CountryCodeDDLKey = model.LandLineCode;
                model.AgentId = model.AgentId.EncryptParameter();
                //model.LocationId = !string.IsNullOrEmpty(model.LocationId) ? model.LocationId.EncryptParameter() : null;
                //model.BusinessType = !string.IsNullOrEmpty(model.BusinessType) ? model.BusinessType.EncryptParameter() : null;
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
                model.ClosingDate = !string.IsNullOrEmpty(model.ClosingDate) ? model.ClosingDate.EncryptParameter() : null;
            }

            TempData["ManageClubModel"] = model;
            TempData["RenderId"] = "Manage";
            TempData["EditPlan"] = model;

            return RedirectToAction("ClubList", "ClubManagement", new { SearchFilter = SearchFilter, StartIndex = StartIndex, PageSize = PageSize });
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
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            var dbResponse = _BUSS.GetClubDetails(id, culture);

            response.AgentId = null;
            response.UserId = null;
            return View(response);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DeleteClub(string AgentId )
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
            string status = "B";
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
        public ActionResult UnBlockClub(string AgentId,string status)
        {
            var response = new CommonDbResponse();
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId)) response = new CommonDbResponse { ErrorCode = 1, Message = "Invalid details" };
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };            
            var dbResponse = _BUSS.ManageClubStatus(aId, status, commonRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
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
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return Json(JsonRequestBehavior.AllowGet);
            //return Json(response.SetMessageInTempData(this));
        }

        [HttpGet]
        public ActionResult ManagePendingClub(string AgentId = "", string holdId = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ManageClubModel model = new ManageClubModel();
            var id = string.Empty;
            if (!string.IsNullOrEmpty(AgentId))
            {
                id = AgentId.DecryptParameter();
                if (string.IsNullOrEmpty(id))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid club details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("ClubList", "ClubManagement", new { TabValue = "02", SearchFilter = SearchFilter, StartIndex2 = StartIndex, PageSize2 = PageSize });
                }
            }
            var holdid = holdId.DecryptParameter();
            if (string.IsNullOrEmpty(holdid))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid club hold details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ClubList", "ClubManagement", new { TabValue = "02", SearchFilter = SearchFilter, StartIndex2 = StartIndex, PageSize2 = PageSize });
            }
            var dbResponse = _BUSS.GetplanPendingDetails(id, holdid, culture);
            model = dbResponse.MapObject<ManageClubModel>();
            model.AgentId = model.AgentId.EncryptParameter();
            model.LocationDDL = !string.IsNullOrEmpty(model.LocationId) ? model.LocationId.EncryptParameter() : null;
            model.BusinessTypeDDL = !string.IsNullOrEmpty(model.BusinessType) ? model.BusinessType.EncryptParameter() : null;
            //model.Prefecture = !string.IsNullOrEmpty(model.Prefecture) ? model.Prefecture.EncryptParameter() : null;
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
            //model.Holiday = !string.IsNullOrEmpty(model.Holiday) ? model.Holiday.EncryptParameter() : null;
            model.ClosingDate = !string.IsNullOrEmpty(model.ClosingDate) ? model.ClosingDate.EncryptParameter() : null;
            model.holdId = !string.IsNullOrEmpty(model.holdId) ? model.holdId.EncryptParameter() : null;
            model.IdentificationType = !string.IsNullOrEmpty(model.IdentificationType) ? model.IdentificationType.EncryptParameter() : null;
            model.SearchFilter = SearchFilter;
            model.StartIndex = StartIndex;
            model.PageSize = PageSize;
            TempData["ManageClubModel"] = model;
            TempData["RenderId"] = "Manage";
            TempData["EditPlan"] = model;

            return RedirectToAction("ClubList", "ClubManagement",new { TabValue = "02", SearchFilter = SearchFilter, StartIndex2 = StartIndex, PageSize2 = PageSize });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageClub(ManageClubModel Model, HttpPostedFileBase Business_Certificate, HttpPostedFileBase Logo_Certificate, HttpPostedFileBase CoverPhoto_Certificate, HttpPostedFileBase KYCDocument_Certificate, HttpPostedFileBase KYCDocumentBack_Certificate, HttpPostedFileBase PassportPhotot_Certificate, HttpPostedFileBase InsurancePhoto_Certificate, HttpPostedFileBase CorporateRegistry_Certificate, string LocationDDL, string BusinessTypeDDL)
        {
            string holidays = "";
            string[] array = Model.HolidayStr;
            string commaSeparatedString = string.Join(", ", array);
            List<string> holidayList = commaSeparatedString.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            ActionResult redirectresult = null;
            if(!string.IsNullOrEmpty(Model.holdId))
            {
                redirectresult = RedirectToAction("ClubList", "ClubManagement", new
                {
                    TabValue="02",
                    SearchFilter = Model.SearchFilter,
                    StartIndex2 = Model.StartIndex,
                    PageSize2 = Model.PageSize
                });
            }
            else
            {
                redirectresult = RedirectToAction("ClubList", "ClubManagement", new
                {
                    SearchFilter = Model.SearchFilter,
                    StartIndex = Model.StartIndex,
                    PageSize = Model.PageSize
                });
            }
            foreach (var holiday in holidayList)
            {
               var item = holiday.DecryptParameter();
                if (string.IsNullOrEmpty(holidays))
                {
                    holidays = item.Trim();
                }
                else
                {
                    holidays = holidays+"," + item.Trim();
                }
               
            }
            Model.Holiday= holidays;    
            string ErrorMessage = string.Empty;
            if (!string.IsNullOrEmpty(BusinessTypeDDL?.DecryptParameter())) ModelState.Remove("BusinessType");

            //if (!string.IsNullOrEmpty(LocationDDL?.DecryptParameter())) ModelState.Remove("LocationId");
            ViewBag.PlansList = ApplicationUtilities.LoadDropdownList("CLUBPLANS") as Dictionary<string, string>;
            //ViewBag.CountryCodeDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("COUNTRYCODE") as Dictionary<string, string>, null);

            ModelState.Remove("LocationURL");
            string concatenateplanvalue = string.Empty;
            string businessCertificatePath = "";
            string kycDocumentPath = "";
            string LogoPath = "";
            string coverPhotoPath = "";
            var allowedContentType = AllowedImageContentType();
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
              string.IsNullOrEmpty(Model.KYCDocument) ||
              string.IsNullOrEmpty(Model.Logo) ||
              string.IsNullOrEmpty(Model.CoverPhoto) ||
              string.IsNullOrEmpty(Model.Gallery) ||
              string.IsNullOrEmpty(Model.KYCDocumentBack) ||
              string.IsNullOrEmpty(Model.PassportPhoto) ||
              string.IsNullOrEmpty(Model.InsurancePhoto)
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


                        if (Model.BusinessTypeDDL.DecryptParameter() == "1")
                        {
                            if (CorporateRegistry_Certificate == null && string.IsNullOrEmpty(Model.CorporateRegistryDocument))
                            {
                                ErrorMessage = "Corporate registry required";
                                allowRedirect = true;
                            }
                        }

                    }
                    if (Model.IdentificationType.DecryptParameter() == "2")
                    {
                        if (PassportPhotot_Certificate == null && string.IsNullOrEmpty(Model.PassportPhoto))
                        {
                            ErrorMessage = "Passport photo required";
                            allowRedirect = true;
                        }
                        else if (InsurancePhoto_Certificate == null && string.IsNullOrEmpty(Model.InsurancePhoto))
                        {
                            ErrorMessage = "Insurance card required";
                            allowRedirect = true;
                        }
                    }
                    else
                    {
                        if (KYCDocument_Certificate == null && string.IsNullOrEmpty(Model.KYCDocument))
                        {
                            ErrorMessage = "KYC front document required";
                            allowRedirect = true;
                        }
                        else if (KYCDocumentBack_Certificate == null && string.IsNullOrEmpty(Model.KYCDocumentBack))
                        {
                            ErrorMessage = "KYC back document required";
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
                        TempData["ManageClubModel"] = Model;
                        TempData["RenderId"] = "Manage";
                        return redirectresult;
                    }
                }

                bool allowRedirectfile = false;
                string businessCertificateFileName = string.Empty;
                if (Business_Certificate != null)
                {
                    var contentType = Business_Certificate.ContentType;
                    var ext = Path.GetExtension(Business_Certificate.FileName);
                    if (allowedContentType.Contains(contentType.ToLower()))
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
                    if (allowedContentType.Contains(contentType.ToLower()))
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
                        if (allowedContentType.Contains(contentType.ToLower()))
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
                        if (allowedContentType.Contains(contentType.ToLower()))
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
                        if (allowedContentType.Contains(contentType.ToLower()))
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
                        if (allowedContentType.Contains(contentType.ToLower()))
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
                    //Model.BusinessCertificate =  Business_Certificate.FileName;
                    //Model.CoverPhoto =  CoverPhoto_Certificate.FileName;
                    //Model.Logo=  Logo_Certificate.FileName;
                    TempData["ManageClubModel"] = Model;
                    TempData["RenderId"] = "Manage";
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
                        TempData["RenderId"] = "Manage";
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
                        TempData["RenderId"] = "Manage";
                        return redirectresult;
                    }
                }
                commonModel.LocationId = LocationDDL?.DecryptParameter();
                commonModel.BusinessType = BusinessTypeDDL?.DecryptParameter();
                //commonModel.Holiday = commonModel.Holiday?.DecryptParameter();
                commonModel.Prefecture = commonModel.Prefecture?.DecryptParameter();
                commonModel.ClosingDate = commonModel.ClosingDate?.DecryptParameter();
                commonModel.IdentificationType = commonModel.IdentificationType?.DecryptParameter();
                Model.BusinessType = BusinessTypeDDL;
                var returntype = string.Empty;
                var dbResponse = _BUSS.ManageClub(commonModel);
                if (dbResponse != null && dbResponse.Code == 0)
                {
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
                        NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
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

                    TempData["ManageClubModel"] = Model;
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
            TempData["ManageClubModel"] = Model;
            TempData["RenderId"] = "Manage";

            return redirectresult;
        }


        [HttpGet]
        public ActionResult ApproveRejectClub(string holdid, string type, string AgentId, string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            ClubDetailModel response = new ClubDetailModel();
            var id = holdid.DecryptParameter();
            if (string.IsNullOrEmpty(id))
            {
                this.ShowPopup(1, "Invalid club details");
                return RedirectToAction("ClubList", "ClubManagement", new { TabValue = "02", SearchFilter = SearchFilter, StartIndex2 = StartIndex, PageSize2 = PageSize });

            }

            if (!string.IsNullOrEmpty(AgentId))
            {
                AgentId = AgentId.DecryptParameter();
            }
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ManageClubCommon model = new ManageClubCommon();
            if (type.ToLower() == "a")
            {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(AgentId))
                {
                    type = "rcm_a";
                    var dbResponse1 = _BUSS.GetplanPendingDetails(AgentId, id, culture);
                    model = dbResponse1.MapObject<ManageClubCommon>();
                }
                else
                {
                    type = "r_cha";
                }

            }
            else if (type.ToLower() == "r")
            {
                type = "rc_r";
            }
            var dbResponse = _BUSS.ManageApproveReject(id, type, AgentId, culture, model);
            response.AgentId = null;
            response.UserId = null;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = dbResponse.Message ?? "Something went wrong. Please try again later",
                Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return RedirectToAction("ClubList", "ClubManagement", new { TabValue = "02", SearchFilter = SearchFilter, StartIndex2 = StartIndex, PageSize2 = PageSize });
        }

        [HttpGet]
        [OverrideActionFilters]
        public ActionResult ClubHoldDetails(string Holdid = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            var ResponseModel = new ManageClubModel();
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            //var availabilityInfo = new List<AvailabilityTagModel>();
            var Id = Holdid?.DecryptParameter();
            if (string.IsNullOrEmpty(Id))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ClubList", "ClubManagement", new { TabValue = "02", SearchFilter = SearchFilter, StartIndex2 = StartIndex, PageSize2 = PageSize });
            }
            else
            {

                var dbResponseInfo = _BUSS.GetClubPendingDetails("", Id, "");
                //var dbAvailabilityInfo = _BUSS.GetAvailabilityList(cId);
                ResponseModel = dbResponseInfo.MapObject<ManageClubModel>();
                ResponseModel.LandlineNumber = !string.IsNullOrEmpty(ResponseModel.LandLineCode)
                    ? ResponseModel.LandLineCode + '-' + ResponseModel.LandlineNumber
                    : ResponseModel.LandlineNumber; ResponseModel.LocationDDL = !string.IsNullOrEmpty(ResponseModel.LocationId) ? ResponseModel.LocationId.EncryptParameter() : null;

                ResponseModel.Logo = !string.IsNullOrEmpty(ResponseModel.Logo) ?
                                      ImageHelper.ProcessedImage(ResponseModel.Logo) :
                                      ResponseModel.Logo; ;
                ResponseModel.BusinessCertificate = !string.IsNullOrEmpty(ResponseModel.BusinessCertificate) ?
                                                    ImageHelper.ProcessedImage(ResponseModel.BusinessCertificate) :
                                                    ResponseModel.BusinessCertificate;

                if (ResponseModel.IdentificationType == "2")
                {
                    ResponseModel.PassportPhoto = !string.IsNullOrEmpty(ResponseModel.KYCDocument) ?
                                                   ImageHelper.ProcessedImage(ResponseModel.KYCDocument) :
                                                   ResponseModel.KYCDocument;
                    ResponseModel.InsurancePhoto = !string.IsNullOrEmpty(ResponseModel.KYCDocumentBack) ?
                                                   ImageHelper.ProcessedImage(ResponseModel.KYCDocumentBack) :
                                                   ResponseModel.KYCDocumentBack;
                }
                else
                {
                    ResponseModel.KYCDocument = !string.IsNullOrEmpty(ResponseModel.KYCDocument) ?
                                                 ImageHelper.ProcessedImage(ResponseModel.KYCDocument) :
                                                 ResponseModel.KYCDocument;
                    ResponseModel.KYCDocumentBack = !string.IsNullOrEmpty(ResponseModel.KYCDocumentBack) ?
                                                    ImageHelper.ProcessedImage(ResponseModel.KYCDocumentBack) :
                                                    ResponseModel.KYCDocumentBack;
                }
                if (ResponseModel.BusinessType == "1")
                {
                    ResponseModel.CorporateRegistryDocument = !string.IsNullOrEmpty(ResponseModel.CorporateRegistryDocument) ?
                                                               ImageHelper.ProcessedImage(ResponseModel.CorporateRegistryDocument) :
                                                               ResponseModel.CorporateRegistryDocument;
                }
                ResponseModel.BusinessTypeDDL = !string.IsNullOrEmpty(ResponseModel.BusinessType) ? ResponseModel.BusinessType.EncryptParameter() : null;
                ResponseModel.Prefecture = !string.IsNullOrEmpty(ResponseModel.Prefecture) ? ResponseModel.Prefecture.EncryptParameter() : null;
                //ViewBag.Pref = DDLHelper.LoadDropdownList("PREF") as Dictionary<string, string>;
                ViewBag.LocationDDLList = ApplicationUtilities.LoadDropdownList("LOCATIONDDLPREFECTURE") as Dictionary<string, string>;
                ViewBag.BusinessTypeDDL = ApplicationUtilities.LoadDropdownList("BUSINESSTYPEDDL") as Dictionary<string, string>;
                ViewBag.Pref = DDLHelper.LoadDropdownList("PREF") as Dictionary<string, string>;
                var value = string.Empty;
                ResponseModel.Prefecture = (ResponseModel.Prefecture != null && ViewBag.Pref?.TryGetValue(ResponseModel.Prefecture, out value)) ? value : "";
                //ResponseModel.Prefecture = ViewBag.Pref.ContainsKey(ResponseModel.Prefecture) ? ViewBag.Pref[ResponseModel.Prefecture] : "";
                ResponseModel.LocationDDL = ViewBag.LocationDDLList.ContainsKey(ResponseModel.LocationDDL) ? ViewBag.LocationDDLList[ResponseModel.LocationDDL] : "";
                //ViewBag.BusinessTypeDDL = ApplicationUtilities.LoadDropdownList("BUSINESSTYPEDDL") as Dictionary<string, string>;

                ViewBag.BusinessTypeDDL = DDLHelper.LoadDropdownList("BUSINESSTYPEDDL", "", culture) as Dictionary<string, string>;
                ResponseModel.BusinessTypeDDL = (ResponseModel.BusinessTypeDDL != null && ViewBag.BusinessTypeDDL?.ContainsKey(ResponseModel.BusinessTypeDDL) == true) ? ViewBag.BusinessTypeDDL[ResponseModel.BusinessTypeDDL] : "";
                //ResponseModel.BusinessTypeDDL = ViewBag.BusinessTypeDDL.ContainsKey(ResponseModel.BusinessTypeDDL) ? ViewBag.BusinessTypeDDL[ResponseModel.BusinessTypeDDL] : "";
                ViewBag.IdentificationType = DDLHelper.LoadDropdownList("DOCUMENTTYPE") as Dictionary<string, string>;
                ResponseModel.IdentificationType = ResponseModel.IdentificationType.EncryptParameter();
                ResponseModel.IdentificationTypeName = (ResponseModel.IdentificationType != null && ViewBag.IdentificationType?.TryGetValue(ResponseModel.IdentificationType, out value)) ? value : "";
                TempData["ClubHoldDetails"] = ResponseModel;
                TempData["RenderId"] = "ClubHoldDetails";
                return RedirectToAction("ClubList", "ClubManagement", new { TabValue = "02", SearchFilter = SearchFilter, StartIndex2 = StartIndex, PageSize2 = PageSize});
            }
        }
        #region Manage Manager
        [HttpGet]
        public ActionResult ManageManager(string AgentId = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            ManageManagerModel model = new ManageManagerModel();
            var agentid = AgentId;
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            ViewBag.EventType = ApplicationUtilities.LoadDropdownValuesList("EVENTTYPE", "", "", culture);
            if (!string.IsNullOrEmpty(AgentId))
            {

                var id = AgentId.DecryptParameter();

                if (string.IsNullOrEmpty(id))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid event details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("ClubList", "ClubManagement", new { SearchFilter = SearchFilter, StartIndex = StartIndex, PageSize = PageSize });
                }

                var dbResponse = _BUSS.GetManagerDetails(id);
                model = dbResponse.MapObject<ManageManagerModel>();
                model.ManagerId = model.ManagerId.EncryptParameter();
                model.ClubId = AgentId;
                model.SearchFilter = SearchFilter;
                model.StartIndex = StartIndex;
                model.PageSize = PageSize;
            }
            TempData["ManageManagerModel"] = model;
            TempData["RenderId"] = "ManageManager";

            return RedirectToAction("ClubList", "ClubManagement", new { SearchFilter = SearchFilter, StartIndex = StartIndex, PageSize = PageSize });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageManager(ManageManagerModel Model)
        {
            //string[] updatedValues = form.GetValues("checkedValues");
            var Request = new ManageManagerModel();
            var ClubId = !string.IsNullOrEmpty(Model.ClubId) ? Model.ClubId.DecryptParameter() : null;
            var managerID = String.Empty;
            if (string.IsNullOrEmpty(ClubId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ClubList", "ClubManagement", new { SearchFilter = Model.SearchFilter, StartIndex = Model.StartIndex, PageSize = Model.PageSize });
            }
            if (!string.IsNullOrEmpty(Model.ManagerId))
            {
                managerID = !string.IsNullOrEmpty(Model.ManagerId) ? Model.ManagerId.DecryptParameter() : null;
            }
            if (ModelState.IsValid)
            {
                var dbRequest = Model.MapObject<ManageManagerCommon>();
                dbRequest.ClubId = ClubId;
                dbRequest.ManagerId = managerID;
                dbRequest.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                dbRequest.ActionIP = ApplicationUtilities.GetIP();
                var dbResponse = _BUSS.ManageManager(dbRequest);
                if (dbResponse != null && dbResponse.Code == 0)
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = dbResponse.Message ?? "Success",
                        Title = NotificationMessage.SUCCESS.ToString(),
                    });
                    return RedirectToAction("ClubList", "ClubManagement", new { SearchFilter = Model.SearchFilter, StartIndex = Model.StartIndex, PageSize = Model.PageSize });
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["ManageManagerModel"] = Model;
                    TempData["RenderId"] = "ManageManager";
                    return RedirectToAction("ClubList", "ClubManagement", new { SearchFilter = Model.SearchFilter, StartIndex = Model.StartIndex, PageSize = Model.PageSize });
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
            TempData["ManageManagerModel"] = Model;
            TempData["RenderId"] = "ManageManager";
            return RedirectToAction("ClubList", "ClubManagement", new { SearchFilter = Model.SearchFilter, StartIndex = Model.StartIndex, PageSize = Model.PageSize });
        }

        #endregion
        #region "Tag Management"
        [HttpGet]
        public ActionResult ManageTag(string ClubId = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            var ResponseModel = new ManageTag();
            //var availabilityInfo = new List<AvailabilityTagModel>();
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            var cId = ClubId?.DecryptParameter();
            if (string.IsNullOrEmpty(cId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ClubList", "ClubManagement", new { SearchFilter = SearchFilter, StartIndex = StartIndex, PageSize = PageSize });
            }
            else
            {
                var dbResponseInfo = _BUSS.GetTagDetails(cId);
                var dbAvailabilityInfo = _BUSS.GetAvailabilityList(cId, culture);

                if (dbResponseInfo == null || dbResponseInfo.Code != "0")
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponseInfo.Message ?? "Invalid details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("ClubList", "ClubManagement", new { SearchFilter = SearchFilter, StartIndex = StartIndex, PageSize = PageSize });
                }

                ResponseModel = dbResponseInfo.MapObject<ManageTag>();
                ResponseModel.ClubId = ClubId;
                ResponseModel.TagId = ResponseModel.TagId.EncryptParameter();
                ResponseModel.Tag1Location = ResponseModel.Tag1Location?.EncryptParameter();
                ResponseModel.Tag2RankName = ResponseModel.Tag2RankName;
                ResponseModel.Tag3CategoryName = ResponseModel.Tag3CategoryName?.EncryptParameter();
                ResponseModel.Tag5StoreName = ResponseModel.Tag5StoreName?.EncryptParameter();
                ResponseModel.GetAvailabilityTagModel = dbAvailabilityInfo.MapObjects<AvailabilityTagModel>();
                ResponseModel.SearchFilter = SearchFilter;
                ResponseModel.StartIndex = StartIndex;
                ResponseModel.PageSize = PageSize;
                TempData["ManageTagModel"] = ResponseModel;
                TempData["AvailabilityModel"] = ResponseModel.GetAvailabilityTagModel;
                TempData["RenderId"] = "ManageTag";
                return RedirectToAction("ClubList", "ClubManagement", new { SearchFilter = SearchFilter, StartIndex = StartIndex, PageSize = PageSize });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageTag(ManageTag Model, string tag1Select, string tag2Select, string tag3Select, string tag5Select, FormCollection form = null)
        {
            string[] updatedValues = form.GetValues("checkedValues");
            var Requests = new AvailabilityTagModelCommon();
            Requests.StaticType = "36";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            var ClubId = !string.IsNullOrEmpty(Model.ClubId) ? Model.ClubId.DecryptParameter() : null;
            var TagId = !string.IsNullOrEmpty(Model.TagId) ? Model.TagId.DecryptParameter() : null;
            var dbAvailabilityInfo = _BUSS.GetAvailabilityList(ClubId, culture);
            Model.GetAvailabilityTagModel = dbAvailabilityInfo.MapObjects<AvailabilityTagModel>();
            Model.Tag1Location = tag1Select;
            Model.Tag3CategoryName = tag3Select;
            Model.Tag5StoreName = tag5Select;
            if (string.IsNullOrEmpty(ClubId) || string.IsNullOrEmpty(TagId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ClubList", "ClubManagement", new { SearchFilter = Model.SearchFilter, StartIndex = Model.StartIndex, PageSize = Model.PageSize });
            }
            if (ModelState.IsValid)
            {
                var dbRequest = Model.MapObject<ManageTagCommon>();
                dbRequest.ClubId = ClubId;
                dbRequest.TagId = TagId;
                dbRequest.Tag1Status = string.IsNullOrEmpty(dbRequest.Tag1Status) ? "B" : dbRequest.Tag1Status;
                dbRequest.Tag1Location = !string.IsNullOrEmpty(dbRequest.Tag1Status) ? tag1Select?.DecryptParameter() : null;
                dbRequest.Tag2Status = string.IsNullOrEmpty(dbRequest.Tag2Status) ? "B" : dbRequest.Tag2Status;
                dbRequest.Tag2RankName = Convert.ToString(dbRequest.Tag2Status) == "B" ? null : dbRequest.Tag2RankName;
                dbRequest.Tag3Status = string.IsNullOrEmpty(dbRequest.Tag3Status) ? "B" : dbRequest.Tag3Status;
                dbRequest.Tag3CategoryName = !string.IsNullOrEmpty(dbRequest.Tag3Status) ? tag3Select?.DecryptParameter() : null;
                dbRequest.Tag4Status = string.IsNullOrEmpty(dbRequest.Tag4Status) ? "B" : dbRequest.Tag4Status;
                dbRequest.Tag5Status = string.IsNullOrEmpty(dbRequest.Tag5Status) ? "B" : dbRequest.Tag5Status;
                dbRequest.Tag5StoreName = !string.IsNullOrEmpty(dbRequest.Tag5Status) ? tag5Select?.DecryptParameter() : null;
                dbRequest.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                dbRequest.ActionIP = ApplicationUtilities.GetIP();
                var dbResponse = _BUSS.ManageTag(dbRequest);
                if (updatedValues != null)
                {
                    var dbAvailability = _BUSS.ManageClubAvailability(Requests, dbRequest, updatedValues);
                    Model.GetAvailabilityTagModel = dbAvailabilityInfo.MapObjects<AvailabilityTagModel>();
                }
                if (dbResponse != null && dbResponse.Code == 0)
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = dbResponse.Message ?? "Success",
                        Title = NotificationMessage.SUCCESS.ToString(),
                    });
                    return RedirectToAction("ClubList", "ClubManagement", new { SearchFilter = Model.SearchFilter, StartIndex = Model.StartIndex, PageSize = Model.PageSize });
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
                    TempData["AvailabilityModel"] = Model.GetAvailabilityTagModel;
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
            TempData["AvailabilityModel"] = Model.GetAvailabilityTagModel;
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
                item.ImagePath = ImageHelper.ProcessedImage(item.ImagePath);
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
        public async Task<ActionResult> ManageGallery(ManageGalleryImageModel Model, HttpPostedFileBase Image_Path)
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
            string fileName = string.Empty;
            var allowedContentType = AllowedImageContentType();
            if (Image_Path != null)
            {
                var contentType = Image_Path.ContentType;
                var ext = Path.GetExtension(Image_Path.FileName);
                if (allowedContentType.Contains(contentType.ToLower()))
                {
                    fileName = $"{AWSBucketFolderNameModel.CLUB}/ClubGallery_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
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
                if (Image_Path != null) await ImageHelper.ImageUpload(fileName, Image_Path);
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
        public ActionResult EventList(string ClubId = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            ViewBag.SearchFilter = null;
            Session["CurrentURL"] = "/ClubManagement/EventList";
            string RenderId = "";
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/ClubManagement/ClubList";
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
                item.Image = ImageHelper.ProcessedImage(item.Image);
            }
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.EventType = ApplicationUtilities.LoadDropdownValuesList("EVENTTYPECLUB", "", "", culture);
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            response.SearchFilter = null;
            response.ManageEventModel.AgentId = ClubId;

            return View(response);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageEvent(EventModel Model, HttpPostedFileBase Image)
        {
            var agentid = string.Empty;
            string fileName = string.Empty;
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
                //Model.Title = "";
                Model.Image = "";
                //ModelState.Remove("Title");
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
                    else if (string.IsNullOrEmpty(Model.Title))
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
                if (Image != null)
                {
                    var contentType = Image.ContentType;
                    var ext = Path.GetExtension(Image.FileName);
                    if (allowedContentType.Contains(contentType.ToLower()))
                    {
                        fileName = $"{AWSBucketFolderNameModel.CLUB}/EventImage_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                        Model.Image = $"/{fileName}";
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
                if (Model.EventId != null)
                {
                    commonModel.EventId = Model.EventId.DecryptParameter();
                }
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                var dbResponse = _BUSS.ManageEvent(commonModel);
                if (dbResponse != null && dbResponse.Code == 0)
                {
                    if (Image != null) await ImageHelper.ImageUpload(fileName, Image);

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
        public ActionResult ManageEvent(string AgentId = "", string EventId = "")
        {
            EventModel model = new EventModel();
            var agentid = AgentId;
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            ViewBag.EventType = ApplicationUtilities.LoadDropdownValuesList("EVENTTYPECLUB", "", "", culture);
            if (!string.IsNullOrEmpty(AgentId))
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
                            ClubId = agentid
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
                    var dbResponse = _BUSS.GetEventDetails(id, Eventid);
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
            EventCommon commonModel = new EventCommon();
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