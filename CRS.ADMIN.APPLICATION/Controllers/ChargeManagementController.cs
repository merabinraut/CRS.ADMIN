using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ChargeManagement;
using CRS.ADMIN.APPLICATION.Models.PointSetup;
using CRS.ADMIN.BUSINESS.ChargeManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ChargeManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PointSetup;
using System.Collections;
using System.Collections.Generic;
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
            var filteredItems = dbResponseModel
           .Where(item => item.agentTypeValue == "6" )
           .ToList();

            response.ChargeTypeList= dbResponseModel.MapObjects<ChargeCategoryManagementModel>();
            response.ChargeTypeList.ForEach(x =>
            {
                x.agentTypeValue = x?.agentTypeValue?.EncryptParameter();
            });

            var dictionary = new Dictionary<string, string>();
            filteredItems.ForEach(item => { dictionary.Add(item.agentTypeValue.EncryptParameter(), item.agentType); });
            ViewBag.RoleTypeList = ApplicationUtilities.SetDDLValue(dictionary, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.UserList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("USERTYPENAME", response.ChargeTypeList.FirstOrDefault().agentTypeValue.DecryptParameter()) as Dictionary<string, string>,"", culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---", false);
            var emptydictionary = new Dictionary<string, string>();
            ViewBag.ChargeCategory = ApplicationUtilities.SetDDLValue(emptydictionary, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            return View(response);
        }
        public ActionResult ChargeCategoryType(string agentTypeValue,string agentType="" ,string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            Session["CurrentURL"] = "/ChargeManagement/Index";
            string RenderId = "";
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/ChargeManagement/Index";
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
            objChargeTypeModel.SearchFilter = SearchFilter;
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
                            agentTypeValue = agentTypeValue
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
                       
                        return RedirectToAction("ChargeCategoryType", "ChargeManagement", new
                        {
                            agentTypeValue = agentTypeValue
                        });
                    }
                  
                }
            }
            return RedirectToAction("ChargeCategoryType", "ChargeManagement", new
            {
                agentTypeValue = agentTypeValue
            });
        }
        #endregion
        #region ChargeCategoryDetail
        public ActionResult ChargeCategoryDetails(string agentTypeValue, string categoryId,string categoryName, string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            Session["CurrentURL"] = "/ChargeManagement/Index";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/ChargeManagement/ChargeCategoryType?agentTypeValue=" + agentTypeValue;
            var response = new ChargeCategoryDetailsModel();
            string RenderId = "";

            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
            };
            var dbResponseModel = _chargeManagementBusiness.GetCharge(categoryId.DecryptParameter(), null,dbRequest);
            response.ChargeCategoryDetailsList = dbResponseModel.MapObjects<ChargeManagementModel>();
            response.ChargeCategoryDetailsList.ForEach(x =>
            {
                x.categoryId = x?.categoryId?.EncryptParameter(); 
                x.categoryDetailId = x?.categoryDetailId?.EncryptParameter();
            });
            if (TempData.ContainsKey("ManageChargeDetailsModel")) response.ManageChargeDetails = TempData["ManageChargeDetailsModel"] as ManageChargeDetailsModel;
            else response.ManageChargeDetails = new ManageChargeDetailsModel();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;

            ViewBag.ChargeTypeDDLList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("COMMISSIONPERCENTAGETYPELIST", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.ChargeTypeIdKey = response.ManageChargeDetails.chargeType;
            response.categoryName = categoryName;
            response.categoryId = categoryId;
            response.agentTypeValue = agentTypeValue;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponseModel != null && dbResponseModel.Any() ? dbResponseModel[0].TotalRecords : 0;
            return View(response);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageChargeCategoryDetails(ManageChargeDetailsModel Model)
        {
            string ErrorMessage = string.Empty;

            if (string.IsNullOrEmpty(Model.categoryId) && !string.IsNullOrEmpty(Model.categoryId))
            {
                ErrorMessage = "Invalid details.";
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });

                return RedirectToAction("ChargeCategoryDetails", "ChargeManagement", new
                {
                    agentTypeValue=Model.agentTypeValue,
                    categoryId = Model.categoryId,
                    categoryName = Model.categoryName
                });
            }

            if (ModelState.IsValid)
            {
                ChargeManagementCommon commonModel = Model.MapObject<ChargeManagementCommon>();
                commonModel.categoryId = Model.categoryId.DecryptParameter();
                commonModel.chargeType = Model.chargeType.DecryptParameter();                               
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                if (!string.IsNullOrEmpty(commonModel.categoryDetailId))
                {
                    commonModel.categoryDetailId = Model.categoryDetailId.DecryptParameter();
                    if (string.IsNullOrEmpty(commonModel.categoryDetailId))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid category points details.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManageChargeDetailsModel"] = Model;
                        TempData["RenderId"] = "Manage";
                        return RedirectToAction("ChargeCategoryDetails", "ChargeManagement", new
                        {
                            agentTypeValue = Model.agentTypeValue,
                            categoryId = Model.categoryId,
                            categoryName = Model.categoryName
                        });
                    }
                    commonModel.sp = "sproc_admin_update_charge_category_detail";
                }
                else
                {
                    commonModel.sp = "sproc_admin_create_charge_category_detail";
                }
                var dbResponse = _chargeManagementBusiness.CreateCharge(commonModel);
                if (dbResponse != null && dbResponse.Code == 0)
                {

                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Success",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });
                    return RedirectToAction("ChargeCategoryDetails", "ChargeManagement", new
                    {
                        agentTypeValue = Model.agentTypeValue,
                        categoryId = Model.categoryId,
                        categoryName = Model.categoryName
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
                    TempData["ManageChargeDetailsModel"] = Model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("ChargeCategoryDetails", "ChargeManagement", new
                    {
                        agentTypeValue = Model.agentTypeValue,
                        categoryId = Model.categoryId,
                        categoryName = Model.categoryName
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
            TempData["ManageChargeDetailsModel"] = Model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("ChargeCategoryDetails", "ChargeManagement", new
            {
                agentTypeValue = Model.agentTypeValue,
                categoryId = Model.categoryId,
                categoryName = Model.categoryName
            });
        }

        [HttpGet]
        public ActionResult ManageChargeCategoryDetails(string agentTypeValue, string categoryId = "", string categoryDetailId = "",string categoryName="")
        {
            ManageChargeDetailsModel model = new ManageChargeDetailsModel();
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();

            if (!string.IsNullOrEmpty(categoryId))
            {
                if (!string.IsNullOrEmpty(categoryId))
                {
                    var id = categoryId.DecryptParameter();
                    var catid = categoryDetailId.DecryptParameter();
                    if (string.IsNullOrEmpty(id))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid details",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });

                        return RedirectToAction("ChargeCategoryDetails", "ChargeManagement", new
                        {
                            agentTypeValue = agentTypeValue,
                            categoryId = categoryId,
                            categoryName = categoryName
                        });
                    }
                    if (string.IsNullOrEmpty(categoryDetailId))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid category details",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });

                        return RedirectToAction("ChargeCategoryDetails", "ChargeManagement", new
                        {
                            agentTypeValue = agentTypeValue,
                            categoryId = categoryId,
                            categoryName = categoryName
                        });
                    }
                  
                    var dbResponse = _chargeManagementBusiness.GetChargeDetails(id, catid);
                    model = dbResponse.MapObject<ManageChargeDetailsModel>();
                    model.categoryDetailId = model.categoryDetailId.EncryptParameter();
                    model.categoryId = model.categoryId.EncryptParameter();
                    model.chargeType = model.chargeType.EncryptParameter();
                }
            }
            TempData["ManageChargeDetailsModel"] = model;
            TempData["RenderId"] = "Manage";


            return RedirectToAction("ChargeCategoryDetails", "ChargeManagement", new
            {
                agentTypeValue = agentTypeValue,
                categoryId = categoryId,
                categoryName = categoryName
            });
        }
        [HttpGet]
        public ActionResult ManageChargeDetailStatus(string agentTypeValue, string categoryId = "", string categoryDetailId = "", string categoryName = "",string status="")
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();

            if (!string.IsNullOrEmpty(categoryId))
            {
                if (!string.IsNullOrEmpty(categoryId))
                {
                    var id = categoryId.DecryptParameter();
                    var catid = categoryDetailId.DecryptParameter();
                    if (string.IsNullOrEmpty(id))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid role type details",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        return RedirectToAction("ChargeCategoryDetails", "ChargeManagement", new
                        {
                            agentTypeValue = agentTypeValue,
                            categoryId = categoryId,
                            categoryName = categoryName
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
                        return RedirectToAction("ChargeCategoryDetails", "ChargeManagement", new
                        {
                            agentTypeValue =agentTypeValue,
                            categoryId = categoryId,
                            categoryName = categoryName
                        });
                    }
                    ChargeStatusManagementCommon objCategoryCommon = new ChargeStatusManagementCommon();
                    objCategoryCommon.categoryId = id;
                    objCategoryCommon.categoryDetailId = catid;
                    objCategoryCommon.status = status;
                    objCategoryCommon.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                    objCategoryCommon.ActionIP = ApplicationUtilities.GetIP();
                    var dbResponse = _chargeManagementBusiness.ManageChargeStatus(objCategoryCommon);
                    if (dbResponse != null && dbResponse.Code == 0)
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                            Message = dbResponse.Message ?? "Failed",
                            Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                        });
                        return RedirectToAction("ChargeCategoryDetails", "ChargeManagement", new
                        {
                            agentTypeValue = agentTypeValue,
                            categoryId = categoryId,
                            categoryName = categoryName
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

                        return RedirectToAction("ChargeCategoryDetails", "ChargeManagement", new
                        {
                            agentTypeValue = agentTypeValue,
                            categoryId = categoryId,
                            categoryName = categoryName
                        });
                    }

                }
            }
            return RedirectToAction("ChargeCategoryDetails", "ChargeManagement", new
            {
                agentTypeValue = agentTypeValue,
                categoryId = categoryId,
                categoryName = categoryName
            });
        }

        #endregion
        #region Assign Charge Category
        [HttpGet, OverrideActionFilters]
        public JsonResult GetUserListByRoleTypeId(string RoleTypeId)
        {
            List<SelectListItem> userNameList = new List<SelectListItem>();
            List<SelectListItem> categoryNamelist = new List<SelectListItem>();
            RoleTypeId = !string.IsNullOrEmpty(RoleTypeId) ? RoleTypeId.DecryptParameter() : null;
            if (!string.IsNullOrEmpty(RoleTypeId))
                userNameList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("USERTYPENAME", RoleTypeId) as Dictionary<string, string>, "--- Select ---");
            categoryNamelist = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("POINTSCATEGORY", RoleTypeId) as Dictionary<string, string>, "--- Select ---");
            var data = new
            {
                userNameList = new SelectList(userNameList, "Value", "Text"),
                categoryNamelist = new SelectList(categoryNamelist, "Value", "Text")
            };

           
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        [HttpGet, OverrideActionFilters]
        public JsonResult GetcategoryByUserId(string agentTypeValue,string agentId)
        {
            string currentChargeCategory = null;
            List<SelectListItem> categoryNamelist = new List<SelectListItem>();
            agentTypeValue = !string.IsNullOrEmpty(agentTypeValue) ? agentTypeValue.DecryptParameter() : "6";
            agentId = !string.IsNullOrEmpty(agentId) ? agentId.DecryptParameter() : null;
            if (!string.IsNullOrEmpty(agentTypeValue))              
            categoryNamelist = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CHARGECATEGORY", agentTypeValue, agentId) as Dictionary<string, string>, "--- Select ---");
            var dbresponse=_chargeManagementBusiness.GetCurrentCategory(agentTypeValue, agentId);
            
            var data = new
            {
                currentChargeCategory= dbresponse.Extra1,
                categoryNamelist = new SelectList(categoryNamelist, "Value", "Text")
            };           
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AssingChargeCategory(ChargeTypeModel Model)
        {
            string ErrorMessage = string.Empty;
            if (string.IsNullOrEmpty(Model.agentTypeValue))
            {
                ErrorMessage = "agent type is required.";
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });

                return RedirectToAction("Index", "ChargeManagement");
            }
            if (string.IsNullOrEmpty(Model.AgentId))
            {
                ErrorMessage = "User name is required.";
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });

                return RedirectToAction("Index", "ChargeManagement");
            }

            if (string.IsNullOrEmpty(Model.newChargeCategoryId))
            {
                ErrorMessage = "New category name is required.";
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });

                return RedirectToAction("Index", "ChargeManagement");
            }

            if (ModelState.IsValid)
            {
                var dbResponse = new CommonDbResponse();
                PointSetupCommon objPointSetupCommon = new PointSetupCommon();                               
                    dbResponse = _chargeManagementBusiness.AssignCategory(Model.agentTypeValue.DecryptParameter(), Model.AgentId.DecryptParameter(), Model.newChargeCategoryId.DecryptParameter());
                if (dbResponse.Code == 0)
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Success",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
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
                }

                return RedirectToAction("Index", "ChargeManagement");

            }
            return RedirectToAction("Index", "ChargeManagement");
        }
        #endregion
    }
}