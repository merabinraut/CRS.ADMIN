namespace CRS.ADMIN.SHARED.ReviewAndRatingsManagement
{
    public class ReviewCommon : Common
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