using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ReviewAndRatingsManagement;
using System.Collections.Generic;
using System.Data;

namespace CRS.ADMIN.REPOSITORY.ReviewAndRatingsManagement
{
    public class ReviewAndRatingsRepository : IReviewAndRatingsRepository
    {
        private readonly RepositoryDao _repositoryDao;

        public ReviewAndRatingsRepository() => _repositoryDao = new RepositoryDao();

        public CommonDbResponse DeleteReview(string reviewId, string actionUser, string actionIp)
        {
            var sql = "sproc_review_and_ratings_admin_setup @Flag='dre'";
            sql += ",@ReviewId=" + _repositoryDao.FilterString(reviewId);
            sql += ", @ActionUser=" + _repositoryDao.FilterString(actionUser);
            sql += ", @ActionIP=" + _repositoryDao.FilterString(actionIp);
            return _repositoryDao.ParseCommonDbResponse(sql);
        }

        public List<ReviewCommon> GetReviews(string reviewId = "", string searchText = "")
        {
            var reviews = new List<ReviewCommon>();
            var sql = "sproc_review_and_ratings_admin_setup @Flag='srnr'";
            if (!string.IsNullOrWhiteSpace(reviewId))
                sql += " ,@ReviewId=" + _repositoryDao.FilterString(reviewId);
            if (!string.IsNullOrWhiteSpace(searchText))
                sql += " ,@SearchText=N" + _repositoryDao.FilterString(searchText);
            var dt = _repositoryDao.ExecuteDataTable(sql);
            if (null != dt)
            {
                foreach (DataRow item in dt.Rows)
                {
                    var review = new ReviewCommon()
                    {
                        ReviewId = item["ReviewId"].ToString(),
                        UserImage = item["UserImage"].ToString(),
                        NickName = item["NickName"].ToString(),
                        FullName = item["FullName"].ToString(),
                        ClubNameEng = item["ClubNameEng"].ToString(),
                        ClubNameJap = item["ClubNameJap"].ToString(),
                        EnglishRemark = item["EnglishRemark"].ToString(),
                        JapaneseRemark = item["JapaneseRemark"].ToString(),
                        RemarkType = item["RemarkType"].ToString(),
                        Rating = item["Rating"].ToString(),
                        ReviewedOn = item["ReviewedOn"].ToString(),
                    };
                    reviews.Add(review);
                }
            }
            return reviews;
        }
    }
}