using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models;
using CRS.ADMIN.APPLICATION.Models.PlanManagement;
using CRS.ADMIN.BUSINESS.CommonManagement;
using CRS.ADMIN.BUSINESS.PlanManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PlanManagement;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class PlanManagementController : BaseController
    {
        private readonly IPlanManagementBusiness _business;
        private readonly ICommonManagementBusiness _common;

        public PlanManagementController(IPlanManagementBusiness business, ICommonManagementBusiness common)
        {
            _business = business;
            _common = common;
        }

        [HttpGet]
        public ActionResult PlanList(string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            Session["CurrentURL"] = "/PlanManagement/PlanList";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            PlansManagementModel ResModel = new PlansManagementModel();
            #region DDL
            var planTypeDBResponse = _business.GetDDL("7");
            var timeDBResponse = _business.GetDDL("8");
            var liquorDBResponse = _business.GetDDL("9");
            var planCategoryDBResponse = _business.GetDDL("35");

            var planTypeDictionary = GetDictionaryFromResponse(_business.GetDDL("7"), culture);
            var timeDictionary = GetDictionaryFromResponse(_business.GetDDL("8"), culture);
            var liquorDictionary = GetDictionaryFromResponse(_business.GetDDL("9"), culture);
            var planCategoryDictionary = GetDictionaryFromResponse(_business.GetDDL("35"), culture);

            #endregion
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            var planLists = _business.GetPlanList(dbRequest);
            ResModel.PlanManagementModel = planLists.MapObjects<PlanManagementModel>();
            ResModel.PlanManagementModel.ForEach(x =>
            {
                x.PlanId = x.PlanId.EncryptParameter();
                x.PlanImage = ImageHelper.ProcessedImage(x.PlanImage);
                x.PlanImage2 = ImageHelper.ProcessedImage(x.PlanImage2);
                x.Price = Convert.ToInt64(x.Price).ToString("N0");
            });
            ResModel.PlanManagementModel.ForEach(x => x.PlanStatus = x.PlanStatus.Trim().ToUpper() == "A" ? "A" : "B");

            string RenderId = "";

            if (TempData.ContainsKey("PlanManagementModel")) ResModel.PlanMgmt = TempData["PlanManagementModel"] as PlanManagementModel;
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;

            ViewBag.PlanList = ApplicationUtilities.SetDDLValue(planTypeDictionary as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.TimeList = ApplicationUtilities.SetDDLValue(timeDictionary as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.LiquorList = ApplicationUtilities.SetDDLValue(liquorDictionary as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.PlanCategoryDDL = ApplicationUtilities.SetDDLValue(planCategoryDictionary as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.SearchFilter = SearchFilter;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = planLists != null && planLists.Any() ? planLists[0].TotalRecords : 0;
            return View(ResModel);
        }

        public ActionResult PlanDetails(string id = "")
        {
            var viewModel = new PlanManagementModel();

            if (!string.IsNullOrWhiteSpace(id))
            {
                var common = new PlanManagementCommon()
                {
                    PlanId = id.DecryptParameter(),
                    ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
                    ActionIP = ApplicationUtilities.GetIP(),
                    ActionPlatform = "Admin"
                };

                var serviceResp = _business.GetPlanDetail(common);

                TempData["PlanModel"] = serviceResp.MapObject<PlanManagementModel>();
                TempData["RenderId"] = "ManagePlan";
            }
            return RedirectToAction("PlanList", "PlanManagement");
        }

        public ActionResult ManagePlan(string planId = "")
        {
            var i = !string.IsNullOrEmpty(planId) ? planId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(i))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("PlanList", "PlanManagement");
            }
            var viewModel = new PlanManagementModel();
            var common = new PlanManagementCommon()
            {
                PlanId = i
            };
            var serviceResp = _business.GetPlanDetail(common);
            viewModel = serviceResp.MapObject<PlanManagementModel>();
            viewModel.PlanId = viewModel.PlanId.EncryptParameter();
            viewModel.PlanTime = viewModel.PlanTime.EncryptParameter();
            viewModel.PlanType = viewModel.PlanType.EncryptParameter();
            viewModel.Liquor = viewModel.Liquor.EncryptParameter();
            viewModel.PlanCategory = viewModel.PlanCategory.EncryptParameter();
            TempData["PlanManagementModel"] = viewModel;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("PlanList", "PlanManagement");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ManagePlan(PlanManagementModel model, HttpPostedFileBase ImageFile, HttpPostedFileBase ImageFile2)
        {
            if (!ModelState.IsValid)
            {
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
                TempData["PlanManagementModel"] = model;
                TempData["RenderId"] = "Manage";
                return RedirectToAction("PlanList", "PlanManagement");
            }
            if (ImageFile == null && ImageFile2 == null)
            {
                if (string.IsNullOrEmpty(model.PlanImage) || string.IsNullOrEmpty(model.PlanImage2))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Image required",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["PlanManagementModel"] = model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("PlanList", "PlanManagement");
                }
            }
            var common = model.MapObject<PlanManagementCommon>();
            common.PlanType = !string.IsNullOrEmpty(common.PlanType) ? common.PlanType.DecryptParameter() : null;
            common.PlanTime = !string.IsNullOrEmpty(common.PlanTime) ? common.PlanTime.DecryptParameter() : null;
            common.Liquor = !string.IsNullOrEmpty(common.Liquor) ? common.Liquor.DecryptParameter() : null;
            common.PlanId = !string.IsNullOrEmpty(common.PlanId) ? common.PlanId.DecryptParameter() : null;
            common.PlanCategory = !string.IsNullOrEmpty(common.PlanCategory) ? common.PlanCategory.DecryptParameter() : null;
            if (string.IsNullOrEmpty(common.PlanType) || string.IsNullOrEmpty(common.PlanTime) || (!string.IsNullOrEmpty(model.PlanId) && string.IsNullOrEmpty(common.PlanId)))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                TempData["PlanManagementModel"] = model;
                TempData["RenderId"] = "Manage";
                return RedirectToAction("PlanList", "PlanManagement");
            }
            string PlanImageFileName = string.Empty;
            string PlanImageFileName2 = string.Empty;
            if (ImageFile != null)
            {
                var allowedContenttype = AllowedImageContentType();

                var contentType = ImageFile.ContentType;
                var ext = System.IO.Path.GetExtension(ImageFile.FileName);
                if (allowedContenttype.Contains(contentType.ToLower()))
                {
                    PlanImageFileName = $"{AWSBucketFolderNameModel.ADMIN}/PlanImage_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                    common.PlanImage = $"/{PlanImageFileName}";
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "File Must be .jpg, .png, .jpeg, .heif",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["PlanManagementModel"] = model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("PlanList", "PlanManagement");
                }
            }
            if (ImageFile2 != null)
            {
                var allowedContenttype = AllowedImageContentType();
                var contentType2 = ImageFile2.ContentType;
                var ext2 = System.IO.Path.GetExtension(ImageFile2.FileName);
                if (allowedContenttype.Contains(contentType2.ToLower()))
                {
                    PlanImageFileName2 = $"{AWSBucketFolderNameModel.ADMIN}/PlanImage2_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext2.ToLower()}";
                    common.PlanImage2 = $"/{PlanImageFileName2}";
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "File Must be .jpg, .png, .jpeg, .heif",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["PlanManagementModel"] = model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("PlanList", "PlanManagement");
                }
            }
            common.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
            common.ActionIP = ApplicationUtilities.GetIP();
            if (string.IsNullOrEmpty(common.IsStrikeOut))
                common.IsStrikeOut = "D";
            else if (common.IsStrikeOut.Trim().ToUpper() == "ON")
                common.IsStrikeOut = "A";
            else
                common.IsStrikeOut = "D";
            var serviceResp = _business.ManagePlan(common);
            if (serviceResp != null && serviceResp.Code == ResponseCode.Success)
            {
                if (ImageFile != null) await ImageHelper.ImageUpload(PlanImageFileName, ImageFile);
                if (ImageFile2 != null) await ImageHelper.ImageUpload(PlanImageFileName2, ImageFile2);
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = serviceResp.Message ?? "Success",
                    Title = NotificationMessage.SUCCESS.ToString(),
                });
                return RedirectToAction("PlanList", "PlanManagement");
            }
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = NotificationMessage.INFORMATION,
                Message = serviceResp.Message ?? "Failed",
                Title = NotificationMessage.INFORMATION.ToString()
            });
            TempData["PlanManagementModel"] = model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("PlanList", "PlanManagement");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult EnableDisablePlans(string Id)
        {
            var i = !string.IsNullOrEmpty(Id) ? Id.DecryptParameter() : null;
            if (string.IsNullOrEmpty(i))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
            }
            var promoImageCommon = new PlanManagementCommon()
            {
                PlanId = i,
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
                ActionIP = ApplicationUtilities.GetIP()
            };
            var DbResponse = _business.EnableDisablePlans(promoImageCommon);
            if (DbResponse.Code == ResponseCode.Success)
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = DbResponse.Message,
                    Title = NotificationMessage.SUCCESS.ToString()
                });
            }
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = DbResponse.Message ?? "Failed",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
            }
            return Json("ok");
        }

        private object LoadDropdownList(string ForMethod, string search1 = "", string search2 = "")
        {
            switch (ForMethod)
            {
                case "planlist":
                    return CreateDropdownList(_common.GetDropDown("007", search1, search2));
                case "timelist":
                    return CreateDropdownList(_common.GetDropDown("008", search1, search2));
                case "liquorlist":
                case "nominationlist":
                    return CreateDropdownList(_common.GetDropDown("009", search1, search2));
                default:
                    return new Dictionary<string, string>();
            }
        }

        private object CreateDropdownList(Dictionary<string, string> dbResponse)
        {
            var response = new Dictionary<string, string>();
            dbResponse.ForEach(item => { response.Add(item.Key, item.Value); });
            return response;
        }

        Dictionary<string, string> GetDictionaryFromResponse(List<StaticDataCommon> response, string culture)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (var item in response)
            {
                dictionary.Add(item.StaticValue.EncryptParameter(), culture == "en" ? item.StaticLabelEnglish : item.StaticLabelJapanese);
            }
            return dictionary;
        }
    }
}