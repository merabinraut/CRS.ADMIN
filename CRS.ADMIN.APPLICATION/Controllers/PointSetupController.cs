
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ClubManagement;
using CRS.ADMIN.APPLICATION.Models.PointSetup;
using CRS.ADMIN.BUSINESS.ClubManagement;
using CRS.ADMIN.BUSINESS.PointSetup;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PointSetup;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using static Google.Apis.Requests.BatchRequest;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class PointSetupController : BaseController
    {
        private readonly IPointSetupBusiness _BUSS;
        public PointSetupController(IPointSetupBusiness BUSS)
        {
            _BUSS = BUSS;
        }
        [HttpGet]
        public ActionResult PointSetupUserTypeList(string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            ViewBag.SearchFilter = null;
            Session["CurrentURL"] = "/PointSetup/PointSetupUserTypeList";
            //string RenderId = "";
            var objPointSetupModel = new PointSetupModel();
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            var dbResponse = _BUSS.GetUsertypeList(dbRequest);
            var filteredItems = dbResponse
            .Where(item => item.RoleTypeId == "3" || item.RoleTypeId == "4" || item.RoleTypeId == "6")
            .ToList();
            objPointSetupModel.UserTypeList = filteredItems.MapObjects<UserTypeModel>();

            if (dbResponse.Count > 0)
            {

                objPointSetupModel.UserTypeList.ForEach(x => x.RoleTypeId = !string.IsNullOrEmpty(x.RoleTypeId) ? x.RoleTypeId.EncryptParameter() : x.RoleTypeId);

            }
            var dictionary = new Dictionary<string, string>();
            objPointSetupModel.UserTypeList.ForEach(item => { dictionary.Add(item.RoleTypeId, item.RoleTypeName); });
            ViewBag.RoleTypeList = ApplicationUtilities.SetDDLValue(dictionary, null, "--- Select ---");
            var dictionaryempty = new Dictionary<string, string>();
            ViewBag.UserList = ApplicationUtilities.SetDDLValue(dictionaryempty, null, "--- Select ---");
            ViewBag.PointsCategoryList = ApplicationUtilities.SetDDLValue(dictionaryempty, null, "--- Select ---");
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            return View(objPointSetupModel);
        }
        #region Points category
        [HttpGet]
        public ActionResult PointsCategoryList(string RoleTypeId, string SearchFilter = "", string value = "", int StartIndex = 0, int PageSize = 10)
        {
            ViewBag.SearchFilter = null;
            Session["CurrentURL"] = "/PointSetup/ManagePointsCategory";
            string RenderId = "";
            var objPointSetupModel = new PointSetupModel();
            if (TempData.ContainsKey("ManageCategoryModel")) objPointSetupModel.ManageCategory = TempData["ManageCategoryModel"] as CategoryModel;
            else objPointSetupModel.ManageCategory = new CategoryModel();
            objPointSetupModel.ManageCategory.RoleTypeId = RoleTypeId;
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            var dbResponse = _BUSS.GetCategoryList(dbRequest, RoleTypeId.DecryptParameter());

            objPointSetupModel.CategoryList = dbResponse.MapObjects<CategoryModel>();
            if (dbResponse.Count > 0)
            {
                objPointSetupModel.CategoryList.ForEach(x => x.RoleTypeId = !string.IsNullOrEmpty(x.RoleTypeId) ? x.RoleTypeId.EncryptParameter() : x.RoleTypeId);
                objPointSetupModel.CategoryList.ForEach(x => x.CategoryId = !string.IsNullOrEmpty(x.CategoryId) ? x.CategoryId.EncryptParameter() : x.CategoryId);

            }
            ViewBag.RoleName = objPointSetupModel.CategoryList.FirstOrDefault().RoleName;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            objPointSetupModel.SearchFilter = null;
            return View(objPointSetupModel);
        }
        [HttpGet]
        public ActionResult ManageCategory(string roleTypeId = "", string categoryId = "")
        {
            CategoryModel model = new CategoryModel();
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();

            if (!string.IsNullOrEmpty(roleTypeId))
            {
                if (!string.IsNullOrEmpty(categoryId))
                {
                    var id = roleTypeId.DecryptParameter();
                    var catid = categoryId.DecryptParameter();
                    if (string.IsNullOrEmpty(id))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid role type details",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        return RedirectToAction("PointsCategoryList", "PointSetup", new
                        {
                            RoleTypeId = roleTypeId
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
                        return RedirectToAction("PointsCategoryList", "PointSetup", new
                        {
                            RoleTypeId = roleTypeId
                        });
                    }
                    var dbResponse = _BUSS.GetCategoryDetails(id, catid);
                    model = dbResponse.MapObject<CategoryModel>();
                    model.RoleTypeId = model.RoleTypeId.EncryptParameter();
                    model.CategoryId = model.CategoryId.EncryptParameter();
                }
            }
            TempData["ManageCategoryModel"] = model;
            TempData["RenderId"] = "Manage";

            return RedirectToAction("PointsCategoryList", "PointSetup", new
            {
                RoleTypeId = roleTypeId
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageCategory(CategoryModel Model)
        {
            string ErrorMessage = string.Empty;
            if (string.IsNullOrEmpty(Model.RoleTypeId))
            {
                ErrorMessage = "Role Type is Required.";
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ManagePointsCategory", "PointSetup", new
                {
                    RoleTypeId = Model.RoleTypeId
                });
            }

            if (ModelState.IsValid)
            {
                CategoryCommon commonModel = Model.MapObject<CategoryCommon>();
                commonModel.RoleTypeId = Model.RoleTypeId.DecryptParameter();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                if (!string.IsNullOrEmpty(commonModel.CategoryId))
                {
                    commonModel.CategoryId = commonModel.CategoryId.DecryptParameter();
                    if (string.IsNullOrEmpty(commonModel.CategoryId))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid category details.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManageCategoryModel"] = Model;
                        TempData["RenderId"] = "Manage";
                        return RedirectToAction("PointsCategoryList", "PointSetup", new
                        {
                            RoleTypeId = Model.RoleTypeId
                        });
                    }
                }
                var dbResponse = _BUSS.ManageCategory(commonModel);
                if (dbResponse != null && dbResponse.Code == 0)
                {

                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });
                    return RedirectToAction("PointsCategoryList", "PointSetup", new
                    {
                        RoleTypeId = Model.RoleTypeId
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
                    TempData["ManageCategoryModel"] = Model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("PointsCategoryList", "PointSetup", new
                    {
                        RoleTypeId = Model.RoleTypeId
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
            TempData["ManageCategoryModel"] = Model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("PointsCategoryList", "PointSetup", new
            {
                RoleTypeId = Model.RoleTypeId
            });
        }

        [HttpGet]
        public ActionResult BlockUnblockCategory(string roleTypeId = "", string categoryId = "", string status = "")
        {
            CategoryModel model = new CategoryModel();
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();

            if (!string.IsNullOrEmpty(roleTypeId))
            {
                if (!string.IsNullOrEmpty(categoryId))
                {
                    var id = roleTypeId.DecryptParameter();
                    var catid = categoryId.DecryptParameter();
                    if (string.IsNullOrEmpty(id))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid role type details",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        return RedirectToAction("PointsCategoryList", "PointSetup", new
                        {
                            RoleTypeId = roleTypeId
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
                        return RedirectToAction("PointsCategoryList", "PointSetup", new
                        {
                            RoleTypeId = roleTypeId
                        });
                    }
                    CategoryCommon objCategoryCommon = new CategoryCommon();
                    objCategoryCommon.CategoryId = catid;
                    objCategoryCommon.RoleTypeId = id;
                    objCategoryCommon.Status = status;
                    objCategoryCommon.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                    objCategoryCommon.ActionIP = ApplicationUtilities.GetIP();
                    var dbResponse = _BUSS.BlockUnblockCategory(objCategoryCommon);
                    model = dbResponse.MapObject<CategoryModel>();
                    model.RoleTypeId = model.RoleTypeId.EncryptParameter();
                    model.CategoryId = model.CategoryId.EncryptParameter();
                }
            }
            return RedirectToAction("PointsCategoryList", "PointSetup", new
            {
                RoleTypeId = roleTypeId
            });
        }

        #endregion

        #region Points setup
        [HttpGet]
        public ActionResult PointsCategorySlabList(string roleTypeId, string categoryId, string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            ViewBag.SearchFilter = null;
            Session["CurrentURL"] = "/PointSetup/ManagePointsCategory";
            string RenderId = "";
            var objPointSetupModel = new PointSetupModel();
            if (TempData.ContainsKey("ManageCategorySlab")) objPointSetupModel.ManageCategorySlab = TempData["ManageCategorySlab"] as CategorySlabModel;
            else objPointSetupModel.ManageCategorySlab = new CategorySlabModel();
            objPointSetupModel.ManageCategorySlab.RoleTypeId = roleTypeId;
            objPointSetupModel.ManageCategorySlab.CategoryId = categoryId;
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };

            ViewBag.Categorys = ApplicationUtilities.LoadDropdownList("POINTSCATEGORY", roleTypeId.DecryptParameter()) as Dictionary<string, string>;
            ViewBag.Category = !string.IsNullOrEmpty(objPointSetupModel.ManageCategorySlab.CategoryId) ? ViewBag.Categorys[objPointSetupModel.ManageCategorySlab.CategoryId] : null;

            //Vievar wBag.Category = selectedItems.Select(item => item.Text).ToString();
            
            var dbResponse = _BUSS.GetCategorySlabList(dbRequest, roleTypeId.DecryptParameter(), categoryId.DecryptParameter());

            objPointSetupModel.CategorySlabList = dbResponse.MapObjects<CategorySlabModel>();
            if (dbResponse.Count > 0)
            {
                objPointSetupModel.CategorySlabList.ForEach(x => x.RoleTypeId = !string.IsNullOrEmpty(x.RoleTypeId) ? x.RoleTypeId.EncryptParameter() : x.RoleTypeId);
                objPointSetupModel.CategorySlabList.ForEach(x => x.CategoryId = !string.IsNullOrEmpty(x.CategoryId) ? x.CategoryId.EncryptParameter() : x.CategoryId);
                objPointSetupModel.CategorySlabList.ForEach(x => x.CategorySlabId = !string.IsNullOrEmpty(x.CategorySlabId) ? x.CategorySlabId.EncryptParameter() : x.CategorySlabId);

            }
            ViewBag.PointTypeDDLList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("COMMISSIONPERCENTAGETYPELIST") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.PointTypeIdKey = objPointSetupModel.ManageCategorySlab.PointType;
            ViewBag.PointTypeIdKey = objPointSetupModel.ManageCategorySlab.PointType;
            if (objPointSetupModel.ManageCategorySlab.RoleTypeId.DecryptParameter() == "6")
            {
                ViewBag.PointTypeIdKey2 = objPointSetupModel.ManageCategorySlab.PointType2;
                ViewBag.PointTypeIdKey3 = objPointSetupModel.ManageCategorySlab.PointType3;
            }
     
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            objPointSetupModel.SearchFilter = null;
            return View(objPointSetupModel);
        }
        [HttpGet]
        public ActionResult ManageCategoryPointsSlab(string roleTypeId = "", string categoryId = "", string categorySlabId = "")
        {
            CategorySlabModel model = new CategorySlabModel();
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();

            if (!string.IsNullOrEmpty(roleTypeId))
            {
                if (!string.IsNullOrEmpty(categoryId))
                {
                    var id = roleTypeId.DecryptParameter();
                    var catid = categoryId.DecryptParameter();
                    if (string.IsNullOrEmpty(id))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid role type details",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManageCategorySlab"] = model;
                        TempData["RenderId"] = "Manage";
                        return RedirectToAction("PointsCategorySlabList", "PointSetup", new
                        {
                            RoleTypeId = roleTypeId,
                            CategoryId = categoryId
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
                        TempData["ManageCategorySlab"] = model;
                        TempData["RenderId"] = "Manage";
                        return RedirectToAction("PointsCategorySlabList", "PointSetup", new
                        {
                            RoleTypeId = roleTypeId,
                            CategoryId = categoryId
                        });
                    }
                    if (!string.IsNullOrEmpty(categorySlabId))
                    {
                        var categoryslabid = categorySlabId.DecryptParameter();
                        var dbResponse = _BUSS.GetCategorySlabDetails(id, catid, categoryslabid);
                        model = dbResponse.MapObject<CategorySlabModel>();
                    }
                    if (model.RoleTypeId == "6")
                    {
                        ViewBag.PointTypeIdKey2 = model.PointType2;
                        ViewBag.PointTypeIdKey3 = model.PointType3;
                    }
                    model.RoleTypeId = model.RoleTypeId.EncryptParameter();
                    model.CategorySlabId = model.CategorySlabId.EncryptParameter();
                    model.CategoryId = model.CategoryId.EncryptParameter();
                    model.PointType = model.PointType.EncryptParameter();
                    model.PointType2 = model.PointType2.EncryptParameter();
                    model.PointType3 = model.PointType3.EncryptParameter();

                }
            }
            TempData["ManageCategorySlab"] = model;
            TempData["RenderId"] = "Manage";

            return RedirectToAction("PointsCategorySlabList", "PointSetup", new
            {
                RoleTypeId = roleTypeId,
                CategoryId = categoryId
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageCategoryPointsSlab(CategorySlabModel Model)
        {
            string ErrorMessage = string.Empty;
            if (string.IsNullOrEmpty(Model.RoleTypeId) && !string.IsNullOrEmpty(Model.CategoryId))
            {
                ErrorMessage = "Invalid details.";
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });

                return RedirectToAction("PointsCategorySlabList", "PointSetup", new
                {
                    RoleTypeId = Model.RoleTypeId,
                    CategoryId = Model.CategoryId
                });
            }

            if (ModelState.IsValid)
            {
                CategorySlabCommon commonModel = Model.MapObject<CategorySlabCommon>();
                commonModel.RoleTypeId = Model.RoleTypeId.DecryptParameter();
                commonModel.PointType = Model.PointType.DecryptParameter();
                if (commonModel.RoleTypeId == "6")
                {
                    commonModel.PointType2 = Model.PointType2.DecryptParameter();
                    commonModel.PointType3 = Model.PointType3.DecryptParameter();
                }

                commonModel.CategoryId = Model.CategoryId.DecryptParameter();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                if (!string.IsNullOrEmpty(commonModel.CategorySlabId))
                {
                    commonModel.CategorySlabId = Model.CategorySlabId.DecryptParameter();
                    if (string.IsNullOrEmpty(commonModel.CategorySlabId))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid category points details.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManageCategorySlab"] = Model;
                        TempData["RenderId"] = "Manage";
                        return RedirectToAction("PointsCategorySlabList", "PointSetup", new
                        {
                            RoleTypeId = Model.RoleTypeId,
                            CategoryId = Model.CategoryId
                        });
                    }
                }
                var dbResponse = _BUSS.ManageCategorySlab(commonModel);
                if (dbResponse != null && dbResponse.Code == 0)
                {

                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Success",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });
                    return RedirectToAction("PointsCategorySlabList", "PointSetup", new
                    {
                        RoleTypeId = Model.RoleTypeId,
                        CategoryId = Model.CategoryId
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
                    TempData["ManageCategorySlab"] = Model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("PointsCategorySlabList", "PointSetup", new
                    {
                        RoleTypeId = Model.RoleTypeId,
                        CategoryId = Model.CategoryId
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
            TempData["ManageCategorySlab"] = Model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("PointsCategorySlabList", "PointSetup", new
            {
                RoleTypeId = Model.RoleTypeId,
                CategoryId = Model.CategoryId
            });
        }

        [HttpGet]
        public ActionResult DeleteCategoryPointsSlab(string roleTypeId = "", string categoryId = "", string categorySlabId = "")
        {
            CategoryModel model = new CategoryModel();
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();

            if (string.IsNullOrEmpty(roleTypeId) && string.IsNullOrEmpty(categoryId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid category details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("PointsCategorySlabList", "PointSetup", new
                {
                    RoleTypeId = roleTypeId,
                    CategoryId = categoryId
                });
            }
            var roleid = roleTypeId.DecryptParameter();
            var catid = categoryId.DecryptParameter();
            if (string.IsNullOrEmpty(categorySlabId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid category details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("PointsCategoryList", "PointSetup", new
                {
                    RoleTypeId = roleTypeId,
                    CategoryId = categoryId
                });
            }
            var categoryslabid = categorySlabId.DecryptParameter();
            CategorySlabCommon objCategorySlabCommon = new CategorySlabCommon();
            objCategorySlabCommon.CategoryId = catid;
            objCategorySlabCommon.CategorySlabId = categoryslabid;
            objCategorySlabCommon.RoleTypeId = roleid;
            objCategorySlabCommon.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
            objCategorySlabCommon.ActionIP = ApplicationUtilities.GetIP();
            objCategorySlabCommon.Status = "D";
            var dbResponse = _BUSS.DeleteCategorySlab(objCategorySlabCommon);
            model = dbResponse.MapObject<CategoryModel>();
            model.RoleTypeId = model.RoleTypeId.EncryptParameter();
            model.CategoryId = model.CategoryId.EncryptParameter();

            return RedirectToAction("PointsCategorySlabList", "PointSetup", new
            {
                RoleTypeId = roleTypeId,
                CategoryId = categoryId
            });
        }
        #endregion

        #region Assign points setup

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

            //ViewBag.AgentIdKey = userNameList;
            //ViewBag.NewCategoryIdKey = categoryNamelist;
            // Allow GET requests
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        [HttpGet, OverrideActionFilters]
        public JsonResult GetPointsCategoryListByAgentId(string RoleTypeId, string AgentId)
        {
            List<SelectListItem> userNameList = new List<SelectListItem>();
            List<SelectListItem> categoryNamelist = new List<SelectListItem>();
            RoleTypeId = !string.IsNullOrEmpty(RoleTypeId) ? RoleTypeId.DecryptParameter() : null;
            AgentId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            PointSetupCommon objPointSetupCommon = new PointSetupCommon();
            objPointSetupCommon.UserTypeId = RoleTypeId;
            var currentcategory = new CommonDbResponse();
            var currentcategoryname = string.Empty;
            objPointSetupCommon.AgentId = AgentId;
            if (!(string.IsNullOrEmpty(RoleTypeId)) && !(string.IsNullOrEmpty(AgentId)))
                currentcategory = _BUSS.AssignCategory(objPointSetupCommon);
            if (currentcategory.Code == 0)
            {
                currentcategoryname = currentcategory.Message;
            }

            return Json(currentcategoryname, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AssingPointsCategory(PointSetupModel Model)
        {
            string ErrorMessage = string.Empty;
            if (string.IsNullOrEmpty(Model.UserTypeId))
            {
                ErrorMessage = "Role type is required.";
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });

                return RedirectToAction("PointSetupUserTypeList", "PointSetup");
            }
            if(string.IsNullOrEmpty(Model.AgentId))
            {
                ErrorMessage = "User name is required.";
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });

                return RedirectToAction("PointSetupUserTypeList", "PointSetup");
            }

            if(string.IsNullOrEmpty(Model.NewCategoryId))
            {
                ErrorMessage = "New category name is required.";
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });

                return RedirectToAction("PointSetupUserTypeList", "PointSetup");
            }

            if (ModelState.IsValid)
            {
                var dbResponse = new CommonDbResponse();
                PointSetupCommon objPointSetupCommon = new PointSetupCommon();
                objPointSetupCommon.UserTypeId = Model.UserTypeId.DecryptParameter();
                objPointSetupCommon.NewCategoryId = Model.NewCategoryId.DecryptParameter();
                objPointSetupCommon.AgentId = Model.AgentId.DecryptParameter();
                if (!(string.IsNullOrEmpty(Model.UserTypeId)) && !(string.IsNullOrEmpty(Model.NewCategoryId))&& !(string.IsNullOrEmpty(Model.AgentId)))
                    dbResponse = _BUSS.AssignCategory(objPointSetupCommon);
                if (dbResponse.Code== 0)
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
                
                return RedirectToAction("PointSetupUserTypeList", "PointSetup");

            }
            return RedirectToAction("PointSetupUserTypeList", "PointSetup");
        }
        #endregion
    }
}