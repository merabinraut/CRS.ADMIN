using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.ReviewAndRatingsManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.ReviewAndRatingsManagement
{
    public interface IReviewAndRatingsRepository
    {
        List<ReviewCommon> GetReviews(PaginationFilterCommon Request, string reviewId = "");
        CommonDbResponse DeleteReview(string reviewId, string actionUser, string actionIp);
    }
}