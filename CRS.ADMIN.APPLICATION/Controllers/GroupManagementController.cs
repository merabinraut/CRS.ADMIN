using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models;
using CRS.ADMIN.APPLICATION.Models.GroupManagement;
using CRS.ADMIN.BUSINESS.GroupManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.GroupManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using Syncfusion.XlsIO.Implementation.PivotAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class GroupManagementController : BaseController
    {
        private readonly IGroupManagementBusiness _business;
        public GroupManagementController(IGroupManagementBusiness business)
        {
            _business = business;
        }

        #region MANAGE GROUP
        [HttpGet]
        public ActionResult Index(string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            Session["CurrentURL"] = "GroupManagement/Index";
            string RenderId = string.Empty;
            ViewBag.SearchFilter = null;
            PaginationFilterCommon paginationFilter = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            GroupOverviewModel commonResponse = new GroupOverviewModel();
            if (TempData.ContainsKey("ManageGroup")) commonResponse.ManageGroup = TempData["ManageGroup"] as ManageGroupModel;
            else commonResponse.ManageGroup = new ManageGroupModel();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            commonResponse.SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : string.Empty;
            var dbGroupResponse = _business.GetGroupList(paginationFilter);
            var dbGroupAnalyticResponse = _business.GetGroupAnalytic();
            commonResponse.GetGroupList = dbGroupResponse.MapObjects<GroupInfoModel>();
            foreach (var item in commonResponse.GetGroupList)
            {
                item.groupId = ApplicationUtilities.EncryptParameter(item.groupId);
                item.groupImage = ImageHelper.ProcessedImage(item.groupImage);
            }
            commonResponse.GetGroupAnalytic = dbGroupAnalyticResponse.MapObject<GroupAnalyticModel>();
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            return View(commonResponse);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageGroup(ManageGroupModel model, HttpPostedFileBase groupCoverImage)
        {
            ActionResult redirectUrl = null;
            bool allowRedirect = false;
            ActionResult redirectresult = null;
            redirectUrl = RedirectToAction("Index", "GroupManagement", new
            {
                SearchFilter = model.SearchFilter,
                StartIndex = model.Skip,
                PageSize = model.Take == 0 ? 10 : model.Take
            });
            string ErrorMessage = string.Empty;
            string coverPhotoPath = string.Empty;
            var allowedContentType = AllowedImageContentType();
            if (ModelState.IsValid)
            {

                if (string.IsNullOrEmpty(model.GroupCoverPhoto))
                {

                    if (groupCoverImage == null && string.IsNullOrEmpty(model.GroupCoverPhoto))
                    {
                        ErrorMessage = "Cover Image Required";
                        allowRedirect = true;
                    }
                    if (allowRedirect)
                    {
                        this.AddNotificationMessage(new SHARED.NotificationModel()
                        {
                            NotificationType = SHARED.NotificationMessage.INFORMATION,
                            Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                            Title = SHARED.NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManageGroup"] = model;
                        TempData["RenderId"] = "Manage";
                        return redirectUrl;
                    }
                }
                string coverFileName = string.Empty;
                if (groupCoverImage != null)
                {
                    var contentType = groupCoverImage.ContentType;
                    var extension = Path.GetExtension(groupCoverImage.FileName);
                    if (allowedContentType.Contains(contentType.ToLower()))
                    {
                        coverFileName = $"{AWSBucketFolderNameModel.ADMIN}/Cover_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{extension.ToLower()}";
                        model.GroupCoverPhoto = $"/{coverFileName}";
                    }
                    else
                    {
                        this.AddNotificationMessage(new SHARED.NotificationModel()
                        {
                            NotificationType = SHARED.NotificationMessage.INFORMATION,
                            Message = "Invalid Image Formate",
                            Title = SHARED.NotificationMessage.INFORMATION.ToString(),
                        });
                        allowRedirect = true;
                    }
                }
                if (allowRedirect == true)
                {
                    TempData["ManageGroup"] = model;
                    TempData["RenderId"] = "Manage";
                    return redirectUrl;
                }
                ManageGroupModelCommon commonModel = model.MapObject<ManageGroupModelCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                if (!string.IsNullOrEmpty(commonModel.GroupId))
                {
                    commonModel.GroupId = commonModel.GroupId.DecryptParameter();
                    if (string.IsNullOrEmpty(commonModel.GroupId))
                    {
                        this.AddNotificationMessage(new SHARED.NotificationModel()
                        {
                            NotificationType = SHARED.NotificationMessage.INFORMATION,
                            Message = "Invalid group detail",
                            Title = SHARED.NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManageGroup"] = model;
                        TempData["RenderId"] = "Manage";
                        return redirectUrl;
                    }
                }
                var dbResponse = _business.ManageGroup(commonModel);
                if (dbResponse != null && dbResponse.Code == 0)
                {
                    if (groupCoverImage != null)
                    {
                        await ImageHelper.ImageUpload(coverFileName, groupCoverImage);
                        this.AddNotificationMessage(new SHARED.NotificationModel()
                        {
                            NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : SHARED.NotificationMessage.INFORMATION,
                            Message = dbResponse.Message ?? "Failed",
                            Title = dbResponse.Code == SHARED.ResponseCode.Success ? SHARED.ResponseCode.Success.ToString() : SHARED.NotificationMessage.INFORMATION.ToString()
                        });
                        return redirectUrl;
                    }
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["ManageGroup"] = model;
                    TempData["RenderId"] = "Manage";
                    return redirectUrl;
                }
            }
            var errorMessage = ModelState.Where(x => x.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors.Select(e => $"{x.Key}:{e.ErrorMessage}")).ToList();
            TempData["ManageGroup"] = model;
            TempData["RenderId"] = "Manage";
            return redirectUrl;
        }
        [HttpGet]
        public ActionResult ManageGroup(string GroupId = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;

            ManageGroupModel model = new ManageGroupModel();
            if (!string.IsNullOrEmpty(GroupId))
            {
                string groupId = GroupId.DecryptParameter();
                if (string.IsNullOrEmpty(groupId))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid group detail",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("Index", "GroupManagement", new
                    {
                        SearchFilter = model.SearchFilter,
                        StartIndex = model.Skip,
                        PageSize = model.Take == 0 ? 10 : model.Take
                    });
                }
                var dbResponse = _business.GetGroupDetail(groupId);
                model = dbResponse.MapObject<ManageGroupModel>();
                model.GroupId = model.GroupId.EncryptParameter();
            }
            TempData["RenderId"] = "Manage";
            TempData["ManageGroup"] = model;
            return RedirectToAction("Index", "GroupManagement", new
            {
                SearchFilter = model.SearchFilter,
                StartIndex = model.Skip,
                PageSize = model.Take == 0 ? 10 : model.Take
            });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DeleteGroup(string GroupId = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            var response = new CommonDbResponse();
            var groupId = !string.IsNullOrEmpty(GroupId) ? GroupId.DecryptParameter() : string.Empty;
            if (string.IsNullOrEmpty(groupId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid detail",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
            }
            var request = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            var dbResponse = _business.DeleteGroup(groupId, request);
            response = dbResponse;
            if (dbResponse.Code == ResponseCode.Success && dbResponse.Code == 0)
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                    Message = response.Message ?? "Something went wrong while deleting the group. Please try again later.!!",
                    Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString(),
                });
                return Json(response.Message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = response.Code == ResponseCode.Warning ? NotificationMessage.WARNING : NotificationMessage.ERROR,
                    Message = response.Message ?? "Something went wrong while deleting the group. Please try again later.!!",
                    Title = response.Code == ResponseCode.Warning ? NotificationMessage.WARNING.ToString() : NotificationMessage.ERROR.ToString(),
                });
                return Json(response.Message, JsonRequestBehavior.AllowGet);
            }


        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult BlockGroup(string GroupId = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            var response = new CommonDbResponse();
            string groupId = !string.IsNullOrEmpty(GroupId) ? GroupId.DecryptParameter() : string.Empty;
            if (string.IsNullOrEmpty(groupId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    Message = "Invalid details",
                    NotificationType = NotificationMessage.INFORMATION,
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
            }
            var request = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            var dbResponse = _business.BlockGroup(groupId, request);
            response = dbResponse;
            if (dbResponse.Code == 0 && dbResponse.Code == ResponseCode.Success)
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                    Message = response.Message ?? "something went wrong. Please try again later",
                    Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString(),
                });
                return Json(response.Message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = response.Code == ResponseCode.Warning ? NotificationMessage.WARNING : NotificationMessage.INFORMATION,
                    Message = response.Message ?? "something went wrong. Please try again later",
                    Title = response.Code == ResponseCode.Warning ? NotificationMessage.WARNING.ToString() : NotificationMessage.INFORMATION.ToString(),
                });
                return Json(response.Message, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult UnblockGroup(string GroupId = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            var response = new CommonDbResponse();
            string groupId = !string.IsNullOrEmpty(GroupId) ? GroupId.DecryptParameter() : string.Empty;
            if (string.IsNullOrEmpty(groupId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    Message = "Invalid details",
                    NotificationType = NotificationMessage.INFORMATION,
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
            }
            var request = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            var dbResponse = _business.UnblockGroup(groupId, request);

            response = dbResponse;
            if (dbResponse.Code == 0 && dbResponse.Code == ResponseCode.Success)
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                    Message = response.Message ?? "something went wrong. Please try again later",
                    Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString(),
                });
                return Json(response.Message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = response.Code == ResponseCode.Warning ? NotificationMessage.WARNING : NotificationMessage.INFORMATION,
                    Message = response.Message ?? "something went wrong. Please try again later",
                    Title = response.Code == ResponseCode.Warning ? NotificationMessage.WARNING.ToString() : NotificationMessage.INFORMATION.ToString(),
                });
                return Json(response.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region MANAGE SUB-GROUP
        [HttpGet]
        public ActionResult SubGroup(string GroupId = "", int StartIndex = 0, int PageSize = 10, string SearchFilter = "")
        {
            Session["CurrentURL"] = "GroupManagement/SubGroup";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            string RenderId = string.Empty;
            PaginationFilterCommon paginationFilter = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = SearchFilter
            };
            string groupId = string.Empty;
            if (!string.IsNullOrEmpty(GroupId))
                groupId = GroupId.DecryptParameter();
            CommonSubGroupModel commonModel = new CommonSubGroupModel();

            if (TempData.ContainsKey("ManageSubGroup")) commonModel.ManageSubGroup = TempData["ManageSubGroup"] as ManageSubGroupModel;
            else commonModel.ManageSubGroup = new ManageSubGroupModel();

            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();

            if (TempData.ContainsKey("ManageSubGroupClub")) commonModel.ManageSubGroupClub = TempData["ManageSubGroupClub"] as ManageSubGroupClubModel;
            else commonModel.ManageSubGroupClub = new ManageSubGroupClubModel();

            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();

            var dbResponse = _business.GetSubGroupByGroupId(groupId, paginationFilter);
            commonModel.SubGroupInfoList = dbResponse.SubGroupInfoList.MapObjects<SubGroupInfoModel>();
            commonModel.ClubShortInfo = dbResponse.ClubShortInfo.MapObjects<SubGroupClubInfo>();
            foreach (var item in commonModel.SubGroupInfoList)
            {
                item.GroupId = item.GroupId.EncryptParameter();
                item.SubGroupId = item.SubGroupId.EncryptParameter();
            }
            foreach (var item in commonModel.ClubShortInfo)
            {
                item.ClubId = item.ClubId.EncryptParameter();
            }
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.GroupId = GroupId;
            ViewBag.GroupName = commonModel.SubGroupInfoList[0].GroupName;
            ViewBag.GroupNameKatakana = commonModel.SubGroupInfoList[0].GroupNameKatakana;
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.SearchFilter = SearchFilter;
            ViewBag.LocationDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("LOCATIONDDL", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.ClubStoreDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBLIST", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.SubGroupList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("SUBGROUPDDL", groupId, culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            return View(commonModel);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageSubGroup(ManageSubGroupModel model)
        {
            ActionResult redirectUrl = null;
            bool allowRedirect = false;
            redirectUrl = RedirectToAction("SubGroup", "GroupManagement", new
            {
                GroupId = model.GroupId,
                SearchFilter = model.SearchFilter,
                StartIndex = model.Skip,
                PageSize = model.Take == 0 ? 10 : model.Take
            });
            string ErrorMessage = string.Empty;
            if (ModelState.IsValid)
            {
                ManageSubGroupModelCommon commonRequest = model.MapObject<ManageSubGroupModelCommon>();
                commonRequest.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonRequest.ActionIP = ApplicationUtilities.GetIP();
                if (!string.IsNullOrEmpty(commonRequest.SubGroupId))
                {
                    commonRequest.SubGroupId = commonRequest.SubGroupId.DecryptParameter();
                    if (string.IsNullOrEmpty(commonRequest.SubGroupId))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = SHARED.NotificationMessage.INFORMATION,
                            Message = "Invalid sub group detail",
                            Title = SHARED.NotificationMessage.INFORMATION.ToString()
                        });
                        TempData["ManageSubGroup"] = model;
                        TempData["RenderId"] = "ManageSG";
                        return redirectUrl;
                    }
                }
                if (!string.IsNullOrEmpty(model.GroupId))
                {
                    commonRequest.GroupId = commonRequest.GroupId.DecryptParameter();
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = SHARED.NotificationMessage.INFORMATION,
                        Message = "Invalid sub group detail",
                        Title = SHARED.NotificationMessage.INFORMATION.ToString()
                    });
                    TempData["ManageSubGroup"] = model;
                    TempData["RenderId"] = "ManageSG";
                    return redirectUrl;
                }
                var dbResponse = _business.ManageSubGroup(commonRequest);
                if (dbResponse.Code == ResponseCode.Success && dbResponse.Code == 0)
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Success ? SHARED.NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });
                    TempData["ManageSubGroup"] = model;
                    TempData["RenderId"] = "ManageSG";
                    return redirectUrl;
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Failed ? SHARED.NotificationMessage.WARNING : NotificationMessage.WARNING,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.WARNING.ToString() : NotificationMessage.WARNING.ToString()
                    });
                    TempData["ManageSubGroup"] = model;
                    TempData["RenderId"] = "ManageSG";
                    return redirectUrl;
                }
            }
            var errorMessage = ModelState.Where(x => x.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors.Select(e => $"{x.Key}:{e.ErrorMessage}")).ToList();
            TempData["ManageSubGroup"] = model;
            TempData["RenderId"] = "ManageSG";
            return redirectUrl;
        }
        [HttpGet]
        public ActionResult ManageSubGroup(string SubGroupId = "", string GroupId = "")
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ManageSubGroupModel model = new ManageSubGroupModel();
            if (!string.IsNullOrEmpty(SubGroupId))
            {
                string subGroupId = ApplicationUtilities.DecryptParameter(SubGroupId);
                if (string.IsNullOrEmpty(subGroupId))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid sub group detail",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });
                    return RedirectToAction("SubGroup", "GroupManagement", new
                    {
                        GroupId = GroupId,
                        SearchFilter = model.SearchFilter,
                        StartIndex = model.Skip,
                        PageSize = model.Take == 0 ? 10 : model.Take,
                    });
                }
                var dbResponse = _business.GetSubGroupDetail(subGroupId);
                model = dbResponse.MapObject<ManageSubGroupModel>();
                model.GroupId = model.GroupId.EncryptParameter();
                model.SubGroupId = model.SubGroupId.EncryptParameter();
            }
            TempData["RenderId"] = "ManageSG";
            TempData["ManageSubGroup"] = model;
            return RedirectToAction("SubGroup", "GroupManagement", new
            {
                GroupId = GroupId,
                SearchFilter = model.SearchFilter,
                StartIndex = model.Skip,
                PageSize = model.Take == 0 ? 10 : model.Take,
            });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DeleteSubGroup(string SubGroupId = "")
        {
            var response = new CommonDbResponse();
            var subGroupId = !string.IsNullOrEmpty(SubGroupId) ? SubGroupId.DecryptParameter() : string.Empty;
            if (string.IsNullOrEmpty(subGroupId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid Sub Group Details",
                    Title = NotificationMessage.INFORMATION.ToString()
                });
            }
            var request = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            var dbResponse = _business.DeleteSubGroup(subGroupId, request);
            response = dbResponse;
            if (dbResponse.Code == ResponseCode.Success && dbResponse.Code == 0)
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                    Message = response.Message ?? "Something went wrong while deleting the sub group. Please try again later",
                    Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                });
            }
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = response.Code == ResponseCode.Failed ? NotificationMessage.ERROR : NotificationMessage.ERROR,
                    Message = response.Message ?? "Something went wrong while deleting sub group. Please try again later",
                    Title = response.Code == ResponseCode.Failed ? NotificationMessage.ERROR.ToString() : NotificationMessage.ERROR.ToString()
                });
            }
            return Json(response.Message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageSubGroupClub(ManageSubGroupClubModel model)
        {
            ActionResult redirectUrl = null;
            bool allowRedirect = false;
            redirectUrl = RedirectToAction("SubGroup", "GroupManagement", new
            {
                GroupId = model.GroupId,
                SearchFilter = model.SearchFilter,
                StartIndex = model.Skip,
                PageSize = model.Take == 0 ? 10 : model.Take,
            });
            string ErrorMessage = string.Empty;
            if (ModelState.IsValid)
            {
                ManageSubGroupClubModelCommon commonModel = model.MapObject<ManageSubGroupClubModelCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                if (!string.IsNullOrEmpty(commonModel.SubGroupId))
                {
                    commonModel.SubGroupId = commonModel.SubGroupId.DecryptParameter();
                    if (string.IsNullOrEmpty(commonModel.SubGroupId))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid sub group detail",
                            Title = NotificationMessage.INFORMATION.ToString()
                        });
                        TempData["ManageSubGroupClub"] = model;
                        TempData["RenderId"] = "ManageSGC";
                        return redirectUrl;
                    }
                }
                var dbResponse = _business.ManageSubGroupClub(commonModel);
                if (dbResponse.Code == ResponseCode.Success && dbResponse.Code == 0)
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });
                    TempData["ManageSubGroupClub"] = model;
                    TempData["RenderId"] = "ManageSGC";
                    return redirectUrl;
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Failed ? SHARED.NotificationMessage.WARNING : NotificationMessage.WARNING,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.WARNING.ToString() : NotificationMessage.WARNING.ToString()
                    });
                    TempData["ManageSubGroupClub"] = model;
                    TempData["RenderId"] = "ManageSGC";
                    return redirectUrl;
                }
            }
            var errorMessage = ModelState.Where(x => x.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors.Select(e => $"{x.Key}:{e.ErrorMessage}")).ToList();
            TempData["ManageSubGroupClub"] = model;
            TempData["RenderId"] = "ManageSGC";
            return redirectUrl;
        }
        [HttpGet]
        public ActionResult ManageSubGroupClub(string GroupId = "", string SubGroupId = "")
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ManageSubGroupClubModel model = new ManageSubGroupClubModel();
            if (!string.IsNullOrEmpty(SubGroupId))
            {
                string subGroupId = SubGroupId.DecryptParameter();
                if (string.IsNullOrEmpty(subGroupId))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "invalid sub group club detail",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });
                    return RedirectToAction("SubGroup", "GroupManagement", new
                    {
                        GroupId = GroupId,
                        SearchFilter = model.SearchFilter,
                        StartIndex = model.Skip,
                        PageSize = model.Take == 0 ? 10 : model.Take,
                    });
                }
                var dbResponse = _business.GetSubGroupClubDetailById(subGroupId);
                model = dbResponse.MapObject<ManageSubGroupClubModel>();
                model.GroupId = model.GroupId.EncryptParameter();
                model.SubGroupId = model.SubGroupId.EncryptParameter();
                model.ClubId = model.ClubId.EncryptParameter();
                model.LocationId = model.LocationId.EncryptParameter();
                TempData["RenderId"] = "ManageSG";
                TempData["ManageSubGroup"] = model;
                return RedirectToAction("SubGroup", "GroupManagement", new
                {
                    GroupId = GroupId,
                    SearchFilter = model.SearchFilter,
                    StartIndex = model.Skip,
                    PageSize = model.Take == 0 ? 10 : model.Take,
                });
            }
            TempData["RenderId"] = "ManageSGC";
            TempData["ManageSubGroupClub"] = model;
            return RedirectToAction("SubGroup", "GroupManagement", new
            {
                GroupId = GroupId,
                SearchFilter = model.SearchFilter,
                StartIndex = model.Skip,
                PageSize = model.Take == 0 ? 10 : model.Take,
            });
        }

        [HttpPost, ValidateAntiForgeryToken, OverrideActionFilters]
        public JsonResult GetClubListByLocationId(string LocationId = "")
        {
            var locationId = !string.IsNullOrEmpty(LocationId) ? LocationId.DecryptParameter() : string.Empty;
            List<SelectListItem> clubDDL = new List<SelectListItem>();
            if (string.IsNullOrEmpty(locationId)) return null;
            clubDDL = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("CLUBDDLBYLOCATIONID", locationId, "") as Dictionary<string, string>, null);
            return Json(new { clubDDL }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GROUP GALLERY
        [HttpGet]
        public ActionResult GroupGallery(string GroupId = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            Session["CurrentURL"] = "GroupManagement/GroupGallery";
            string RenderId = string.Empty;

            CommonGroupGalleryModel commonResponse = new CommonGroupGalleryModel();

            if (TempData.ContainsKey("ManageGroupImage")) commonResponse.ManageGroupGallery = TempData["ManageGroupImage"] as ManageGroupGalleryModel;
            else commonResponse.ManageGroupGallery = new ManageGroupGalleryModel();

            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();

            commonResponse.SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : string.Empty;

            if (!string.IsNullOrEmpty(GroupId))
            {
                string groupId = GroupId.DecryptParameter();
                var dbResponse = _business.GetGalleryListById(groupId);
                commonResponse.GroupGalleryList = dbResponse.MapObjects<GroupGalleryInfoModel>();
                foreach (var item in commonResponse.GroupGalleryList)
                {
                    item.GroupId = item.GroupId.EncryptParameter();
                    item.ImageId = item.ImageId.EncryptParameter();
                    item.ImagePath = ImageHelper.ProcessedImage(item.ImagePath);
                }
                ViewBag.GroupId = GroupId;
                ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
                return View(commonResponse);
            }
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.WARNING,
                    Title = NotificationMessage.WARNING.ToString(),
                    Message = "Invalid Group Id"
                });
                return RedirectToAction("Index", "GroupManagement", new
                {
                    SearchFilter = SearchFilter,
                    StartIndex = StartIndex,
                    PageSize = PageSize
                });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageGroupGallery(ManageGroupGalleryModel model, HttpPostedFileBase Cover_Image_Path)
        {
            bool allowRedirect = false;
            string ErrorMessage = string.Empty;
            string coverPhotoPath = string.Empty;
            var allowedContentType = AllowedImageContentType();
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.GalleryImage))
                {
                    if (Cover_Image_Path == null && string.IsNullOrEmpty(model.GalleryImage))
                    {
                        ErrorMessage = "Cover Image Required";
                        allowRedirect = true;
                    }
                    if (allowRedirect)
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = SHARED.NotificationMessage.INFORMATION,
                            Message = ErrorMessage ?? "Something went wrong. Please try again later",
                            Title = SHARED.NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManageGroupImage"] = model;
                        TempData["RenderId"] = "ManageGG";
                        return RedirectToAction("GroupGallery", "GroupManagement", new
                        {
                            GroupId = model.GroupId
                        });
                    }
                }
                string CoverFileName = string.Empty;
                if (Cover_Image_Path != null)
                {
                    var contentType = Cover_Image_Path.ContentType;
                    var extension = Path.GetExtension(Cover_Image_Path.FileName);
                    if (allowedContentType.Contains(contentType.ToLower()))
                    {
                        CoverFileName = $"{AWSBucketFolderNameModel.ADMIN}/Cover_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{extension}";
                        model.GalleryImage = $"/{CoverFileName}";
                    }
                    else
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid Image Formate",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        return RedirectToAction("GroupGallery", "GroupManagement", new
                        {
                            GroupId = model.GroupId
                        });
                    }
                }
                ManageGroupGalleryModelCommon commonModel = model.MapObject<ManageGroupGalleryModelCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                if (!string.IsNullOrEmpty(commonModel.ImageId))
                {
                    commonModel.ImageId = commonModel.ImageId.DecryptParameter();
                    if (string.IsNullOrEmpty(commonModel.ImageId))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid image details",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManageGroupImage"] = model;
                        TempData["RenderId"] = "ManageGG";
                        return RedirectToAction("GroupGallery", "GroupManagement", new
                        {
                            GroupId = model.GroupId
                        });
                    }
                }
                commonModel.GroupId = commonModel.GroupId.DecryptParameter();
                var dbresponse = _business.ManageGroupGallery(commonModel);
                if (dbresponse != null && dbresponse.Code == 0)
                {
                    if (Cover_Image_Path != null)
                    {
                        await ImageHelper.ImageUpload(CoverFileName, Cover_Image_Path);
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = dbresponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                            Message = dbresponse.Message ?? "Failed",
                            Title = dbresponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                        });
                        TempData["ManageGroupImage"] = model;
                        TempData["RenderId"] = "ManageGG";
                        return RedirectToAction("GroupGallery", "GroupManagement", new
                        {
                            GroupId = model.GroupId
                        });
                    }

                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbresponse.Message ?? "Failed",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["ManageGroupImage"] = model;
                    TempData["RenderId"] = "ManageGG";
                    return RedirectToAction("GroupGallery", "GroupManagement", new
                    {
                        GroupId = model.GroupId
                    });
                }
            }
            var errorMessage = ModelState.Where(x => x.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors.Select(e => $"{x.Key}:{e.ErrorMessage}")).ToList();
            TempData["ManageGroupImage"] = model;
            TempData["RenderId"] = "ManageGG";
            return RedirectToAction("GroupGallery", "GroupManagement", new
            {
                GroupId = model.GroupId
            });
        }
        [HttpGet]
        public ActionResult ManageGroupGallery(string ImageId = "", string GroupId = "")
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;

            ManageGroupGalleryModel model = new ManageGroupGalleryModel();
            if (!string.IsNullOrEmpty(ImageId))
            {
                string imageid = ImageId.DecryptParameter();
                if (string.IsNullOrEmpty(imageid))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid image detail",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("GroupGallery", "GroupManagement", new
                    {
                        GroupId = GroupId
                    });
                }
                var dbResponse = _business.GetGroupGalleryDetail(imageid);
                model = dbResponse.MapObject<ManageGroupGalleryModel>();
                model.ImageId = model.ImageId.EncryptParameter();
            }
            TempData["RenderId"] = "ManageGG";
            TempData["ManageGroupImage"] = model;
            return RedirectToAction("GroupGallery", "GroupManagement", new
            {
                GroupId = GroupId
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DeleteGalleryImage(string ImageId = "")
        {
            return Json(null);
        }

        #endregion
    }
}