using CRS.ADMIN.REPOSITORY.ConversionMetrixManagement;
using CRS.ADMIN.SHARED.ConversionMetrixManagement;
using CRS.ADMIN.SHARED.LocationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.BUSINESS.ConversionMetrixManagement
{
    public class ConversionMetrixBusiness :IConversionMetrixBusiness
    {
        private readonly IConversionMetrixRepository _repo;
        public ConversionMetrixBusiness(ConversionMetrixRepository conversionMetrixRepository) => this._repo = conversionMetrixRepository;

        #region Conversion Summary
        public ConversionSummaryCommon GetConversionSummary(string clubCode)
        {
            try
            {
                var data = _repo.GetConversionSummary(clubCode);
                return data ?? new ConversionSummaryCommon();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Click Analytics
        public ClickAnalyticsResult GetClickAnalytics(string clubCode, string channel, string timeZoneOffsetValue)
        {
            try
            {
                return _repo.GetClickAnalytics(clubCode, channel, timeZoneOffsetValue) ?? new ClickAnalyticsResult();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion


        #region Ranked Stores
        public List<RankedStoreCommon> GetRankedStores(
            long? locationId, string searchFilter, int topCount,
            long? fromDateMs, long? toDateMs, string timeZoneOffsetValue)
        {
            try
            {
                return _repo.GetRankedStores(locationId, searchFilter, topCount, fromDateMs, toDateMs, timeZoneOffsetValue)
                    ?? new List<RankedStoreCommon>();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Activity Log
        public List<ActivityLogCommon> GetActivityLog(long? clubId, string searchFilter, string actionType, string sourcePageType, string userStatus, long? fromDateMs, long? toDateMs, int pageNo, int pageSize, string timeZoneOffsetValue)
        {
            try
            {
                return _repo.GetActivityLog(clubId, searchFilter, actionType, sourcePageType, userStatus, fromDateMs, toDateMs, pageNo, pageSize, timeZoneOffsetValue)
                    ?? new List<ActivityLogCommon>();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Store Performance
        public List<StorePerformanceCommon> GetStorePerformance(
            long? clubId, string searchFilter, long? fromDateMs, long? toDateMs,
            int pageNo, int pageSize, string timeZoneOffsetValue)
        {
            try
            {
                return _repo.GetStorePerformance(clubId, searchFilter, fromDateMs, toDateMs, pageNo, pageSize, timeZoneOffsetValue)
                    ?? new List<StorePerformanceCommon>();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        public long? ResolveClubCodeToAgentId(string clubCode)
        {
            try
            {
                return _repo.ResolveClubCodeToAgentId(clubCode);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #region Location List
        public List<LocationCommon> GetLocationList()
        {
            try
            {
                return _repo.GetLocationList() ?? new List<LocationCommon>();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

    }
}
