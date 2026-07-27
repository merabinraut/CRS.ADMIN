using Aspose.Cells;
using CRS.ADMIN.SHARED.ConversionMetrixManagement;
using CRS.ADMIN.SHARED.LocationManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PlanManagement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.ConversionMetrixManagement
{
    public class ConversionMetrixRepository : IConversionMetrixRepository
    {
        private readonly AnalyticsDao _dao;
        private readonly RepositoryDao _repositoryDao;
        public ConversionMetrixRepository()
        {
            _dao = new AnalyticsDao();
            _repositoryDao = new RepositoryDao();
        }

        #region Conversion Summary
        public ConversionSummaryCommon GetConversionSummary(long clubId)
        {
            var response = new ConversionSummaryCommon();
            try
            {
                long? resolvedClubId = clubId;

                var sql = "EXEC [analytics].[sproc_admin_get_conversion_matrix_overview]"
          + $" @clubId={(resolvedClubId.HasValue ? resolvedClubId.Value.ToString() : "NULL")}";

                var dbResponse = _dao.ExecuteDataRow(sql);
                if (dbResponse != null)
                {
                    response.TotalClicks = SafeInt(_dao.ParseColumnValue(dbResponse, "totalClickCount"));
                    response.ReservationClicks = SafeInt(_dao.ParseColumnValue(dbResponse, "reservationClickCount"));
                    response.PhoneClicks = SafeInt(_dao.ParseColumnValue(dbResponse, "phoneClickCount"));
                    response.ReservationConvertedCount = SafeInt(_dao.ParseColumnValue(dbResponse, "reservationConvertedCount"));
                    response.AverageCTR = SafeDecimal(_dao.ParseColumnValue(dbResponse, "reservationConvertedPercentage"));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }
        #endregion

        #region Ranked Stores
        public List<RankedStoreCommon> GetRankedStores(long? locationId, string searchFilter, int topCount, long? fromDateMs, long? toDateMs, string timeZoneOffsetValue)
        {
            var response = new List<RankedStoreCommon>();
            try
            {
                var sql = "EXEC [analytics].[sproc_admin_get_conversion_top_store_list]"
                    + $" @locationId={(locationId.HasValue ? locationId.Value.ToString() : "NULL")}"
                    + $",@searchFilter={(string.IsNullOrEmpty(searchFilter) ? "NULL" : $"N'{_dao.FilterString(searchFilter)}'")}"
                    + $",@topCount={topCount}"
                    + $",@fromDateMs={(fromDateMs.HasValue ? fromDateMs.Value.ToString() : "NULL")}"
                    + $",@toDateMs={(toDateMs.HasValue ? toDateMs.Value.ToString() : "NULL")}"
                    + $",@timeZoneOffsetValue={(string.IsNullOrEmpty(timeZoneOffsetValue) ? "NULL" : $"'{_dao.FilterString(timeZoneOffsetValue)}'")}";

                var dt = ExecuteDataSetFirstTable(sql);

                if (dt != null)
                {
                    if (dt.Columns.Contains("code") && dt.Columns.Contains("message") && !dt.Columns.Contains("rankNo"))
                    {
                        var msg = dt.Rows.Count > 0 ? dt.Rows[0]["message"]?.ToString() : "Unknown date range error.";
                        throw new InvalidOperationException(msg);
                    }

                    foreach (DataRow item in dt.Rows)
                    {
                        response.Add(new RankedStoreCommon
                        {
                            Rank = SafeInt(_dao.ParseColumnValue(item, "rankNo")),
                            ClubId = SafeLong(_dao.ParseColumnValue(item, "clubId")) ?? 0,
                            ClubCode = _dao.ParseColumnValue(item, "clubCode")?.ToString(),
                            ClubName = _dao.ParseColumnValue(item, "clubName")?.ToString(),
                            ClubNameJp = _dao.ParseColumnValue(item, "clubNameJp")?.ToString(),
                            LocationId = SafeLong(_dao.ParseColumnValue(item, "locationId")),
                            LocationName = _dao.ParseColumnValue(item, "LocationName")?.ToString(),
                            ReservationClickCount = SafeInt(_dao.ParseColumnValue(item, "reservationClickCount")),
                            PhoneClickCount = SafeInt(_dao.ParseColumnValue(item, "phoneClickCount")),
                            TapCount = SafeInt(_dao.ParseColumnValue(item, "tapCount"))
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }
        #endregion

        #region Activity Log
        public List<ActivityLogCommon> GetActivityLog(long? clubId, string searchFilter, string actionType, string sourcePageType, string userStatus, string fromDate, string toDate, int pageNo, int pageSize, string timeZoneOffsetValue)
        {
            var response = new List<ActivityLogCommon>();
            try
            {
                var sql = "EXEC [analytics].[sproc_admin_get_conversion_activity_log]"
                    + $" @clubId={(clubId.HasValue ? clubId.Value.ToString() : "NULL")}"
                    + $",@searchFilter={(string.IsNullOrEmpty(searchFilter) ? "NULL" : $"N'{_dao.FilterString(searchFilter)}'")}"
                    + $",@actionType={(string.IsNullOrEmpty(actionType) ? "NULL" : $"'{_dao.FilterString(actionType)}'")}"
                    + $",@sourcePageType={(string.IsNullOrEmpty(sourcePageType) ? "NULL" : $"'{_dao.FilterString(sourcePageType)}'")}"
                    + $",@userStatus={(string.IsNullOrEmpty(userStatus) ? "NULL" : $"'{_dao.FilterString(userStatus)}'")}"
                    + $",@fromDateMs={(string.IsNullOrEmpty(fromDate) ? "NULL" : $"'{_dao.FilterString(fromDate)}'")}"
                    + $",@toDateMs={(string.IsNullOrEmpty(toDate) ? "NULL" : $"'{_dao.FilterString(toDate)}'")}"
                    + $",@pageNo={pageNo}"
                    + $",@pageSize={pageSize}"
                    + $",@timeZoneOffsetValue={(string.IsNullOrEmpty(timeZoneOffsetValue) ? "NULL" : $"'{_dao.FilterString(timeZoneOffsetValue)}'")}";

                var dbResponse = _dao.ExecuteDataTable(sql);
                if (dbResponse != null)
                {
                    foreach (DataRow item in dbResponse.Rows)
                    {
                        response.Add(new ActivityLogCommon
                        {
                            SNO = SafeInt(_dao.ParseColumnValue(item, "sn")),
                            ClubId = SafeLong(_dao.ParseColumnValue(item, "clubId")),
                            ClubCode = _dao.ParseColumnValue(item, "clubCode")?.ToString(),
                            ActivityId = _dao.ParseColumnValue(item, "activityId")?.ToString(),
                            ClubName = _dao.ParseColumnValue(item, "clubName")?.ToString(),
                            ActionType = _dao.ParseColumnValue(item, "actionType")?.ToString(),
                            SourcePage = _dao.ParseColumnValue(item, "sourcePage")?.ToString(),
                            TargetName = _dao.ParseColumnValue(item, "targetName")?.ToString(),
                            SessionId = _dao.ParseColumnValue(item, "conversionSessionSno")?.ToString(),
                            UserStatus = _dao.ParseColumnValue(item, "userStatus")?.ToString(),
                            Prefecture = _dao.ParseColumnValue(item, "prefecture")?.ToString(),
                            Browser = _dao.ParseColumnValue(item, "browser")?.ToString(),
                            UserAgent = _dao.ParseColumnValue(item, "userAgent")?.ToString(),
                            DateMs = SafeLong(_dao.ParseColumnValue(item, "activityDateMS")),
                            TotalRecords = SafeInt(_dao.ParseColumnValue(item, "totalRecords"))
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }
        #endregion
        #region Activity Log List
        public List<ActivityLogCommon> GetActivityLogList(ActivityLogFilterCommon request)
        {
            var response = new List<ActivityLogCommon>();
            try
            {
                var sql = "EXEC [analytics].[sproc_admin_get_conversion_activity_log]"
                    + $" @clubId={(string.IsNullOrEmpty(request.ClubId) ? "NULL" : $"N'{_dao.FilterString(request.ClubId)}'")}"
                    + $",@searchFilter={(string.IsNullOrEmpty(request.SearchFilter) ? "NULL" : $"N'{_dao.FilterString(request.SearchFilter)}'")}"
                    + $",@actionType={(string.IsNullOrEmpty(request.ActionType) ? "NULL" : $"'{_dao.FilterString(request.ActionType)}'")}"
                    + $",@sourcePageType={(string.IsNullOrEmpty(request.SourcePageType) ? "NULL" : $"'{_dao.FilterString(request.SourcePageType)}'")}"
                    + $",@userStatus={(string.IsNullOrEmpty(request.UserStatus) ? "NULL" : $"'{_dao.FilterString(request.UserStatus)}'")}"
                    + $",@fromDateMs={(string.IsNullOrEmpty(request.FromDate) ? "NULL" : $"'{_dao.FilterString(request.FromDate)}'")}"
                    + $",@toDateMs={(string.IsNullOrEmpty(request.ToDate) ? "NULL" : $"'{_dao.FilterString(request.ToDate)}'")}"
                    + $",@pageNo={request.Skip}"
                    + $",@pageSize={request.Take}";
                //+ $",@timeZoneOffsetValue={(string.IsNullOrEmpty(timeZoneOffsetValue) ? "NULL" : $"'{_dao.FilterString(timeZoneOffsetValue)}'")}";

                var dbResponse = _dao.ExecuteDataTable(sql);
                if (dbResponse != null)
                {
                    foreach (DataRow item in dbResponse.Rows)
                    {
                        response.Add(new ActivityLogCommon
                        {
                            SNO = SafeInt(_dao.ParseColumnValue(item, "sn")),
                            ClubId = SafeLong(_dao.ParseColumnValue(item, "clubId")),
                            ClubCode = _dao.ParseColumnValue(item, "clubCode")?.ToString(),
                            HostCode = _dao.ParseColumnValue(item, "hostCode")?.ToString(),
                            ActivityId = _dao.ParseColumnValue(item, "activityId")?.ToString(),
                            CustomerlocationJson = _dao.ParseColumnValue(item, "customerLocationJson")?.ToString(),
                            ClubName = _dao.ParseColumnValue(item, "clubName")?.ToString(),
                            ActionType = _dao.ParseColumnValue(item, "actionType")?.ToString(),
                            SourcePage = _dao.ParseColumnValue(item, "sourcePage")?.ToString(),
                            TargetName = _dao.ParseColumnValue(item, "targetName")?.ToString(),
                            SessionId = _dao.ParseColumnValue(item, "conversionSessionSno")?.ToString(),
                            UserStatus = _dao.ParseColumnValue(item, "userStatus")?.ToString(),
                            Prefecture = _dao.ParseColumnValue(item, "prefecture")?.ToString(),
                            Browser = _dao.ParseColumnValue(item, "browser")?.ToString(),
                            UserAgent = _dao.ParseColumnValue(item, "userAgent")?.ToString(),
                            DateMs = SafeLong(_dao.ParseColumnValue(item, "activityDateMS")),
                            TotalRecords = SafeInt(_dao.ParseColumnValue(item, "totalRecords"))
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }
        #endregion

        #region Store Performance
        public List<StorePerformanceCommon> GetStorePerformance(
            long? clubId, string searchFilter, long? fromDateMs, long? toDateMs,
            int pageNo, int pageSize, string timeZoneOffsetValue)
        {
            var response = new List<StorePerformanceCommon>();
            try
            {
                var sql = "EXEC [analytics].[sproc_admin_get_conversion_store_performance]"
                    + $" @clubId={(clubId.HasValue ? clubId.Value.ToString() : "NULL")}"
                    + $",@searchFilter={(string.IsNullOrEmpty(searchFilter) ? "NULL" : $"N'{_dao.FilterString(searchFilter)}'")}"
                    + $",@fromDateMs={(fromDateMs.HasValue ? fromDateMs.Value.ToString() : "NULL")}"
                    + $",@toDateMs={(toDateMs.HasValue ? toDateMs.Value.ToString() : "NULL")}"
                    + $",@pageNo={pageNo}"
                    + $",@pageSize={pageSize}"
                    + $",@timeZoneOffsetValue={(string.IsNullOrEmpty(timeZoneOffsetValue) ? "NULL" : $"'{_dao.FilterString(timeZoneOffsetValue)}'")}";

                var dbResponse = _dao.ExecuteDataTable(sql);
                if (dbResponse != null)
                {
                    foreach (DataRow item in dbResponse.Rows)
                    {
                        response.Add(new StorePerformanceCommon
                        {
                            ClubId = SafeLong(_dao.ParseColumnValue(item, "clubId"))?.ToString(),
                            StoreName = _dao.ParseColumnValue(item, "clubName")?.ToString(),
                            LocationName = _dao.ParseColumnValue(item, "LocationName")?.ToString(),
                            BookingStorePage = SafeInt(_dao.ParseColumnValue(item, "reservationStore")),
                            BookingHostDetails = SafeInt(_dao.ParseColumnValue(item, "hostDetail")),
                            PhoneStorePage = SafeInt(_dao.ParseColumnValue(item, "phoneStore")),
                            PhoneHostDetails = SafeInt(_dao.ParseColumnValue(item, "phoneHost")),
                            TotalClicks = SafeInt(_dao.ParseColumnValue(item, "totalCount")),
                            TotalRecords = SafeInt(_dao.ParseColumnValue(item, "totalRecords"))
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }
        #endregion

        #region Click Analytics (Today Click + Action Source + Click Origin)
        public ClickAnalyticsResult GetClickAnalytics(long clubId, string channel, string timeZoneOffsetValue)
        {
            var result = new ClickAnalyticsResult();
            try
            {
                //long? resolvedClubId = null;

                var sql = "EXEC [analytics].[sproc_admin_get_conversion_click_analytics]"
                    + $" @clubId='{_dao.FilterString(clubId.ToString())}'"
                    + $",@channel='{_dao.FilterString(string.IsNullOrEmpty(channel) ? "all" : channel)}'"
                    + $",@timeZoneOffsetValue='{_dao.FilterString(string.IsNullOrEmpty(timeZoneOffsetValue) ? "+05:45" : timeZoneOffsetValue)}'";

                var ds = _dao.ExecuteDataSet(sql);

                if (ds != null && ds.Tables.Count >= 3)
                {
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        result.TodayClicks.Add(new TodayClickBucket
                        {
                            Time = _dao.ParseColumnValue(item, "time")?.ToString(),
                            Value = SafeInt(_dao.ParseColumnValue(item, "value"))
                        });
                    }

                    foreach (DataRow item in ds.Tables[1].Rows)
                    {
                        result.ActionSource.Add(new ActionSourceCommon
                        {
                            Label = _dao.ParseColumnValue(item, "actionSource")?.ToString(),
                            StorePage = _dao.ParseColumnValue(item, "actionSource")?.ToString(),
                            Clicks = SafeInt(_dao.ParseColumnValue(item, "clickCount")),
                            Percentage = SafeDecimal(_dao.ParseColumnValue(item, "percentage"))
                        });
                    }

                    foreach (DataRow item in ds.Tables[2].Rows)
                    {
                        result.ClickOrigin.Add(new ClickOriginCommon
                        {
                            Area = _dao.ParseColumnValue(item, "areaName")?.ToString(),
                            Clicks = SafeInt(_dao.ParseColumnValue(item, "clickCount")),
                            Percentage = SafeDecimal(_dao.ParseColumnValue(item, "percentage"))
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }
        #endregion
        public List<LocationCommon> GetLocationList()
        {
            var response = new List<LocationCommon>();
            try
            {
                var environment = ConfigurationManager.AppSettings["DBEnvironmentConversionMetrix"];
                var sql = $"SELECT LocationId, LocationName FROM [{environment}].[dbo].[tbl_location] WHERE Status = 'A' ORDER BY LocationName";
                var dt = ExecuteDataSetFirstTable(sql);
                if (dt != null)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        response.Add(new LocationCommon
                        {
                            LocationId = _dao.ParseColumnValue(item, "LocationId")?.ToString(),
                            LocationName = _dao.ParseColumnValue(item, "LocationName")?.ToString()
                        });
                    }
                }
            }
            catch (Exception) { throw; }
            return response;
        }
        public long? ResolveClubCodeToAgentId(string clubCode)
        {
            if (string.IsNullOrEmpty(clubCode)) return null;

            var environment = ConfigurationManager.AppSettings["DBEnvironmentConversionMetrix"];
            var lookupSql = $"SELECT AgentId FROM [{environment}].[dbo].[tbl_club_details]"
                + $" WHERE clubCode = '{_dao.FilterString(clubCode)}'";

            var lookupRow = _dao.ExecuteDataRow(lookupSql);
            return lookupRow != null ? SafeLong(_dao.ParseColumnValue(lookupRow, "AgentId")) : null;
        }

        private static long? SafeLong(object value)
        {
            if (value == null || value == DBNull.Value) return null;
            return long.TryParse(value.ToString(), out var result) ? (long?)result : null;
        }

        private static decimal SafeDecimal(object value)
        {
            if (value == null || value == DBNull.Value) return 0m;
            return decimal.TryParse(value.ToString(), out var result) ? result : 0m;
        }

        private static int SafeInt(object value)
        {
            if (value == null || value == DBNull.Value) return 0;
            return int.TryParse(value.ToString(), out var result) ? result : 0;
        }

        private DataTable ExecuteDataSetFirstTable(string sql)
        {
            var ds = _dao.ExecuteDataSet(sql);
            return (ds != null && ds.Tables.Count > 0) ? ds.Tables[0] : null;
        }

        public List<StorePerformanceCommon> GetConversionSummaryPerformanceRepost(PaginationFilterCommon dbRequest, string clubId)
        {
            var response = new List<StorePerformanceCommon>();

            var sql = "EXEC [analytics].[sproc_admin_get_conversion_store_performance]";

            sql += " @SearchFilter=" + (string.IsNullOrEmpty(dbRequest.SearchFilter) ? "null" : "N'" + _dao.FilterString(dbRequest.SearchFilter) + "'");

            sql += ",@pageNo=" +
                   (dbRequest.Skip > 0 ? dbRequest.Skip.ToString() : "0");

            sql += ",@pageSize=" +
                   (dbRequest.Take > 0 ? dbRequest.Take.ToString() : "10");

            sql += ",@fromDateMs=" +
                   (!string.IsNullOrWhiteSpace(dbRequest.FromDate)
                       ? _dao.FilterString(dbRequest.FromDate)
                       : "NULL");

            sql += ",@toDateMs=" +
                   (!string.IsNullOrWhiteSpace(dbRequest.ToDate)
                       ? _dao.FilterString(dbRequest.ToDate)
                       : "NULL");

            sql += ",@clubId=" +
                (!string.IsNullOrWhiteSpace(clubId)
                    ? _dao.FilterString(clubId)
                    : "NULL");

            var dbResponse = _dao.ExecuteDataTable(sql);
            if (dbResponse != null && dbResponse.Rows.Count > 0)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new StorePerformanceCommon()
                    {
                        Sno = SafeInt(_dao.ParseColumnValue(item, "Sno")),
                        ClubId = SafeLong(_dao.ParseColumnValue(item, "clubId"))?.ToString(),
                        StoreName = _dao.ParseColumnValue(item, "clubName")?.ToString(),
                        LocationName = _dao.ParseColumnValue(item, "LocationName")?.ToString(),
                        BookingStorePage = SafeInt(_dao.ParseColumnValue(item, "reservationStore")),
                        BookingHostDetails = SafeInt(_dao.ParseColumnValue(item, "hostDetail")),
                        PhoneStorePage = SafeInt(_dao.ParseColumnValue(item, "phoneStore")),
                        PhoneHostDetails = SafeInt(_dao.ParseColumnValue(item, "phoneHost")),
                        TotalClicks = SafeInt(_dao.ParseColumnValue(item, "totalCount")),
                        TotalRecords = SafeInt(_dao.ParseColumnValue(item, "totalRecords")),
                    });
                }
            }
            return response;
        }

        public StorePerformanceCommon GetClubName(string clubId)
        {
            var response = new StorePerformanceCommon();

            var sql = "EXEC [dbo].[sproc_admin_get_club_detail]";
            sql += " @clubId=" + clubId;
            var dbResponse = _repositoryDao.ExecuteDataRow(sql);
            if (dbResponse != null)
            {
                response.ClubId = _dao.ParseColumnValue(dbResponse, "clubId").ToString();
                response.StoreName = _dao.ParseColumnValue(dbResponse, "clubName").ToString();
                response.LocationName = _dao.ParseColumnValue(dbResponse, "LocationDisplayName").ToString();
            }
            return response;

        }
    }
}
