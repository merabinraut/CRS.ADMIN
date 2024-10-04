using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models;
using CRS.ADMIN.APPLICATION.Models.BasicClubManagement;
using CRS.ADMIN.APPLICATION.Models.ClubManagement;
using CRS.ADMIN.BUSINESS.BasicClubManagement;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Google.Apis.Requests.BatchRequest;
using System.IO;
using CRS.ADMIN.SHARED.BasicClubManagement;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class BasicClubManagementController : BaseController
    {
        private readonly IBasicClubManagementBusiness _buss;
        public BasicClubManagementController(IBasicClubManagementBusiness buss)
        {
            _buss = buss;
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
            var dbResponse = _buss.GetBasicClubList(dbRequest);
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            ViewBag.ClosingDate = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLOSINGDATE", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.ClosingDateIdKey = objBasicClubManagementModel.ManageBasicClub.ClosingDate;
            ViewBag.Holiday = ApplicationUtilities.SetDDLValue(DDLHelper.LoadDropdownList("Holiday", "", culture) as Dictionary<string, string>, null, "");
            ViewBag.HolidayIdKey = !string.IsNullOrEmpty(objBasicClubManagementModel.ManageBasicClub.Holiday) ? objBasicClubManagementModel.ManageBasicClub.Holiday : null;
            ViewBag.LocationDDLList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("LOCATIONDDLPREFECTURE", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.LocationIdKey = objBasicClubManagementModel.ManageBasicClub.LocationDDL;
            objBasicClubManagementModel.SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null;
            return View(objBasicClubManagementModel);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageBasicClub(ManageBasicClubModel Model, HttpPostedFileBase Logo_Certificate, HttpPostedFileBase CoverPhoto_Certificate,   string LocationDDL, string BusinessTypeDDL)
        {
            string holidays = "";
            string[] array = Model.HolidayStr;
            string commaSeparatedString = string.Join(", ", array);
            List<string> holidayList = commaSeparatedString.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            ActionResult redirectresult = null;           
            redirectresult = RedirectToAction("BasicClubManagementList", "BasicClubManagement", new
            {                    
                SearchFilter = Model.SearchFilter,
                StartIndex = Model.StartIndex,
                PageSize = Model.PageSize
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
                    if ( Logo_Certificate == null || CoverPhoto_Certificate == null)
                    {                       
                        if (Logo_Certificate == null && string.IsNullOrEmpty(Model.Logo))
                        {
                            ErrorMessage = "Logo required";
                            allowRedirect = true;
                        }
                        else if (CoverPhoto_Certificate == null && string.IsNullOrEmpty(Model.CoverPhoto))
                        {
                            ErrorMessage = "Cover photo required";
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


        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DeleteBasicClub(string AgentId)
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
            var dbResponse = _buss.ManageBasicClubStatus(aId, status, commonRequest);
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
        public JsonResult BlockBasicClub(string AgentId)
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
            var dbResponse = _buss.ManageBasicClubStatus(aId, status, commonRequest);
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
        public ActionResult UnBlockBasicClub(string AgentId, string status)
        {
            var response = new CommonDbResponse();
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId)) response = new CommonDbResponse { ErrorCode = 1, Message = "Invalid details" };
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            var dbResponse = _buss.ManageBasicClubStatus(aId, status, commonRequest);
            response = dbResponse;
            return Json(response.SetMessageInTempData(this));
        }

    }
}
