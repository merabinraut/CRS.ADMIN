using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.StaticDataManagement;
using CRS.ADMIN.BUSINESS.StaticDataManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.StaticDataManagement;
using System.Linq;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    [OverrideActionFilters]
    public class StaticDataManagementController : BaseController
    {
        private readonly IStaticDataManagementBusiness _business;
        public StaticDataManagementController(IStaticDataManagementBusiness business)
        {
            _business = business;
        }
        #region MANAGE STATIC DATA TYPE
        public ActionResult Index(string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            Session["CurrentURL"] = "/StaticDataManagement/Index";
            string RenderId = "";
            StaticDataManagement responseInfo = new StaticDataManagement();
            if (TempData.ContainsKey("ManageStaticDataType")) responseInfo.ManageStaticDataType = TempData["ManageStaticDataType"] as ManageStaticDataType;
            else responseInfo.ManageStaticDataType = new ManageStaticDataType();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            var dbResponseInfo = _business.GetStatiDataTypeList(dbRequest);
            responseInfo.GetStaticDataTypeList = dbResponseInfo.MapObjects<StaticDataTypeModel>();
            foreach (var item in responseInfo.GetStaticDataTypeList)
            {
                item.Id = item.Id.EncryptParameter();
            }
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponseInfo != null && dbResponseInfo.Any() ? dbResponseInfo[0].TotalRecords : 0;
            responseInfo.SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null;
            return View(responseInfo);
        }
        [HttpGet]
        public ActionResult ManageStaticDataType(string id = "")
        {
            ManageStaticDataType model = new ManageStaticDataType();
            if (!string.IsNullOrEmpty(id))
            {
                if (string.IsNullOrEmpty(id))
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                }
                string Id = id.DecryptParameter();
                var dbResponse = _business.GetStaticDataTypeDetail(Id);
                model = dbResponse.MapObject<ManageStaticDataType>();
                model.Id = Id.EncryptParameter();
            }
            TempData["ManageStaticDataType"] = model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("Index");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageStaticDataType(ManageStaticDataType model)
        {
            if (ModelState.IsValid)
            {
                ManageStaticDataTypeCommon commonModel = model.MapObject<ManageStaticDataTypeCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                if (!string.IsNullOrEmpty(commonModel.Id))
                    commonModel.Id = commonModel.Id.DecryptParameter();

                var dbResponse = _business.ManageStaticDataType(commonModel);
                if (dbResponse != null)
                {
                    if (dbResponse.Code == ResponseCode.Success)
                        AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.SUCCESS,
                            Message = dbResponse.Message ?? "Static Data Type Added Successfully",
                            Title = NotificationMessage.SUCCESS.ToString()
                        });
                    else
                        AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = dbResponse.Message ?? "Duplicate data found !",
                            Title = NotificationMessage.INFORMATION.ToString()
                        });
                    return RedirectToAction("Index");
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Something went wrong try again later",
                        Title = NotificationMessage.WARNING.ToString()
                    });
                    return RedirectToAction("Index");
                }
            }
            TempData["ManageStaticDataType"] = model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("Index");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteStaticDataType(string id = "")
        {
            if (!string.IsNullOrEmpty(id))
            {
                var request = new ManageStaticDataTypeCommon()
                {
                    ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
                    Id = id.DecryptParameter(),
                };
                var dbResponse = _business.DeleteStaticDataType(request);
                if (dbResponse == null)
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.WARNING,
                        Message = dbResponse.Message ?? "Something went wrong try again later",
                        Title = NotificationMessage.WARNING.ToString()
                    });
                    return Json(dbResponse.SetMessageInTempData(this));
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = dbResponse.Message ?? "Data has been deleted successfully",
                        Title = NotificationMessage.SUCCESS.ToString()
                    });
                    return Json(dbResponse.SetMessageInTempData(this));
                }
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = "Invalid Request",
                    Title = NotificationMessage.SUCCESS.ToString()
                });
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region MANAGE STATIC DATA
        public ActionResult StaticDataIndex(string staticDataTypeId = "")
        {
            Session["CurrentUrl"] = "/StaticDataManagement/StaticDataIndex";
            string RenderId = "";
            StaticDataManagement responseInfo = new StaticDataManagement();
            if (TempData.ContainsKey("ManageStaticData")) responseInfo.ManageStaticData = TempData["ManageStaticData"] as ManageStaticData;
            else responseInfo.ManageStaticData = new ManageStaticData();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            var dbResponseInfo = _business.GetStaticDataList(staticDataTypeId);
            responseInfo.GetStaticDataList = dbResponseInfo.MapObjects<StaticDataModel>();
            foreach (var item in responseInfo.GetStaticDataList)
            {
                item.Id = item.Id.EncryptParameter();
            }
            ViewBag.PopUpRenderValue1 = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            return View(responseInfo);
        }
        [HttpGet]
        public ActionResult ManageStaticData(string id = "", string staticDataTypeId = "")
        {
            ManageStaticData model = new ManageStaticData();
            if (!string.IsNullOrEmpty(id) || !string.IsNullOrEmpty(staticDataTypeId))
            {
                string Id = id.DecryptParameter();
                var dbResponse = _business.GetStaticDataDetail(Id);
                model = dbResponse.MapObject<ManageStaticData>();
                model.Id = Id.EncryptParameter();
                TempData["ManageStaticData"] = model;
                TempData["RenderId"] = "Manage";
                return RedirectToAction("StaticDataIndex", new { staticDataTypeId = staticDataTypeId });
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("StaticDataIndex");
            }

        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageStaticData(ManageStaticData Model)
        {
            if (ModelState.IsValid)
            {
                ManageStaticDataCommon commonModel = Model.MapObject<ManageStaticDataCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                if (!string.IsNullOrEmpty(commonModel.Id))
                    commonModel.Id = commonModel.Id.DecryptParameter();
                var dbResponse = _business.ManageStaticData(commonModel);
                if (dbResponse != null)
                {
                    if (dbResponse.Code == ResponseCode.Success)
                    {
                        AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.SUCCESS,
                            Message = dbResponse.Message ?? "",
                            Title = NotificationMessage.SUCCESS.ToString(),
                        });
                        return RedirectToAction("StaticDataIndex", new { staticDataTypeId = commonModel.StaticDataType });
                    }
                    else
                        AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = dbResponse.Message ?? "Duplicate data found !",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                    TempData["ManageStaticData"] = Model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("StaticDataIndex", new { staticDataTypeId = commonModel.StaticDataType });
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Something went wrong try again later",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("StaticDataIndex");
                }
            }
            else
            {
                TempData["ManageStaticData"] = Model;
                TempData["RenderId"] = "Manage";
                return RedirectToAction("StaticDataIndex", new { staticDataTypeId = Model.StaticDataType });
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteStaticData(string id = "")
        {
            if (!string.IsNullOrEmpty(id))
            {
                var request = new ManageStaticDataCommon()
                {
                    ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
                    Id = id.DecryptParameter(),
                };
                var dbResponse = _business.DeleteStaticData(request);
                if (dbResponse == null)
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.WARNING,
                        Message = dbResponse.Message ?? "Something went wrong try again later",
                        Title = NotificationMessage.WARNING.ToString()
                    });
                    return Json(dbResponse.SetMessageInTempData(this));
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = dbResponse.Message ?? "Static Data has been deleted successfully",
                        Title = NotificationMessage.SUCCESS.ToString()
                    });
                    return Json(dbResponse.SetMessageInTempData(this));
                }
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = "Invalid Request",
                    Title = NotificationMessage.SUCCESS.ToString()
                });
                return RedirectToAction("Index");
            }
        }

        #endregion
    }
}