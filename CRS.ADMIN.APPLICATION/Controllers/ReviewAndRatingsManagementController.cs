using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ReviewAndRatingsManagement;
using CRS.ADMIN.BUSINESS.ReviewAndRatingsManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Linq;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class ReviewAndRatingsManagementController : BaseController
    {
        private readonly IReviewAndRatingsBusiness _buss;
        public ReviewAndRatingsManagementController(IReviewAndRatingsBusiness buss) => _buss = buss;
        public ActionResult Index(string searchText = "", int StartIndex = 0, int PageSize = 10)
        {
            Session["CurrentUrl"] = "/ReviewAndRatingsManagement/Index";
            var reviewAndRatingsViewModel = new ReviewAndRatingsModel();
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(searchText) ? searchText : null
            };
            var reviewCommon = _buss.GetReviews(dbRequest);
            if (reviewCommon != null && reviewCommon.Count > 0)
            {
                reviewAndRatingsViewModel.Reviews = reviewCommon.MapObjects<ReviewModel>();
                reviewAndRatingsViewModel.Reviews.ForEach(x =>
                    {
                        x.ReviewId = x.ReviewId.EncryptParameter();
                        x.UserImage = ImageHelper.ProcessedImage(x.UserImage);
                    }
                );
            }
            ViewBag.SearchFilter = searchText;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = reviewCommon != null && reviewCommon.Any() ? reviewCommon[0].TotalRecords : 0;
            return View(reviewAndRatingsViewModel);
        }

        [HttpPost]
        public JsonResult RemoveReviewAndRatings(string reviewId)
        {
            var rId = string.IsNullOrEmpty(reviewId) ? string.Empty : reviewId.DecryptParameter();
            if (string.IsNullOrEmpty(rId))
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.WARNING,
                    Message = "Invalid Details",
                    Title = NotificationMessage.WARNING.ToString()
                });
                return Json(new { success = false, message = "Invalid Details" });
            }

            string actionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
            string actionIp = ApplicationUtilities.GetIP();

            var dbResp = _buss.DeleteReview(rId, actionUser, actionIp);
            if (dbResp != null && dbResp.Code == ResponseCode.Success)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResp?.Message,
                    Title = NotificationMessage.SUCCESS.ToString()
                });
                return Json(new { success = true, message = dbResp?.Message });
            }
            else
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.ERROR,
                    Message = dbResp?.Message ?? "Something went wrong",
                    Title = NotificationMessage.ERROR.ToString()
                });
                return Json(new { success = false, message = dbResp?.Message ?? "Something went wrong" });
            }
        }
    }
}