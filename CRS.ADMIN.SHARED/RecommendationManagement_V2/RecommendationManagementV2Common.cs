using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.SHARED.RecommendationManagement_V2
{
    #region "Club Recommedation Request List Model"
    public class ClubRecommendationManagementListModelCommon
    {
        public string RecommendationHoldId { get; set; }
        public string ClubId { get; set; }
        public string ClubName { get; set; }
        public string ClubLogo { get; set; }
        public string ClubCategory { get; set; }
        public string DisplayPage { get; set; }
        public string DisplayOrder { get; set; }
        public string RequestedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string Status { get; set; }
        public string DisplayId { get; set; }
        public string LocationId { get; set; }
        public string LocationName { get; set; }
    }
    #endregion

    #region "Location Model"
    public class LocationListModelCommon
    {
        public string LocationId { get; set; }
        public string LocationName { get; set; }
    }
    #endregion

    #region "Display Page"
    public class DisplayPageListModelCommon
    {
        public string PageId { get; set; }
        public string PageLabel { get; set; }
    }
    #endregion

    #region "Manage Group"
    public class GroupListModelCommon
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string TotalClubs { get; set; }
        public string Descriptions { get; set; }
        public string DisplayOrderId { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedIp { get; set; }
        public string LocationId { get; set; }
        public string RequestedDate { get; set; }
    }

    public class ManageGroupCommon : Common
    {
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string DisplayOrderId { get; set; }
        public string LocationId { get; set; }
    }
    #endregion

    #region "Shuffle Time Model"
    public class ShufflingTimeListModelCommon
    {
        public string DisplayId { get; set; }
        public string DisplayName { get; set; }
        public string DisplayTime { get; set; }
        public string LabelName { get; set; }
    }
    public class ManageShufflingTimeCommon : Common
    {
        public string DisplayId { get; set; }
        public string DisplayTime { get; set; }
        public string ShufflingTimeCSValue { get; set; }
    }
    #endregion

    #region "Pages Wise Club Request Model "
    public class HomePageClubRequestListModelCommon
    {
        public string RecommendationId { get; set; }
        public string ClubId { get; set; }
        public string ClubName { get; set; }
        public string ClubCategory { get; set; }
        public string ClubLogo { get; set; }
        public string DisplayId { get; set; }
        public string RequestedDate { get; set; }
        public string DisplayPageLabel { get; set; }
        public string HostName { get; set; }
        public string Status { get; set; }
        public string UpdatedDate { get; set; }
    }
    public class ManageHomePageRequestCommon : Common
    {
        public string ClubId { get; set; }
        public string LocationId { get; set; }
        public string HostId { get; set; }
    }
    public class SearchPageClubRequestListModelCommon : PaginationResponseCommon
    {
        public string RecommendationId { get; set; }
        public string ClubId { get; set; }
        public string ClubName { get; set; }
        public string ClubCategory { get; set; }
        public string ClubLogo { get; set; }
        public string DisplayId { get; set; }
        public string RequestedDate { get; set; }
        public string DisplayPageLabel { get; set; }
        public string HostName { get; set; }
        public string Status { get; set; }
        public string UpdatedDate { get; set; }
    }
    public class ManageSearchPageRequestCommon : Common
    {
        public string ClubId { get; set; }
        public string LocationId { get; set; }
        public string HostId { get; set; }
    }
    public class MainPageClubRequestListModelCommon
    {
        public string RecommendationId { get; set; }
        public string ClubId { get; set; }
        public string ClubName { get; set; }
        public string ClubCategory { get; set; }
        public string ClubLogo { get; set; }
        public string DisplayId { get; set; }
        public string RequestedDate { get; set; }
        public string DisplayPageLabel { get; set; }
        public string HostName { get; set; }
        public string Status { get; set; }
        public string UpdatedDate { get; set; }
    }
    public class ManageMainPageRequestCommon : Common
    {
        public string ClubId { get; set; }
        public string LocationId { get; set; }
        public string HostId { get; set; }
    }
    #endregion

    #region "Main Page Club Request"
    public class ClubRequestListModelCommon
    {
        public string RecommendationId { get; set; }
        public string ClubId { get; set; }
        public string ClubName { get; set; }
        public string ClubCategory { get; set; }
        public string ClubLogo { get; set; }
        public string DisplayOrder { get; set; }
        public string ActionDate { get; set; }
        public string DisplayPage { get; set; }
    }
    public class ManageClubRecommendationRequestCommon : Common
    {
        public string ClubId { get; set; }
        public string GroupId { get; set; }
        public string DisplayPageId { get; set; }
        public string DisplayOrderId { get; set; }
        public string LocationId { get; set; }
        public string HostRecommendationCSValue { get; set; }
        public string RecommendationId { get; set; }
        public string DisplayId { get; set; }

    }
    public class ManageManPageRecommendationReqUpdateCommon : Common
    {
        public string ClubId { get; set; }
        public string RecommendationHoldId { get; set; }
        public string LocationId { get; set; }
        public string DisplayHoldId { get; set; }
        public string GroupId { get; set; }
    }
    public class DisplayPageOrderDetailModelCommon
    {
        public string RecommendationId { get; set; }
        public string DisplayPageId { get; set; }
    }
    public class HostRecommendationDetailModelCommon
    {
        public string RecommendationId { get; set; }
        public string RecommendationHostId { get; set; }
        public string HostId { get; set; }
        public string HostDisplayOrderId { get; set; }
    }
    #endregion

    #region "Host List Model"
    public class HostListModelCommon
    {
        public string RecommendationId { get; set; }
        public string HostId { get; set; }
        public string RecommendationHostId { get; set; }
        public string ClubId { get; set; }
        public string HostName { get; set; }
        public string HostImage { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
        public string DisplayOrder { get; set; }
    }
    public class MainPageHostListForUpdateCommon
    {
        public string HostId { get; set; }
        public string RecommendationHoldId { get; set; }
        public string RecommendationHostHoldId { get; set; }
        public string HostDisplayOrderHoldId { get; set; }
        public string HostName { get; set; }
    }
    #endregion

    #region "Search And Home Page Recommendation Request Host List"
    public class SearchAndHomePageRecommendationReqHostListModelCommon
    {
        public string RecommendationHoldId { get; set; }
        public string ClubId { get; set; }
        public string RecommendationHostHoldId { get; set; }
        public string HostName { get; set; }
        public string HostImage { get; set; }
        public string DisplayId { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }
    #endregion

    #region "Main Page Club Recommendation Request Host List Model"
    public class MainPageClubRecommendationReqHostListModelCommon
    {
        public string RecommendationHoldId { get; set; }
        public string ClubId { get; set; }
        public string RecommendationHostHoldId { get; set; }
        public string HostName { get; set; }
        public string HostImage { get; set; }
        public string DisplayId { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }
    #endregion
}
