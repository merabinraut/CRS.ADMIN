using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.RecommendationManagement;
using CRS.ADMIN.BUSINESS.RecommendationManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.RecommendationManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class RecommendationManagementController : BaseController
    {
        private readonly IRecommendationManagementBusiness _business;
        public RecommendationManagementController(IRecommendationManagementBusiness business)
        {
            _business = business;
        }
        #region "Group"
        [HttpGet]
        public ActionResult Index()
        {
            Session["CurrentUrl"] = "/RecommendationManagement/Index";
            ManageGroupCommonModel responseInfo = new ManageGroupCommonModel();
            #region "Location List"
            var dbLocationRes = _business.GetLocationList();
            responseInfo.LocationList = dbLocationRes.MapObjects<LocationListModel>();
            foreach (var item in responseInfo.LocationList)
            {
                item.LocationId = item.LocationId.EncryptParameter();
            }
            #endregion

            #region "Recommendation Request List"
            var dbRecommendationRes = _business.GetRecommendationRequestList();
            responseInfo.RecommendationRequestList = dbRecommendationRes.MapObjects<RecommendationRequestListModel>();
            #endregion

            #region "Recommendation Analytics"
            var dbAnalyticsRes = _business.GetRecommendationAnalyticList();
            responseInfo.RecommendationAnalyticsList = dbAnalyticsRes.MapObjects<RecommendationAnalyticListModel>();
            #endregion
            return View(responseInfo);
        }
        [HttpGet]
        public ActionResult ManageGroup(string locationId = "")
        {
            #region "Group List"
            Session["CurrentUrl"] = "/RecommendationManagement/ManageGroup";
            string RenderId = "";
            string LocationId = locationId.DecryptParameter();
            ManageGroupCommonModel responseInfo = new ManageGroupCommonModel();
            if (TempData.ContainsKey("ManageGroupModel")) responseInfo.ManageGroupModel = TempData["ManageGroupModel"] as ManageGroup;
            else responseInfo.ManageGroupModel = new ManageGroup();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            var dbResponseInfo = _business.GetGroupList(LocationId);
            responseInfo.GroupListModel = dbResponseInfo.MapObjects<RecommendationGroupListModel>();
            foreach (var item in responseInfo.GroupListModel)
            {
                item.RequestedDate = DateTime.Parse(item.RequestedDate).ToString("MMM d, yyyy");
                item.GroupId = item.GroupId.EncryptParameter();
            }
            ViewBag.DisplayOrderDDL = ApplicationUtilities.LoadDropdownList("DISPLAYORDERDDL", "", "") as Dictionary<string, string>;
            #endregion

            #region "Shuffling Time List"
            if (TempData.ContainsKey("ManageShufflingTime")) responseInfo.ManageShufflingTime = TempData["ManageShufflingTime"] as ManageShufflingTime;
            else responseInfo.ManageShufflingTime = new ManageShufflingTime();
            var dbShufflingResponseInfo = _business.GetShufflingTimeList();
            responseInfo.ShufflingTimeListModels = dbShufflingResponseInfo.MapObjects<ShufflingTimeListModel>();
            #endregion
            ViewBag.LocationId = locationId;
            TempData["OriginalUrl"] = Request.Url.ToString();
            return View(responseInfo);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageGroup(ManageGroup Model, string DisplayOrder = "")
        {
            string originalUrl = TempData["OriginalUrl"] as string;
            Uri redirectURL = new Uri(originalUrl);
            string query = redirectURL.Query;
            var queryParams = HttpUtility.ParseQueryString(query);
            string locationId = queryParams["locationId"];
            if (ModelState.IsValid)
            {
                ManageGroupCommon commonModel = Model.MapObject<ManageGroupCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                if (!string.IsNullOrEmpty(DisplayOrder?.DecryptParameter())) ModelState.Remove("DisplayOrderId");
                commonModel.DisplayOrderId = DisplayOrder?.DecryptParameter();
                var dbGroupResponseInfo = _business.ManageGroup(commonModel);
                if (dbGroupResponseInfo == null)
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.WARNING,
                        Message = dbGroupResponseInfo.Message ?? "Something went wrong try again later",
                        Title = NotificationMessage.WARNING.ToString()
                    });
                    return RedirectToAction("ManageGroup", new { locationId = locationId });
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = dbGroupResponseInfo.Message ?? "Group Added Successfully",
                        Title = NotificationMessage.SUCCESS.ToString()
                    });
                    return RedirectToAction("ManageGroup", new { locationId = locationId });
                }
            }
            TempData["ManageGroupModel"] = Model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("ManageGroup", new { locationId = locationId }); ;
        }
        #endregion

        #region "Manage Shuffling Time"
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageShufflingTime(List<ShufflingTimeListModel> list, string dataString)
        {
            string originalUrl = TempData["OriginalUrl"] as string;
            Uri redirectURL = new Uri(originalUrl);
            string query = redirectURL.Query;
            var queryParams = HttpUtility.ParseQueryString(query);
            string locationId = queryParams["locationId"];
            var commonModel = new ManageShufflingTimeCommon()
            {
                ShufflingTimeCSValue = dataString,
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            if (dataString != null)
            {
                var dbResponseInfo = _business.ManageShufflingTime(commonModel);
                if (dbResponseInfo != null && dbResponseInfo.Code == ResponseCode.Success)
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = dbResponseInfo.Message ?? "Shuffling Time Updated",
                        Title = NotificationMessage.SUCCESS.ToString()
                    });
                    return RedirectToAction("ManageGroup", new { locationId = locationId });
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.WARNING,
                        Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                        Title = NotificationMessage.WARNING.ToString()
                    });
                    TempData["ManageShufflingTime"] = list;
                    TempData["RenderId"] = "ManageShuffle";
                    return RedirectToAction("ManageGroup", new { locationId = locationId });
                }
            }
            AddNotificationMessage(new NotificationModel()
            {
                NotificationType = NotificationMessage.WARNING,
                Message = "Invalid Detail",
                Title = NotificationMessage.WARNING.ToString()
            });
            TempData["ManageShufflingTime"] = list;
            TempData["RenderId"] = "ManageShuffle";
            return RedirectToAction("ManageGroup", new { locationId = locationId });
        }

        #endregion

        #region " Manage Club Request"
        [HttpGet]
        public ActionResult ClubRequestView(string GroupId = "", string locationId = "")
        {
            Session["CurrentUrl"] = "/RecommendationManagement/ClubRequestView";
            List<ClubRequestListModel> responseInfo = new List<ClubRequestListModel>();
            var clubLocationId = locationId.DecryptParameter();
            var dbResponseInfo = _business.GetClubRequestList(clubLocationId);
            responseInfo = dbResponseInfo.MapObjects<ClubRequestListModel>();
            ViewBag.GroupId = GroupId;
            ViewBag.LocationId = locationId;
            TempData["OriginalUrl"] = Request.Url.ToString();
            return View(responseInfo);
        }
        [HttpGet]
        public ActionResult ManageClubRecommendationRequest(string groupId = "", string locationId = "", string recommendationId = "")
        {
            #region "Get All Required DDL"
            var Groupid = groupId.DecryptParameter();
            var LocationId = locationId.DecryptParameter();
            ViewBag.ClubLocation = ApplicationUtilities.LoadDropdownList("LOCATIONDDL", LocationId, "") as Dictionary<string, string>;
            ViewBag.GroupDDLByLocationId = ApplicationUtilities.LoadDropdownList("GROUPDDLBYLOCATIONID", LocationId, "") as Dictionary<string, string>;
            ViewBag.ClubDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBLIST", LocationId, "") as Dictionary<string, string>, null, "--- Select ---");
            if (!string.IsNullOrEmpty(recommendationId))
            {
                ViewBag.HostDDLBYClubId = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("HOSTDDLBYCLUBID", "", "") as Dictionary<string, string>, null);
                ViewBag.DisplayOrderDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("DISPLAYORDERDDL", "", "") as Dictionary<string, string>, null);
                ViewBag.DisplayPageOrderDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("DISPLAYPAGEORDERDDL", "", "") as Dictionary<string, string>, null);
            }
            else
            {
                ViewBag.DisplayOrderDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("DISPLAYORDERDDL", "", "") as Dictionary<string, string>, null, "--- Select ---");
                ViewBag.HostDDLBYClubId = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("HOSTDDLBYCLUBID", "", "") as Dictionary<string, string>, null, "--- Select ---");
                ViewBag.DisplayPageOrderDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("DISPLAYPAGEORDERDDL", "", "") as Dictionary<string, string>, null);
            }
            #endregion

            #region "Get Recommendation Detail"
            RecommendationDetailCommonModel responseInfo = new RecommendationDetailCommonModel();
            if (!string.IsNullOrEmpty(recommendationId))
            {
                var dbResponseInfo = _business.GetRecommendationDetail(recommendationId, Groupid, LocationId);
                responseInfo.GetRecommendationDetailModel = dbResponseInfo.MapObject<ManageClubRecommendationRequest>();
                responseInfo.GetRecommendationDetailModel.RecommendationId = responseInfo.GetRecommendationDetailModel.RecommendationId.EncryptParameter();
                responseInfo.GetRecommendationDetailModel.LocationId = responseInfo.GetRecommendationDetailModel.LocationId.EncryptParameter();
                responseInfo.GetRecommendationDetailModel.DisplayOrderId = responseInfo.GetRecommendationDetailModel.DisplayOrderId.EncryptParameter();
                responseInfo.GetRecommendationDetailModel.ClubId = responseInfo.GetRecommendationDetailModel.ClubId.EncryptParameter();
                responseInfo.GetRecommendationDetailModel.GroupId = responseInfo.GetRecommendationDetailModel.GroupId.EncryptParameter();
                var dbDisplayPOrderInfo = _business.GetDisplayPageDetail(recommendationId);
                responseInfo.GetDisplayPOrderDetailModel = dbDisplayPOrderInfo.MapObjects<DisplayPageOrderDetailModel>();
                var dbHostDetailInfo = _business.GetHostRecommendationDetail(recommendationId);
                responseInfo.GetHostRecommendationDetailModel = dbHostDetailInfo.MapObjects<HostRecommendationDetailModel>();
                foreach (var item in responseInfo.GetHostRecommendationDetailModel)
                {
                    item.HostId = item.HostId.EncryptParameter();
                    item.OrderId = item.OrderId.EncryptParameter();
                }
                foreach (var item in responseInfo.GetDisplayPOrderDetailModel)
                {
                    item.DisplayPageId = item.DisplayPageId.EncryptParameter();
                }
                ViewBag.RecommendationId = recommendationId;
                return View(responseInfo);
            }
            else
            {
                return View(responseInfo);
            }
            #endregion
        }
        [HttpPost, OverrideActionFilters, ValidateAntiForgeryToken]
        public async Task<JsonResult> GetHostListByClubId(string clubid = "")
        {
            var cId = !string.IsNullOrEmpty(clubid) ? clubid.DecryptParameter() : null;
            List<SelectListItem> hostdDDL = new List<SelectListItem>();
            List<SelectListItem> displayOrder = new List<SelectListItem>();
            if (string.IsNullOrEmpty(cId)) { return null; }
            hostdDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("HOSTDDLBYCLUBID", cId, "") as Dictionary<string, string>, null);
            displayOrder = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("DISPLAYORDERDDL", "", "") as Dictionary<string, string>, null);
            return Json(new { hostdDDL, displayOrder }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageClubRecommendationRequest(ManageClubRecommendationRequest Model, string GroupDDLId = "", string ClubLocationId = "", string ClubId = "", string DisplayOrder = "", string[] DisplayPageOrderId = null, string[] HostDDLByClubId = null, string[] DisplayOrderDDL = null, string recommendationId = "")
        {
            string originalUrl = TempData["OriginalUrl"] as string;
            Uri redirectURL = new Uri(originalUrl);
            string query = redirectURL.Query;
            var queryParams = HttpUtility.ParseQueryString(query);
            string locationId = queryParams["locationId"];
            string groupId = queryParams["GroupId"];
            if (ModelState.IsValid)
            {
                ManageClubRecommendationRequestCommon commonModel = Model.MapObject<ManageClubRecommendationRequestCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                commonModel.GroupId = GroupDDLId.DecryptParameter();
                commonModel.LocationId = ClubLocationId.DecryptParameter();
                commonModel.ClubId = ClubId.DecryptParameter();
                commonModel.DisplayOrderId = DisplayOrder.DecryptParameter();
                commonModel.RecommendationId = recommendationId.DecryptParameter();
                var hostDDLByClubId = "";
                var displayOrderDDL = "";
                var displayPageOrderId = "";
                if (DisplayPageOrderId != null)
                {
                    foreach (var item in DisplayPageOrderId)
                    {
                        displayPageOrderId = displayPageOrderId + "," + item.DecryptParameter();
                    }
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.WARNING,
                        Message = "Please select Display Order Page",
                        Title = NotificationMessage.WARNING.ToString()
                    });
                }
                if (!string.IsNullOrEmpty(displayPageOrderId)) commonModel.DisplayPageId = displayPageOrderId.Substring(1);
                if (HostDDLByClubId != null)
                {
                    foreach (var item in HostDDLByClubId)
                    {
                        hostDDLByClubId = hostDDLByClubId + "," + item.DecryptParameter();
                    }
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.WARNING,
                        Message = "Please select Host",
                        Title = NotificationMessage.WARNING.ToString()
                    });
                }
                if (!string.IsNullOrEmpty(hostDDLByClubId)) hostDDLByClubId = hostDDLByClubId.Substring(1);
                if (DisplayOrderDDL != null)
                {
                    foreach (var item in DisplayOrderDDL)
                    {
                        displayOrderDDL = displayOrderDDL + "," + item.DecryptParameter();
                    }
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.WARNING,
                        Message = "Please select Display order",
                        Title = NotificationMessage.WARNING.ToString()
                    });
                }
                if (!string.IsNullOrEmpty(displayOrderDDL)) displayOrderDDL = displayOrderDDL.Substring(1);
                var HostRecommendationCSValue = "";
                if (!string.IsNullOrEmpty(hostDDLByClubId) && !string.IsNullOrEmpty(displayOrderDDL))
                {
                    var hostList = hostDDLByClubId.Split(',');
                    var displayList = displayOrderDDL.Split(',');
                    for (int i = 0; i < Math.Min(hostList.Length, displayList.Length); i++)
                    {
                        HostRecommendationCSValue += $"{hostList[i]}:{displayList[i]},";
                    }
                    HostRecommendationCSValue = HostRecommendationCSValue.TrimEnd(',');
                }
                if (!string.IsNullOrEmpty(HostRecommendationCSValue)) commonModel.HostRecommendationCSValue = HostRecommendationCSValue;
                if (!string.IsNullOrEmpty(commonModel.HostRecommendationCSValue) && !string.IsNullOrEmpty(displayPageOrderId))
                {
                    var dbResponseInfo = _business.ManageClubRecommendationRequest(commonModel);
                    if (dbResponseInfo != null && dbResponseInfo.Code == ResponseCode.Success)
                    {
                        AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.SUCCESS,
                            Message = dbResponseInfo.Message ?? "Recommendation assigned successfully",
                            Title = NotificationMessage.SUCCESS.ToString()

                        });
                        return RedirectToAction("ClubRequestView", new { GroupId = groupId, locationId = locationId });
                    }
                    else
                    {
                        AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.WARNING,
                            Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                            Title = NotificationMessage.WARNING.ToString()
                        });
                        return RedirectToAction("ClubRequestView", new { GroupId = groupId, locationId = locationId });
                    }
                }
            }
            return RedirectToAction("ClubRequestView", new { GroupId = groupId, locationId = locationId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteClubRecommendatioRequest(string recommendationId = "'")
        {
            var responseInfo = new CommonDbResponse();
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
            };
            var dbResponseInfo = _business.DeleteClubRecommendatioRequest(recommendationId, commonRequest);
            responseInfo = dbResponseInfo;
            if (dbResponseInfo == null)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.WARNING,
                    Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                    Title = NotificationMessage.WARNING.ToString()
                });
                return Json(responseInfo.SetMessageInTempData(this));
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponseInfo.Message ?? "Club Recommendation has been deleted successfully",
                    Title = NotificationMessage.SUCCESS.ToString()
                });

            }
            return Json(responseInfo.SetMessageInTempData(this));
        }
        #endregion
    }
}