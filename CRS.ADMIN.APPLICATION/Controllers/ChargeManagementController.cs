using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ChargeManagement;
using CRS.ADMIN.APPLICATION.Models.PointSetup;
using CRS.ADMIN.BUSINESS.ChargeManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ChargeManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PointSetup;
using System.Linq;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class ChargeManagementController : BaseController
    {
        private readonly IChargeManagementBusiness _chargeManagementBusiness;
        public ChargeManagementController(IChargeManagementBusiness chargeManagementBusiness)
        {
            _chargeManagementBusiness = chargeManagementBusiness;
        }

        #region Charge Category Management
        public ActionResult Index()
        {
            Session["CurrentURL"] = "/ChargeManagement/Index";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            var response = new ChargeTypeModel();
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = 0,
                Take = 10,               
            };
            var dbResponseModel = _chargeManagementBusiness.GetChargeCategory(null,null, dbRequest);
            response.ChargeTypeList= dbResponseModel.MapObjects<ChargeCategoryManagementModel>();
            response.ChargeTypeList.ForEach(x =>
            {
                x.agentTypeValue = x?.agentTypeValue?.EncryptParameter();
            });
            return View(response);
        }
        public ActionResult ChargeCategoryType(string agentTypeValue,string agentType="" ,string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            Session["CurrentURL"] = "/ChargeManagement/Index";
            string RenderId = "";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ChargeTypeModel objChargeTypeModel = new ChargeTypeModel();
            if (TempData.ContainsKey("ManageChargeCategoryModel")) objChargeTypeModel.ManageCategory = TempData["ManageChargeCategoryModel"] as ManageChargeCategoryModel;
            else objChargeTypeModel.ManageCategory = new ManageChargeCategoryModel();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            objChargeTypeModel.ManageCategory.agentTypeValue = agentTypeValue;
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            var dbResponseModel = _chargeManagementBusiness.GetChargeCategory(agentTypeValue?.DecryptParameter(),null, dbRequest);
            objChargeTypeModel.CategoryTypeList = dbResponseModel.MapObjects<ChargeCategoryManagementModel>();
            objChargeTypeModel.CategoryTypeList.ForEach(x =>
            {
                x.agentTypeValue = x?.agentTypeValue?.EncryptParameter();
                x.categoryId= x?.categoryId?.EncryptParameter();
            });
            ViewBag.RoleName = agentType;
            objChargeTypeModel.agentTypeValue = agentTypeValue;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponseModel != null && dbResponseModel.Any() ? dbResponseModel[0].TotalRecords : 0;
            return View(objChargeTypeModel);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageChargeCategory(ManageChargeCategoryModel Model)
        {
            string ErrorMessage = string.Empty;
            if (string.IsNullOrEmpty(Model.agentTypeValue))
            {
                ErrorMessage = "Charge Type is Required.";
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ChargeCategoryType", "ChargeManagement", new
                {
                    agentTypeValue = Model.agentTypeValue
                });
            }

            if (ModelState.IsValid)
            {
                ChargeCategoryManagementCommon commonModel = Model.MapObject<ChargeCategoryManagementCommon>();
                commonModel.agentType= Model.agentTypeValue.DecryptParameter();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                if (!string.IsNullOrEmpty(commonModel.categoryId))
                {
                    commonModel.categoryId = commonModel.categoryId.DecryptParameter();
                    if (string.IsNullOrEmpty(commonModel.categoryId))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid category details.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManageChargeCategoryModel"] = Model;
                        TempData["RenderId"] = "Manage";
                        return RedirectToAction("ChargeCategoryType", "ChargeManagement", new
                        {
                            agentTypeValue = Model.agentTypeValue
                        });
                    }
                    commonModel.sp = "sproc_admin_update_charge_category";
                }
                else
                {
                    commonModel.sp = "sproc_admin_create_charge_category";
                }
                var dbResponse = _chargeManagementBusiness.CreateChargeCategory(commonModel);
                if (dbResponse != null && dbResponse.Code == 0)
                {

                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });
                    return RedirectToAction("ChargeCategoryType", "ChargeManagement", new
                    {
                        agentTypeValue = Model.agentTypeValue
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
                    TempData["ManageChargeCategoryModel"] = Model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("ChargeCategoryType", "ChargeManagement", new
                    {
                        agentTypeValue = Model.agentTypeValue
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
            TempData["ManageChargeCategoryModel"] = Model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("ChargeCategoryType", "ChargeManagement", new
            {
                agentTypeValue = Model.agentTypeValue
            });
        }



        [HttpGet]
        public ActionResult ManageChargeCategory(string agentTypeValue = "", string categoryId = "")
        {
            ManageChargeCategoryModel model = new ManageChargeCategoryModel();
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();

            if (!string.IsNullOrEmpty(agentTypeValue))
            {
                if (!string.IsNullOrEmpty(categoryId))
                {
                    var id = agentTypeValue.DecryptParameter();
                    var catid = categoryId.DecryptParameter();
                    if (string.IsNullOrEmpty(id))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid role type details",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        return RedirectToAction("ChargeCategoryType", "ChargeManagement", new
                        {
                            agentTypeValue = agentTypeValue
                        });
                    }
                    if (string.IsNullOrEmpty(catid))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid category details",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        return RedirectToAction("ChargeCategoryType", "ChargeManagement", new
                        {
                            agentTypeValue = agentTypeValue
                        });
                    }
                    PaginationFilterCommon dbRequest = new PaginationFilterCommon()
                    {
                        Skip = 0,
                        Take = 10,
                    };
                    var dbResponse = _chargeManagementBusiness.GetChargeCategoryDetails(agentTypeValue?.DecryptParameter(),categoryId?.DecryptParameter(), dbRequest);
                    model = dbResponse.MapObject<ManageChargeCategoryModel>();
                    model.agentTypeValue = model.agentTypeValue.EncryptParameter();
                    model.categoryId = model.categoryId.EncryptParameter();
                }
            }
            TempData["ManageChargeCategoryModel"] = model;
            TempData["RenderId"] = "Manage";

            return RedirectToAction("ChargeCategoryType", "ChargeManagement", new
            {
                agentTypeValue = agentTypeValue
            });
        }
        [HttpGet]
        public ActionResult ManageChargeStatus(string agentTypeValue = "", string categoryId = "", string status = "")
        {
            CategoryModel model = new CategoryModel();
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();

            if (!string.IsNullOrEmpty(agentTypeValue))
            {
                if (!string.IsNullOrEmpty(categoryId))
                {
                    var id = agentTypeValue.DecryptParameter();
                    var catid = categoryId.DecryptParameter();
                    if (string.IsNullOrEmpty(id))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid role type details",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        return RedirectToAction("ChargeCategoryType", "ChargeManagement", new
                        {
                            agentTypeValue = agentTypeValue
                        });
                    }
                    if (string.IsNullOrEmpty(catid))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid charge category details",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        return RedirectToAction("ChargeCategoryType", "ChargeManagement", new
                        {
                            agentTypeValue = agentTypeValue
                        });
                    }
                    ChargeCategoryStatusManagementCommon objCategoryCommon = new ChargeCategoryStatusManagementCommon();
                    objCategoryCommon.categoryId = catid;
                    objCategoryCommon.status = status;
                    objCategoryCommon.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                    objCategoryCommon.ActionIP = ApplicationUtilities.GetIP();
                    var dbResponse = _chargeManagementBusiness.ManageChargeCategoryStatus(objCategoryCommon);
                    model = dbResponse.MapObject<CategoryModel>();
                    model.RoleTypeId = model.RoleTypeId.EncryptParameter();
                    model.CategoryId = model.CategoryId.EncryptParameter();
                }
            }
            return RedirectToAction("ChargeCategoryType", "ChargeManagement", new
            {
                agentTypeValue = agentTypeValue
            });
        }
        #endregion
    }
}