using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.DiscountManagement;
using CRS.ADMIN.APPLICATION.Models.ReviewAndRatingsManagement;
using CRS.ADMIN.BUSINESS.CommissionManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.DiscountManagementCommon;
using CRS.ADMIN.SHARED.PaginationManagement;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class DiscountManagementController : BaseController
    {
        private readonly ICommissionManagementBusiness _business;
        public DiscountManagementController(ICommissionManagementBusiness business)
        {
            _business = business;
        }
        [HttpGet]
        public ActionResult Index(string TabValue = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10, string ClubId = "")
        {
            ViewBag.SearchFilter = SearchFilter;
            Session["CurrentURL"] = "/DiscountManagement/Index";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            string RenderId = "";
            DiscountManagementListResponseModel discountModel = new DiscountManagementListResponseModel();
            if (TabValue == "")
            {
                PaginationFilterCommon discountRequest = new PaginationFilterCommon()
                {
                    Skip = StartIndex,
                    Take = PageSize,
                    SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
                };
                var discountCategoryList = _business.GetDiscountCategoryList(discountRequest);
                discountModel.listDiscountManagement = discountCategoryList.MapObjects<DiscountManagementResponseModel>();
                discountModel.listDiscountManagement.ForEach(x =>
                {
                    x.CategoryId = x.CategoryId.EncryptParameter();
                    x.CategoryName = x.CategoryName;
                    x.Description = x.Description;
                    x.Status = x.Status;
                    x.CreatedDate = x.CreatedDate;
                    x.UpdatedDate = x.UpdatedDate;
                });
                if (TempData.ContainsKey("DiscountCategoryManagementModel")) discountModel.discountCategoryModel = TempData["DiscountCategoryManagementModel"] as DiscountManagementModel;
                if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
                ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
                ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
                ViewBag.TotalData = discountCategoryList != null && discountCategoryList.Any() ? discountCategoryList[0].TotalRecords : 0;
            }
            if (TabValue == "02")
            {


            }
            ViewBag.LocationList = ApplicationUtilities.SetDDLValue(ApplicationUtilities
              .LoadDropdownList("LocationDdl") as Dictionary<string, string>, "", culture.ToLower() == "ja" ? "場所を選択" : "Select Location");
            ViewBag.CommissionCategoryList = ApplicationUtilities.SetDDLValue(ApplicationUtilities
                .LoadDropdownList("DISCOUNTCATEGORY") as Dictionary<string, string>, "", culture.ToLower() == "ja" ? "割引カテゴリを選択" : "Select Discount Category");
            ViewBag.SearchFilter = SearchFilter;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            discountModel.TabValue = TabValue;
            discountModel.ListType = TabValue;
            ViewBag.TabValue = TabValue;
            return View(discountModel);
        }

        [HttpPost, OverrideActionFilters, ValidateAntiForgeryToken]
        public async Task<JsonResult> GetDiscountClubListByLocation(string locationId, string agentId)
        {
            var lId = !string.IsNullOrEmpty(locationId) ? locationId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(lId)) { return null; }
            var clubLists = ApplicationUtilities.SetDDLValue(ApplicationUtilities
                .LoadDropdownList("ClubList", lId) as Dictionary<string, string>, null);
            return Json(new { clubLists }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, OverrideActionFilters, ValidateAntiForgeryToken]
        public async Task<JsonResult> GetCurrentDiscountByClub(string agentId)
        {
            var lId = !string.IsNullOrEmpty(agentId) ? agentId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(lId)) { return null; }
            var commissionLists = ApplicationUtilities.SetDDLValue(ApplicationUtilities
                .LoadDropdownList("DISCOUNTCATEGORYVIACLUBDDL", lId) as Dictionary<string, string>, null);
            return Json(new { commissionLists }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AssignDiscount(string NewDiscountCategoryDDL, string ClubDDLList)
        {
            if (!string.IsNullOrEmpty(NewDiscountCategoryDDL))
                NewDiscountCategoryDDL = NewDiscountCategoryDDL.DecryptParameter();
            if (!string.IsNullOrEmpty(ClubDDLList))
                ClubDDLList = ClubDDLList.DecryptParameter();
            var requestMapped = new AssignDiscountCommon()
            {
                AgentId = ClubDDLList,
                CategoryId = NewDiscountCategoryDDL,
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
                ActionIP = ApplicationUtilities.GetIP()
            };
            var respMapped = _business.AssignDiscount(requestMapped);
            if (respMapped != null && respMapped.Code == 0)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    Message = respMapped.Message,
                    NotificationType = NotificationMessage.SUCCESS,
                    Title = NotificationMessage.SUCCESS.ToString(),
                });

                return RedirectToAction("Index", "DiscountManagement", new { TabValue = "02" });
            }
            AddNotificationMessage(new NotificationModel()
            {
                Message = respMapped?.Message ?? "Something went wrong",
                NotificationType = NotificationMessage.ERROR,
                Title = NotificationMessage.ERROR.ToString(),
            });
            return RedirectToAction("Index", "DiscountManagement", new { TabValue = "02" });
        }
        [HttpGet]
        public ActionResult ManageDiscountCategory(string categoryId)
        {
            var i = !string.IsNullOrEmpty(categoryId) ? categoryId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(i))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("Index", "DiscountManagement");
            }
            var viewModel = new DiscountManagementModel();
            var common = new DiscountManagementRequestCommon()
            {
                categoryId = i
            };
            var respMapped = _business.GetDiscountDetails(common);
            viewModel = respMapped.MapObject<DiscountManagementModel>();
            viewModel.categoryId = respMapped.CategoryId.EncryptParameter();
            TempData["DiscountCategoryManagementModel"] = viewModel;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("Index", "DiscountManagement");
        }
        [HttpPost]
        public ActionResult ManageDiscountCategory(DiscountManagementModel requestModel)
        {
            var viewModel = new DiscountManagementModel();
            var mappedRequest = requestModel.MapObject<DiscountManagementRequestCommon>();
            if (!string.IsNullOrEmpty(requestModel.categoryId))
            {
                mappedRequest.categoryId = requestModel.categoryId.DecryptParameter();
            }
            var dbResp = _business.ManageDiscountCategory(mappedRequest);
            if (dbResp.Code == 0)
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResp.Message,
                    Title = NotificationMessage.SUCCESS.ToString(),
                });
                TempData["DiscountCategoryManagementModel"] = viewModel;
                TempData["RenderId"] = "Manage";
                return RedirectToAction("Index", "DiscountManagement");
            }
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.ERROR,
                    Message = !string.IsNullOrEmpty(dbResp.Message) ? dbResp.Message : "Invalid request",
                    Title = NotificationMessage.ERROR.ToString(),
                });
            }
            TempData["DiscountCategoryManagementModel"] = viewModel;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("Index", "DiscountManagement");
        }

        public ActionResult DiscountCategoryStatusUpdate(string categoryId, string status, string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            if (!string.IsNullOrEmpty(categoryId))
                categoryId = categoryId.DecryptParameter();
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.WARNING,
                    Message = "Invalid category",
                    Title = NotificationMessage.WARNING.ToString(),
                });
                return RedirectToAction("Index", "DiscountManagement");
            }
            string actionUser = "";
            string actionIP = "";
            var response = _business.ChangeDiscountCategoryStatus(categoryId, "", status, actionUser, actionIP);
            if (response.Code == 0)
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = response.Message,
                    Title = NotificationMessage.SUCCESS.ToString(),
                });
                return RedirectToAction("Index", "DiscountManagement");
            }
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.ERROR,
                    Message = !string.IsNullOrEmpty(response.Message) ? response.Message : "Invalid request",
                    Title = NotificationMessage.ERROR.ToString(),
                });
            }
            return RedirectToAction("Index", "DiscountManagement");
        }

        [HttpGet]
        public ActionResult GetCategoryDetailsList(string categoryId, string categoryName, string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            ViewBag.SearchFilter = SearchFilter;
            Session["CurrentURL"] = "/DiscountManagement/GetCategoryDetailsList";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            string RenderId = "";
            if (!string.IsNullOrEmpty(categoryId))
            {
                categoryId = categoryId.DecryptParameter();
            }

            DiscountManagementListResponseModel discountSubCategoryModel = new DiscountManagementListResponseModel();
            PaginationFilterCommon discountRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            var discountSubCategoryList = _business.GetDiscountSubCategoryList(discountRequest, categoryId);
            discountSubCategoryModel.listDiscountSubCategoryManagement = discountSubCategoryList.MapObjects<DiscountCategoryDetailsResponseModel>();
            discountSubCategoryModel.listDiscountSubCategoryManagement.ForEach(x =>
            {
                x.CategoryId = x.CategoryId.EncryptParameter();
                x.SubCategoryId = x.SubCategoryId.EncryptParameter();
                x.categoryName = x.categoryName;
                x.FromAmount = x.FromAmount;
                x.ToAmount = x.ToAmount;
                x.Status = x.Status;
                x.DiscountType = x.DiscountType;
                x.Value = x.Value;
                x.MinValue = x.MinValue;
                x.MaxValue = x.MaxValue;
                x.createdDate = x.createdDate;
                x.updatedDate = x.updatedDate;
            });
            if (TempData.ContainsKey("DiscountsubCategoryManagementModel")) discountSubCategoryModel.discountSubCategoryModel = TempData["DiscountSubCategoryManagementModel"] as DiscountSubCategoryManagementRequestModel;
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            discountSubCategoryModel.discountSubCategoryModel.discountTypeList =
             ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("COMMISSIONPERCENTAGETYPELIST", "", culture)
                 as Dictionary<string, string>, "", culture.ToLower() == "ja" ? "--- ?? ---" : "--- Select ---");

            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.TotalData = discountSubCategoryList != null && discountSubCategoryList.Any() ? discountSubCategoryList[0].TotalRecords : 0;
            //ViewBag.discountTypeIdKey = TempData["discountTypeId"];
            ViewBag.discountTypeIdKey = discountSubCategoryModel.discountSubCategoryModel.DiscountType;

            ViewBag.SearchFilter = SearchFilter;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.DiscountCategoryName = categoryName;
            ViewBag.DiscountCategoryId = categoryId;
            discountSubCategoryModel.discountSubCategoryModel.categoryName = categoryName;
            discountSubCategoryModel.discountSubCategoryModel.categoryId = categoryId;
            return View(discountSubCategoryModel);
        }
        [HttpGet]
        public ActionResult ManageDiscountSubCategoryDetails(string categoryId = "", string subCategoryId = "", string categoryName = "")
        {
            var i = !string.IsNullOrEmpty(categoryId) ? categoryId.DecryptParameter() : null;
            var j = !string.IsNullOrEmpty(subCategoryId) ? subCategoryId.DecryptParameter() : null;
            var k = categoryName;
            if (string.IsNullOrEmpty(i))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("Index", "DiscountManagement");
            }
            var viewModel = new DiscountSubCategoryManagementRequestModel();
            var common = new DiscountSubCategoryRequestCommon()
            {
                categoryId = i,
                subCategoryId = j
            };
            var respMapped = _business.GetSubCategoryDiscountDetails(common);
            viewModel = respMapped.MapObject<DiscountSubCategoryManagementRequestModel>();
            viewModel.categoryId = respMapped.CategoryId.EncryptParameter();
            viewModel.subCategoryId = respMapped.SubCategoryId.EncryptParameter();
            viewModel.DiscountType = respMapped.DiscountType.Trim().EncryptParameter();
            ViewBag.discountTypeIdKey = respMapped.DiscountType.Trim().EncryptParameter();
            TempData["DiscountsubCategoryManagementModel"] = viewModel;
            TempData["RenderId"] = "ManageSubCategory";
            TempData["discountTypeId"] = respMapped.DiscountType.EncryptParameter();
            return RedirectToAction("GetCategoryDetailsList", "DiscountManagement", new { categoryId = i.EncryptParameter(), categoryName = k });
        }

        [HttpPost]
        public ActionResult ManageDiscountSubCategoryDetails(DiscountSubCategoryManagementRequestModel requestModel)
        {
            if (!string.IsNullOrEmpty(requestModel.categoryId))
            {
                requestModel.categoryId = requestModel.categoryId.DecryptParameter();
            }
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("GetCategoryDetailsList", "DiscountManagement");
            }
            if (!string.IsNullOrEmpty(requestModel.subCategoryId))
                requestModel.subCategoryId = requestModel.subCategoryId.DecryptParameter();
            var viewModel = new DiscountSubCategoryManagementRequestModel();
            if (ModelState.IsValid)
            {
                var mappedRequest = requestModel.MapObject<DiscountSubCategoryManagementRequestCommon>();
                if (!string.IsNullOrEmpty(requestModel.DiscountType))
                    mappedRequest.DiscountType = requestModel.DiscountType.DecryptParameter();
                var dbResp = _business.ManageDiscountSubCategory(mappedRequest);
                if (dbResp.Code == 0)
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = dbResp.Message,
                        Title = NotificationMessage.SUCCESS.ToString(),
                    });
                    TempData["DiscountCategoryManagementModel"] = viewModel;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("GetCategoryDetailsList", "DiscountManagement", new { categoryId = requestModel.categoryId.EncryptParameter(), categoryName = requestModel.categoryName });
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.ERROR,
                        Message = !string.IsNullOrEmpty(dbResp.Message) ? dbResp.Message : "Invalid request",
                        Title = NotificationMessage.ERROR.ToString(),
                    });
                }
                TempData["DiscountCategoryManagementModel"] = viewModel;
                TempData["RenderId"] = "Manage";
                return RedirectToAction("GetCategoryDetailsList", "DiscountManagement", new { categoryId = requestModel.categoryId.EncryptParameter(), categoryName = requestModel.categoryName });
            }
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = NotificationMessage.ERROR,
                Message =  "Invalid request",
                Title = NotificationMessage.ERROR.ToString(),
            });
            TempData["DiscountCategoryManagementModel"] = viewModel;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("GetCategoryDetailsList", "DiscountManagement", new { categoryId = requestModel.categoryId.EncryptParameter(), categoryName = requestModel.categoryName });
        }

        public ActionResult DiscountSubCategoryStatus(string categoryId, string subCategoryId, string categoryName, string status, string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            if (!string.IsNullOrEmpty(categoryId))
                categoryId = categoryId.DecryptParameter();
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.WARNING,
                    Message = "Invalid category",
                    Title = NotificationMessage.WARNING.ToString(),
                });
                return RedirectToAction("Index", "DiscountManagement");
            }
            if (!string.IsNullOrEmpty(subCategoryId))
                subCategoryId = subCategoryId.DecryptParameter();
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.WARNING,
                    Message = "Invalid sub category",
                    Title = NotificationMessage.WARNING.ToString(),
                });
                return RedirectToAction("Index", "DiscountManagement");
            }
            string actionUser = "";
            string actionIP = "";
            var response = _business.ChangeDiscountCategoryStatus(categoryId, subCategoryId, status, actionUser, actionIP);
            if (response.Code == 0)
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = response.Message,
                    Title = NotificationMessage.SUCCESS.ToString(),
                });
                return RedirectToAction("GetCategoryDetailsList", "DiscountManagement", new { categoryId = categoryId.EncryptParameter(), categoryName = categoryName });
            }
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.ERROR,
                    Message = !string.IsNullOrEmpty(response.Message) ? response.Message : "Invalid request",
                    Title = NotificationMessage.ERROR.ToString(),
                });
            }
            return RedirectToAction("GetCategoryDetailsList", "DiscountManagement", new { categoryId = categoryId, categoryName = categoryName });
        }
    }
}