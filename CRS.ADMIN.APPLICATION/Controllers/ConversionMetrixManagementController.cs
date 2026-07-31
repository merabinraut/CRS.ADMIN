using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ConversionMetrixManagement;
using CRS.ADMIN.APPLICATION.Models.DiscountManagement;
using CRS.ADMIN.BUSINESS.ConversionMetrixManagement;
using CRS.ADMIN.SHARED.ConversionMetrixManagement;
using CRS.ADMIN.SHARED.LocationManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class ConversionMetrixManagementController : BaseController
    {
        private readonly IConversionMetrixBusiness _service;
        public ConversionMetrixManagementController(IConversionMetrixBusiness conversionMetrixBusiness)
        {
            _service = conversionMetrixBusiness;
        }

        #region Index
        [HttpGet]
        public ActionResult Index(ActivityLogModel Requests,string clubId ="", string SearchFilter = null, int StartIndex = 0, int PageSize = 10, string ToDate = "", string FromDate = "", string ToDateMs = "", string FromDateMs = "", string TabValue = "",
            string actionType = null, string sourcePageType = null, string search = null, string userStatus = null, int timezoneOffset = 0, int StartIndex2 = 0, int PageSize2 = 10)
        {
            ViewBag.SearchFilter = SearchFilter;
            Session["CurrentURL"] = "/ConversionMetrixManagement/Index";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            string RenderId = "";
            ConversionMetrixOverviewModel listModel = new ConversionMetrixOverviewModel();
            Requests.ClubId = !string.IsNullOrEmpty(clubId) ? clubId : Requests.ClubId;
            var dbClubResp = new StorePerformanceCommon();
            if (!string.IsNullOrEmpty(Requests.ClubId))
            {
                 dbClubResp = _service.GetClubName(Requests.ClubId.DecryptParameter());
            }
            //var listModel=new ConversionMetrixOverviewModel();
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null,
                FromDate = FromDate,
                ToDate = ToDate
            };

            var model = new ConversionMetrixOverviewModel
            {
                SearchFilter = SearchFilter,
                TabValue = TabValue,
                ActivityLogFilterModel = new ActivityLogFilterModel(),
                ConversionSummaryModel = new ConversionSummaryModel(),
                StorePerformanceModel = new List<StorePerformanceModel>(),
                ClickChartList = new List<ClickChartModel>(),
                ActivityLogModel = new List<ActivityLogModel>(),
                StoreRankingList = new List<StoreRankingModel>()
            };
            long decryptedClubId = 0;
            var clubName = "";
            if (!string.IsNullOrEmpty(Requests.ClubId))
            {
                var data = ApplicationUtilities.DecryptParameter(Requests.ClubId);
                decryptedClubId = Convert.ToInt64(ApplicationUtilities.DecryptParameter(Requests.ClubId));
            }

            var locations = _service.GetLocationList() ?? new List<LocationCommon>();
            ViewBag.LocationList = locations.Select(x => new SelectListItem
            {
                Value = x.LocationId,
                Text = x.LocationName
            }).ToList();

            if (TabValue == "")
            {
                var summaryPerformance = _service.GetConversionSummaryPerformanceRepost(dbRequest, decryptedClubId.ToString());
                model.StorePerformanceModel = summaryPerformance.MapObjects<StorePerformanceModel>();
                model.StorePerformanceModel.ForEach(x =>
                {
                    x.SNo = x.SNo;
                    x.StoreName = x.StoreName;
                    x.LocationName = x.LocationName;
                    x.BookingStorePage = x.BookingStorePage;
                    x.BookingHostDetails = x.BookingHostDetails;
                    x.PhoneStorePage = x.PhoneStorePage;
                    x.PhoneHostDetails = x.PhoneHostDetails;
                    x.TotalClicks = x.TotalClicks;
                });


                if (TempData.ContainsKey("StorePerformanceModel"))
                    listModel.storePerformanceModels = TempData["StorePerformanceModel"] as StorePerformanceModel;

                ViewBag.TotalData = summaryPerformance != null && summaryPerformance.Any() ? summaryPerformance[0].TotalRecords : 0;  
                try
                {
                    var summary = _service.GetConversionSummary(decryptedClubId);
                    if (summary != null)
                    {
                        model.ConversionSummaryModel = new ConversionSummaryModel
                        {
                            TotalSumClicks = summary.TotalClicks,
                            ReservationClicks = summary.ReservationClicks,
                            PhoneCallClicks = summary.PhoneClicks,
                            AverageCTR = summary.AverageCTR
                        };
                    }

                    var rankings = _service.GetRankedStores(locationId: null, searchFilter: null, topCount: 10, fromDateMs: null, toDateMs: null, timeZoneOffsetValue: "+09:00");
                    if (rankings != null && rankings.Any())
                    {
                        int rank = 1;
                        model.StoreRankingList = rankings.Select(x => new StoreRankingModel
                        {
                            Rank = rank++,
                            ClubId = ApplicationUtilities.EncryptParameter(x.ClubId.ToString()),
                            ClubName = x.ClubName,
                            ClubNameJp = x.ClubNameJp,
                            Area = x.Area,
                            ClickCount = x.TapCount
                        }).ToList();
                    }
                }

                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ViewBag.TotalData = 0;
            }

            if (TabValue == "02")
            {

                if (string.IsNullOrWhiteSpace(actionType) || actionType == "All") actionType = null;
                if (string.IsNullOrWhiteSpace(sourcePageType) || sourcePageType == "All") sourcePageType = null;
                if (string.IsNullOrWhiteSpace(userStatus) || userStatus == "All") userStatus = null;
                if (string.IsNullOrWhiteSpace(Requests.ClubId)) Requests.ClubId = null;

                var sign = timezoneOffset < 0 ? "-" : "+";
                var abs = Math.Abs(timezoneOffset);
                var timeZoneOffsetValue = $"{sign}{(abs / 60):D2}:{(abs % 60):D2}";

                var dbResponse = Requests.MapObject<ActivityLogFilterModel>();
                var dbRequests = Requests.MapObject<ActivityLogFilterCommon>();

                dbRequests.Skip = StartIndex2;
                dbRequests.Take = PageSize2;
                dbRequests.SearchFilter = SearchFilter;
                dbRequests.ClubId = decryptedClubId.ToString();
                FromDate = FromDateMs;
                ToDate = ToDateMs;
     

                var result = _service.GetActivityLogList(dbRequests);


                model.ActivityLogModel = result.MapObjects<ActivityLogModel>();
                model.ActivityLogModel.ForEach(x =>
                {
                    x.ClubId = x.ClubId.EncryptParameter();
                });

                ViewBag.TotalActivityData = (model.ActivityLogModel.Any() && model?.ActivityLogModel?.FirstOrDefault()?.TotalRecords != null) ? model?.ActivityLogModel[0].TotalRecords : 0;
            }
            else
            {
                ViewBag.TotalActivityData = 0;
            }
            ViewBag.ClubName =!string.IsNullOrEmpty($"{dbClubResp.StoreName} {dbClubResp.LocationName}") ? $"{dbClubResp.StoreName} {dbClubResp.LocationName}": null;
            ViewBag.ClubId = Requests.ClubId;
            ViewBag.SearchFilter = SearchFilter;
            ViewBag.SearchFilter2 = SearchFilter;
            ViewBag.StartIndex2 =  StartIndex2;
            ViewBag.PageSize2 = PageSize2;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TabValue = TabValue;
            model.ActionType = actionType;
            model.SourcePageType = sourcePageType;
            model.UserStatus = userStatus;

            ViewBag.ActionType = actionType;
            ViewBag.SourcePageType = sourcePageType;
            ViewBag.UserStatus = userStatus;

            model.clubId = Requests.ClubId;
            model.clubName = clubName;
            model.TabValue = TabValue;
            

            return View(model);
        }
        #endregion

        #region Click Analytics
        [HttpGet, OverrideActionFilters]
        public JsonResult GetClickAnalytics(string clubId = null, string channel = "", int timezoneOffset = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(clubId)) clubId = null;
                var sign = timezoneOffset < 0 ? "-" : "+";
                var abs = Math.Abs(timezoneOffset);
                var tzString = $"{sign}{(abs / 60):D2}:{(abs % 60):D2}";

                long decryptedClubId = 0;
                if (!string.IsNullOrEmpty(clubId))
                {
                    var data = ApplicationUtilities.DecryptParameter(clubId);
                    decryptedClubId = Convert.ToInt64(ApplicationUtilities.DecryptParameter(clubId));
                }

                var result = _service.GetClickAnalytics(decryptedClubId, channel, tzString) ?? new ClickAnalyticsResult();

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
        [HttpGet, OverrideActionFilters]
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
                    clubId = ApplicationUtilities.EncryptParameter(x.ClubId.ToString()),
                    clubCode = x.ClubCode,
                    clubNameJp = x.ClubNameJp,
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
        #region Store Performance
        [HttpGet, OverrideActionFilters]
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
                    locationName = x.LocationName,
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