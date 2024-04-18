
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ClubManagement;
using CRS.ADMIN.APPLICATION.Models.PointSetup;
using CRS.ADMIN.BUSINESS.ClubManagement;
using CRS.ADMIN.BUSINESS.PointSetup;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PointSetup;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult PointSetupUserTypeList(string SearchFilter = "",  int StartIndex = 0, int PageSize = 10)
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
           
            objPointSetupModel.UserTypeList = dbResponse.MapObjects<UserTypeModel>();            
                               
            if (dbResponse.Count > 0)
            {

                objPointSetupModel.UserTypeList.ForEach(x => x.RoleTypeId = !string.IsNullOrEmpty(x.RoleTypeId) ? x.RoleTypeId.EncryptParameter() : x.RoleTypeId);

            }
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            return View(objPointSetupModel);
        }
        [HttpGet]
        public ActionResult PointsCategoryList(string RoleTypeId ,string SearchFilter = "", string value = "", int StartIndex = 0, int PageSize = 10)
        {
            ViewBag.SearchFilter = null;
            Session["CurrentURL"] = "/PointSetup/ManagePointsCategory";
            string RenderId = "";
            var objPointSetupModel = new PointSetupModel();
            if (TempData.ContainsKey("ManageCategoryModel")) objPointSetupModel.ManageCategory = TempData["ManageCategoryModel"] as CategoryModel;
            else objPointSetupModel.ManageCategory = new CategoryModel();
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
                objPointSetupModel.CategoryList.ForEach(x => x.CategoryId = !string.IsNullOrEmpty(x.CategoryId) ? x.CategoryId.EncryptParameter() : x.RoleTypeId);

            }
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
        public ActionResult BlockUnblockCategory(string roleTypeId = "", string categoryId = "",string status = "")
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
                    objCategoryCommon.CategoryId = categoryId;
                    objCategoryCommon.RoleTypeId = roleTypeId;
                    objCategoryCommon.Status = status;
                    var dbResponse = _BUSS.BlockUnblockCategory(objCategoryCommon);
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
    }
}