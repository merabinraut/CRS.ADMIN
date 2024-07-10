using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.NotificationManagement;
using CRS.ADMIN.BUSINESS.NotificationManagement;
using CRS.ADMIN.SHARED.NotificationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CRS.ADMIN.APPLICATION.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SessionExpiryFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (ctx.Session["UserName"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary {
                        { "Controller", "Home" },
                        { "Action", "LogOff" }
                        });
            }
            else
            {
                var notificationBussiness = new NotificationManagementBusiness();
                var notificationList = new List<NotificationDetailCommon>();
                notificationList = notificationBussiness.GetNotification(ctx.Session["UserId"].ToString().DecryptParameter()).ToList();
                if (notificationList.Count > 0)
                {
                    notificationList.ForEach(x =>
                    {
                        x.NotificationId = x.NotificationId.EncryptParameter();
                        x.NotificationImageURL = ImageHelper.ProcessedImage(x.NotificationImageURL);
                        x.Time = !string.IsNullOrEmpty(x.Time) ? DateTime.Parse(x.Time).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : x.Time;
                        //x.UpdatedDate = !string.IsNullOrEmpty(x.UpdatedDate) ? DateTime.Parse(x.UpdatedDate).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : x.UpdatedDate;
                    });
                }
                ctx.Session["Notifications"] = notificationList.MapObjects<NotificationDetailModel>().ToList();
                var controllerName = string.Empty;
                var actionName = string.Empty;
                var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
                var dataTokens = HttpContext.Current.Request.RequestContext.RouteData.DataTokens;
                if (routeValues != null)
                {
                    if (routeValues.ContainsKey("action"))
                    {
                        actionName = routeValues["action"].ToString();
                    }
                    if (routeValues.ContainsKey("controller"))
                    {
                        controllerName = routeValues["controller"].ToString();
                    }
                    #region check function rights
                    var functions = ctx.Session["Functions"] as List<string>;
                    if ((controllerName.ToUpper() == "HOME" && (actionName.ToUpper() == "INDEX" || actionName.ToUpper() == "LOGOFF"))
                        || (controllerName.ToUpper() == "ERROR"))
                    { }
                    else
                    {
                        var func = functions.ConvertAll(x => x.ToUpper());
                        var actionUrl = "/" + (controllerName + "/" + actionName).ToUpper();
                        if (func.Contains(actionUrl) == false && func.Equals(actionUrl) == false)
                        {
                            filterContext.Result = new RedirectToRouteResult(
                                new RouteValueDictionary
                                {
                                        {"Controller", "ErrorManagement"},
                                        {"Action", "Error_403"}
                                });
                        }
                    }
                    #endregion check function rights
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}