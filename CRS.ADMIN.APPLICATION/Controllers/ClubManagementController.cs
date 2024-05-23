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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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
        public ActionResult ClubList(string SearchFilter = "", string value = "", int StartIndex = 0, int PageSize = 10, int StartIndex2 = 0, int PageSize2 = 10, int StartIndex3 = 0, int PageSize3 = 10)
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
            PaginationFilterCommon dbRequestall = new PaginationFilterCommon()
            {
                Skip = value == "" ? StartIndex : 0,
                Take = value == "" ? PageSize : 10,
                SearchFilter = value == "" ? !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null : null
            };

            PaginationFilterCommon dbRequestpending = new PaginationFilterCommon()
            {
                Skip = StartIndex2,
                Take = PageSize2,
                SearchFilter = value == "p" ? !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null : null
            };
            PaginationFilterCommon dbRequestreject = new PaginationFilterCommon()
            {
                Skip = StartIndex3,
                Take = PageSize3,
                SearchFilter = value == "r" ? !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null : null
            };
            var dbResponse = _BUSS.GetClubList(dbRequestall);
            var dbResponsePending = _BUSS.GetClubPendingList(dbRequestpending);
            var dbResponseRejected = _BUSS.GetClubRejectedList(dbRequestreject);
            ViewBag.PlansList = ApplicationUtilities.LoadDropdownList("CLUBPLANS") as Dictionary<string, string>;
            if (TempData.ContainsKey("EditPlan"))
            {

                response.ManageClubModel = TempData["EditPlan"] as ManageClubModel;
                int plancount = response.ManageClubModel.PlanDetailList.Count;
                if (plancount == 0)
                {
                    List<PlanListCommon> planlist = _BUSS.GetClubPlanIdentityList(culture);
                    response.ManageClubModel.PlanDetailList = planlist.MapObjects<PlanList>();

                    response.ManageClubModel.PlanDetailList.ForEach(planList =>
                    {
                        planList.PlanIdentityList.ForEach(planIdentity =>
                        {
                            planIdentity.StaticDataValue = planIdentity.StaticDataValue.EncryptParameter(); // Call your encryption method here
                        });
                    });
                }
            }

            else
            {
                int plancount = response.ManageClubModel.PlanDetailList.Count;
                if (plancount == 0)
                {
                    List<PlanListCommon> planlist = _BUSS.GetClubPlanIdentityList(culture);
                    response.ManageClubModel.PlanDetailList = planlist.MapObjects<PlanList>();
                    response.ManageClubModel.PlanDetailList.ForEach(planList =>
                    {
                        planList.PlanIdentityList.ForEach(planIdentity =>
                        {
                            planIdentity.StaticDataValue = planIdentity.StaticDataValue.EncryptParameter(); // Call your encryption method here
                            planIdentity.IdentityDescription = planIdentity.name.ToLower() == "plan" ? planIdentity.IdentityDescription.EncryptParameter() : planIdentity.IdentityDescription; // Call your encryption method here
                            planIdentity.PlanId = planIdentity.name.ToLower() == "plan" ? ViewBag.PlansList[planIdentity.IdentityDescription] : planIdentity.IdentityDescription;  // Call your encryption method here
                            planIdentity.IdentityDescription = planIdentity.name.ToLower() == "status" ?( string.IsNullOrEmpty(planIdentity.IdentityDescription)? "B" : planIdentity.IdentityDescription) : planIdentity.IdentityDescription;  // Call your encryption method here

                        });
                    });
                }
            }

            response.ClubListModel = dbResponse.MapObjects<ClubListModel>();
            response.ClubPendingListModel = dbResponsePending.MapObjects<ClubListModel>();
            response.ClubRejectedListModel = dbResponseRejected.MapObjects<ClubListModel>();
            foreach (var item in response.ClubListModel)
            {
                item.AgentId = item.AgentId?.EncryptParameter();
                item.ClubLogo = ImageHelper.ProcessedImage(item.ClubLogo);
            }
            response.ClubPendingListModel.ForEach(item =>
            {
                item.SNO = item.SNO?.EncryptParameter();
                item.ClubLogo = ImageHelper.ProcessedImage(item.ClubLogo);
            });
            response.ClubPendingListModel.ForEach(item => item.AgentId = item.AgentId?.EncryptParameter());
            response.ClubRejectedListModel.ForEach(item =>
            {
                item.SNO = item.SNO?.EncryptParameter();
                item.ClubLogo = ImageHelper.ProcessedImage(item.ClubLogo);
            });
            ViewBag.Pref = DDLHelper.LoadDropdownList("PREF") as Dictionary<string, string>;
            ViewBag.Holiday = ApplicationUtilities.SetDDLValue(DDLHelper.LoadDropdownList("Holiday") as Dictionary<string, string>, null, "--- Select ---");

            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.LocationDDLList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("LOCATIONDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.BusinessTypeDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("BUSINESSTYPEDDL") as Dictionary<string, string>, null, "");
            ViewBag.RankDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("RANKDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.ClubStoreDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBSTOREDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.ClubCategoryDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBCATEGORYDDL") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.RankDDLKey = response.ManageTag.Tag2RankName;
            ViewBag.ClubStoreDDLKey = response.ManageTag.Tag5StoreName;
            ViewBag.ClubCategoryDDLKey = response.ManageTag.Tag3CategoryName;
            ViewBag.LocationIdKey = response.ManageClubModel.LocationDDL;

            ViewBag.PrefIdKey = !string.IsNullOrEmpty(response.ManageClubModel.Prefecture) ? ViewBag.Pref[response.ManageClubModel.Prefecture] : null;
            ViewBag.HolidayIdKey = !string.IsNullOrEmpty(response.ManageClubModel.Holiday) ? response.ManageClubModel.Holiday : null;
            ViewBag.BusinessTypeKey = response.ManageClubModel.BusinessTypeDDL;
            ViewBag.ClosingDate = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLOSINGDATE") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.ClosingDateIdKey = response.ManageClubModel.ClosingDate;

            ViewBag.StartIndex2 = StartIndex2;
            ViewBag.PageSize2 = PageSize2;
            ViewBag.StartIndex3 = StartIndex3;
            ViewBag.PageSize3 = PageSize3;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;

            response.ListType = value;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            ViewBag.TotalData2 = dbResponsePending != null && dbResponsePending.Any() ? dbResponsePending[0].TotalRecords : 0;
            ViewBag.TotalData3 = dbResponseRejected != null && dbResponseRejected.Any() ? dbResponseRejected[0].TotalRecords : 0;
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
                var dbResponse = _BUSS.GetClubDetails(id, culture);
                List<PlanListCommon> planlist = new List<PlanListCommon>(dbResponse.PlanDetailList);
                var i = 0;
                foreach (var planDetail in planlist)
                {
                    // Filter the PlanIdentityList based on the condition where PlanStatus is not equal to "B"
                    var filteredPlanIdentityList = planDetail.PlanIdentityList
                          .Where(planIdentity => planIdentity.PlanStatus != "B")
                          .ToList();
                    if (filteredPlanIdentityList.Count>0)
                    {
                        var distinctPlanListIds = filteredPlanIdentityList
                                                .Select(planIdentity => planIdentity.PlanListId)
                                                .Distinct()
                                                .ToList();

                        // Filter the list again to remove elements with PlanStatus equal to "B" and whose PlanListId matches any of the distinct PlanListId values
                        planDetail.PlanIdentityList = filteredPlanIdentityList
                            .Where(planIdentity => !planIdentity.PlanListId.Contains("B") || !distinctPlanListIds.Contains(planIdentity.PlanListId))
                            .ToList();
                        i++;
                    }
                    else if (planDetail.PlanIdentityList.Any(planIdentity => planIdentity.PlanStatus == "B"))
                    {
                        dbResponse.PlanDetailList.RemoveAt(i);
                        
                    }

                    
                }

                model = dbResponse.MapObject<ManageClubModel>();

                ViewBag.PlansList = ApplicationUtilities.LoadDropdownList("CLUBPLANS") as Dictionary<string, string>;

                model.PlanDetailList.ForEach(planList =>
                {
                    planList.PlanIdentityList.ForEach(planIdentity =>
                    {
                        // Encrypt specific properties                        
                        planIdentity.StaticDataValue = planIdentity.StaticDataValue.EncryptParameter(); // Call your encryption method here                                      
                        planIdentity.IdentityDescription = planIdentity.name.ToLower() == "plan" ? planIdentity.IdentityDescription.EncryptParameter() : planIdentity.IdentityDescription; // Call your encryption method here
                        planIdentity.PlanId = planIdentity.name.ToLower() == "plan" ? ViewBag.PlansList[planIdentity.IdentityDescription] : planIdentity.IdentityDescription;  // Call your encryption method here

                    });
                });

                model.AgentId = model.AgentId.EncryptParameter();
                //model.LocationId = !string.IsNullOrEmpty(model.LocationId) ? model.LocationId.EncryptParameter() : null;
                //model.BusinessType = !string.IsNullOrEmpty(model.BusinessType) ? model.BusinessType.EncryptParameter() : null;
                model.LocationDDL = !string.IsNullOrEmpty(model.LocationId) ? model.LocationId.EncryptParameter() : null;
                model.BusinessTypeDDL = !string.IsNullOrEmpty(model.BusinessType) ? model.BusinessType.EncryptParameter() : null;
                model.Prefecture = !string.IsNullOrEmpty(model.Prefecture) ? model.Prefecture.EncryptParameter() : null;
                model.Holiday = !string.IsNullOrEmpty(model.Holiday) ? model.Holiday.EncryptParameter() : null;
                model.ClosingDate = !string.IsNullOrEmpty(model.ClosingDate) ? model.ClosingDate.EncryptParameter() : null;
                //model.holdId = !string.IsNullOrEmpty(model.holdId) ? model.holdId.EncryptParameter() : null;
            }
            TempData["ManageClubModel"] = model;
            TempData["RenderId"] = "Manage";
            TempData["EditPlan"] = model;

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
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            var dbResponse = _BUSS.GetClubDetails(id, culture);

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
        public ActionResult ManagePendingClub(string AgentId = "", string holdId = "")
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
                    return RedirectToAction("ClubList", "ClubManagement");
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
                return RedirectToAction("ClubList", "ClubManagement");
            }
            var dbResponse = _BUSS.GetplanPendingDetails(id, holdid, culture);
            List<PlanListCommon> planlist = new List<PlanListCommon>(dbResponse.PlanDetailList);
            var i = 0;
            foreach (var planDetail in planlist)
            {
                // Filter the PlanIdentityList based on the condition where PlanStatus is not equal to "B"
                var filteredPlanIdentityList = planDetail.PlanIdentityList
                        .Where(planIdentity => planIdentity.PlanStatus != "B")
                        .ToList();
                if (filteredPlanIdentityList.Count > 0)
                {
                    var distinctPlanListIds = filteredPlanIdentityList
                                            .Select(planIdentity => planIdentity.PlanListId)
                                            .Distinct()
                                            .ToList();

                    // Filter the list again to remove elements with PlanStatus equal to "B" and whose PlanListId matches any of the distinct PlanListId values
                    planDetail.PlanIdentityList = filteredPlanIdentityList
                        .Where(planIdentity => !planIdentity.PlanListId.Contains("B") || !distinctPlanListIds.Contains(planIdentity.PlanListId))
                        .ToList();
                    i++;
                }
                else if (planDetail.PlanIdentityList.Any(planIdentity => planIdentity.PlanStatus == "B"))
                {
                    dbResponse.PlanDetailList.RemoveAt(i);

                }
            }
                model = dbResponse.MapObject<ManageClubModel>();

            ViewBag.PlansList = ApplicationUtilities.LoadDropdownList("CLUBPLANS") as Dictionary<string, string>;

            model.PlanDetailList.ForEach(planList =>
            {
                planList.PlanIdentityList.ForEach(planIdentity =>
                {
                    // Encrypt specific properties                        
                    planIdentity.StaticDataValue = planIdentity.StaticDataValue.EncryptParameter(); // Call your encryption method here                                      
                    planIdentity.IdentityDescription = planIdentity.name.ToLower() == "plan" ? planIdentity.IdentityDescription.EncryptParameter() : planIdentity.IdentityDescription; // Call your encryption method here
                    planIdentity.PlanId = planIdentity.name.ToLower() == "plan" ? ViewBag.PlansList[planIdentity.IdentityDescription] : planIdentity.IdentityDescription;  // Call your encryption method here

                });
            });

            model.AgentId = model.AgentId.EncryptParameter();
            model.LocationDDL = !string.IsNullOrEmpty(model.LocationId) ? model.LocationId.EncryptParameter() : null;
            model.BusinessTypeDDL = !string.IsNullOrEmpty(model.BusinessType) ? model.BusinessType.EncryptParameter() : null;
            model.Prefecture = !string.IsNullOrEmpty(model.Prefecture) ? model.Prefecture.EncryptParameter() : null;
            model.Holiday = !string.IsNullOrEmpty(model.Holiday) ? model.Holiday.EncryptParameter() : null;
            model.ClosingDate = !string.IsNullOrEmpty(model.ClosingDate) ? model.ClosingDate.EncryptParameter() : null;
            model.holdId = !string.IsNullOrEmpty(model.holdId) ? model.holdId.EncryptParameter() : null;
            //}
            TempData["ManageClubModel"] = model;
            TempData["RenderId"] = "Manage";
            TempData["EditPlan"] = model;

            return RedirectToAction("ClubList", "ClubManagement");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageClub(ManageClubModel Model, HttpPostedFileBase Business_Certificate, HttpPostedFileBase Logo_Certificate, HttpPostedFileBase CoverPhoto_Certificate, HttpPostedFileBase KYCDocument_Certificate, string LocationDDL, string BusinessTypeDDL)
       {
            string ErrorMessage = string.Empty;
            //if (!string.IsNullOrEmpty(BusinessTypeDDL?.DecryptParameter())) ModelState.Remove("BusinessType");
            //if (!string.IsNullOrEmpty(LocationDDL?.DecryptParameter())) ModelState.Remove("LocationId");
            ViewBag.PlansList = ApplicationUtilities.LoadDropdownList("CLUBPLANS") as Dictionary<string, string>;
            ModelState.Remove("LocationURL");
            string concatenateplanvalue = string.Empty;

            bool isduplicate = false;
            Model.PlanDetailList.ForEach(planList =>
            {
                concatenateplanvalue += ", ";
                planList.PlanIdentityList.ForEach(planIdentity =>
                {

                    planIdentity.PlanId = planIdentity.name.ToLower() == "plan" ? ViewBag.PlansList[planIdentity.IdentityDescription] : planIdentity.IdentityDescription;  // Call your encryption method here

                    if (planIdentity.name.ToLower() == "plan")
                    {

                        if (concatenateplanvalue.Contains(planIdentity.IdentityDescription.DecryptParameter()))
                        {
                            isduplicate = true;
                        }
                        concatenateplanvalue += planIdentity.IdentityDescription.DecryptParameter();
                    }

                });
            });

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
                if (string.IsNullOrEmpty(Model.Representative2_ContactName))
                {
                    ModelState.AddModelError("Representative2_ContactName", "Required");
                }
                if (string.IsNullOrEmpty(Model.Representative2_MobileNo))
                {
                    ModelState.AddModelError("Representative2_MobileNo", "Required");
                }
                if (string.IsNullOrEmpty(Model.Representative2_Email))
                {
                    ModelState.AddModelError("Representative2_Email", "Required");
                }
                                
            }
            else
            {
                //ModelState.Remove("Representative1_ContactName");
                //ModelState.Remove("Representative1_MobileNo");
                //ModelState.Remove("Representative1_Email");
                //ModelState.Remove("Representative2_ContactName");
                //ModelState.Remove("Representative2_MobileNo");
                //ModelState.Remove("Representative2_Email");
                ModelState.Remove("CompanyName");
            }
            //ModelState.Remove("LoginId");
            if (ModelState.IsValid)
            {
                if (isduplicate == true)
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Duplicate plan name.",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });

                    TempData["ManageClubModel"] = Model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("ClubList", "ClubManagement");
                }
                if
           (
              string.IsNullOrEmpty(Model.BusinessCertificate) ||
              string.IsNullOrEmpty(Model.KYCDocument) ||
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
                        else if (KYCDocument_Certificate == null && string.IsNullOrEmpty(Model.KYCDocument))
                        {
                            ErrorMessage = "KYC document required";
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
                string KYCDocumentFileName = string.Empty;
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
                    //Model.BusinessCertificate =  Business_Certificate.FileName;
                    //Model.CoverPhoto =  CoverPhoto_Certificate.FileName;
                    //Model.Logo=  Logo_Certificate.FileName;
                    TempData["ManageClubModel"] = Model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("ClubList", "ClubManagement");
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
                        return RedirectToAction("ClubList", "ClubManagement");
                    }
                }
                commonModel.LocationId = LocationDDL?.DecryptParameter();
                commonModel.BusinessType = BusinessTypeDDL?.DecryptParameter();
                commonModel.Holiday = commonModel.Holiday?.DecryptParameter();
                commonModel.Prefecture = commonModel.Prefecture?.DecryptParameter();
                commonModel.ClosingDate = commonModel.ClosingDate?.DecryptParameter();
                var returntype = string.Empty;


                commonModel.PlanDetailList.ForEach(planList =>
                {
                    planList.PlanIdentityList.ForEach(planIdentity =>
                    {
                        string decryptedDescription = planIdentity.name.ToLower() == "plan" ? planIdentity.IdentityDescription.DecryptParameter() : planIdentity.IdentityDescription;
                        planIdentity.StaticDataValue = planIdentity.StaticDataValue.DecryptParameter();
                        planIdentity.IdentityDescription = planIdentity.name.ToLower() == "plan" ? decryptedDescription : planIdentity.IdentityDescription;

                    });
                });

                var blockplanlistid = 1;


                //for (int i = 0; i < commonModel.PlanDetailList.Count; i++)
                //{
                //    if (i == blockplanlistid)
                //    {
                //        List<planIdentityDataCommon> listForRow2 = new List<planIdentityDataCommon>();

                //        planIdentityDataCommon item1ForRow2 = new planIdentityDataCommon
                //        {
                //            English = "EnglishValue1",
                //            StaticDataValue = "StaticDataValue1",
                //            japanese = "JapaneseValue1",
                //            inputtype = "InputTypeValue1",
                //            name = "NameValue1",
                //            IdentityLabel = "IdentityLabelValue1",
                //            IdentityDescription = "IdentityDescriptionValue1",
                //            PlanListId = "PlanListIdValue1",
                //            Id = "IdValue1",
                //            PlanId = "PlanIdValue1"
                //        };

                //        listForRow2.Add(item1ForRow2);

                //        commonModel.PlanDetailList[blockplanlistid].PlanIdentityList.Insert(blockplanlistid - 1, listForRow2);

                //    }
                //    else
                //    {
                //        commonModel.PlanDetailList[i].PlanIdentityList.ForEach(planIdentity =>
                //        {
                //            string decryptedDescription = planIdentity.name.ToLower() == "plan" ? planIdentity.IdentityDescription.DecryptParameter() : planIdentity.IdentityDescription;
                //            planIdentity.StaticDataValue = planIdentity.StaticDataValue.DecryptParameter();
                //            planIdentity.IdentityDescription = planIdentity.name.ToLower() == "plan" ? decryptedDescription : planIdentity.IdentityDescription;
                //        });
                //    }
                //}

                var dbResponse = _BUSS.ManageClub(commonModel);
                if (dbResponse != null && dbResponse.Code == 0)
                {
                    if (Business_Certificate != null) await ImageHelper.ImageUpload(businessCertificateFileName, Business_Certificate);
                    if (Logo_Certificate != null) await ImageHelper.ImageUpload(LogoFileName, Logo_Certificate);
                    if (CoverPhoto_Certificate != null) await ImageHelper.ImageUpload(CoverPhotoFileName, CoverPhoto_Certificate);
                    if (KYCDocument_Certificate != null) await ImageHelper.ImageUpload(KYCDocumentFileName, KYCDocument_Certificate);
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
        public ActionResult ApproveRejectClub(string holdid, string type, string AgentId)
        {
            ClubDetailModel response = new ClubDetailModel();
            var id = holdid.DecryptParameter();
            if (string.IsNullOrEmpty(id))
            {
                this.ShowPopup(1, "Invalid club details");
                return RedirectToAction("ClubList");

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
            return RedirectToAction("ClubList", "ClubManagement");
        }

        [HttpGet]
        [OverrideActionFilters]
        public ActionResult ClubHoldDetails(string Holdid = "")
        {
            var ResponseModel = new ManageClubModel();
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
                return RedirectToAction("ClubList", "ClubManagement");
            }
            else
            {

                var dbResponseInfo = _BUSS.GetClubPendingDetails("", Id, "");

                //var dbAvailabilityInfo = _BUSS.GetAvailabilityList(cId);
                ResponseModel = dbResponseInfo.MapObject<ManageClubModel>();
                ResponseModel.LocationDDL = !string.IsNullOrEmpty(ResponseModel.LocationId) ? ResponseModel.LocationId.EncryptParameter() : null;
                ResponseModel.BusinessTypeDDL = !string.IsNullOrEmpty(ResponseModel.BusinessType) ? ResponseModel.BusinessType.EncryptParameter() : null;
                ResponseModel.Prefecture = !string.IsNullOrEmpty(ResponseModel.Prefecture) ? ResponseModel.Prefecture.EncryptParameter() : null;
                //ViewBag.Pref = DDLHelper.LoadDropdownList("PREF") as Dictionary<string, string>;
                ViewBag.LocationDDLList = ApplicationUtilities.LoadDropdownList("LOCATIONDDL") as Dictionary<string, string>;
                ViewBag.BusinessTypeDDL = ApplicationUtilities.LoadDropdownList("BUSINESSTYPEDDL") as Dictionary<string, string>;



                ViewBag.Pref = DDLHelper.LoadDropdownList("PREF") as Dictionary<string, string>;
                var value=string.Empty;
                ResponseModel.Prefecture = (ResponseModel.Prefecture != null && ViewBag.Pref?.TryGetValue(ResponseModel.Prefecture, out value)) ? value : "";

                //ResponseModel.Prefecture = ViewBag.Pref.ContainsKey(ResponseModel.Prefecture) ? ViewBag.Pref[ResponseModel.Prefecture] : "";
                ResponseModel.LocationDDL = ViewBag.LocationDDLList.ContainsKey(ResponseModel.LocationDDL) ? ViewBag.LocationDDLList[ResponseModel.LocationDDL] : "";
                ViewBag.BusinessTypeDDL = ApplicationUtilities.LoadDropdownList("BUSINESSTYPEDDL") as Dictionary<string, string>;

                ResponseModel.BusinessTypeDDL = (ResponseModel.BusinessTypeDDL != null && ViewBag.BusinessTypeDDL?.ContainsKey(ResponseModel.BusinessTypeDDL) == true)? ViewBag.BusinessTypeDDL[ResponseModel.BusinessTypeDDL]:"";

                //ResponseModel.BusinessTypeDDL = ViewBag.BusinessTypeDDL.ContainsKey(ResponseModel.BusinessTypeDDL) ? ViewBag.BusinessTypeDDL[ResponseModel.BusinessTypeDDL] : "";
                TempData["ClubHoldDetails"] = ResponseModel;
                TempData["RenderId"] = "ClubHoldDetails";
                return RedirectToAction("ClubList", "ClubManagement", new { value = 'p' }

                    );
            }
        }
        #region Manage Manager
        [HttpGet]
        public ActionResult ManageManager(string AgentId = "")
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
                    return RedirectToAction("ClubList", "ClubManagement");
                }

                var dbResponse = _BUSS.GetManagerDetails(id);
                model = dbResponse.MapObject<ManageManagerModel>();
                model.ManagerId = model.ManagerId.EncryptParameter();
                model.ClubId = AgentId;
            }
            TempData["ManageManagerModel"] = model;
            TempData["RenderId"] = "ManageManager";

            return RedirectToAction("ClubList", "ClubManagement");
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
                return RedirectToAction("ClubList", "ClubManagement");
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
                    TempData["ManageManagerModel"] = Model;
                    TempData["RenderId"] = "ManageManager";
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
            TempData["ManageManagerModel"] = Model;
            TempData["RenderId"] = "ManageManager";
            return RedirectToAction("ClubList", "ClubManagement");
        }

        #endregion
        #region "Tag Management"
        [HttpGet]
        public ActionResult ManageTag(string ClubId = "")
        {
            var ResponseModel = new ManageTag();
            //var availabilityInfo = new List<AvailabilityTagModel>();
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
                var dbAvailabilityInfo = _BUSS.GetAvailabilityList(cId);
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
                ResponseModel.Tag2RankName = ResponseModel.Tag2RankName;
                ResponseModel.Tag3CategoryName = ResponseModel.Tag3CategoryName?.EncryptParameter();
                ResponseModel.Tag5StoreName = ResponseModel.Tag5StoreName?.EncryptParameter();
                ResponseModel.GetAvailabilityTagModel = dbAvailabilityInfo.MapObjects<AvailabilityTagModel>();
                TempData["ManageTagModel"] = ResponseModel;
                TempData["AvailabilityModel"] = ResponseModel.GetAvailabilityTagModel;
                TempData["RenderId"] = "ManageTag";
                return RedirectToAction("ClubList", "ClubManagement");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageTag(ManageTag Model, string tag1Select, string tag2Select, string tag3Select, string tag5Select, FormCollection form = null)
        {
            string[] updatedValues = form.GetValues("checkedValues");
            var Request = new AvailabilityTagModelCommon();
            Request.StaticType = "36";
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
                //dbRequest.Tag2RankName = !string.IsNullOrEmpty(dbRequest.Tag2Status) ? "B": dbRequest.Tag2RankName;
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
                    var dbAvailability = _BUSS.ManageClubAvailability(Request, dbRequest, updatedValues);
                }
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