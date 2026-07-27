using CRS.ADMIN.SHARED.ConversionMetrixManagement;
using CRS.ADMIN.SHARED.LocationManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.ConversionMetrixManagement
{
    public interface IConversionMetrixRepository
    {
        ConversionSummaryCommon GetConversionSummary(long clubId);
        List<RankedStoreCommon> GetRankedStores(long? locationId, string searchFilter, int topCount, long? fromDateMs, long? toDateMs, string timeZoneOffsetValue);
        List<ActivityLogCommon> GetActivityLog(long? clubId, string searchFilter, string actionType, string sourcePageType, string userStatus, string fromDateMs, string toDateMs, int pageNo, int pageSize, string timeZoneOffsetValue);
        List<ActivityLogCommon> GetActivityLogList(ActivityLogFilterCommon request);
        List<StorePerformanceCommon> GetStorePerformance(long? clubId, string searchFilter, long? fromDateMs, long? toDateMs, int pageNo, int pageSize, string timeZoneOffsetValue);
        ClickAnalyticsResult GetClickAnalytics(long clubId, string channel, string timeZoneOffsetValue);
        long? ResolveClubCodeToAgentId(string clubCode);
        List<LocationCommon> GetLocationList();
        List<StorePerformanceCommon> GetConversionSummaryPerformanceRepost(PaginationFilterCommon dbRequest, string clubId);
        StorePerformanceCommon GetClubName(string clubId);
    }
}
