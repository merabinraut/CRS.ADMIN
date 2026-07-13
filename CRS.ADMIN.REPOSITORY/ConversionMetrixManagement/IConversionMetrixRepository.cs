using CRS.ADMIN.SHARED.ConversionMetrixManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.ConversionMetrixManagement
{
    public interface IConversionMetrixRepository
    {
        ConversionSummaryCommon GetConversionSummary(string clubCode);
        List<RankedStoreCommon> GetRankedStores(long? locationId, string searchFilter, int topCount,long? fromDateMs, long? toDateMs, string timeZoneOffsetValue);
        List<ActivityLogCommon> GetActivityLog(long? clubId, string searchFilter, string actionType, string sourcePageType, long? fromDateMs, long? toDateMs, int pageNo, int pageSize, string timeZoneOffsetValue);
        List<StorePerformanceCommon> GetStorePerformance(long? clubId, string searchFilter, long? fromDateMs, long? toDateMs,int pageNo, int pageSize, string timeZoneOffsetValue);
        ClickAnalyticsResult GetClickAnalytics(string clubCode, string channel, string timeZoneOffsetValue);
        long? ResolveClubCodeToAgentId(string clubCode);

    }
}
