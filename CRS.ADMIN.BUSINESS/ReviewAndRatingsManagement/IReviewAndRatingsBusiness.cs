using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.ReviewAndRatingsManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.ReviewAndRatingsManagement
{
    public interface IReviewAndRatingsBusiness
    {
        List<ReviewCommon> GetReviews(PaginationFilterCommon Request, string reviewId = "");
        CommonDbResponse DeleteReview(string reviewId, string actionUser, string actionIp);

    }
}