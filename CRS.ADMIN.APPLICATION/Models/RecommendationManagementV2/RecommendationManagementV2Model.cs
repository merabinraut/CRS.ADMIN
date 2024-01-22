using CRS.ADMIN.SHARED;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.RecommendationManagementV2
{
    public class CommonRecommendationModel
    {
        public List<LocationListModel> GetLocationList { get; set; }
        public List<DisplayPageListModel> GetDisplayPageList { get; set; }
        public List<GroupListModel> GetGroupList { get; set; }
        public List<ShufflingTimeListModel> GetShufflingTimeList { get; set; }
        public ManageGroup ManageGroup { get; set; }
        public ManageShufflingTime ManageShufflingTime { get; set; }
        public List<HomePageClubRequestListModel> GetClubRequestListByHomePage { get; set; }
        public List<SearchPageClubRequestListModel> GetClubRequestListBySearchPage { get; set; }
        public List<MainPageClubRequestListModel> GetClubRequestListByMainPage { get; set; }
        public List<ClubRecommendationManagementListModel> GetClubRecommendationrequestList { get; set; }
        public List<SearchAndHomePageRecommendationReqHostListModel> GetHomeAndSearchRecommendationHostList { get; set; }
        public List<MainPageClubRecommendationReqHostListModel> GetMainPageClubRecommendationReHostList { get; set; }
    }

    #region "Search And Home Page Recommendation Request Host List"
    public class SearchAndHomePageRecommendationReqHostListModel
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
    public class MainPageClubRecommendationReqHostListModel
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

    #region "Club Recommedation Request List Model"
    public class ClubRecommendationManagementListModel
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
    public class LocationListModel
    {
        public string LocationId { get; set; }
        public string LocationName { get; set; }
    }
    #endregion

    #region "Display Page"
    public class DisplayPageListModel
    {
        public string PageId { get; set; }
        public string PageLabel { get; set; }
    }
    #endregion

    #region "Manage Group"
    public class GroupListModel
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

    public class ManageGroup
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string GroupName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Description { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string DisplayOrderId { get; set; }
    }
    #endregion

    #region "Shuffle Time Model"
    public class ShufflingTimeListModel
    {
        public string DisplayId { get; set; }
        public string DisplayName { get; set; }
        public string DisplayTime { get; set; }
        public string LabelName { get; set; }
    }
    public class ManageShufflingTime
    {
        public string DisplayId { get; set; }
        public string DisplayTime { get; set; }
        public string ShufflingTimeCSValue { get; set; }
    }
    #endregion

    #region "Club Request Model"
    public class HomePageClubRequestListModel
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
    public class ManageHomePageRequest
    {
        [Required(ErrorMessage = "Required")]
        public string ClubId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string LocationId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string HostId { get; set; }
    }

    public class SearchPageClubRequestListModel
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
    public class ManageSearchPageRequest : Common
    {
        [Required(ErrorMessage = "Required")]
        public string ClubId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string LocationId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string HostId { get; set; }
    }
    public class MainPageClubRequestListModel
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
    public class ManageMainPageRequest : Common
    {
        public string ClubId { get; set; }
        public string LocationId { get; set; }
        public string HostId { get; set; }
    }

    #endregion

    #region "Recommendation Detail Common Model"
    public class RecommendationDetailCommonModel
    {
        public ManageClubRecommendationRequest GetMPageRecommendationDetailModel { get; set; } = new ManageClubRecommendationRequest();
        public List<HostRecommendationDetailModel> GetMPageHostRecommendationDetailModel { get; set; } = new List<HostRecommendationDetailModel>();
        public List<HostListModel> GetSelectedHostList { get; set; } = new List<HostListModel>();
        public List<MainPageHostListForUpdate> GetMainPageHostListForUpdate { get; set; } = new List<MainPageHostListForUpdate>();

    }
    public class ManageClubRecommendationRequest
    {
        public string ClubId { get; set; }
        public string GroupId { get; set; }
        public string DisplayPageId { get; set; }
        public string DisplayOrderId { get; set; }
        public string LocationId { get; set; }
        public string HostRecommendationCSValue { get; set; }
        public string RecommendationId { get; set; }
        public string DisplayId { get; set; }
        public string[] HostDDLByClubId { get; set; } = null;
        public string[] DisplayOrderDDL { get; set; } = null;
    }
    public class ManageManPageRecommendationReqUpdate
    {
        public string ClubId { get; set; }
        public string RecommendationHoldId { get; set; }
        public string LocationId { get; set; }
        public string DisplayHoldId { get; set; }
        public string GroupId { get; set; }
    }
    public class DisplayPageOrderDetailModel
    {
        public string RecommendationId { get; set; }
        public string DisplayPageId { get; set; }
    }
    public class HostRecommendationDetailModel
    {
        public string RecommendationId { get; set; }
        public string RecommendationHostId { get; set; }
        public string ClubId { get; set; }
        public string HostId { get; set; }
        public string OrderId { get; set; }
    }
    #endregion

    #region "Host List Model"
    public class HostListModel
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
    public class MainPageHostListForUpdate
    {
        public string HostId { get; set; }
        public string RecommendationHoldId { get; set; }
        public string RecommendationHostHoldId { get; set; }
        public string HostDisplayOrderHoldId { get; set; }
        public string HostName { get; set; }
    }
    #endregion
}