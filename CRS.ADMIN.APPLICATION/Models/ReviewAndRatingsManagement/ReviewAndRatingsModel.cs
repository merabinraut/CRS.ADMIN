using System.Collections.Generic;

namespace CRS.ADMIN.APPLICATION.Models.ReviewAndRatingsManagement
{
    public class ReviewAndRatingsModel
    {
        public List<ReviewModel> Reviews { get; set; } = new List<ReviewModel>();
    }
    public class ReviewModel
    {
        public string ReviewId { get; set; }
        public string UserImage { get; set; }
        public string NickName { get; set; }
        public string FullName { get; set; }
        public string ClubNameEng { get; set; }
        public string ClubNameJap { get; set; }
        public string EnglishRemark { get; set; }
        public string JapaneseRemark { get; set; }
        public string RemarkType { get; set; }
        public string Rating { get; set; }
        public string ReviewedOn { get; set; }
    }
}