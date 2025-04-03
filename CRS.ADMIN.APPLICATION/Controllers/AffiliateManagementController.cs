using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Middleware;
using CRS.ADMIN.APPLICATION.Models.AffiliateManagement;
using CRS.ADMIN.APPLICATION.Models.ClubManagement;
using CRS.ADMIN.BUSINESS.AffiliateManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.AffiliateManagement;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using static Google.Apis.Requests.BatchRequest;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class AffiliateManagementController : BaseController
    {
        private readonly IAffiliateManagementBusiness _affiliateBuss;
        private readonly AmazonCognitoMiddleware _amazonCognitoMiddleware;
        public AffiliateManagementController(IAffiliateManagementBusiness affiliateBuss, AmazonCognitoMiddleware amazonCognitoMiddleware)
        {
            _affiliateBuss = affiliateBuss;
            _amazonCognitoMiddleware = amazonCognitoMiddleware;
            _amazonCognitoMiddleware.SetConfigNameViaUserType("affiliate");
        }
        public ActionResult Index(string SearchFilter1 = "", string SearchFilter2 = "", string value = "", int StartIndex = 0, int PageSize = 10, int StartIndex2 = 0, int PageSize2 = 10)
        {
            Session["CurrentURL"] = "/AffiliateManagement/Index";
            var ResponseModel = new ReferalCommonModel();
            var dbAffiliateListRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = SearchFilter1
            };
            string RenderId = "";
            if (TempData.ContainsKey("ManageAffiliateModel")) ResponseModel.ManageAffiliate = TempData["ManageAffiliateModel"] as ManageAffiliateModel;
            else ResponseModel.ManageAffiliate = new ManageAffiliateModel();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.BusinessTypeDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("BUSINESSTYPEDDL") as Dictionary<string, string>, null, "");
            //ViewBag.Pref = DDLHelper.LoadDropdownList("PREF") as Dictionary<string, string>;
            //ViewBag.PrefIdKey = !string.IsNullOrEmpty(ResponseModel.ManageAffiliate.Prefecture) ? ViewBag.Pref[ResponseModel.ManageAffiliate.Prefecture] : null;
            ViewBag.BusinessTypeKey = ResponseModel.ManageAffiliate.BusinessType;
            var dbResponse = _affiliateBuss.GetAffiliateList(dbAffiliateListRequest);
            var dbCustomerListRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex2,
                Take = PageSize2,
                SearchFilter = SearchFilter2
            };
            var dbReferalRes = _affiliateBuss.GetReferalConvertedCustomerList("", "", dbCustomerListRequest);
            var analyticDBResponse = _affiliateBuss.GetAffiliateAnalytic();
            if (dbResponse.Count > 0) ResponseModel.GetAffiliateList = dbResponse.MapObjects<AffiliateManagementModel>();
            ResponseModel.GetAffiliateList.ForEach(x => x.AffiliateId = x.AffiliateId.EncryptParameter());
            ResponseModel.GetAffiliateList.ForEach(x =>
            {
                x.HoldAffiliateId = x.HoldAffiliateId.EncryptParameter();
                x.AffiliateImage = ImageHelper.ProcessedImage(x.AffiliateImage);
            });
            ResponseModel.GetReferalConvertedCustomerList = dbReferalRes.MapObjects<ReferralConvertedCustomerListModel>();
            ResponseModel.GetReferalConvertedCustomerList.ForEach(x =>
            {
                x.CustomerImage = ImageHelper.ProcessedImage(x.CustomerImage);
                x.AffiliateImaeg = ImageHelper.ProcessedImage(x.AffiliateImaeg);
            });
            ResponseModel.AffiliatePageAnalyticModel = analyticDBResponse.MapObject<AffiliatePageAnalyticModel>();
            ViewBag.SearchFilter1 = SearchFilter1;
            ViewBag.SearchFilter2 = SearchFilter2;
            var ActiveTab = value;
            //if (!string.IsNullOrEmpty(SearchFilter1)) ActiveTab = "Affiliates";
            //else if (!string.IsNullOrEmpty(SearchFilter2)) ActiveTab = "Converted Customers";
            ResponseModel.ListType = value;
            ViewBag.ActiveTab = ActiveTab ?? "";
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData1 = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;

            ViewBag.StartIndex2 = StartIndex2;
            ViewBag.PageSize2 = PageSize2;
            ViewBag.TotalData2 = dbReferalRes != null && dbReferalRes.Any() ? dbReferalRes[0].TotalRecords : 0;
            return View(ResponseModel);
        }

        [HttpGet]
        public ActionResult ManageAffiliate(string AffiliateId = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ManageAffiliateModel model = new ManageAffiliateModel();
            if (!string.IsNullOrEmpty(AffiliateId))
            {
                var id = AffiliateId.DecryptParameter();
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
                var dbResponse = _affiliateBuss.GetAffiliateDetails(id, culture);
                model = dbResponse.MapObject<ManageAffiliateModel>();
                model.AffiliateId = !string.IsNullOrEmpty(model.AffiliateId) ? model.AffiliateId.EncryptParameter() : null;
                //model.Prefecture = !string.IsNullOrEmpty(model.Prefecture) ? model.Prefecture.EncryptParameter() : null;
                model.BusinessType = !string.IsNullOrEmpty(model.BusinessType) ? model.BusinessType.EncryptParameter() : null;
            }
            TempData["ManageAffiliateModel"] = model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("Index", "AffiliateManagement", new { SearchFilter1 = SearchFilter, value = "a", StartIndex = StartIndex, PageSize = PageSize });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageAffiliate(ManageAffiliateModel model)
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            if (model.BusinessType.DecryptParameter() == "1")
            {
                if (string.IsNullOrEmpty(model.CEOName))
                {
                    ModelState.AddModelError("CEOName", "Required");
                }
                if (string.IsNullOrEmpty(model.CEONameFurigana))
                {
                    ModelState.AddModelError("CEONameFurigana", "Required");
                }
                if (string.IsNullOrEmpty(model.CompanyAddress))
                {
                    ModelState.AddModelError("CompanyAddress", "Required");
                }
                if (string.IsNullOrEmpty(model.CompanyName))
                {
                    ModelState.AddModelError("CompanyName", "Required");
                }
            }
            if (ModelState.IsValid)
            {
                ManageAffiliateCommon commonModel = model.MapObject<ManageAffiliateCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                commonModel.AffiliateId = model.AffiliateId?.DecryptParameter();
                commonModel.BusinessType = model.BusinessType?.DecryptParameter();
                //commonModel.Prefecture = commonModel.Prefecture?.DecryptParameter();
                var dbResponse = _affiliateBuss.ManageAffiliate(commonModel);
                if (dbResponse != null && dbResponse.Code == 0)
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });
                    return RedirectToAction("Index", "AffiliateManagement");
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });

                    TempData["ManageClubModel"] = model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("Index", "AffiliateManagement");
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
            TempData["ManageAffiliateModel"] = model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("Index", "AffiliateManagement");
        }

        [HttpPost, ValidateAntiForgeryToken, OverrideActionFilters]
        public JsonResult ApproveRejectAffiliateRequest(string HoldAgentId, string Status)
        {
            var response = new CommonDbResponse();
            var hAgentId = !string.IsNullOrEmpty(HoldAgentId) ? HoldAgentId.DecryptParameter() : null;
            var ApprovalStatus = (!string.IsNullOrEmpty(Status)
                && (Status.Trim().ToUpper() == "A" || Status.Trim().ToUpper() == "R")) ? Status : null;
            if (string.IsNullOrEmpty(hAgentId) || string.IsNullOrEmpty(ApprovalStatus))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return Json(response.Message, JsonRequestBehavior.AllowGet);
            }
            var dbRequest = new ApproveRejectAffiliateCommon()
            {
                HoldAgentId = hAgentId,
                ApprovedStatus = ApprovalStatus,
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
                ActionIP = ApplicationUtilities.GetIP()
            };
            var dbResponse = _affiliateBuss.ApproveRejectAffiliateRequest(dbRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return Json(response.Message, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, OverrideActionFilters]
        public ActionResult ManageAffiliateStatus(string AgentId, string Status)
        {
            var response = new CommonDbResponse();
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            var manageStatus = (!string.IsNullOrEmpty(Status)
                && (Status.Trim().ToUpper() == "A" || Status.Trim().ToUpper() == "B")) ? Status : null;
            if (string.IsNullOrEmpty(aId) || string.IsNullOrEmpty(manageStatus))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("Index", "AffiliateManagement");
            }
            var dbRequest = new ManageAffiliateStatusCommon()
            {
                AgentId = aId,
                Status = manageStatus,
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
                ActionIP = ApplicationUtilities.GetIP()
            };
            var dbResponse = _affiliateBuss.ManageAffiliateStatus(dbRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return RedirectToAction("Index", "AffiliateManagement");
        }

        [HttpGet, OverrideActionFilters]
        public async Task<ActionResult> ResetAffiliatePassword(string AgentId)
        {
            var response = new CommonDbResponse();
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId)) response = new CommonDbResponse { ErrorCode = 1, Message = "Invalid details" };
            var commonRequest = new ManageAffiliateStatusCommon()
            {
                AgentId = aId,
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            var _sqlTransactionHandler = new RepositoryDaoWithTransaction(null, null);
            _sqlTransactionHandler.BeginTransaction();
            var dbResponse = _affiliateBuss.ResetPassword(commonRequest, _sqlTransactionHandler.GetCurrentConnection(), _sqlTransactionHandler.GetCurrentTransaction());
            response = dbResponse;
            if (response?.Code != CRS.ADMIN.SHARED.ResponseCode.Success || string.IsNullOrEmpty(response.Extra1) || string.IsNullOrEmpty(response.Extra2))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = response.Code == CRS.ADMIN.SHARED.ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                    Message = response.Message ?? "Something went wrong. Please try again later",
                    Title = response.Code == CRS.ADMIN.SHARED.ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                });
                _sqlTransactionHandler.RollbackTransaction();
                return RedirectToAction("Index", "AffiliateManagement");
            }
            var resetPasswordResponse = await _amazonCognitoMiddleware.SetPasswordAsync(new CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.Password.SetPasswordModel.Request
            {
                Username = response.Extra1,
                Password = response.Extra2,
                IsPermanent = true
            });

            if (resetPasswordResponse?.Code != CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.ResponseCode.Success)
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Something went wrong. Please try again later",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                _sqlTransactionHandler.RollbackTransaction();
                return RedirectToAction("Index", "AffiliateManagement");
            }

            var signOutResponse = await _amazonCognitoMiddleware.AdminSignOutUserAsync(new SHARED.Middleware.AmazonCognitoModel.Auth.AdminSignOutModel.Request
            {
                Username = response.Extra1
            });

            if (signOutResponse?.Code != CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.ResponseCode.Success)
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Something went wrong. Please try again later",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                _sqlTransactionHandler.RollbackTransaction();
                return RedirectToAction("Index", "AffiliateManagement");
            }

            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            _sqlTransactionHandler.CommitTransaction();
            return RedirectToAction("Index", "AffiliateManagement");
        }
    }
}