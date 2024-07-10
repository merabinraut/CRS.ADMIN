using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.NotificationManagement;
using CRS.ADMIN.BUSINESS.NotificationManagement;
using CRS.ADMIN.SHARED;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using static Google.Apis.Requests.BatchRequest;

namespace CRS.ADMIN.APPLICATION.Filters
{
    public class ActivityLogFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext httpctx = HttpContext.Current;
            string url = httpctx.Request.Url.AbsoluteUri;
            string pageName = httpctx.Request.RequestContext.RouteData.GetRequiredString("action");
            var browserDetails = ApplicationUtilities.GetBrowserDetails();// httpctx.Request.Headers["User-Agent"];
            string username = httpctx.Session["UserName"] == null ? "system" : httpctx.Session["UserName"].ToString();
            string ip = ApplicationUtilities.GetIP();// httpctx.Request.UserHostAddress;
            AddActitivies(pageName, url, browserDetails, ip, "", username);
            //check session
            var getSession = ApplicationUtilities.GetSessionValue("SessionGuid").ToString();
            var getUserId = ApplicationUtilities.GetSessionValue("UserId").ToString().DecryptParameter();
            var roleid = ApplicationUtilities.GetSessionValue("RoleId").ToString().DecryptParameter();
            CommonDbResponse resp = CheckSession(getUserId, getSession, roleid);
            if (resp != null)
            {
                if (resp.Code != 0)
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary {
                        { "Controller", "Home" },
                        { "Action", "LogOff" }
                        });
                }
                var _notificationBuss = new NotificationManagementBusiness();
                var notifications = _notificationBuss.GetNotification(getUserId);
                if (notifications != null &&  notifications.Count>0)              
                    notifications.ForEach(x =>
                    {
                        x.NotificationId = x.NotificationId.EncryptParameter();
                        x.NotificationImageURL = ImageHelper.ProcessedImage(x.NotificationImageURL);
                        x.Time = !string.IsNullOrEmpty(x.Time) ? DateTime.Parse(x.Time).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : x.Time;
                        //x.UpdatedDate = !string.IsNullOrEmpty(x.UpdatedDate) ? DateTime.Parse(x.UpdatedDate).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : x.UpdatedDate;
                    });
                
                httpctx.Session["Notifications"] = notifications.MapObjects<NotificationDetailModel>().ToList() ;
            }
            base.OnActionExecuting(filterContext);
        }
        public CommonDbResponse AddActitivies(string page_name, string page_url, string browser, string ip, string logtype, string usernname)
        {
            CommonDbResponse cResponse = new CommonDbResponse();
            //ActivityLogBusiness ab = new ActivityLogBusiness();
            //ActivityLog al = new ActivityLog()
            //{
            //    page_name = page_name,
            //    page_url = page_url,
            //    browser_detail = browser,
            //    ipaddress = ip,
            //    log_type = logtype,
            //    user_name = usernname
            //};
            //cResponse = ab.InsertActivityLog(al);
            return cResponse;
        }
        public CommonDbResponse CheckSession(string UserId, string SessionId, string RoleId)
        {
            CommonDbResponse cResponse = new CommonDbResponse();
            //LoginBusiness buss = new LoginBusiness();
            //CheckSession model = new CheckSession()
            //{
            //    UserId = UserId,
            //    SessionId = SessionId,
            //    RoleId = RoleId
            //};
            //cResponse = buss.CheckUserSession(model);
            return cResponse;
        }
    }
}