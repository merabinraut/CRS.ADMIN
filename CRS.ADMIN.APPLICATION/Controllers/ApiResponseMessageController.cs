using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ApiResponseMessage;
using CRS.ADMIN.BUSINESS.ApiResponseMessage;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ApiResponseMessage;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.StaffManagement;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    //[OverrideActionFilters]
    public class ApiResponseMessageController : BaseController
    {

        private readonly IApiResponseMessageBusiness _BUSS;
        private readonly HttpClient _httpClient;
        public ApiResponseMessageController(IApiResponseMessageBusiness BUSS, HttpClient httpClient)
        {
            _BUSS = BUSS;
            _httpClient = httpClient;
        }

        [HttpGet]
        public ActionResult ApiResponseMessageList(string SearchFilter = "", string value = "", int StartIndex = 0, int PageSize = 10, string Category = "", string ModuleName = "", string UserCategory = "")
        {
            ViewBag.SearchFilter = SearchFilter;
            Session["CurrentURL"] = "/ApiResponseMessage/ApiResponseMessageList";
            string RenderId = "";
            ApiResponseModel obj = new ApiResponseModel();

            var response = new ApiResponseMessageModel();

            List<ApiResponseMessageModel> responseInfo = new List<ApiResponseMessageModel>();

            ApiResponseMessageFilterCommon dbRequestall = new ApiResponseMessageFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null,
                userCategory = !string.IsNullOrEmpty(UserCategory) ? UserCategory : null,
                category = !string.IsNullOrEmpty(Category) ? Category : null,
                moduleName = !string.IsNullOrEmpty(ModuleName) ? ModuleName : null,
            };
            var dbResponse = _BUSS.ApiResponseMessageList(dbRequestall);
            obj.ApiResponseMessageList = dbResponse.MapObjects<ApiResponseMessageModel>();
            if (dbResponse.Count > 0)
            {
                obj.ApiResponseMessageList.ForEach(x => x.MessageId = !string.IsNullOrEmpty(x.MessageId) ? x.MessageId.EncryptParameter() : x.MessageId);
            }
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            if (TempData.ContainsKey("ManageResponseEditModel")) obj.ManageResponse = TempData["ManageResponseEditModel"] as ApiResponseMessageModel;
            else obj.ManageResponse = new ApiResponseMessageModel();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();

            if (value == "G")
            {
                if (TempData.ContainsKey("ManageResponseGetModel")) obj.ManageResponse = TempData["ManageResponseGetModel"] as ApiResponseMessageModel;
                else obj.ManageResponse = new ApiResponseMessageModel();
                if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();

            }
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            response.SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null;
            ViewBag.UserCategoryKey = UserCategory;
            obj.moduleName = ModuleName;
            ViewBag.CategoryKey = Category;
            obj.SearchFilter = SearchFilter;
            return View(obj);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApiResponseMessageList(ApiResponseMessageModel model)
        {
            if (ModelState.IsValid)
            {
                ApiResponseMessageCommon commonModel = model.MapObject<ApiResponseMessageCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.MessageId = model.MessageId.DecryptParameter();
                var dbResponseInfo = _BUSS.StoreApiResponseMessage(commonModel);
                if (dbResponseInfo == null)
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.WARNING,
                        Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                        Title = NotificationMessage.WARNING.ToString()
                    });
                    return RedirectToAction("ApiResponseMessageList");
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = dbResponseInfo.Message ?? "Message Added Successfully",
                        Title = NotificationMessage.SUCCESS.ToString()
                    });
                    return RedirectToAction("ApiResponseMessageList");
                }
            }

            TempData["ManageResponseModel"] = model;
            return RedirectToAction("ApiResponseMessageList");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateResponseMessage(ApiResponseMessageModel model)
        {

            if (ModelState.IsValid)
            {
                ApiResponseMessageCommon commonModel = model.MapObject<ApiResponseMessageCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.MessageId = !string.IsNullOrEmpty(model.MessageId) ? model.MessageId.DecryptParameter() : null;

                var dbResponseInfo = _BUSS.UpdateApiResponseMessage(commonModel);

                if (dbResponseInfo.Code.ToString().ToUpper() != "SUCCESS")
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.WARNING,
                        Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                        Title = NotificationMessage.WARNING.ToString()
                    });
                    return RedirectToAction("ApiResponseMessageList", "ApiResponseMessage", new
                    {
                        SearchFilter = !string.IsNullOrEmpty(model.SearchFilter) ? model.SearchFilter : null,
                        StartIndex = model.StartIndex,
                        PageSize = model.PageSize,
                        Category = model.CategoryFilter,
                        ModuleName = model.ModuleNameFilter,
                        UserCategory = model.UserCategoryFilter

                    });
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = dbResponseInfo.Message ?? "Message Updated Successfully",
                        Title = NotificationMessage.SUCCESS.ToString()
                    });
                    return RedirectToAction("ApiResponseMessageList", "ApiResponseMessage", new
                    {
                        SearchFilter = !string.IsNullOrEmpty(model.SearchFilter) ? model.SearchFilter : null,
                        StartIndex = model.StartIndex,
                        PageSize = model.PageSize,
                        Category = model.CategoryFilter,
                        ModuleName = model.ModuleNameFilter,
                        UserCategory = model.UserCategoryFilter

                    });
                }
            }

            TempData["ManageResponseModel"] = model;
            return RedirectToAction("ApiResponseMessageList", "ApiResponseMessage", new
            {
                SearchFilter = !string.IsNullOrEmpty(model.SearchFilter) ? model.SearchFilter : null,
                StartIndex = model.StartIndex,
                PageSize = model.PageSize,
                Category = model.CategoryFilter,
                ModuleName = model.ModuleNameFilter,
                UserCategory = model.UserCategoryFilter

            });
        }
        [HttpGet]
        public async Task<ActionResult> UpdateResponseMessage(string id, string SearchFilter = "", string value = "", int StartIndex = 0, int PageSize = 10, string Category = "", string ModuleName = "", string UserCategory = "")
        {
            var messgaeId = "";
            if (!string.IsNullOrEmpty(id))
            {
                messgaeId = id.DecryptParameter();
                if (string.IsNullOrEmpty(messgaeId))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("ApiResponseMessageList", "ApiResponseMessage", new
                    {
                        SearchFilter = SearchFilter,
                        StartIndex = StartIndex,
                        PageSize = PageSize,
                        Category = Category,
                        ModuleName = ModuleName,
                        UserCategory = UserCategory,

                    });
                }
            }
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Message Id is required",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ApiResponseMessageList", "ApiResponseMessage", new
                {
                    SearchFilter = SearchFilter,
                    StartIndex = StartIndex,
                    PageSize = PageSize,
                    Category = Category,
                    ModuleName = ModuleName,
                    UserCategory = UserCategory,

                });
            }
            var dbResponse = _BUSS.ApiResponseMessageDetail(messgaeId);
            ApiResponseMessageModel request = new ApiResponseMessageModel();
            request = dbResponse.MapObject<ApiResponseMessageModel>();
            request.MessageId = id;
            request.SearchFilter = SearchFilter;
            request.ModuleNameFilter = ModuleName;
            request.UserCategoryFilter = UserCategory;
            request.CategoryFilter = Category;
            request.StartIndex = StartIndex;
            request.PageSize = PageSize;
            TempData["ManageResponseEditModel"] = request;
            TempData["RenderId"] = "Edit";
            return RedirectToAction("ApiResponseMessageList", "ApiResponseMessage", new
            {
                SearchFilter = SearchFilter,
                StartIndex = StartIndex,
                PageSize = PageSize,
                Category = Category,
                ModuleName = ModuleName,
                UserCategory = UserCategory,

            });
        }

        [HttpGet]
        public async Task<ActionResult> GetApiResponseMessage(string id, string SearchFilter = "", string value = "", int StartIndex = 0, int PageSize = 10, string Category = "", string ModuleName = "", string UserCategory = "")
        {
            var messgaeId = "";
            if (!string.IsNullOrEmpty(id))
            {
                messgaeId = id.DecryptParameter();
                if (string.IsNullOrEmpty(messgaeId))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("ApiResponseMessageList", "ApiResponseMessage", new
                    {
                        SearchFilter = SearchFilter,
                        StartIndex = StartIndex,
                        PageSize = PageSize,
                        Category = Category,
                        ModuleName = ModuleName,
                        UserCategory = UserCategory,

                    });
                }
            }
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Message Id is required",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ApiResponseMessageList", "ApiResponseMessage", new
                {
                    SearchFilter = SearchFilter,
                    StartIndex = StartIndex,
                    PageSize = PageSize,
                    Category = Category,
                    ModuleName = ModuleName,
                    UserCategory = UserCategory,

                });
            }
            var dbResponse = _BUSS.ApiResponseMessageDetail(messgaeId);
            ApiResponseMessageModel request = new ApiResponseMessageModel();
            request = dbResponse.MapObject<ApiResponseMessageModel>();
            request.MessageId = id;
            TempData["ManageResponseGetModel"] = request;
            TempData["RenderId"] = "Get";
            return RedirectToAction("ApiResponseMessageList", new
            {
                SearchFilter = SearchFilter,
                StartIndex = StartIndex,
                PageSize = PageSize,
                Category = Category,
                ModuleName = ModuleName,
                UserCategory = UserCategory,
                value = "G"
            });

        }

        //public ActionResult Index(string SearchFilter = "", string FromDate = "", string ToDate = "")
        //{
        //    Session["CurrentURL"] = "/EmailLog/Index";
        //    List<EmailLogModel> responseInfo = new List<EmailLogModel>();
        //    var dbResponseInfo = _buss.GetEmailLog(SearchFilter, FromDate, ToDate);
        //    responseInfo = dbResponseInfo.MapObjects<EmailLogModel>();
        //    return View(responseInfo);
        //}
    }
}