
using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ApiResponseMessage;
using CRS.ADMIN.APPLICATION.Models.ClubManagement;

using CRS.ADMIN.BUSINESS.ApiResponseMessage;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ApiResponseMessage;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.StaffManagement;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using System.Reflection;
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

        public ActionResult ApiResponseMessageList(string SearchFilter = "", string value = "", int StartIndex = 0, int PageSize = 10)
        {
            ViewBag.SearchFilter = SearchFilter;
            //Session["CurrentURL"] = "/ClubManagement/ClubList";
            string RenderId = "";
            ApiResponseModel obj = new ApiResponseModel();

            var response = new ApiResponseMessageModel();

            List<ApiResponseMessageModel> responseInfo = new List<ApiResponseMessageModel>();

            PaginationFilterCommon dbRequestall = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            var dbResponse = _BUSS.ApiResponseMessageList(dbRequestall);
            obj.ApiResponseMessageList = dbResponse.MapObjects<ApiResponseMessageModel>();
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

            return View(obj);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApiResponseMessageList(ApiResponseMessageModel model)
        {
            if (ModelState.IsValid)
            {
                ApiResponseMessageCommon commonModel = model.MapObject<ApiResponseMessageCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                //commonModel.ActionIP = ApplicationUtilities.GetIP();

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
                //commonModel.ActionIP = ApplicationUtilities.GetIP();

                var dbResponseInfo = _BUSS.UpdateApiResponseMessage(commonModel);

                if (dbResponseInfo.Code.ToString().ToUpper() != "SUCCESS")
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
                        Message = dbResponseInfo.Message ?? "Message Updated Successfully",
                        Title = NotificationMessage.SUCCESS.ToString()
                    });
                    return RedirectToAction("ApiResponseMessageList");
                }
            }

            TempData["ManageResponseModel"] = model;

            return RedirectToAction("ApiResponseMessageList");
        }
        [HttpGet]
        public async Task<ActionResult> UpdateResponseMessage(string id, string Code, string Category, string Message, string HttpStatusCode, string MessageEng, string Description, string Module, string UserCategory)
        {
            var request = new ApiResponseMessageModel()
            {
                MessageId = id,
                Code = Code,
                Category = Category,
                Message = Message,
                HttpStatusCode = HttpStatusCode,
                MessageEng = MessageEng,
                Description = Description,
                Module = Module,
                UserCategory = UserCategory
            };
            ApiResponseMessageModel model = new ApiResponseMessageModel();
            TempData["ManageResponseEditModel"] = request;
            TempData["RenderId"] = "Edit";
            return RedirectToAction("ApiResponseMessageList");

        }

        [HttpGet]
        public async Task<ActionResult> GetApiResponseMessage(string id, string Code, string Category, string Message, string HttpStatusCode, string MessageEng, string Description, string Module, string UserCategory)
        {
            var request = new ApiResponseMessageModel()
            {
                MessageId = id,
                Code = Code,
                Category = Category,
                Message = Message,
                HttpStatusCode = HttpStatusCode,
                MessageEng = MessageEng,
                Description = Description,
                Module = Module,
                UserCategory = UserCategory
            };
            ApiResponseMessageModel model = new ApiResponseMessageModel();
            TempData["ManageResponseGetModel"] = request;
            TempData["RenderId"] = "Get";
            return RedirectToAction("ApiResponseMessageList", new { value = "G" });

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