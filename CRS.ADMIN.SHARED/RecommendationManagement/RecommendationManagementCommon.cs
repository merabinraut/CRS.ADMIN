using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.SHARED.RecommendationManagement
{
    #region "Group"
    public class ManageGroupCommon : Common
    {
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string DisplayOrderId { get; set; }
    }
    public class RecommendationGroupListModelCommon
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string TotalClubs { get; set; }
        public string Descriptions { get; set; }
        public string RequestedDate { get; set; }
    }
    #endregion

    #region "Location Model"
    public class LocationListModelCommon
    {
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public string LocationImage { get; set; }
    }
    #endregion

    #region "Shuffle Time"
    public class ShufflingTimeListModelCommon
    {
        public string DisplayId { get; set; }
        public string DisplayName { get; set; }
        public string DisplayTime { get; set; }
    }
    public class ManageShufflingTimeCommon : Common
    {
        public string DisplayId { get; set; }
        public string DisplayTime { get; set; }
        public string ShufflingTimeCSValue { get; set; }
    }
    public class RecommendationRequestListModelCommon
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

    #region "Club Request"
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
        public string ClubId { get; set; }
        public string HostId { get; set; }
        public string OrderId { get; set; }
    }
    #endregion

    #region "Recommendation Analytics"
    public class RecommendationAnalyticListModelCommon
    {
        public string StatusLabel { get; set; }
        public string StatusCount { get; set; }
    }
    #endregion
}
