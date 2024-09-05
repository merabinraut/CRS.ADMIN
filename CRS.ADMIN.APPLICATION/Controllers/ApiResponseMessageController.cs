using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ApiResponseMessage;

using CRS.ADMIN.BUSINESS.ApiResponseMessage;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ApiResponseMessage;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.StaffManagement;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        public ActionResult ApiResponseMessageList(string SearchFilter = "", string value = "", int StartIndex = 0, int PageSize = 10, int StartIndex2 = 0, int PageSize2 = 10, int StartIndex3 = 0, int PageSize3 = 10)
        {
            ViewBag.SearchFilter = SearchFilter;
            //Session["CurrentURL"] = "/ClubManagement/ClubList";
            string RenderId = "";
            
            var response = new ApiResponseMessageModel();

            List<ApiResponseMessageModel> responseInfo = new List<ApiResponseMessageModel>();

            PaginationFilterCommon dbRequestall = new PaginationFilterCommon()
            {
                Skip = value == "" ? StartIndex : 0,
                Take = value == "" ? PageSize : 10,
                SearchFilter = value == "" ? !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null : null
            };

          
            var dbResponse = _BUSS.ApiResponseMessageList(dbRequestall);

            responseInfo = dbResponse.MapObjects<ApiResponseMessageModel>();
           
            //foreach (var item in responseInfo)
            //{
            //    item.MessageId = item.MessageId?.EncryptParameter();
                
            //}
            

            //ViewBag.Pref = DDLHelper.LoadDropdownList("PREF") as Dictionary<string, string>;
            //ViewBag.IdentificationType = DDLHelper.LoadDropdownList("DOCUMENTTYPE") as Dictionary<string, string>;
            

            
            ViewBag.StartIndex2 = StartIndex2;
            ViewBag.PageSize2 = PageSize2;
            ViewBag.StartIndex3 = StartIndex3;
            ViewBag.PageSize3 = PageSize3;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;

            ViewBag.TotalData = 10;//dbResponse != null && dbResponse.Any() ? dbResponse[0].Total : 0;

            response.SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null;

            return View(responseInfo);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StoreResponseMessage(ApiResponseMessageModel model)
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
        public ActionResult UpdateResponseMessage(ApiResponseMessageModel model) {
            if (ModelState.IsValid)
            {
                ApiResponseMessageCommon commonModel = model.MapObject<ApiResponseMessageCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                //commonModel.ActionIP = ApplicationUtilities.GetIP();

                var dbResponseInfo = _BUSS.UpdateApiResponseMessage(commonModel);
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
                        Message = dbResponseInfo.Message ?? "Message Updated Successfully",
                        Title = NotificationMessage.SUCCESS.ToString()
                    });
                    return RedirectToAction("ApiResponseMessageList");
                }
            }

            TempData["ManageResponseModel"] = model;

            return RedirectToAction("ApiResponseMessageList");
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