using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CRS.ADMIN.SHARED.ConversionMetrixManagement
{
    public class ClickChartCommon
    {
        public int TotalClicks { get; set; }
        public int ReservationsClicks { get; set; }
        public int PhoneClicks { get; set; }
        public string TimeSlot { get; set; }
    }
    public class  ConversionSummaryCommon
    {
        public int TotalClicks { get; set; }
        public int ReservationClicks { get; set; }
        public int PhoneClicks { get; set; }
        public int ReservationConvertedCount { get; set; }
        public int AverageCTR { get; set; }
    }
    public class ActionSourceCommon
    {
        public string Label { get; set; }
        public int Clicks { get; set; }
        public int PhoneClicks { get; set; }
        public int ReservationClicks { get; set; }
        public decimal Percentage { get; set; }
        public string StorePage { get; set; }
    }
    public class ClickOriginCommon
    {
        public string Area { get; set; }
        public int Clicks { get; set; }
        public decimal Percentage { get; set; }
    }
    public class ActivityLogCommon
    {
        public int SNO { get; set; }
        public long? ClubId { get; set; }
        public string ClubCode { get; set; }
        public string ClubName { get; set; }
        public long? DateMs { get; set; }

        public string ActionType { get; set; }
        public string SourcePage { get; set; }
        public string TargetName { get; set; }
        public string SessionId { get; set; }
        public string UserStatus { get; set; }
        public string Prefecture { get; set; }
        public string Browser { get; set; }
        public DateTime? Date { get; set; }
        public int TotalRecords { get; set; }
    }
    public class ActiveLogCommon
    {
        public long EventSno { get; set; }
        public string EventId { get; set; }
        public string EventName { get; set; }
        public string StepCode { get; set; }
        public string EventStatus { get; set; }
        public string EventSource { get; set; }
        public string ClubId { get; set; }
        public string HostId { get; set; }
        public string SourcePageType { get; set; }
        public string PageUrl { get; set; }
        public string ClientUTCDate { get; set; }
        public string ServerReceivedUTCDate { get; set; }
        public string DeviceType { get; set; }
        public string LandingPageUrl { get; set; }
        public string JourneyStatus { get; set; }
        public string ReservationId { get; set; }
        public int TotalRecords { get; set; }
    }
    public class StorePerformanceCommon
    {
        public string ClubId { get; set; }
        public string StoreName { get; set; }
        public int BookingStorePage { get; set; }
        public int BookingHostDetails { get; set; }
        public int PhoneStorePage { get; set; }
        public int PhoneHostDetails { get; set; }
        public int TotalClicks { get; set; }
        public int TotalRecords { get; set; }
    }
    public class ActiveLogRequestDtoCommon
    {
        public string ClubId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class StorePerformanceRequestCommon
    {
        public string SearchFilter { get; set; }
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class RankedStoreCommon
    {
        public int Rank { get; set; }
        public long ClubId { get; set; }
        public string ClubCode { get; set; }
        public string ClubName { get; set; }
        public long? LocationId { get; set; }
        public string LocationName { get; set; }
        public int ReservationClickCount { get; set; }
        public int PhoneClickCount { get; set; }
        public string Area { get; set; }
        public int TapCount { get; set; }
    }

    public class ClickAnalyticsResult
    {
        public List<TodayClickBucket> TodayClicks { get; set; } = new List<TodayClickBucket>();
        public List<ActionSourceCommon> ActionSource { get; set; } = new List<ActionSourceCommon>();
        public List<ClickOriginCommon> ClickOrigin { get; set; } = new List<ClickOriginCommon>();
    }

    public class TodayClickBucket
    {
        public string Time { get; set; }
        public int Value { get; set; }
        public string channel { get; set; }
    }

}
