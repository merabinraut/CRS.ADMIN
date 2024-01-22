using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRS.ADMIN.APPLICATION.Models.RecommendationManagement
{
    #region Common Model"
    public class ManageGroupCommonModel
    {
        public ManageGroup ManageGroupModel { get; set; }
        public List<LocationListModel> LocationList { get; set; }
        public List<RecommendationGroupListModel> GroupListModel { get; set; }
        public List<ShufflingTimeListModel> ShufflingTimeListModels { get; set; }
        public ManageShufflingTime ManageShufflingTime { get; set; }
        public List<RecommendationRequestListModel> RecommendationRequestList { get; set; }
        public List<RecommendationAnalyticListModel> RecommendationAnalyticsList { get; set; }
    }
    #endregion

    #region "Location Model"
    public class LocationListModel
    {
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public string LocationImage { get; set; }
    }
    #endregion

    #region "Group Model"
    public class RecommendationGroupListModel
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string TotalClubs { get; set; }
        public string Descriptions { get; set; }
        public string RequestedDate { get; set; }
    }

    public class ManageGroup
    {
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string DisplayOrderId { get; set; }
    }
    #endregion

    #region "Shuffle Time Model"
    public class ShufflingTimeListModel
    {
        public string DisplayId { get; set; }
        public string DisplayName { get; set; }
        public string DisplayTime { get; set; }
    }
    public class ManageShufflingTime
    {
        public string DisplayId { get; set; }
        public string DisplayTime { get; set; }
        public string ShufflingTimeCSValue { get; set; }
    }
    public class RecommendationRequestListModel
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
    #endregion

    #region "Recommendation Detail Common Model"
    public class RecommendationDetailCommonModel
    {
        public ManageClubRecommendationRequest GetRecommendationDetailModel { get; set; } = new ManageClubRecommendationRequest();
        public List<DisplayPageOrderDetailModel> GetDisplayPOrderDetailModel { get; set; }
        public List<HostRecommendationDetailModel> GetHostRecommendationDetailModel { get; set; }

    }
    #endregion
    #region "Club Request Model"
    public class ClubRequestListModel
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
    public class ManageClubRecommendationRequest
    {
        public string ClubId { get; set; }
        public string GroupId { get; set; }
        public string DisplayPageId { get; set; }
        public string DisplayOrderId { get; set; }
        public string LocationId { get; set; }
        public string HostRecommendationCSValue { get; set; }
        public string RecommendationId { get; set; }
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

    #region "Recommendation Analytics"
    public class RecommendationAnalyticListModel
    {
        public string StatusLabel { get; set; } 
        public string StatusCount { get; set; } 
    }
    #endregion
}