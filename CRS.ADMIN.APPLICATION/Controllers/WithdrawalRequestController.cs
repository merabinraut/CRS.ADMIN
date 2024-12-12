

using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models;
using CRS.ADMIN.APPLICATION.Models.PointsManagement;
using CRS.ADMIN.APPLICATION.Models.WithdrawalRequest;
using CRS.ADMIN.BUSINESS.WithdrawalRequest;
using CRS.ADMIN.BUSINESS.WithdrawSetup;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PointsManagement;
using CRS.ADMIN.SHARED.WithdrawalRequest;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Google.Apis.Requests.BatchRequest;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class WithdrawalRequestController : BaseController
    {
        private readonly IWithdrawalRequestBusiness _withdrawBuss;
        public WithdrawalRequestController(IWithdrawalRequestBusiness withdrawBuss)
        {
            _withdrawBuss = withdrawBuss;
        }
        [HttpGet]
        public ActionResult WithdrawalRequestMonthlyList(string SearchFilter = "", int StartIndex = 0, int PageSize = 10,string FromDate="", string ToDate = "")
        {
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                FromDate = FromDate,
                ToDate=ToDate,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            WithdrawalRequestModel obj = new WithdrawalRequestModel();
            var DBResponse = _withdrawBuss.GetWithdrawal(dbRequest);
            var mappedResponseObjects = DBResponse.MapObjects<WithdrawalMonthlyList>();
            obj.WithdrawalMonthlyList = mappedResponseObjects;
            obj.FromDate = FromDate;
            obj.ToDate = ToDate;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = DBResponse.Count > 0 ? DBResponse.FirstOrDefault().TotalRecords : 0 ;
            return View(obj);
        }
        [HttpGet]
        public ActionResult WithdrawalRequestDetailList(string SearchFilter = "", int StartIndex = 0, int PageSize = 10, WithdrawalDetailsModel objWithdrawalDetailsModel=null)
        {
          
            WithdrawalFilterCommon common = new WithdrawalFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                FromDate = objWithdrawalDetailsModel.FromDate,
                ToDate = objWithdrawalDetailsModel.ToDate,
                id = objWithdrawalDetailsModel.id,
                //bankType = objWithdrawalDetailsModel.bankType,
                //bankName = objWithdrawalDetailsModel.bankName,
                //branchName = objWithdrawalDetailsModel.branchName,
                //affiliateInfo = objWithdrawalDetailsModel.affiliateInfo,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            var DBResponse = _withdrawBuss.GetWithdrawalDetails(common);
            var mappedResponseObjects = DBResponse.MapObjects<WithdrawalMonthlyDetailsModel>();
            objWithdrawalDetailsModel.WithdrawalMonthlyList = mappedResponseObjects;
            objWithdrawalDetailsModel.WithdrawalMonthlyList.ForEach(item =>
            {
                item.id = item.id?.EncryptParameter();
                item.requestedDate = !string.IsNullOrEmpty(item.requestedDate) ? DateTime.Parse(item.requestedDate).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : item.requestedDate;
                item.requestedAmount = Convert.ToInt64(item.requestedAmount).ToString("N0");
                item.chargeAmount = Convert.ToInt64(item.chargeAmount).ToString("N0");
                item.transferAmount = Convert.ToInt64(item.transferAmount).ToString("N0");
            });
            objWithdrawalDetailsModel.name = objWithdrawalDetailsModel.WithdrawalMonthlyList.FirstOrDefault().name;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;           
            ViewBag.TotalData = DBResponse.Count > 0 ? DBResponse.FirstOrDefault().TotalRecords : 0;
            return View("WithdrawalDetailList",objWithdrawalDetailsModel);
        }
        [HttpPost, ValidateAntiForgeryToken, OverrideActionFilters]
        public JsonResult ApproveWithdrawalRequest(string id)
        {
            var response = new CommonDbResponse();
            var Id = !string.IsNullOrEmpty(id) ? id.DecryptParameter() : null;
            if (string.IsNullOrEmpty(Id))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
            }
            
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            string status = "A";
            var dbResponse = _withdrawBuss.ManageRequestStatus(Id, status, commonRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return Json(dbResponse.Message, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateAntiForgeryToken, OverrideActionFilters]
        public JsonResult RejectWithdrawalRequest(string id)
        {
            var response = new CommonDbResponse();
            var Id = !string.IsNullOrEmpty(id) ? id.DecryptParameter() : null;
            if (string.IsNullOrEmpty(Id))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
            }
            
            var commonRequest = new Common()
            {
                ActionIP = ApplicationUtilities.GetIP(),
                ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString()
            };
            string status = "R";
            var dbResponse = _withdrawBuss.ManageRequestStatus(Id, status, commonRequest);
            response = dbResponse;
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                Message = response.Message ?? "Something went wrong. Please try again later",
                Title = response.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
            });
            return Json(dbResponse.Message, JsonRequestBehavior.AllowGet);
        }
    }
}