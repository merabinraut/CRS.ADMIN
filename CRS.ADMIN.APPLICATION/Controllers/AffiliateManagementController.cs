using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.AffiliateManagement;
using CRS.ADMIN.BUSINESS.AffiliateManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.AffiliateManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Linq;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class AffiliateManagementController : BaseController
    {
        private readonly IAffiliateManagementBusiness _affiliateBuss;
        public AffiliateManagementController(IAffiliateManagementBusiness affiliateBuss)
        {
            _affiliateBuss = affiliateBuss;
        }
        public ActionResult Index(string SearchFilter1 = "", string SearchFilter2 = "", int StartIndex = 0, int PageSize = 10, int StartIndex2 = 0, int PageSize2 = 10)
        {
            Session["CurrentURL"] = "/AffiliateManagement/Index";
            var ResponseModel = new ReferalCommonModel();
            var dbAffiliateListRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = SearchFilter1
            };
            var dbResponse = _affiliateBuss.GetAffiliateList(dbAffiliateListRequest);
            var dbCustomerListRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex2,
                Take = PageSize2,
                SearchFilter = SearchFilter2
            };
            var dbReferalRes = _affiliateBuss.GetReferalConvertedCustomerList("", "", dbCustomerListRequest);
            var analyticDBResponse = _affiliateBuss.GetAffiliateAnalytic();
            if (dbResponse.Count > 0) ResponseModel.GetAffiliateList = dbResponse.MapObjects<AffiliateManagementModel>();
            ResponseModel.GetAffiliateList.ForEach(x => x.AffiliateId = x.AffiliateId.EncryptParameter());
            ResponseModel.GetAffiliateList.ForEach(x =>
            {
                x.HoldAffiliateId = x.HoldAffiliateId.EncryptParameter();
                x.AffiliateImage = ImageHelper.ProcessedImage(x.AffiliateImage);
            });
            ResponseModel.GetReferalConvertedCustomerList = dbReferalRes.MapObjects<ReferralConvertedCustomerListModel>();
            ResponseModel.GetReferalConvertedCustomerList.ForEach(x =>
            {
                x.CustomerImage = ImageHelper.ProcessedImage(x.CustomerImage);
                x.AffiliateImaeg = ImageHelper.ProcessedImage(x.AffiliateImaeg);
            });
            ResponseModel.AffiliatePageAnalyticModel = analyticDBResponse.MapObject<AffiliatePageAnalyticModel>();
            ViewBag.SearchFilter1 = SearchFilter1;
            ViewBag.SearchFilter2 = SearchFilter2;
            var ActiveTab = "";
            if (!string.IsNullOrEmpty(SearchFilter1)) ActiveTab = "Affiliates";
            else if (!string.IsNullOrEmpty(SearchFilter2)) ActiveTab = "Converted Customers";
            ViewBag.ActiveTab = ActiveTab ?? "";
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;

            ViewBag.StartIndex2 = StartIndex2;
            ViewBag.PageSize2 = PageSize2;
            ViewBag.TotalData2 = dbReferalRes != null && dbReferalRes.Any() ? dbReferalRes[0].TotalRecords : 0;
            return View(ResponseModel);
        }

        [HttpPost, ValidateAntiForgeryToken, OverrideActionFilters]
        public JsonResult ApproveRejectAffiliateRequest(string HoldAgentId, string Status)
        {
            var response = new CommonDbResponse();
            var hAgentId = !string.IsNullOrEmpty(HoldAgentId) ? HoldAgentId.DecryptParameter() : null;
            var ApprovalStatus = (!string.IsNullOrEmpty(Status)
                && (Status.Trim().ToUpper() == "A" || Status.Trim().ToUpper() == "R")) ? Status : null;
            if (string.IsNullOrEmpty(hAgentId) || string.IsNullOrEmpty(ApprovalStatus))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return Json(response.Message, JsonRequestBehavior.AllowGet);
            }
            var dbRequest = new ApproveRejectAffiliateCommon()
            {
                HoldAgentId = hAgentId,
                ApprovedStatus = ApprovalStatus,
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
                ActionIP = ApplicationUtilities.GetIP()
            };
            var dbResponse = _affiliateBuss.ApproveRejectAffiliateRequest(dbRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return Json(response.Message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken, OverrideActionFilters]
        public JsonResult ManageAffiliateStatus(string AgentId, string Status)
        {
            var response = new CommonDbResponse();
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            var manageStatus = (!string.IsNullOrEmpty(Status)
                && (Status.Trim().ToUpper() == "A" || Status.Trim().ToUpper() == "B")) ? Status : null;
            if (string.IsNullOrEmpty(aId) || string.IsNullOrEmpty(manageStatus))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return Json(response.Message, JsonRequestBehavior.AllowGet);
            }
            var dbRequest = new ManageAffiliateStatusCommon()
            {
                AgentId = aId,
                Status = manageStatus,
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString(),
                ActionIP = ApplicationUtilities.GetIP()
            };
            var dbResponse = _affiliateBuss.ManageAffiliateStatus(dbRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return Json(response.Message, JsonRequestBehavior.AllowGet);
        }
    }
}