using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.CommissionManagement;
using CRS.ADMIN.BUSINESS.CommissionManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.CommissionManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    [OverrideActionFilters]
    public class CommissionManagementController : BaseController
    {
        private readonly ICommissionManagementBusiness _CategoryBuss;

        public CommissionManagementController(ICommissionManagementBusiness CategoryBuss) => _CategoryBuss = CategoryBuss;

        #region Commission Category
        [HttpGet]
        public ActionResult CategoryList()
        {
            var viewModel = new CommissionCategoryRazorViewModel();
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            Session["CurrentURL"] = "/CommissionManagement/CategoryList";
            var dbResponse = _CategoryBuss.GetCategoryList();
            dbResponse.ForEach(x => x.CategoryId = x.CategoryId.EncryptParameter());

            viewModel.CommissionCategoryModelGrid = dbResponse.MapObjects<CommissionCategoryModel>();
            viewModel.CommissionCategoryModelGrid.ForEach(x => x.CreatedByImage = ImageHelper.ProcessedImage(x.CreatedByImage));

            if (TempData.ContainsKey("ManageCategoryModel"))
                viewModel.AddEditCommissionCategory = TempData["ManageCategoryModel"] as ManageCommissionCategoryModel;

            if (TempData.ContainsKey("RenderId"))
                ViewBag.PopUpRenderValue = TempData["RenderId"].ToString();

            ViewBag.LocationList = ApplicationUtilities.SetDDLValue(ApplicationUtilities
                .LoadDropdownList("LocationDdl") as Dictionary<string, string>, "", culture.ToLower()=="ja"?"場所を選択":"Select Location" );
            ViewBag.CommissionCategoryList = ApplicationUtilities.SetDDLValue(ApplicationUtilities
                .LoadDropdownList("COMMISSIONCATEGORYLIST") as Dictionary<string, string>, "", culture.ToLower() == "ja" ? "コミッションカテゴリを選択" : "Select Commission Category");
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult ManageCategory(string categoryId)
        {
            var catId = !string.IsNullOrEmpty(categoryId) ? categoryId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(catId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("CategoryList");
            }

            var dbResp = _CategoryBuss.GetCategoryById(catId);

            var manageCategoryModel = dbResp.MapObject<ManageCommissionCategoryModel>();

            TempData["ManageCategoryModel"] = manageCategoryModel;
            TempData["RenderId"] = "Manage";

            return RedirectToAction("CategoryList");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageCategory(ManageCommissionCategoryModel Request)
        {
            if (ModelState.IsValid)
            {
                var requestCommon = Request.MapObject<ManageCommissionCategoryCommon>();
                requestCommon.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                requestCommon.ActionIP = ApplicationUtilities.GetIP();
                var dbResponse = _CategoryBuss.ManageCommissionCategory(requestCommon);
                if (dbResponse != null && dbResponse.Code == 0)
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = dbResponse.Message,
                        Title = NotificationMessage.SUCCESS.ToString(),
                    });

                    return RedirectToAction("CategoryList");
                }
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.ERROR,
                    Message = dbResponse.Message ?? "Something went wrong",
                    Title = NotificationMessage.ERROR.ToString(),
                });

                return RedirectToAction("CategoryList");
            }

            var errorMessages = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => $"{e.ErrorMessage}"))
                        .ToList();

            var notificationModels = errorMessages.Select(errorMessage => new NotificationModel
            {
                NotificationType = NotificationMessage.ERROR,
                Message = errorMessage,
                Title = NotificationMessage.ERROR.ToString(),
            }).ToArray();

            AddNotificationMessage(notificationModels);

            return View(Request);
        }

        [HttpGet]
        public ActionResult AssignedList(string CategoryId)
        {
            var cId = !string.IsNullOrEmpty(CategoryId) ? CategoryId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(cId))
            {
                this.ShowPopup(1, "Invalid category details");
                return RedirectToAction("CategoryList", "CommissionManagement");
            }
            var dbResponse = _CategoryBuss.GetCategoryAssignedList(cId);
            List<CommissionAssignedClubsModel> response = dbResponse.MapObjects<CommissionAssignedClubsModel>();
            foreach (var item in response)
            {
                item.Status = "<span class='badge badge-" + (item.Status.Trim().ToUpper() == "A" ? "success" : "danger") + "'>" + (item.Status.Trim().ToUpper() == "A" ? "Active" : "Blocked") + "</span>";
                item.Logo = "<img class=\"mImg\"  height=\"100\" width=\"100\" style=\"border: none; object-fit: contain; \" src=\"" + item.Logo + "\" onClick=\"ShowImage(\'" + item.Logo + "\')\" >";
            }
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add("ClubName", "Club Name");
            param.Add("Logo", "Logo");
            param.Add("EmailAddress", "Email Address");
            param.Add("MobileNumber", "Mobile Number");
            param.Add("Status", "Status");
            param.Add("CreatedDate", "Created On");
            param.Add("UpdatedDate", "Updated On");
            ProjectGrid.column = param;
            var grid = ProjectGrid.MakeGrid(response, "Commission Assigned List", "", 0, false, "", "", "", "", "", "", "datatable-total", "false");
            ViewData["Grid"] = grid;
            return View();
        }

        [HttpGet]
        public ActionResult BlockCategory(string CategoryId)
        {
            var response = new CommonDbResponse();
            var cId = !string.IsNullOrEmpty(CategoryId) ? CategoryId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(cId)) response = new CommonDbResponse { ErrorCode = 1, Message = "Invalid details" };
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            var dbResponse = _CategoryBuss.ManageCommissionStatus("B", cId, commonRequest);
            if (dbResponse.Code == ResponseCode.Success)
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponse.Message,
                    Title = NotificationMessage.SUCCESS.ToString(),
                });
            else
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.ERROR,
                    Message = dbResponse.Message ?? "Something went wrong",
                    Title = NotificationMessage.ERROR.ToString(),
                });

            return RedirectToAction("CategoryList");
        }

        [HttpGet]
        public ActionResult UnBlockCategory(string CategoryId)
        {
            var response = new CommonDbResponse();
            var cId = !string.IsNullOrEmpty(CategoryId) ? CategoryId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(cId)) response = new CommonDbResponse { ErrorCode = 1, Message = "Invalid details" };
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            var dbResponse = _CategoryBuss.ManageCommissionStatus("A", cId, commonRequest);
            if (dbResponse.Code == ResponseCode.Success)
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponse.Message,
                    Title = NotificationMessage.SUCCESS.ToString(),
                });
            else
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.ERROR,
                    Message = dbResponse.Message ?? "Something went wrong",
                    Title = NotificationMessage.ERROR.ToString(),
                });

            return RedirectToAction("CategoryList");
        }

        #endregion

        #region Commission Setup

        [HttpGet]
        public ActionResult CommissionDetailList(string CategoryId, string CategoryName = "", string AdminCommissionTypeId = "")
        {
            var viewModel = new CommissionDetailRazorViewModel();

            var cId = !string.IsNullOrEmpty(CategoryId) ? CategoryId.DecryptParameter() : null;
            var adminCmsTypeId = !string.IsNullOrEmpty(AdminCommissionTypeId) ? AdminCommissionTypeId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(cId) && string.IsNullOrEmpty(adminCmsTypeId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {

                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid category details",
                    Title = NotificationMessage.INFORMATION.ToString()
                });

                return RedirectToAction("CategoryList", "CommissionManagement");
            }
            var dbResponse = _CategoryBuss.GetCommissionDetailList(cId, adminCmsTypeId);
            viewModel.ManageCommissionDetailGrid = dbResponse.MapObjects<ManageCommissionDetailModel>();
            viewModel.ManageCommissionDetailGrid.ForEach(x =>
            {
                x.CategoryDetailId = x.CategoryDetailId.EncryptParameter();
                x.CategoryId = x.CategoryId.EncryptParameter();
                x.AdminCommissionTypeId = x.AdminCommissionTypeId.EncryptParameter();
            });

            if (TempData.ContainsKey("ManageCommissionDetailModel"))
                viewModel.ManageCommissionDetailAddEdit = TempData["ManageCommissionDetailModel"] as ManageCommissionDetailModel;

            if (TempData.ContainsKey("RenderId"))
                ViewBag.PopUpRenderValue = TempData["RenderId"].ToString();

            viewModel.ManageCommissionDetailAddEdit.CommissionPercentageTypeList =
                ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("COMMISSIONPERCENTAGETYPELIST")
                    as Dictionary<string, string>, "", "--- Select ---");

            ViewBag.CommissionPercentTypeIdKey = viewModel.ManageCommissionDetailAddEdit.CommissionPercentageType;
            ViewBag.CategoryId = CategoryId;
            ViewBag.CategoryName = CategoryName;
            ViewBag.AdminCommissionTypeId = AdminCommissionTypeId;
            viewModel.ManageCommissionDetailAddEdit.CategoryId = CategoryId;
            viewModel.ManageCommissionDetailAddEdit.CategoryName = CategoryName;
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/CommissionManagement/AdminCommissionList?CategoryId=" + ViewBag.CategoryId + "&CategoryName=" + ViewBag.CategoryName;
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult ManageCommissionDetail(string id, string CategoryId, string CategoryName, string AdminCommissionTypeId = "")
        {
            var i = !string.IsNullOrEmpty(id) ? id.DecryptParameter() : null;
            if (string.IsNullOrEmpty(i))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("CommissionDetailList");
            }

            var commissionDetailCommon = _CategoryBuss.GetCommissionDetailById(i);
            if (commissionDetailCommon == null)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Something went wrong",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("CommissionDetailList", new { CategoryId = id, CategoryName, AdminCommissionTypeId = AdminCommissionTypeId });
            }

            var viewModel = commissionDetailCommon.MapObject<ManageCommissionDetailModel>();
            viewModel.CategoryId = viewModel?.CategoryId?.EncryptParameter();
            viewModel.CategoryDetailId = viewModel?.CategoryDetailId?.EncryptParameter();
            viewModel.CommissionPercentageType = viewModel?.CommissionPercentageType?.EncryptParameter();
            TempData["ManageCommissionDetailModel"] = viewModel;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("CommissionDetailList", new { CategoryId = CategoryId, CategoryName, AdminCommissionTypeId = AdminCommissionTypeId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageCommissionDetail(ManageCommissionDetailModel Request)
        {
            var requestCommmon = Request.MapObject<ManageCommissionDetailCommon>();
            requestCommmon.CategoryId = !string.IsNullOrEmpty(requestCommmon.CategoryId) ? requestCommmon.CategoryId.DecryptParameter() : null;
            requestCommmon.CategoryDetailId = !string.IsNullOrEmpty(requestCommmon.CategoryDetailId) ? requestCommmon.CategoryDetailId.DecryptParameter() : null;
            requestCommmon.CommissionPercentageType = requestCommmon?.CommissionPercentageType?.DecryptParameter();
            requestCommmon.AdminCommissionTypeId = requestCommmon?.AdminCommissionTypeId?.DecryptParameter();
            if (string.IsNullOrEmpty(requestCommmon.CategoryId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    Message = "Invalid category details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                    NotificationType = NotificationMessage.INFORMATION
                });
                return RedirectToAction("CategoryList", "CommissionManagement");
            }
            requestCommmon.ActionIP = ApplicationUtilities.GetIP();
            requestCommmon.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
            var dbResponse = _CategoryBuss.ManageCommissionDetail(requestCommmon);
            if (dbResponse.Code == ResponseCode.Success)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    Message = dbResponse.Message,
                    Title = NotificationMessage.SUCCESS.ToString(),
                    NotificationType = NotificationMessage.SUCCESS
                });
                return RedirectToAction("CommissionDetailList", new { CategoryId = Request.CategoryId, CategoryName = Request.CategoryName, AdminCommissionTypeId = Request.AdminCommissionTypeId });
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    Message = dbResponse.Message,
                    Title = NotificationMessage.INFORMATION.ToString(),
                    NotificationType = NotificationMessage.INFORMATION
                });
                TempData["ManageCommissionDetailModel"] = Request.MapObject<ManageCommissionDetailModel>();
                TempData["RenderId"] = "Manage";
                return RedirectToAction("CommissionDetailList", new { CategoryId = Request.CategoryId, CategoryName = Request.CategoryName, AdminCommissionTypeId = Request.AdminCommissionTypeId });
            }
        }

        [HttpGet]
        public ActionResult DeleteCommissionDetail(string CategoryId, string CategoryDetailId, string CategoryName, string AdminCommissionTypeId)
        {
            var response = new CommonDbResponse();
            var cId = !string.IsNullOrEmpty(CategoryId) ? CategoryId.DecryptParameter() : null;
            var cdId = !string.IsNullOrEmpty(CategoryDetailId) ? CategoryDetailId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(cId) || string.IsNullOrEmpty(cdId))
                response = new CommonDbResponse { ErrorCode = 1, Message = "Invalid details" };
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            var dbResponse = _CategoryBuss.DeleteCommissionDetail(cId, cdId, commonRequest);
            if (dbResponse.Code == ResponseCode.Success)
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponse.Message,
                    Title = NotificationMessage.SUCCESS.ToString(),
                });
            else
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.ERROR,
                    Message = dbResponse.Message ?? "Something went wrong",
                    Title = NotificationMessage.ERROR.ToString(),
                });

            return RedirectToAction("CommissionDetailList", new { CategoryId, CategoryName , AdminCommissionTypeId });
        }
        #endregion

        #region Assign Commission
        [HttpGet]
        public ActionResult AssignCommission()
        {
            ViewBag.ClubList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("ClubList") as Dictionary<string, string>, "", "--- Select ---");
            ViewBag.CommissionCategoryList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CommissionCategoryList") as Dictionary<string, string>, "", "--- Select ---");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AssignCommission(string ClubDDL, string NewCommissionCategoryDDL)
        {
            var agentId = !string.IsNullOrEmpty(ClubDDL) ? ClubDDL.DecryptParameter() : null;
            var commissionCategoryId = !string.IsNullOrEmpty(NewCommissionCategoryDDL) ? NewCommissionCategoryDDL.DecryptParameter() : null;

            if (string.IsNullOrEmpty(agentId) || string.IsNullOrWhiteSpace(commissionCategoryId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    Message = "Something went wrong",
                    NotificationType = NotificationMessage.ERROR,
                    Title = NotificationMessage.ERROR.ToString(),
                });

                return RedirectToAction("CategoryList");
            }

            var requestModel = new AssignCommissionModel()
            {
                AgentId = agentId,
                CategoryId = commissionCategoryId
            };

            var requestCommon = requestModel.MapObject<AssignCommissionCommon>();
            requestCommon.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
            requestCommon.ActionIP = ApplicationUtilities.GetIP();
            var dbResponse = _CategoryBuss.AssignCommission(requestCommon);
            if (dbResponse != null && dbResponse.Code == 0)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    Message = dbResponse.Message,
                    NotificationType = NotificationMessage.SUCCESS,
                    Title = NotificationMessage.SUCCESS.ToString(),
                });

                return RedirectToAction("CategoryList");
            }
            AddNotificationMessage(new NotificationModel()
            {
                Message = dbResponse?.Message ?? "Something went wrong",
                NotificationType = NotificationMessage.ERROR,
                Title = NotificationMessage.ERROR.ToString(),
            });
            return RedirectToAction("CategoryList");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult GetClubListByAgentId(string AgentId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            AgentId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            if (!string.IsNullOrEmpty(AgentId))
                list = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("ClubCommissionDetail", AgentId) as Dictionary<string, string>, "");
            return Json(new SelectList(list, "Value", "Text", JsonRequestBehavior.AllowGet));
        }

        [HttpPost, OverrideActionFilters, ValidateAntiForgeryToken]
        public async Task<JsonResult> GetClubListByLocation(string locationId)
        {
            var lId = !string.IsNullOrEmpty(locationId) ? locationId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(lId)) { return null; }
            var clubLists = ApplicationUtilities.SetDDLValue(ApplicationUtilities
                .LoadDropdownList("ClubList", lId) as Dictionary<string, string>, null);

            return Json(new { clubLists }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, OverrideActionFilters]
        public async Task<JsonResult> GetCommissionCategoryByClub(string agentId)
        {
            var aId = !string.IsNullOrEmpty(agentId) ? agentId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId)) { return null; }
            var commissionLists = ApplicationUtilities.SetDDLValue(ApplicationUtilities
                .LoadDropdownList("CommissionCategoryViaClubDDL", aId) as Dictionary<string, string>, null);

            return Json(new { commissionLists }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region AdminCommission Section
        public ActionResult AdminCommissionList(string CategoryId, string CategoryName = "")
        {
            Session["CurrentURL"] = "/CommissionManagement/CategoryList";
            List<AdminCommissionModel> responseInfo = new List<AdminCommissionModel>();
            var dbAdminResponseInfo = _CategoryBuss.GetAdminCommissionList();
            responseInfo = dbAdminResponseInfo.MapObjects<AdminCommissionModel>();
            foreach (var item in responseInfo)
            {
                item.AdminCommissionTypeId = item.AdminCommissionTypeId.EncryptParameter();
            }
            ViewBag.CategoryId = CategoryId;
            ViewBag.CategoryName = CategoryName;
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/CommissionManagement/CategoryList";
            return View(responseInfo);
        }
        #endregion
    }
}