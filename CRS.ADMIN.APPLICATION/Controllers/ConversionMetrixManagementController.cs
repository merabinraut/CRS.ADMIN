 using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ConversionMetrixManagement;
using CRS.ADMIN.APPLICATION.Models.DiscountManagement;
using CRS.ADMIN.BUSINESS.ConversionMetrixManagement;
using CRS.ADMIN.SHARED.ConversionMetrixManagement;
using CRS.ADMIN.SHARED.LocationManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    [OverrideActionFilters]
    public class ConversionMetrixManagementController : BaseController
    {
        private readonly IConversionMetrixBusiness _service;
        public ConversionMetrixManagementController(IConversionMetrixBusiness conversionMetrixBusiness)
        {
            _service = conversionMetrixBusiness;
        }

        #region Index
        [HttpGet]
        public ActionResult Index(string SearchFilter = null, string clubId = null, int StartIndex = 1, int PageSize = 10,string ToDate="",string FromDate="")
        {
            ViewBag.SearchFilter = SearchFilter;
            Session["CurrentURL"] = "/ConversionMetrixManagement/Index";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            string RenderId = "";
            ConversionMetrixOverviewModel listModel = new ConversionMetrixOverviewModel();
            //var listModel=new ConversionMetrixOverviewModel();
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null,
                FromDate=FromDate,
                ToDate=ToDate
            };
            var summaryPerformance = _service.GetConversionSummaryPerformanceRepost(dbRequest,clubId);

            var model = new ConversionMetrixOverviewModel
            {
                SearchFilter = SearchFilter,
                ActivityLogFilterModel = new ActivityLogFilterModel(),
                ConversionSummaryModel = new ConversionSummaryModel(),
                StorePerformanceModel = new List<StorePerformanceModel>(),
                ClickChartList = new List<ClickChartModel>(),
                ActivityLogModel = new List<ActivityLogModel>(),
                StoreRankingList = new List<StoreRankingModel>()
            };

            model.StorePerformanceModel = summaryPerformance.MapObjects<StorePerformanceModel>();
            model.StorePerformanceModel.ForEach(x =>
            {
                x.SNo = x.SNo;
                x.StoreName = x.StoreName;
                x.BookingStorePage = x.BookingStorePage;
                x.BookingHostDetails = x.BookingHostDetails;
                x.PhoneStorePage = x.PhoneStorePage;
                x.PhoneHostDetails =x.PhoneHostDetails;
                x.TotalClicks= x.TotalClicks;
            });


            if (TempData.ContainsKey("StorePerformanceModel"))
                listModel.storePerformanceModels = TempData["StorePerformanceModel"] as StorePerformanceModel;
           
            ViewBag.TotalData = summaryPerformance != null && summaryPerformance.Any() ? summaryPerformance[0].TotalRecords : 0;


            //------------------------------

            //var model = new ConversionMetrixOverviewModel
            //{
            //    SearchFilter = SearchFilter,
            //    ActivityLogFilterModel = new ActivityLogFilterModel(),
            //    ConversionSummaryModel = new ConversionSummaryModel(),
            //    StorePerformanceModel = new List<StorePerformanceModel>(),
            //    ClickChartList = new List<ClickChartModel>(),
            //    ActivityLogModel = new List<ActivityLogModel>(),
            //    StoreRankingList = new List<StoreRankingModel>()
            //};

            try
            {
                var summary = _service.GetConversionSummary(clubId);
                if (summary != null)
                {
                    model.ConversionSummaryModel = new ConversionSummaryModel
                    {
                        TotalClicks = summary.TotalClicks,
                        ReservationClicks = summary.ReservationClicks,
                        PhoneCallClicks = summary.PhoneClicks,
                        AverageCTR = summary.AverageCTR
                    };
                }

                var rankings = _service.GetRankedStores(locationId: null,  searchFilter: null, topCount: 10, fromDateMs: null, toDateMs: null, timeZoneOffsetValue: "+09:00" );
                if (rankings != null && rankings.Any())
                {
                    int rank = 1;
                    model.StoreRankingList = rankings.Select(x => new StoreRankingModel
                    {
                        Rank = rank++,
                        ClubId = x.ClubId,
                        ClubName = x.ClubName,
                        Area = x.Area,
                        ClickCount = x.TapCount
                    }).ToList();
                }
                var locations = _service.GetLocationList() ?? new List<LocationCommon>();
                ViewBag.LocationList = locations.Select(x => new SelectListItem
                {
                    Value = x.LocationId,
                    Text = x.LocationName
                }).ToList();
            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            ViewBag.SearchFilter = SearchFilter;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.ClubId = clubId;

            return View(model);
        }
        #endregion
        #region Conversion Summary
        [HttpGet]
        public JsonResult GetConversionSummaryJson(string clubId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(clubId)) clubId = null;

                var summary = _service.GetConversionSummary(clubId) ?? new ConversionSummaryCommon();

                return Json(new
                {
                    success = true,
                    totalClicks = summary.TotalClicks,
                    reservationClicks = summary.ReservationClicks,
                    phoneCallClicks = summary.PhoneClicks,
                    reservationConvertedCount = summary.ReservationConvertedCount,
                    averageCTR = summary.AverageCTR
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Click Analytics
        [HttpGet]
        public JsonResult GetClickAnalytics(string clubId = null, string channel="", int timezoneOffset = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(clubId)) clubId = null;
                var sign = timezoneOffset < 0 ? "-" : "+";
                var abs = Math.Abs(timezoneOffset);
                var tzString = $"{sign}{(abs / 60):D2}:{(abs % 60):D2}";

                var result = _service.GetClickAnalytics(clubId, channel, tzString) ?? new ClickAnalyticsResult();

                return Json(new
                {
                    success = true,
                    chartData = new
                    {
                        labels = result.TodayClicks.Select(x => x.Time).ToList(),
                        datasets = new List<object>
                        {
                            new
                            {
                                type = channel,
                                data = result.TodayClicks.Select(x => x.Value).ToList()
                            }
                        }
                    },
                    actionSource = result.ActionSource.Select(x => new
                    {
                        label = x.StorePage,
                        clicks = x.Clicks,
                        percentage = x.Percentage
                    }).ToList(),
                    clickOrigin = result.ClickOrigin.Select(x => new
                    {
                        area = x.Area,
                        clicks = x.Clicks,
                        percentage = x.Percentage
                    }).ToList()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Ranked Stores
        [HttpGet]
        public JsonResult GetRankedStores(
            long? locationId = null, string search = null, int topN = 10,
            long? fromDateMs = null, long? toDateMs = null, int timezoneOffset = 0)
        {
            try
            {
                var sign = timezoneOffset < 0 ? "-" : "+";
                var abs = Math.Abs(timezoneOffset);
                var tzString = $"{sign}{(abs / 60):D2}:{(abs % 60):D2}";

                var result = _service.GetRankedStores(locationId, search, topN, fromDateMs, toDateMs, tzString)
                    ?? new List<RankedStoreCommon>();

                var mapped = result.Select(x => new
                {
                    rank = x.Rank,
                    clubId = x.ClubCode,
                    clubName = x.ClubName,
                    locationId = x.LocationId,
                    area = x.LocationName,
                    tapCount = x.TapCount,
                    reservationClicks = x.ReservationClickCount,
                    phoneClicks = x.PhoneClickCount
                }).ToList();

                return Json(new
                {
                    success = true,
                    data = mapped
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Activity Log
        [HttpGet]
        public JsonResult GetActiveLog( string clubId = null, string actionType = null, string sourcePageType = null, string search = null, long? fromDateMs = null, long? toDateMs = null, string userStatus = null, int timezoneOffset = 0, int pageNo = 1, int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(actionType) || actionType == "All") actionType = null;
                if (string.IsNullOrWhiteSpace(sourcePageType) || sourcePageType == "All") sourcePageType = null;
                if (string.IsNullOrWhiteSpace(userStatus) || userStatus == "All") userStatus = null;
                if (string.IsNullOrWhiteSpace(clubId)) clubId = null;

                long? resolvedClubId = clubId != null ? _service.ResolveClubCodeToAgentId(clubId) : null;             

                var sign = timezoneOffset < 0 ? "-" : "+";
                var abs = Math.Abs(timezoneOffset);
                var timeZoneOffsetValue = $"{sign}{(abs / 60):D2}:{(abs % 60):D2}";

                var result = _service.GetActivityLog(resolvedClubId, search, actionType, sourcePageType,userStatus, fromDateMs, toDateMs, pageNo, pageSize, timeZoneOffsetValue)
                    ?? new List<ActivityLogCommon>();

                var mapped = result.Select(x => new
                {
                    sn = x.SNO,
                    clubId = x.ClubId,
                    clubCode = x.ClubCode,
                    clubName = x.ClubName,
                    actionType = x.ActionType,
                    sourcePage = x.SourcePage,
                    targetName = x.TargetName,
                    activityId = x.ActivityId,
                    sessionId = x.SessionId,
                    userStatus = x.UserStatus,
                    prefecture = x.Prefecture,
                    browser = x.Browser,
                    userAgent = x.UserAgent,
                    dateMs = x.DateMs
                }).ToList();

                return Json(new
                {
                    success = true,
                    totalCount = result.Any() ? result[0].TotalRecords : 0,
                    pageNo,
                    pageSize,
                    data = mapped
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message },
                    JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Store Performance
        [HttpGet]
        public JsonResult GetStorePerformance(
            string searchFilter = null, string clubId = null, long? fromDateMs = null, long? toDateMs = null,
            int pageNo = 1, int pageSize = 10, int timezoneOffset = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(clubId)) clubId = null;
                long? resolvedClubId = clubId != null ? _service.ResolveClubCodeToAgentId(clubId) : null;

                var sign = timezoneOffset < 0 ? "-" : "+";
                var abs = Math.Abs(timezoneOffset);
                var timeZoneOffsetValue = $"{sign}{(abs / 60):D2}:{(abs % 60):D2}";

                var result = _service.GetStorePerformance(resolvedClubId, searchFilter, fromDateMs, toDateMs, pageNo, pageSize, timeZoneOffsetValue)
                    ?? new List<StorePerformanceCommon>();

                var mapped = result.Select((x, i) => new
                {
                    sNo = (pageNo - 1) * pageSize + i + 1,
                    clubName = x.StoreName,
                    reservationClick = x.BookingStorePage,
                    reservationDetailClick = x.BookingHostDetails,
                    phoneCallClick = x.PhoneStorePage,
                    phoneCallDetailClick = x.PhoneHostDetails,
                    totalClick = x.TotalClicks
                }).ToList();

                return Json(new
                {
                    success = true,
                    totalCount = result.Any() ? result[0].TotalRecords : 0,
                    pageNo,
                    pageSize,
                    data = mapped
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message },
                    JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

    }

}