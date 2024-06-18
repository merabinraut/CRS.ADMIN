using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.ReviewAndRatingsManagement;
using DocumentFormat.OpenXml.Office2016.Excel;
using System;
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

        public List<ReviewCommon> GetReviews(PaginationFilterCommon Request, string reviewId = "")
        {
            var reviews = new List<ReviewCommon>();
            var sql = "sproc_review_and_ratings_admin_setup @Flag='srnr'";
            if (!string.IsNullOrWhiteSpace(reviewId))
                sql += " ,@ReviewId=" + _repositoryDao.FilterString(reviewId);
            sql += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchText=N" + _repositoryDao.FilterString(Request.SearchFilter) : null;
            sql += ",@Skip=" + Request.Skip;
            sql += ",@Take=" + Request.Take;
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
                        ReviewedOn = !string.IsNullOrEmpty(item["ReviewedOn"].ToString()) ? DateTime.Parse(item["ReviewedOn"].ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : item["ReviewedOn"].ToString(),
                        TotalRecords = Convert.ToInt32(_repositoryDao.ParseColumnValue(item, "TotalRecords").ToString()),
                        SNO = Convert.ToInt32(_repositoryDao.ParseColumnValue(item, "SNO").ToString())
                    };
                    reviews.Add(review);
                }
            }
            return reviews;
        }
    }
}