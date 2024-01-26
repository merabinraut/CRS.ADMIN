using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.SHARED.ClubManagement
{
    public class ClubListCommon : PaginationResponseCommon
    {
        public string LoginId { get; set; }
        public string AgentId { get; set; }
        public string Status { get; set; }
        public string ClubNameEng { get; set; }
        public string ClubNameJap { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string Rank { get; set; }
        public string Ratings { get; set; }
        public string Sno { get; set; }
        public string ClubLogo { get; set; }
        public string ClubCategory { get; set; }
    }

    public class ClubDetailCommon
    {
        public string AgentId { get; set; }
        public string UserId { get; set; }
        public string LoginId { get; set; }
        public string LocationId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string ClubName1 { get; set; }
        public string ClubName2 { get; set; }
        public string BusinessType { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string LocationURL { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Status { get; set; }
        public string Logo { get; set; }
        public string CoverPhoto { get; set; }
        public string BusinessCertificate { get; set; }
        public string Gallery { get; set; }
        public string WebsiteLink { get; set; }
        public string TiktokLink { get; set; }
        public string TwitterLink { get; set; }
        public string InstagramLink { get; set; }
        public string CompanyName { get; set; }
    }

    public class ManageClubCommon : Common
    {
        public string AgentId { get; set; }
        public string LoginId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string ClubName1 { get; set; }
        public string ClubName2 { get; set; }
        public string BusinessType { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string LocationURL { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Logo { get; set; }
        public string CoverPhoto { get; set; }
        public string BusinessCertificate { get; set; }
        public string Gallery { get; set; }
        public string WebsiteLink { get; set; }
        public string TiktokLink { get; set; }
        public string TwitterLink { get; set; }
        public string InstagramLink { get; set; }
        public string LocationId { get; set; }
        public string CompanyName { get; set; }
    }
}
