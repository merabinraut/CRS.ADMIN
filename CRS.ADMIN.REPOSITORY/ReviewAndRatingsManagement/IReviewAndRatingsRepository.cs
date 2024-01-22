using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ReviewAndRatingsManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.ReviewAndRatingsManagement
{
    public interface IReviewAndRatingsRepository
    {
        List<ReviewCommon> GetReviews(string reviewId = "", string searchText = "");
        CommonDbResponse DeleteReview(string reviewId, string actionUser, string actionIp);

    }
}