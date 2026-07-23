using CRS.ADMIN.SHARED.ConversionMetrixManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRS.ADMIN.APPLICATION.Models.ConversionMetrixManagement
{
    public class ConversionMetrixOverviewModel
    {
        public string SearchFilter { get; set; }
        public string TabValue { get; set; }
        public string ActionType { get; set; }
        public string SourcePageType { get; set; }
        public string UserStatus { get; set; }
        public string clubId { get; set; }
        public string clubName { get; set; }
        public ConversionSummaryModel ConversionSummaryModel { get; set; }
        public List<ClickChartModel> ClickChartList { get; set; }
        public List<ActionSourceModel> ActionSourceModel { get; set; }
        public List<ClickOriginModel> ClickOriginList { get; set; }

        public StorePerformanceModel storePerformanceModels { get; set; }

        public List<StorePerformanceModel> StorePerformanceModel { get; set; }
        public ActivityLogFilterModel ActivityLogFilterModel { get; set; }
        public List<ActivityLogModel> ActivityLogModel { get; set; }
        public List<StoreRankingModel> StoreRankingList { get; set; }
    }
    public class ConversionSummaryModel
    {
        public int TotalSumClicks { get; set; }
        public int ReservationClicks { get; set; }
        public int PhoneCallClicks { get; set; }
        public decimal AverageCTR { get; set; }
    }
    public class ActivityLogModel
    {
        public int SNO { get; set; }
        public string ActivityId { get; set; }
        public string ActionType { get; set; }
        public string SessionId { get; set; }
        public string SourcePage { get; set; }
        public string ClubId { get; set; }
        public string ClubCode { get; set; }
        public string TargetName { get; set; }
        public string UserStatus { get; set; }
        public string SourcePageType { get; set; }
        public string Prefecture { get; set; }
        public string Browser { get; set; }
        public string UserAgent { get; set; }
        public string Date { get; set; }
        public int TotalRecords { get; set; }
        public string TabValue { get; set; }
        public string FromDate { get; set; }




    }
    public class ClickChartModel
    {
        public string TimeLabel { get; set; }
        public int ClickCount { get; set; }
    }
    public class ActionSourceModel
    {
        public string StorePage { get; set; }
        public decimal Percentage { get; set; }
    }


    public class ClickOriginModel
    {
        public string AreaName { get; set; }
        public decimal Percentage { get; set; }
    }
    public class StorePerformanceModel
    {
        public int SNo { get; set; }
        public string ClubId { get; set; }
        public string StoreName { get; set; }
        public string ClubName { get; set; }
        public int BookingStorePage { get; set; }
        public int BookingHostDetails { get; set; }
        public int PhoneStorePage { get; set; }
        public int PhoneHostDetails { get; set; }
        public int TotalClicks { get; set; }
        public int TotalRecords { get; set; }
    }
    public class ActivityLogFilterModel
    {
        public string ActionType { get; set; }
        public string TargetType { get; set; }
        public string UserStatus { get; set; }
        public List<ActivityLogModel> ActivityListModel { get; set; } = new List<ActivityLogModel>();
    }
    public class StoreRankingModel
    {
        public int Rank { get; set; }
        public string ClubName { get; set; }
        public string ClubCode { get; set; }
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public string ReservationClickCount { get; set; }
        public string PhoneClickCount { get; set; }
        public string TotalCount { get; set; }
        public string ClubId { get; set; }
        public string Area { get; set; }
        public int ClickCount { get; set; }
    }


}