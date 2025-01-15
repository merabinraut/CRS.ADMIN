using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.RecommendationManagementV2;
using CRS.ADMIN.BUSINESS.RecommendationManagement_V2;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.RecommendationManagement_V2;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Google.Apis.Requests.BatchRequest;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class RecommendationManagementV2Controller : BaseController
    {
        private readonly IRecommendationManagementV2Business _business;
        public RecommendationManagementV2Controller(IRecommendationManagementV2Business business)
        {
            _business = business;
        }
        #region "Recommendation Request and Location Management"

        public ActionResult Index()
        {
            Session["CurrentURL"] = "/RecommendationManagementV2/Index";
            CommonRecommendationModel responseInfo = new CommonRecommendationModel();
            var dbLocationRes = _business.GetLocationList();
            responseInfo.GetLocationList = dbLocationRes.MapObjects<LocationListModel>();
            foreach (var item in responseInfo.GetLocationList)
            {
                item.LocationId = item.LocationId.EncryptParameter();
            }
            var dbClubRecommendationReq = _business.GetClubRecommendationReqList();
            responseInfo.GetClubRecommendationrequestList = dbClubRecommendationReq.MapObjects<ClubRecommendationManagementListModel>();
            foreach (var item in responseInfo.GetClubRecommendationrequestList)
            {
                item.RecommendationHoldId = item.RecommendationHoldId.EncryptParameter();
                item.ClubId = item.ClubId.EncryptParameter();
                item.DisplayId = item.DisplayId.EncryptParameter();
                item.LocationId = item.LocationId.EncryptParameter();
                item.ClubLogo = ImageHelper.ProcessedImage(item.ClubLogo);
            }
            return View(responseInfo);
        }
        #endregion

        #region "Display Pages"
        public ActionResult DisplayPageView(string locationid = "")
        {
            CommonRecommendationModel responseInfo = new CommonRecommendationModel();
            var dbDisplayPageRes = _business.GetDisplayPageList();
            responseInfo.GetDisplayPageList = dbDisplayPageRes.MapObjects<DisplayPageListModel>();
            foreach (var item in responseInfo.GetDisplayPageList)
            {
                item.PageId = item.PageId.EncryptParameter();
            }
            ViewBag.LocationId = locationid;
            TempData["OriginalUrl"] = Request.Url.ToString();
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/RecommendationManagementV2/Index";
            return View(responseInfo);
        }
        #endregion

        #region "Manage Group"
        public ActionResult GroupView(string locationid = "", string SearchFilter = "")
        {
            Session["CurrentUrl"] = "/RecommendationManagementV2/Index";
            ViewBag.SearchFilter = SearchFilter;
            string LocationId = "";
            string RenderId = "";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            if (!string.IsNullOrEmpty(locationid)) LocationId = locationid.DecryptParameter();
            CommonRecommendationModel responseInfo = new CommonRecommendationModel();
            if (TempData.ContainsKey("ManageGroupModel")) responseInfo.ManageGroup = TempData["ManageGroupModel"] as ManageGroup;
            else responseInfo.ManageGroup = new ManageGroup();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            var dbResponseInfo = _business.GetGroupList(LocationId, SearchFilter);
            responseInfo.GetGroupList = dbResponseInfo.MapObjects<GroupListModel>();
            foreach (var item in responseInfo.GetGroupList)
            {
                item.GroupId = item.GroupId.EncryptParameter();
            }
            if (TempData.ContainsKey("ManageShufflingTime")) responseInfo.ManageShufflingTime = TempData["ManageShufflingTime"] as ManageShufflingTime;
            else responseInfo.ManageShufflingTime = new ManageShufflingTime();
            var dbShufflingRes = _business.GetShufflingTimeList();
            responseInfo.GetShufflingTimeList = dbShufflingRes.MapObjects<ShufflingTimeListModel>();
            List<ShufflingTimeListModel> GroupList = new List<ShufflingTimeListModel>();
            List<ShufflingTimeListModel> SetShufflingTime = new List<ShufflingTimeListModel>();
            foreach (var item in responseInfo.GetShufflingTimeList)
            {
                if (item.LabelName == "Group Shuffling")
                {
                    GroupList.Add(item);
                }
                else
                {
                    SetShufflingTime.Add(item);
                }
            }
            ViewBag.GroupShuffling = GroupList;
            ViewBag.SetShufflingTime = SetShufflingTime;
            ViewBag.DisplayOrderDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("DISPLAYORDERDDL", "", "") as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---"); ;
            ViewBag.DisplayOrderDDLKey= responseInfo.ManageGroup.DisplayOrderId;
            TempData["OriginalUrl"] = Request.Url.ToString();
            ViewBag.LocationId = locationid;
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/RecommendationManagementV2/DisplayPageView?locationid=" + locationid;
            return View(responseInfo);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageGroup(ManageGroup Model, string displayOrderId = "")
        {
            string originalUrl = TempData["OriginalUrl"] as string;
            Uri redirectURL = new Uri(originalUrl);
            string query = redirectURL.Query;
            var queryParams = HttpUtility.ParseQueryString(query);
            string locationId = queryParams["locationId"];
            string pageid = queryParams["displaypageid"];
            if (ModelState.IsValid)
            {
                ManageGroupCommon commonModel = Model.MapObject<ManageGroupCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                commonModel.LocationId = locationId.DecryptParameter();
                if (!string.IsNullOrEmpty(displayOrderId?.DecryptParameter())) ModelState.Remove("DisplayOrderId");
                commonModel.DisplayOrderId = displayOrderId?.DecryptParameter();
                if (!string.IsNullOrEmpty(displayOrderId))
                {
                    var dbGroupResponseInfo = _business.ManageGroup(commonModel);
                    if (dbGroupResponseInfo != null && dbGroupResponseInfo.Code == ResponseCode.Success)
                    {
                        AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.SUCCESS,
                            Message = dbGroupResponseInfo.Message ?? "Group Added Successfully",
                            Title = NotificationMessage.SUCCESS.ToString()
                        });
                        return RedirectToAction("GroupView", new { pageid = pageid, locationId = locationId });
                    }
                    else
                    {
                        AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = dbGroupResponseInfo.Message ?? "Something went wrong try again later",
                            Title = NotificationMessage.INFORMATION.ToString()
                        });
                        TempData["ManageGroupModel"] = Model;
                        TempData["RenderId"] = "Manage";
                        return RedirectToAction("GroupView", new { pageid = pageid, locationId = locationId });
                    }
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Please select display order",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });
                    TempData["ManageGroupModel"] = Model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("GroupView", new { pageid = pageid, locationId = locationId });
                }
            }
            TempData["ManageGroupModel"] = Model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("GroupView", new { pageid = pageid, locationId = locationId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteGroup( string groupid = "",string locationid = "")
        {
            if (!string.IsNullOrEmpty(locationid)) locationid = locationid.DecryptParameter();
            if (!string.IsNullOrEmpty(groupid)) groupid = groupid.DecryptParameter();           
            if (string.IsNullOrEmpty(locationid)|| string.IsNullOrEmpty(groupid))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()

                });
                return RedirectToAction("GroupView", "RecommendationManagementV2");
            }
            var responseInfo = new CommonDbResponse();
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
            };
            var dbResponseInfo = _business.DeleteGroup(groupid, locationid, commonRequest);
            responseInfo = dbResponseInfo;
            if (dbResponseInfo != null && dbResponseInfo.Code == ResponseCode.Success)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponseInfo.Message ?? "Group has been deleted successfully",
                    Title = NotificationMessage.SUCCESS.ToString()
                });
                return Json(responseInfo.SetMessageInTempData(this));
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return Json(responseInfo.SetMessageInTempData(this));
            }
        }

        #endregion

        #region "Manage Shuffling"
        public ActionResult ManageShufflingTime(List<ShufflingTimeListModel> list, string dataString)
        {
            string originalUrl = TempData["OriginalUrl"] as string;
            Uri redirectURL = new Uri(originalUrl);
            string query = redirectURL.Query;
            var queryParams = HttpUtility.ParseQueryString(query);
            string locationId = queryParams["locationId"];
            string pageid = queryParams["pageid"];
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
                    return RedirectToAction("GroupView", new { pageid = pageid, locationId = locationId });
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
                    return RedirectToAction("GroupView", new { pageid = pageid, locationId = locationId });
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
            return RedirectToAction("GroupView", new { pageid = pageid, locationId = locationId });

        }
        #endregion

        #region "Club List"
        public ActionResult ClubListView(string locationid = "", string SearchFilter = "")
        {
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/RecommendationManagementV2/DisplayPageView?locationid=" + locationid;
            string LocationId = locationid.DecryptParameter();
            CommonRecommendationModel responseinfo = new CommonRecommendationModel();
            var dbResponseInfo = _business.GetClubRequestListByHomePage(LocationId, SearchFilter);
            responseinfo.GetClubRequestListByHomePage = dbResponseInfo.MapObjects<HomePageClubRequestListModel>();
            responseinfo.GetClubRequestListByHomePage.ForEach(x => x.ClubLogo = ImageHelper.ProcessedImage(x.ClubLogo));
            ViewBag.LocationId = locationid;
            ViewBag.SearchFilter = SearchFilter;
            TempData["OriginalUrl"] = Request.Url.ToString();
            return View(responseinfo);
        }
        public ActionResult ClubListForSearchPageView(string locationid = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/RecommendationManagementV2/DisplayPageView?locationid=" + locationid;
            string LocationId = locationid.DecryptParameter();
            CommonRecommendationModel responseinfo = new CommonRecommendationModel();
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            var dbResponseInfo = _business.GetClubRequestListBySearchPage(LocationId, dbRequest);
            responseinfo.GetClubRequestListBySearchPage = dbResponseInfo.MapObjects<SearchPageClubRequestListModel>();
            responseinfo.GetClubRequestListBySearchPage.ForEach(x => x.ClubLogo = ImageHelper.ProcessedImage(x.ClubLogo));
            ViewBag.LocationId = locationid;
            ViewBag.SearchFilter = SearchFilter;
            TempData["OriginalUrl"] = Request.Url.ToString();
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponseInfo != null && dbResponseInfo.Any() ? dbResponseInfo[0].TotalRecords : 0;
            return View(responseinfo);
        }
        public ActionResult ClubListForMainPage(string locationid = "", string groupid = "", string SearchFilter = "")
        {
            var LocationId = !string.IsNullOrEmpty(locationid) ? locationid.DecryptParameter() : null;
            var GroupId = !string.IsNullOrEmpty(groupid) ? groupid.DecryptParameter() : null;
            if (string.IsNullOrEmpty(LocationId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()

                });
                return RedirectToAction("Index", "RecommendationManagementV2");
            }
            if (string.IsNullOrEmpty(GroupId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()

                });
                return RedirectToAction("GroupView", "RecommendationManagementV2", new { locationid = locationid });
            }
            CommonRecommendationModel responseinfo = new CommonRecommendationModel();
            var dbResponseInfo = _business.GetClubRequestListByMainPage(LocationId, GroupId, SearchFilter);
            responseinfo.GetClubRequestListByMainPage = dbResponseInfo.MapObjects<MainPageClubRequestListModel>();
            foreach (var item in responseinfo.GetClubRequestListByMainPage)
            {
                item.ClubId = item.ClubId.EncryptParameter();
                item.DisplayId = item.DisplayId.EncryptParameter();
                item.RecommendationId = item.RecommendationId.EncryptParameter();
                item.ClubLogo = ImageHelper.ProcessedImage(item.ClubLogo);
            }
            ViewBag.LocationId = locationid;
            ViewBag.GroupId = groupid;
            ViewBag.SearchFilter = SearchFilter;
            TempData["OriginalUrl"] = Request.Url.ToString();
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/RecommendationManagementV2/GroupView?locationid=" + locationid;
            return View(responseinfo);
        }
        #endregion

        #region "Manage Recommendation Request"
        [HttpGet]
        public ActionResult ManageHomePageRequest(string locationid = "")
        {
            var id = !string.IsNullOrEmpty(locationid) ? locationid.DecryptParameter() : null;
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            if (string.IsNullOrEmpty(id))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()

                });
                return RedirectToAction("Index", "RecommendationManagementV2");
            }
            ViewBag.ClubLocation = ApplicationUtilities.LoadDropdownList("LOCATIONDDL", id, "") as Dictionary<string, string>;
            ViewBag.ClubDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBLIST", id, "") as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---"  );
            ViewBag.HostDDLBYClubId = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("HOSTDDLBYCLUBID", "", "") as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            var Response = new ManageHomePageRequest()
            {
                LocationId = locationid
            };
            return View(Response);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageHomePageRequest(ManageHomePageRequest Model)
        {
            var id = !string.IsNullOrEmpty(Model.LocationId) ? Model.LocationId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(id))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()

                });
                return RedirectToAction("Index", "RecommendationManagementV2");
            }
            ViewBag.ClubLocation = ApplicationUtilities.LoadDropdownList("LOCATIONDDL", id, "") as Dictionary<string, string>;
            ViewBag.ClubDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBLIST", id, "") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.HostDDLBYClubId = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("HOSTDDLBYCLUBID", "", "") as Dictionary<string, string>, null, "--- Select ---");
            if (ModelState.IsValid)
            {
                ManageHomePageRequestCommon commonModel = Model.MapObject<ManageHomePageRequestCommon>();
                commonModel.LocationId = id;
                commonModel.ClubId = commonModel.ClubId.DecryptParameter();
                commonModel.HostId = commonModel.HostId.DecryptParameter();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                var dbResponseInfo = _business.ManageHomePageRequest(commonModel);
                if (dbResponseInfo != null && dbResponseInfo.Code == ResponseCode.Success)
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = dbResponseInfo.Message ?? "Recommendation assigned successfully",
                        Title = NotificationMessage.SUCCESS.ToString()

                    });
                    return RedirectToAction("ClubListView", new { locationId = Model.LocationId });
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });
                    return View(Model);
                }
            }
            AddNotificationMessage(new NotificationModel()
            {
                NotificationType = NotificationMessage.INFORMATION,
                Message = "Please fill all required input fields",
                Title = NotificationMessage.INFORMATION.ToString()

            });
            return View(Model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteHomePageRequest(string clubid = "", string locationid = "", string displayid = "", string recommendationid = "")
        {
            string LocationId = "";
            if (!string.IsNullOrEmpty(locationid)) LocationId = locationid.DecryptParameter();
            if (string.IsNullOrEmpty(LocationId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()

                });
                return Json(JsonRequestBehavior.AllowGet);
            }
            var responseInfo = new CommonDbResponse();
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
            };
            var dbResponseInfo = _business.DeleteHomePageRequest(clubid, LocationId, displayid, recommendationid, commonRequest);
            responseInfo = dbResponseInfo;
            if (responseInfo != null && responseInfo.Code == ResponseCode.Success)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponseInfo.Message ?? "Recommendation assigned successfully",
                    Title = NotificationMessage.SUCCESS.ToString()

                });
                return Json(JsonRequestBehavior.AllowGet);
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return Json(JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ManageSearchPageRequest(string locationid = "")
        {
            var id = !string.IsNullOrEmpty(locationid) ? locationid.DecryptParameter() : null;
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            if (string.IsNullOrEmpty(id))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()

                });
                return RedirectToAction("Index", "RecommendationManagementV2");
            }
            ViewBag.ClubLocation = ApplicationUtilities.LoadDropdownList("LOCATIONDDL", id, "") as Dictionary<string, string>;
            ViewBag.ClubDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBLIST", id, "") as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.HostDDLBYClubId = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("HOSTDDLBYCLUBID", "", "") as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            var Response = new ManageSearchPageRequest
            {
                LocationId = locationid
            };
            return View(Response);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageSearchPageRequest(ManageSearchPageRequest Model)
        {
            var id = !string.IsNullOrEmpty(Model.LocationId) ? Model.LocationId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(id))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()

                });
                return RedirectToAction("Index", "RecommendationManagementV2");
            }
            ViewBag.ClubLocation = ApplicationUtilities.LoadDropdownList("LOCATIONDDL", id, "") as Dictionary<string, string>;
            ViewBag.ClubDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBLIST", id, "") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.HostDDLBYClubId = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("HOSTDDLBYCLUBID", "", "") as Dictionary<string, string>, null, "--- Select ---");
            if (ModelState.IsValid)
            {
                ManageSearchPageRequestCommon commonModel = Model.MapObject<ManageSearchPageRequestCommon>();
                commonModel.LocationId = commonModel.LocationId.DecryptParameter();
                commonModel.ClubId = commonModel.ClubId.DecryptParameter();
                commonModel.HostId = commonModel.HostId.DecryptParameter();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                var dbResponseInfo = _business.ManageSearchPageRequest(commonModel);
                if (dbResponseInfo != null && dbResponseInfo.Code == ResponseCode.Success)
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = dbResponseInfo.Message ?? "Recommendation assigned successfully",
                        Title = NotificationMessage.SUCCESS.ToString()

                    });
                    return RedirectToAction("ClubListForSearchPageView", new { locationId = Model.LocationId });
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });
                    return View(Model);
                }
            }
            AddNotificationMessage(new NotificationModel()
            {
                NotificationType = NotificationMessage.INFORMATION,
                Message = "Please fill all required input fields",
                Title = NotificationMessage.INFORMATION.ToString()

            });
            return View(Model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteSearchPageRequest(string clubid = "", string locationid = "", string displayid = "", string recommendationid = "")
        {
            string LocationId = "";
            if (!string.IsNullOrEmpty(locationid)) LocationId = locationid.DecryptParameter();
            var responseInfo = new CommonDbResponse();
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
            };
            var dbResponseInfo = _business.DeleteSearchPageRequest(clubid, LocationId, displayid, recommendationid, commonRequest);
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
        [HttpGet]
        public ActionResult ManageMainPageRequest(string groupId = "", string locationId = "", string displayId = "")
        {
            var LocationId = !string.IsNullOrEmpty(locationId) ? locationId.DecryptParameter() : null;
            var Groupid = groupId.DecryptParameter();
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ManageClubRecommendationRequest responseInfo = new ManageClubRecommendationRequest
            {
                GroupId = groupId,
                LocationId = locationId,
                DisplayId = displayId,
            };
            if (TempData.ContainsKey("ManageClubRecommendationRequest")) responseInfo = TempData["ManageClubRecommendationRequest"] as ManageClubRecommendationRequest;
            else responseInfo = new ManageClubRecommendationRequest();
            ViewBag.ClubDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBLIST", LocationId, "") as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.DisplayOrderDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("DISPLAYORDERDDL", "", "") as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.DisplayOrderDDLClub = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("DISPLAYORDERDDLCLUB", Groupid, "") as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.ClubId = responseInfo.ClubId;
            ViewBag.DisplayOrderId = responseInfo.DisplayOrderId;
            //ViewBag.DisplayOrderId = responseInfo.DisplayOrderClubId;
            if (string.IsNullOrEmpty(LocationId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()

                });
                return RedirectToAction("Index", "RecommendationManagementV2");
            }
            if (string.IsNullOrEmpty(Groupid))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()

                });
                return RedirectToAction("GroupView", "RecommendationManagementV2", new { locationid = locationId });
            }

            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/RecommendationManagementV2/ClubListForMainPage?locationid=" + locationId + "&groupid=" + groupId;
            #region "Get All Required DDL"
            ViewBag.ClubLocation = ApplicationUtilities.LoadDropdownList("LOCATIONDDL", LocationId, "") as Dictionary<string, string>;
            ViewBag.GroupDDLByLocationId = ApplicationUtilities.LoadDropdownList("GROUPDDLBYLOCATIONID", Groupid, "") as Dictionary<string, string>;
                        
            ViewBag.HostDDLBYClubId = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("HOSTDDLBYCLUBID", "", "") as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            #endregion

            #region "Get Recommendation Detail"
            TempData["OriginalUrl"] = Request.Url.ToString();
            
            return View(responseInfo);
            #endregion           
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageMainPageRequest(ManageClubRecommendationRequest Model)//, string[] HostDDLByClubId = null, string[] DisplayOrderDDL = null)
        {
            string originalUrl = TempData["OriginalUrl"] as string;
            Uri redirectURL = new Uri(originalUrl);
            string query = redirectURL.Query;
            var queryParams = HttpUtility.ParseQueryString(query);
            var LocationId = !string.IsNullOrEmpty(Model.LocationId) ? Model.LocationId.DecryptParameter() : null;
            var GroupId = !string.IsNullOrEmpty(Model.GroupId) ? Model.GroupId.DecryptParameter() : null;
            TempData["ManageClubRecommendationRequest"] = Model;
            if (string.IsNullOrEmpty(LocationId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()

                });
                return RedirectToAction("Index", "RecommendationManagementV2");

            }
            if (string.IsNullOrEmpty(GroupId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()

                });
                return RedirectToAction("GroupView", "RecommendationManagementV2", new { locationid = Model.LocationId });
            }
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/RecommendationManagementV2/ClubListForMainPage?locationid=" + Model.LocationId + "&groupid=" + Model.GroupId;
            #region "Get All Required DDL"
            ViewBag.ClubLocation = ApplicationUtilities.LoadDropdownList("LOCATIONDDL", LocationId, "") as Dictionary<string, string>;
            ViewBag.GroupDDLByLocationId = ApplicationUtilities.LoadDropdownList("GROUPDDLBYLOCATIONID", GroupId, "") as Dictionary<string, string>;
            ViewBag.ClubDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBLIST", LocationId, "") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.DisplayOrderDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("DISPLAYORDERDDL", "", "") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.HostDDLBYClubId = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("HOSTDDLBYCLUBID", "", "") as Dictionary<string, string>, null, "--- Select ---");
            #endregion
            if (ModelState.IsValid)
            {
                ManageClubRecommendationRequestCommon commonModel = Model.MapObject<ManageClubRecommendationRequestCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                commonModel.GroupId = commonModel.GroupId.DecryptParameter();
                commonModel.LocationId = commonModel.LocationId.DecryptParameter();
                commonModel.ClubId = commonModel.ClubId.DecryptParameter();
                commonModel.DisplayOrderId = commonModel.DisplayOrderId.DecryptParameter();
                commonModel.DisplayId = commonModel.DisplayId;
                var hostDDLByClubId = "";
                var displayOrderDDL = "";

                if (Model.HostDDLByClubId != null)
                {
                    foreach (var item in Model.HostDDLByClubId)
                    {
                        hostDDLByClubId = hostDDLByClubId + "," + item.DecryptParameter();
                    }
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Please select Host",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });
                    return RedirectToAction("ManageMainPageRequest", "RecommendationManagementV2", new { groupId = Model.GroupId, locationId = Model.LocationId, displayId = Model.DisplayId });
                    //return View(Model);
                }
                if (!string.IsNullOrEmpty(hostDDLByClubId)) hostDDLByClubId = hostDDLByClubId.Substring(1);
                if (Model.DisplayOrderDDL != null)
                {
                    foreach (var item in Model.DisplayOrderDDL)
                    {
                        displayOrderDDL = displayOrderDDL + "," + item.DecryptParameter();
                    }
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Please select Display order",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });
                    return RedirectToAction("ManageMainPageRequest", "RecommendationManagementV2", new { groupId = Model.GroupId, locationId = Model.LocationId, displayId = Model.DisplayId });
                    //return View(Model);
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
                if (!string.IsNullOrEmpty(commonModel.HostRecommendationCSValue))
                {
                    var dbResponseInfo = _business.ManageMainPageRequest(commonModel);
                    if (dbResponseInfo != null && dbResponseInfo.Code == ResponseCode.Success)
                    {
                        AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.SUCCESS,
                            Message = dbResponseInfo.Message ?? "Recommendation assigned successfully",
                            Title = NotificationMessage.SUCCESS.ToString()

                        });
                        return RedirectToAction("ClubListForMainPage", new { groupId = Model.GroupId, locationId = Model.LocationId });
                    }
                    else
                    {
                        AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                            Title = NotificationMessage.INFORMATION.ToString()
                        });
                        return RedirectToAction("ManageMainPageRequest", "RecommendationManagementV2", new { groupId = Model.GroupId, locationId = Model.LocationId, displayId = Model.DisplayId });
                        //return View(Model);
                    }
                }
            }
            AddNotificationMessage(new NotificationModel()
            {
                NotificationType = NotificationMessage.INFORMATION,
                Message = "Please fill all required fields",
                Title = NotificationMessage.INFORMATION.ToString()
            });
            return RedirectToAction("ManageMainPageRequest", "RecommendationManagementV2", new { groupId = Model.GroupId, locationId = Model.LocationId, displayId = Model.DisplayId });
            //return View(Model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteMainPageRequest(string clubid = "", string locationid = "", string displayid = "", string recommendationid = "", string groupid = "")
        {
            string LocationId = "";
            string GroupId = "";
            string ClubId = "";
            string DisplayId = "";
            string rId = "";
            if (!string.IsNullOrEmpty(groupid)) GroupId = groupid.DecryptParameter();
            if (!string.IsNullOrEmpty(locationid)) LocationId = locationid.DecryptParameter();
            if (!string.IsNullOrEmpty(displayid)) DisplayId = displayid.DecryptParameter();
            if (!string.IsNullOrEmpty(clubid)) ClubId = clubid.DecryptParameter();
            if (!string.IsNullOrEmpty(recommendationid)) rId = recommendationid.DecryptParameter();
            if (string.IsNullOrEmpty(LocationId) || string.IsNullOrEmpty(GroupId) || string.IsNullOrEmpty(DisplayId) || string.IsNullOrEmpty(ClubId) || string.IsNullOrEmpty(rId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()

                });
                return Json(JsonRequestBehavior.AllowGet);
            }
            var responseInfo = new CommonDbResponse();
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
            };
            var dbResponseInfo = _business.DeleteMainPageRequest(ClubId, LocationId, DisplayId, rId, commonRequest, GroupId);
            responseInfo = dbResponseInfo;
            if (dbResponseInfo != null && dbResponseInfo.Code == ResponseCode.Success)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponseInfo.Message ?? "Club Recommendation has been deleted successfully",
                    Title = NotificationMessage.SUCCESS.ToString()
                });
                return Json(responseInfo.SetMessageInTempData(this));
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return Json(responseInfo.SetMessageInTempData(this));
            }
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

        #endregion

        #region "Host List"
        public ActionResult HostListView(string recommendationid = "", string clubid = "", string locationid = "", string groupId = "")
        {
            string originalUrl = TempData["OriginalUrl"] as string;
            if (originalUrl == null) TempData["OriginalUrl"] = Request.Url.ToString();
            originalUrl = TempData["OriginalUrl"] as string;
            Uri redirectURL = new Uri(originalUrl);
            string query = redirectURL.Query;
            var queryParams = HttpUtility.ParseQueryString(query);
            RecommendationDetailCommonModel responseInfo = new RecommendationDetailCommonModel();
            string RecommendationId = "";
            string ClubId = "";
            string LocationId = "";
            string GroupId = "";
            if (!string.IsNullOrEmpty(recommendationid)) RecommendationId = recommendationid.DecryptParameter();
            if (!string.IsNullOrEmpty(clubid)) ClubId = clubid.DecryptParameter();
            if (!string.IsNullOrEmpty(locationid)) LocationId = locationid.DecryptParameter();
            if (!string.IsNullOrEmpty(groupId)) GroupId = groupId.DecryptParameter();

            if (string.IsNullOrEmpty(RecommendationId) || string.IsNullOrEmpty(ClubId) || string.IsNullOrEmpty(LocationId) || string.IsNullOrEmpty(GroupId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()

                });
                return RedirectToAction("Index", "RecommendationManagementV2");
            }
            var dbResponseInfo = _business.GetSelectedHostList(RecommendationId, ClubId, LocationId, GroupId);
            responseInfo.GetSelectedHostList = dbResponseInfo.MapObjects<HostListModel>();
            foreach (var item in responseInfo.GetSelectedHostList)
            {
                item.RecommendationId = item.RecommendationId.EncryptParameter();
                item.ClubId = item.ClubId.EncryptParameter();
                item.HostId = item.HostId.EncryptParameter();
                item.RecommendationHostId = item.RecommendationHostId.EncryptParameter();
                item.HostImage = ImageHelper.ProcessedImage(item.HostImage);
            }
            ViewBag.LocationId = locationid;
            ViewBag.GroupId = groupId;
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/RecommendationManagementV2/ClubListForMainPage?locationid=" + locationid + "&groupid=" + groupId;
            return View(responseInfo);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteSelectedHost(string recommendationhostid = "", string recommendationid = "", string clubid = "", string hostid = "")
        {
            string RecommendationHostId = "";
            string ClubId = "";
            string RecommendationId = "";
            string HostId = "";
            if (!string.IsNullOrEmpty(hostid)) HostId = hostid.DecryptParameter();
            if (!string.IsNullOrEmpty(recommendationhostid)) RecommendationHostId = recommendationhostid.DecryptParameter();
            if (!string.IsNullOrEmpty(recommendationid)) RecommendationId = recommendationid.DecryptParameter();
            if (!string.IsNullOrEmpty(clubid)) ClubId = clubid.DecryptParameter();
            if (string.IsNullOrEmpty(RecommendationId) || string.IsNullOrEmpty(ClubId) || string.IsNullOrEmpty(HostId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString()

                });
                return RedirectToAction("Index", "RecommendationManagementV2");
            }
            var responseInfo = new CommonDbResponse();
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
            };
            var dbResponseInfo = _business.DeleteSelectedHost(ClubId, RecommendationHostId, RecommendationId, HostId, commonRequest);
            responseInfo = dbResponseInfo;
            if (dbResponseInfo != null && dbResponseInfo.Code == ResponseCode.Success)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponseInfo.Message ?? "Host has been deleted successfully",
                    Title = NotificationMessage.SUCCESS.ToString()
                });
                return Json(responseInfo.SetMessageInTempData(this));
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return Json(responseInfo.SetMessageInTempData(this));
            }
        }
        #endregion

        #region "CLUB RECOMMENDATION REQUEST HOST LIST"
        public ActionResult SearchAndHomePageClubRecommendationReqHostList(string recommendationholdid = "", string clubid = "", string locationid = "", string displayid = "")
        {
            CommonRecommendationModel responseInfo = new CommonRecommendationModel();
            string RecommendationHoldId = "";
            string ClubId = "";
            string LocationId = "";
            string DisplayId = "";
            if (!string.IsNullOrEmpty(recommendationholdid)) RecommendationHoldId = recommendationholdid.DecryptParameter();
            if (!string.IsNullOrEmpty(clubid)) ClubId = clubid.DecryptParameter();
            if (!string.IsNullOrEmpty(locationid)) LocationId = locationid.DecryptParameter();
            if (!string.IsNullOrEmpty(displayid)) DisplayId = displayid.DecryptParameter();
            var dbResponseInfo = _business.GetSearchAndHomePageClubRecommendationReqHostList(RecommendationHoldId, ClubId, LocationId, DisplayId);
            responseInfo.GetHomeAndSearchRecommendationHostList = dbResponseInfo.MapObjects<SearchAndHomePageRecommendationReqHostListModel>();
            responseInfo.GetHomeAndSearchRecommendationHostList.ForEach(x => x.HostImage = ImageHelper.ProcessedImage(x.HostImage));
            return View(responseInfo);
        }
        #endregion

        #region "MAIN PAGE CLUB RECOMMENDATION REQUEST HOST LIST"
        public ActionResult MainPageClubRecommendationReqHostList(string recommendationholdid = "", string clubid = "", string locationid = "", string displayid = "")
        {
            CommonRecommendationModel responseInfo = new CommonRecommendationModel();
            string RecommendationHoldId = "";
            string ClubId = "";
            string LocationId = "";
            string DisplayId = "";
            if (!string.IsNullOrEmpty(recommendationholdid)) RecommendationHoldId = recommendationholdid.DecryptParameter();
            if (!string.IsNullOrEmpty(clubid)) ClubId = clubid.DecryptParameter();
            if (!string.IsNullOrEmpty(locationid)) LocationId = locationid.DecryptParameter();
            if (!string.IsNullOrEmpty(displayid)) DisplayId = displayid.DecryptParameter();
            var dbResponse = _business.GetMainPageClubRecommendationReqHostList(RecommendationHoldId, ClubId, LocationId, DisplayId);
            return View();
        }
        #endregion

        #region "HOME AND SEARCH PAGE RECOMMENDATION REQUEST REJECT"
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RejectHomeAndSearchPageRecommendationRequest(string recommendationholdid = "", string clubid = "", string locationid = "", string displayid = "")
        {
            string RecommendationHoldId = "";
            string ClubId = "";
            string LocationId = "";
            string DisplayId = "";
            if (!string.IsNullOrEmpty(recommendationholdid)) RecommendationHoldId = recommendationholdid.DecryptParameter();
            if (!string.IsNullOrEmpty(clubid)) ClubId = clubid.DecryptParameter();
            if (!string.IsNullOrEmpty(locationid)) LocationId = locationid.DecryptParameter();
            if (!string.IsNullOrEmpty(displayid)) DisplayId = displayid.DecryptParameter();
            var responseInfo = new CommonDbResponse();
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
            };
            var dbResponseInfo = _business.RejectHomeAndSearchPageRecommendationRequest(RecommendationHoldId, ClubId, LocationId, DisplayId, commonRequest);
            responseInfo = dbResponseInfo;
            if (dbResponseInfo != null && dbResponseInfo.Code == ResponseCode.Success)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponseInfo.Message ?? "Recommendation Request has been Rejected",
                    Title = NotificationMessage.SUCCESS.ToString()
                });

            }
            else if (dbResponseInfo.Code == ResponseCode.Failed)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = dbResponseInfo.Message ?? "Invalid Details",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return Json(responseInfo.SetMessageInTempData(this));
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.WARNING,
                    Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                    Title = NotificationMessage.WARNING.ToString()
                });
                return Json(responseInfo.SetMessageInTempData(this));
            }
            return Json(responseInfo.SetMessageInTempData(this));
        }
        #endregion

        #region "MAIN PAGE RECOMMENDATION REQUEST REJECT"
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RejectMainPageRecommendationRequest()
        {
            string originalUrl = TempData["OriginalUrl"] as string;
            Uri redirectURL = new Uri(originalUrl);
            string query = redirectURL.Query;
            var queryParams = HttpUtility.ParseQueryString(query);
            string recommendationholdid = queryParams["recommendationholdid"];
            string clubid = queryParams["clubid"];
            string locationid = queryParams["locationid"];
            string displayid = queryParams["displayid"];
            string RecommendationHoldId = "";
            string ClubId = "";
            string LocationId = "";
            string DisplayId = "";
            if (!string.IsNullOrEmpty(recommendationholdid)) RecommendationHoldId = recommendationholdid.DecryptParameter();
            if (!string.IsNullOrEmpty(clubid)) ClubId = clubid.DecryptParameter();
            if (!string.IsNullOrEmpty(locationid)) LocationId = locationid.DecryptParameter();
            if (!string.IsNullOrEmpty(displayid)) DisplayId = displayid.DecryptParameter();
            var responseInfo = new CommonDbResponse();
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
            };
            var dbResponseInfo = _business.RejectMainPageRecommendationRequest(RecommendationHoldId, ClubId, LocationId, DisplayId, commonRequest);
            responseInfo = dbResponseInfo;
            if (dbResponseInfo != null && dbResponseInfo.Code == ResponseCode.Success)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponseInfo.Message ?? "Recommendation Request has been Rejected",
                    Title = NotificationMessage.SUCCESS.ToString()
                });

            }
            else if (dbResponseInfo.Code == ResponseCode.Failed)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = dbResponseInfo.Message ?? "Invalid Details",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return Json(responseInfo.SetMessageInTempData(this));
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.WARNING,
                    Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                    Title = NotificationMessage.WARNING.ToString()
                });
                return Json(responseInfo.SetMessageInTempData(this));
            }
            return Json(responseInfo.SetMessageInTempData(this));
        }
        #endregion

        #region "REMOVE MAIN PAGE SINGLE HOST RECOMMENDATION REQUESTS"
        public ActionResult RemoveMainPageSingleHostRecommendationReq(string recommendationholdid = "", string clubid = "", string recommendationholdhostid = "")
        {
            string RecommendationHoldId = "";
            string ClubId = "";
            string RecommendationHoldHostId = "";
            if (!string.IsNullOrEmpty(recommendationholdid)) RecommendationHoldId = recommendationholdid.DecryptParameter();
            if (!string.IsNullOrEmpty(clubid)) ClubId = clubid.DecryptParameter();
            if (!string.IsNullOrEmpty(recommendationholdhostid)) RecommendationHoldHostId = RecommendationHoldHostId.DecryptParameter();
            var responseInfo = new CommonDbResponse();
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
            };
            var dbResponseInfo = _business.RemoveMainPageSingleHostRecommendationReq(RecommendationHoldId, ClubId, RecommendationHoldHostId, commonRequest);
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
                    Message = dbResponseInfo.Message ?? "Main Page Single Host Recommendation Request has been Removed",
                    Title = NotificationMessage.SUCCESS.ToString()
                });
            }
            return Json(responseInfo.SetMessageInTempData(this));
        }
        #endregion

        #region "Remove Home & Search Page Single Host Recommendation Requests"
        public ActionResult RemoveHomeAndSearchPageSingleHostRecommendationReq(string recommendationholdid = "", string clubid = "", string recommendationholdhostid = "")
        {
            string RecommendationHoldId = "";
            string ClubId = "";
            string RecommendationHoldHostId = "";
            if (!string.IsNullOrEmpty(recommendationholdid)) RecommendationHoldId = recommendationholdid.DecryptParameter();
            if (!string.IsNullOrEmpty(clubid)) ClubId = clubid.DecryptParameter();
            if (!string.IsNullOrEmpty(recommendationholdhostid)) RecommendationHoldHostId = RecommendationHoldHostId.DecryptParameter();
            var responseInfo = new CommonDbResponse();
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
            };
            var dbResponseInfo = _business.RemoveHomeAndSearchPageSingleHostRecommendationReq(RecommendationHoldId, ClubId, RecommendationHoldHostId, commonRequest);
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
                    Message = dbResponseInfo.Message ?? "Main Page Single Host Recommendation Request has been Removed",
                    Title = NotificationMessage.SUCCESS.ToString()
                });
            }
            return Json(responseInfo.SetMessageInTempData(this));
        }
        #endregion

        #region "APPROVE HOME AND SEARCH PAGE RECOMMENDATION REQUEST"
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApproveHomeAndSearchPageRecommendationReq(string recommendationholdid = "", string clubid = "", string displayid = "", string locationid = "")
        {
            string RecommendationHoldId = "";
            string ClubId = "";
            string DisplayIdHold = "";
            string LocationId = "";
            if (!string.IsNullOrEmpty(recommendationholdid)) RecommendationHoldId = recommendationholdid.DecryptParameter();
            if (!string.IsNullOrEmpty(clubid)) ClubId = clubid.DecryptParameter();
            if (!string.IsNullOrEmpty(displayid)) DisplayIdHold = displayid.DecryptParameter();
            if (!string.IsNullOrEmpty(locationid)) LocationId = locationid.DecryptParameter();
            var responseInfo = new CommonDbResponse();
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
            };
            var dbResponseInfo = _business.ApproveHomeAndSearchPageRecommendationReq(RecommendationHoldId, ClubId, DisplayIdHold, LocationId, commonRequest);
            responseInfo = dbResponseInfo;
            if (dbResponseInfo != null && dbResponseInfo.Code == ResponseCode.Success)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponseInfo.Message ?? "Recommendation Request has been Approved",
                    Title = NotificationMessage.SUCCESS.ToString()
                });

            }
            else if (dbResponseInfo.Code == ResponseCode.Failed)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = dbResponseInfo.Message ?? "Invalid Details",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return Json(responseInfo.SetMessageInTempData(this));
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.WARNING,
                    Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                    Title = NotificationMessage.WARNING.ToString()
                });
                return Json(responseInfo.SetMessageInTempData(this));
            }
            return Json(responseInfo.SetMessageInTempData(this));
        }
        #endregion

        #region "APPROVE MAIN PAGE RECOMMENDATION REQUEST"
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApproveMainPageRecommendationReq(string groupid = "")
        {
            string originalUrl = TempData["OriginalUrl"] as string;
            Uri redirectURL = new Uri(originalUrl);
            string query = redirectURL.Query;
            var queryParams = HttpUtility.ParseQueryString(query);
            string recommendationholdid = queryParams["recommendationholdid"];
            string clubid = queryParams["clubid"];
            string locationid = queryParams["locationid"];
            string displayid = queryParams["displayid"];
            string GroupId = "";
            string RecommendationHoldId = "";
            string ClubId = "";
            string DisplayIdHold = "";
            string LocationId = "";
            if (!string.IsNullOrEmpty(recommendationholdid)) RecommendationHoldId = recommendationholdid.DecryptParameter();
            if (!string.IsNullOrEmpty(clubid)) ClubId = clubid.DecryptParameter();
            if (!string.IsNullOrEmpty(displayid)) DisplayIdHold = displayid.DecryptParameter();
            if (!string.IsNullOrEmpty(locationid)) LocationId = locationid.DecryptParameter();
            if (!string.IsNullOrEmpty(groupid)) GroupId = groupid.DecryptParameter();
            var responseInfo = new CommonDbResponse();
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
            };
            var dbResponseInfo = _business.ApproveMainPageRecommendationReq(RecommendationHoldId, ClubId, DisplayIdHold, LocationId, GroupId, commonRequest);
            responseInfo = dbResponseInfo;
            if (dbResponseInfo != null && dbResponseInfo.Code == ResponseCode.Success)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponseInfo.Message ?? "Recommendation Request has been Approved",
                    Title = NotificationMessage.SUCCESS.ToString()
                });

            }
            else if (dbResponseInfo.Code == ResponseCode.Failed)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = dbResponseInfo.Message ?? "Invalid Details",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
                return Json(responseInfo.SetMessageInTempData(this));
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.WARNING,
                    Message = dbResponseInfo.Message ?? "Something went wrong try again later",
                    Title = NotificationMessage.WARNING.ToString()
                });
                return Json(responseInfo.SetMessageInTempData(this));
            }
            return Json(responseInfo.SetMessageInTempData(this));
        }
        #endregion

        #region"MAIN PAGE RECOMMENDATION REQUEST FOR UPDATE"
        [HttpGet]
        public ActionResult ManageMainPageRecommendationReqForUpdate(string clubid = "", string displayid = "", string locationid = "", string recommendationholdid = "")
        {
            TempData["OriginalUrl"] = Request.Url.ToString();
            string ClubId = "";
            string DisplayIdHold = "";
            string LocationId = "";
            string RecommendationHoldId = "";
            if (!string.IsNullOrEmpty(recommendationholdid)) RecommendationHoldId = recommendationholdid.DecryptParameter();
            RecommendationDetailCommonModel responseinfo = new RecommendationDetailCommonModel();
            if (!string.IsNullOrEmpty(clubid)) ClubId = clubid.DecryptParameter();
            if (!string.IsNullOrEmpty(displayid)) DisplayIdHold = displayid.DecryptParameter();
            if (!string.IsNullOrEmpty(locationid)) LocationId = locationid.DecryptParameter();
            ViewBag.GroupDDLByLocationId = ApplicationUtilities.LoadDropdownList("GROUPDDLBYLOCATIONID", LocationId, "") as Dictionary<string, string>;
            var dbResponseInfo = _business.MainPageRecommendationReqForUpdate(ClubId, DisplayIdHold, LocationId);
            responseinfo.GetMPageRecommendationDetailModel = dbResponseInfo.MapObject<ManageClubRecommendationRequest>();
            ViewBag.ClubLocation = ApplicationUtilities.LoadDropdownList("LOCATIONDDL", LocationId, "") as Dictionary<string, string>;
            ViewBag.ClubDDL = ApplicationUtilities.LoadDropdownList("CLUBLIST", LocationId, "") as Dictionary<string, string>;
            ViewBag.DisplayOrderDDL = ApplicationUtilities.LoadDropdownList("DISPLAYORDERDDL", "", "") as Dictionary<string, string>;
            ViewBag.LocationId = locationid;
            ViewBag.DisplayId = displayid;
            ViewBag.ClubId = clubid;
            var dbMainPageHostListInfo = _business.MainPageHostRecommendationReqForUpdate(RecommendationHoldId, ClubId, DisplayIdHold, LocationId);
            responseinfo.GetMainPageHostListForUpdate = dbMainPageHostListInfo.MapObjects<MainPageHostListForUpdate>();
            return View(responseinfo);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageMainPageRecommendationReqForUpdate(ManageClubRecommendationRequest Model)
        {

            return View();
        }
        #endregion

        #region "MAIN PAGE HOST RECOMMENDATION DETAIL FOR UPDATE"
        public ActionResult MainPageHostRecommendationReqForUpdate(string recommendationholdid = "", string clubid = "", string displayidhold = "", string locationid = "")
        {
            return View();
        }
        #endregion
    }
}