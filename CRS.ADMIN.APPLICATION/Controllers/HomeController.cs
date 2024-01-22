using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.Home;
using CRS.ADMIN.APPLICATION.Models.NotificationManagement;
using CRS.ADMIN.BUSINESS.Home;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.Home;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IHomeBusiness _BUSS;
        public HomeController(IHomeBusiness BUSS)
        {
            _BUSS = BUSS;
        }
        #region Login Management
        [OverrideActionFilters]
        public ActionResult Index()
        {
            var userName = ApplicationUtilities.GetSessionValue("Username").ToString();
            if (string.IsNullOrEmpty(userName))
            {
                this.ClearSessionData();
                LoginRequestModel ResponseModel = new LoginRequestModel();
                HttpCookie cookie = Request.Cookies["CRS-ADMIN-LOGINID"];
                if (cookie != null) ResponseModel.Username = cookie.Value.StaticDecryptData() ?? null;
                return View(ResponseModel);
            }
            return RedirectToAction("Dashboard", "Home");
        }

        [OverrideActionFilters]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(LoginRequestModel Model, bool RememberMe = false)
        {
            if (ModelState.IsValid)
            {
                var loginResponse = Login(Model);
                if (loginResponse.Item3 && !string.IsNullOrEmpty(Model.Username) && RememberMe)
                {
                    HttpContext.Response.Cookies.Add(new HttpCookie("CRS-ADMIN-LOGINID", Model.Username.StaticEncryptData())
                    {
                        Expires = DateTime.Now.AddMonths(1)
                    });
                }
                else
                {
                    var LoginId = string.Empty;
                    HttpCookie cookie = Request.Cookies["CRS-ADMIN-LOGINID"];
                    if (cookie != null) LoginId = cookie.Value.StaticDecryptData() ?? null;
                    HttpContext.Response.Cookies.Add(new HttpCookie("CRS-ADMIN-LOGINID", "")
                    {
                        Expires = DateTime.Now.AddMonths(-1)
                    });
                }
                return RedirectToAction(loginResponse.Item1, loginResponse.Item2);
            }
            else
            {
                var errorMessages = ModelState.Where(x => x.Value.Errors.Count > 0)
                                  .SelectMany(x => x.Value.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                                  .ToList();
                var notificationModels = errorMessages.Select(errorMessage => new NotificationModel
                {
                    NotificationType = NotificationMessage.ERROR,
                    Message = errorMessage,
                    Title = NotificationMessage.ERROR.ToString(),
                }).ToArray();
                AddNotificationMessage(notificationModels);
                return View(Model);
            }
        }

        public Tuple<string, string, bool> Login(LoginRequestModel Model)
        {
            LoginRequestCommon commonRequest = Model.MapObject<LoginRequestCommon>();
            commonRequest.SessionId = Session.SessionID;
            var dbResponse = _BUSS.Login(commonRequest);
            try
            {
                if (dbResponse.Code == 0)
                {
                    var response = dbResponse.Data.MapObject<LoginResponseModel>();
                    if (response.Notifications == null || response.Notifications.Count <= 0) response.Notifications = new List<NotificationDetailModel>();
                    Session["SessionGuid"] = commonRequest.SessionId;
                    Session["UserId"] = response.UserId.EncryptParameter();
                    Session["RoleId"] = response.RoleId.EncryptParameter();
                    Session["Username"] = response.Username;
                    Session["FullName"] = response.FullName;
                    Session["ProfileImage"] = response.ProfileImage;
                    Session["IsPasswordForceful"] = response.IsPasswordForceful;
                    Session["RoleName"] = response.RoleName;
                    Session["Menus"] = response.Menus;
                    Session["Functions"] = response.Functions;
                    Session["CreatedOn"] = DateTime.Now;
                    Session["LastPasswordChangeDate"] = response.LastPasswordChangedDate;
                    Session["Notifications"] = response.Notifications;
                    return new Tuple<string, string, bool>("Dashboard", "Home", true);
                }
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = dbResponse.Message,
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return new Tuple<string, string, bool>("Index", "Home", false);
            }
            catch (Exception)
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Something went wrong",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return new Tuple<string, string, bool>("Index", "Home", false);
            }
        }

        [OverrideActionFilters]
        public ActionResult LogOff()
        {
            Session["Username"] = null;
            return RedirectToAction("Index", "Home");
        }
        #endregion

        [HttpGet]
        public ActionResult Dashboard()
        {
            Session["CurrentUrl"] = "/Home/Dashboard";
            #region "Dashboard Top Info"
            var dbResponseInfo = _BUSS.GetDashboardAnalytic();
            var dashboardInfo = dbResponseInfo.MapObject<DashboardInfoModel>();
            string username = ApplicationUtilities.GetSessionValue("Username").ToString();
            dashboardInfo.Username = username;
            ViewBag.DashboardInfo = dashboardInfo;
            #endregion
            #region "Dashboard Host List Info"
            List<HostListModel> hostListInfo = new List<HostListModel>();
            string FileLocationPath = "";
            if (ConfigurationManager.AppSettings["Phase"] != null
               && ConfigurationManager.AppSettings["Phase"].ToString().ToUpper() != "DEVELOPMENT")
                FileLocationPath = ConfigurationManager.AppSettings["ImageVirtualPath"].ToString() + FileLocationPath;
            var dbHostListInfo = _BUSS.GetHostList();
            hostListInfo = dbHostListInfo.MapObjects<HostListModel>();
            hostListInfo.ForEach(x => x.HostImage = FileLocationPath + x.HostImage);
            ViewBag.HostList = hostListInfo;
            #endregion
            #region "Chart Info"
            List<ChartInfo> chartresponseInfo = new List<ChartInfo>();
            var dbChartResponse = _BUSS.GetChartInfoList();
            chartresponseInfo = dbChartResponse.MapObjects<ChartInfo>();
            ViewBag.ChartInfo = chartresponseInfo;
            #endregion
            #region "Top Booked Host Ranking"
            List<TopBookedHostRankingModel> topHostInfo = new List<TopBookedHostRankingModel>();
            var dbTopHostResponse = _BUSS.GetTopBookedHostList();
            topHostInfo = dbTopHostResponse.MapObjects<TopBookedHostRankingModel>();
            topHostInfo.ForEach(x => x.HostImage = FileLocationPath + x.HostImage);
            topHostInfo.ForEach(y => y.ClubImage = FileLocationPath + y.ClubImage);
            ViewBag.TopBookedHost = topHostInfo;
            #endregion
            #region "Sales Through"
            List<ReceivedAmountModel> recievedAmountInfo = new List<ReceivedAmountModel>();
            var dbTotalSaleResponse = _BUSS.GetReceivedAmount();
            recievedAmountInfo = dbTotalSaleResponse.MapObjects<ReceivedAmountModel>();
            ViewBag.RecievedAount = recievedAmountInfo;
            #endregion
            return View();
        }
    }
}