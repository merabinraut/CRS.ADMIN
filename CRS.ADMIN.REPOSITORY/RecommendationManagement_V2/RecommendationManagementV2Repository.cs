using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.RecommendationManagement_V2;
using System;
using System.Collections.Generic;
using System.Data;

namespace CRS.ADMIN.REPOSITORY.RecommendationManagement_V2
{
    public class RecommendationManagementV2Repository : IRecommnedationManagementV2Repository
    {
        private readonly RepositoryDao _dao;
        public RecommendationManagementV2Repository()
        {
            _dao = new RepositoryDao();
        }

        #region "Display Page"
        public List<DisplayPageListModelCommon> GetDisplayPageList()
        {
            List<DisplayPageListModelCommon> responseInfo = new List<DisplayPageListModelCommon>();
            string sp_name = "EXEC dbo.sproc_admin_recommendation_location_page @Flag = 'Page'";
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new DisplayPageListModelCommon()
                    {
                        PageId = row["PageId"].ToString(),
                        PageLabel = row["PageLabel"].ToString()
                    });
                }
            }
            return responseInfo;
        }
        #endregion

        #region "Manage Group"
        public List<GroupListModelCommon> GetGroupList(string LocationId, string SearchFilter = "")
        {
            List<GroupListModelCommon> responseInfo = new List<GroupListModelCommon>();
            string sp_name = "EXEC sproc_recommendation_group_management @Flag = 'grgl'";
            sp_name += ",@LocationId=" + _dao.FilterString(LocationId);
            sp_name += !string.IsNullOrEmpty(SearchFilter) ? ",@SearchField=N" + _dao.FilterString(SearchFilter) : null;
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new GroupListModelCommon()
                    {
                        GroupId = row["GroupId"].ToString(),
                        GroupName = row["GroupName"].ToString(),
                        Descriptions = row["Description"].ToString(),
                        RequestedDate =!string.IsNullOrEmpty(row["CreatedDate"].ToString()) ? DateTime.Parse(row["CreatedDate"].ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : row["CreatedDate"].ToString(),
                        TotalClubs = row["TotalClubs"].ToString(),
                        DisplayOrderId = row["DisplayOrderId"].ToString(),
                        CreatedBy = row["CreatedBy"].ToString(),
                        CreatedIp = row["CreatedIp"].ToString(),
                        LocationId = row["LocationId"].ToString(),
                        Status = row["Status"].ToString(),
                    });
                }
            }
            return responseInfo;
        }
        public CommonDbResponse ManageGroup(ManageGroupCommon commonModel)
        {
            string sp_name = "EXEC sproc_recommendation_group_management @Flag = 'cg'";
            sp_name += ",@GroupName=N" + _dao.FilterString(commonModel.GroupName);
            sp_name += ",@Description=N" + _dao.FilterString(commonModel.Description);
            sp_name += ",@DisplayOrderId=" + _dao.FilterString(commonModel.DisplayOrderId);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonModel.ActionUser);
            sp_name += ",@ActionIP=" + _dao.FilterString(commonModel.ActionIP);
            return _dao.ParseCommonDbResponse(sp_name);
        }
        #endregion

        #region "Recommendation Request and Location Management"
        public List<LocationListModelCommon> GetLocationList()
        {
            List<LocationListModelCommon> responseInfo = new List<LocationListModelCommon>();
            string sp_name = "EXEC dbo.sproc_admin_recommendation_location_page @Flag = 'location'";
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new LocationListModelCommon()
                    {
                        LocationId = row["LocationId"].ToString(),
                        LocationName = row["LocationName"].ToString(),
                    });
                }
            }
            return responseInfo;
        }


        #endregion

        #region "Manage Shuffling Time"
        public List<ShufflingTimeListModelCommon> GetShufflingTimeList()
        {
            List<ShufflingTimeListModelCommon> responseInfo = new List<ShufflingTimeListModelCommon>();
            string sp_name = "EXEC dbo.sproc_shuffling_management @Flag = 'gstl'";
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new ShufflingTimeListModelCommon()
                    {
                        DisplayId = row["DisplayId"].ToString(),
                        DisplayName = row["DisplayName"].ToString(),
                        DisplayTime = row["DisplayTime"].ToString(),
                        LabelName = row["LabelName"].ToString(),
                    });
                }
            }
            return responseInfo;
        }

        public CommonDbResponse ManageshufflingTime(ManageShufflingTimeCommon commonModel)
        {
            string sp_name = "EXEC dbo.sproc_shuffling_management @Flag = 'msf'";
            sp_name += ",@ShufflingTimeCSValue=" + _dao.FilterString(commonModel.ShufflingTimeCSValue);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonModel.ActionUser);
            return _dao.ParseCommonDbResponse(sp_name);
        }


        #endregion


        #region "Manage Home Page Request"
        public List<HomePageClubRequestListModelCommon> GetClubRequestListByHomePage(string locationId, string SearchFilter = "")
        {
            List<HomePageClubRequestListModelCommon> responseInfo = new List<HomePageClubRequestListModelCommon>();
            string sp_name = "EXEC sproc_admin_homepage_recommendation_management_v2 @Flag = 'ghpac'";
            sp_name += ",@LocationId=" + _dao.FilterString(locationId);
            sp_name += !string.IsNullOrEmpty(SearchFilter) ? ",@SearchFilter=" + _dao.FilterString(SearchFilter) : null;
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new HomePageClubRequestListModelCommon()
                    {
                        ClubName = row["ClubName"].ToString(),
                        ClubLogo = row["ClubLogo"].ToString(),
                        HostName = row["HostName"].ToString(),
                        ClubCategory = row["ClubCategory"].ToString(),
                        DisplayPageLabel = row["DisplayPageLabel"].ToString(),
                        RequestedDate = row["RequestedDate"].ToString() != "-" ? DateTime.Parse(row["RequestedDate"].ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : row["RequestedDate"].ToString()  ,
                        ClubId = row["ClubId"].ToString(),
                        DisplayId = row["DisplayId"].ToString(),
                        RecommendationId = row["RecommendationId"].ToString(),
                        Status = row["Status"].ToString(),
                        UpdatedDate =row["UpdatedDate"].ToString() != "-" ? DateTime.Parse(row["UpdatedDate"].ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : row["UpdatedDate"].ToString()
                    });
                }
            }
            return responseInfo;
        }
        public CommonDbResponse ManageHomePageRequest(ManageHomePageRequestCommon commonModel)
        {
            string sp_name = "EXEC sproc_admin_homepage_recommendation_management_v2 @Flag='cchpr'";
            sp_name += ",@ClubId=" + _dao.FilterString(commonModel.ClubId);
            sp_name += ",@LocationId=" + _dao.FilterString(commonModel.LocationId);
            sp_name += ",@HostId=" + _dao.FilterString(commonModel.HostId);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonModel.ActionUser);
            sp_name += ",@ActionIP=" + _dao.FilterString(commonModel.ActionIP);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(commonModel.ActionPlatform);
            return _dao.ParseCommonDbResponse(sp_name);

        }

        public CommonDbResponse DeleteHomePageRequest(string clubid, string locationid, string displayid, string recommendationid, Common commonRequest)
        {
            string sp_name = "EXEC sproc_admin_homepage_recommendation_management_v2 @Flag= 'dhpr'";
            sp_name += ",@ClubId=" + _dao.FilterString(clubid);
            sp_name += ",@LocationId=" + _dao.FilterString(locationid);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonRequest.ActionUser);
            sp_name += ",@ActionIp=" + _dao.FilterString(commonRequest.ActionIP);
            sp_name += ",@DisplayId=" + _dao.FilterString(displayid);
            sp_name += ",@RecommendationId=" + _dao.FilterString(recommendationid);
            return _dao.ParseCommonDbResponse(sp_name);
        }
        #endregion

        #region "Manage Search Page Request"
        public List<SearchPageClubRequestListModelCommon> GetClubRequestListBySearchPage(string locationId, string SearchFilter = "")
        {
            List<SearchPageClubRequestListModelCommon> responseInfo = new List<SearchPageClubRequestListModelCommon>();
            string sp_name = "EXEC sproc_admin_searchpage_recommendation_management_v2 @Flag= 'gspac'";
            sp_name += ",@LocationId=" + _dao.FilterString(locationId);
            sp_name += !string.IsNullOrEmpty(SearchFilter) ? ",@SearchFilter=N" + _dao.FilterString(SearchFilter) : null;
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new SearchPageClubRequestListModelCommon()
                    {
                        ClubName = row["ClubName"].ToString(),
                        ClubLogo = row["ClubLogo"].ToString(),
                        HostName = row["HostName"].ToString(),
                        ClubCategory = row["ClubCategory"].ToString(),
                        DisplayPageLabel = row["DisplayPageLabel"].ToString(),
                        RequestedDate = row["RequestedDate"].ToString() != "-" ? DateTime.Parse(row["RequestedDate"].ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : row["RequestedDate"].ToString(),
                        ClubId = row["ClubId"].ToString(),
                        DisplayId = row["DisplayId"].ToString(),
                        RecommendationId = row["RecommendationId"].ToString(),
                        Status = row["Status"].ToString(),
                        UpdatedDate = row["UpdatedDate"].ToString() != "-" ? DateTime.Parse(row["UpdatedDate"].ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : row["UpdatedDate"].ToString(),
                    });
                }
            }
            return responseInfo;
        }

        public CommonDbResponse ManageSearchPageRequest(ManageSearchPageRequestCommon commonModel)
        {
            string sp_name = "EXEC sproc_admin_searchpage_recommendation_management_v2 @Flag= 'ccspr'";
            sp_name += ",@ClubId=" + _dao.FilterString(commonModel.ClubId);
            sp_name += ",@LocationId=" + _dao.FilterString(commonModel.LocationId);
            sp_name += ",@HostId=" + _dao.FilterString(commonModel.HostId);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonModel.ActionUser);
            sp_name += ",@ActionIP=" + _dao.FilterString(commonModel.ActionIP);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(commonModel.ActionPlatform);
            return _dao.ParseCommonDbResponse(sp_name);
        }

        public CommonDbResponse DeleteSearchPageRequest(string clubid, string locationId, string displayid, string recommendationid, Common commonRequest)
        {
            string sp_name = "EXEC sproc_admin_searchpage_recommendation_management_v2 @Flag= 'dspr'";
            sp_name += ",@ClubId=" + _dao.FilterString(clubid);
            sp_name += ",@LocationId=" + _dao.FilterString(locationId);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonRequest.ActionUser);
            sp_name += ",@ActionIp=" + _dao.FilterString(commonRequest.ActionIP);
            sp_name += ",@DisplayId=" + _dao.FilterString(displayid);
            sp_name += ",@RecommendationId=" + _dao.FilterString(recommendationid);
            return _dao.ParseCommonDbResponse(sp_name);
        }
        #endregion

        #region "Manage Main Page Request"
        public List<MainPageClubRequestListModelCommon> GetClubRequestListByMainPage(string locationId, string groupId, string SearchFilter = "")
        {
            List<MainPageClubRequestListModelCommon> responseInfo = new List<MainPageClubRequestListModelCommon>();
            string sp_name = "EXEC sproc_admin_mainpage_recommendation_management_v2 @Flag= 'ghpac'";
            sp_name += ",@LocationId=" + _dao.FilterString(locationId);
            sp_name += ",@GroupId=" + _dao.FilterString(groupId);
            sp_name += !string.IsNullOrEmpty(SearchFilter) ? ",@SearchFilter= N" + _dao.FilterString(SearchFilter) : null;
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new MainPageClubRequestListModelCommon()
                    {
                        ClubName = row["ClubName"].ToString(),
                        ClubLogo = row["ClubLogo"].ToString(),
                        ClubCategory = row["ClubCategory"].ToString(),
                        DisplayPageLabel = row["DisplayPageLabel"].ToString(),
                        RequestedDate = row["RequestedDate"].ToString() != "-" ? DateTime.Parse(row["RequestedDate"].ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : row["RequestedDate"].ToString(),
                        ClubId = row["ClubId"].ToString(),
                        DisplayId = row["DisplayId"].ToString(),
                        RecommendationId = row["RecommendationId"].ToString(),
                        Status = row["Status"].ToString(),
                        UpdatedDate =row["UpdatedDate"].ToString()!="-"? DateTime.Parse(row["UpdatedDate"].ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : row["UpdatedDate"].ToString(),
                    });
                }
            }
            return responseInfo;
        }

        public ManageClubRecommendationRequestCommon GetMPageRecommendationDetail(string recommendationId, string groupid, string locationId, string displayId, string clubId)
        {
            string sp_name = "EXEC sproc_admin_mainpage_recommendation_management_v2 @Flag= 'gmprd'";
            sp_name += ",@RecommendationId=" + _dao.FilterString(recommendationId);
            sp_name += ",@GroupId=" + _dao.FilterString(groupid);
            sp_name += ",@LocationId=" + _dao.FilterString(locationId);
            sp_name += ",@DisplayId=" + _dao.FilterString(displayId);
            sp_name += ",@ClubId=" + _dao.FilterString(clubId);
            var dbResponseInfo = _dao.ExecuteDataRow(sp_name);
            if (dbResponseInfo != null)
            {
                return new ManageClubRecommendationRequestCommon()
                {
                    RecommendationId = _dao.ParseColumnValue(dbResponseInfo, "RecommendationId").ToString(),
                    LocationId = _dao.ParseColumnValue(dbResponseInfo, "LocationId").ToString(),
                    ClubId = _dao.ParseColumnValue(dbResponseInfo, "ClubId").ToString(),
                    GroupId = _dao.ParseColumnValue(dbResponseInfo, "GroupId").ToString(),
                    DisplayOrderId = _dao.ParseColumnValue(dbResponseInfo, "DisplayOrderId").ToString(),
                    DisplayId = _dao.ParseColumnValue(dbResponseInfo, "DisplayId").ToString(),
                };
            }
            return new ManageClubRecommendationRequestCommon();
        }

        public List<HostRecommendationDetailModelCommon> GetMPageHostRecommendationDetail(string recommendationId, string groupid, string locationId, string displayId, string clubId)
        {
            List<HostRecommendationDetailModelCommon> responseInfo = new List<HostRecommendationDetailModelCommon>();
            string sp_name = "EXEC sproc_admin_mainpage_recommendation_management_v2 @Flag = 'gmphrd'";
            sp_name += ",@RecommendationId=" + _dao.FilterString(recommendationId);
            sp_name += ",@GroupId=" + _dao.FilterString(groupid);
            sp_name += ",@LocationId=" + _dao.FilterString(locationId);
            sp_name += ",@DisplayId=" + _dao.FilterString(displayId);
            sp_name += ",@ClubId=" + _dao.FilterString(clubId);
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new HostRecommendationDetailModelCommon()
                    {
                        HostId = row["HostId"].ToString(),
                        HostDisplayOrderId = row["HostDisplayOrderId"].ToString(),
                        RecommendationHostId = row["RecommendationHostId"].ToString(),
                        RecommendationId = row["RecommendationId"].ToString()
                    });
                }
            }
            return responseInfo;
        }

        public CommonDbResponse ManageMainPageRequest(ManageClubRecommendationRequestCommon commonModel)
        {
            string sp_name = "";
            if (!string.IsNullOrEmpty(commonModel.RecommendationId))
            {
                sp_name = "EXEC sproc_admin_mainpage_recommendation_management_v2 @Flag = 'ucmpr'";
                sp_name += ",@RecommendationId=" + _dao.FilterParameter(commonModel.RecommendationId);
                sp_name += ",@DisplayId=" + _dao.FilterParameter(commonModel.DisplayId);
            }
            else
            {
                sp_name = "EXEC sproc_admin_mainpage_recommendation_management_v2 @Flag = 'ccmpr'";
            }
            sp_name += ",@ClubId=" + _dao.FilterString(commonModel.ClubId);
            sp_name += ",@GroupId=" + _dao.FilterString(commonModel.GroupId);
            sp_name += ",@OrderId=" + _dao.FilterString(commonModel.DisplayOrderId);
            sp_name += ",@LocationId=" + _dao.FilterString(commonModel.LocationId);
            sp_name += ",@HostRecommendationCSValue=" + _dao.FilterString(commonModel.HostRecommendationCSValue);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonModel.ActionUser);
            sp_name += ",@ActionIp=" + _dao.FilterString(commonModel.ActionIP);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(commonModel.ActionPlatform);
            return _dao.ParseCommonDbResponse(sp_name);
        }

        public CommonDbResponse DeleteMainPageRequest(string clubid, string locationId, string displayid, string recommendationid, Common commonRequest, string groupId)
        {
            string sp_name = "EXEC sproc_admin_mainpage_recommendation_management_v2 @Flag = 'dmpr'";
            sp_name += ",@ClubId=" + _dao.FilterString(clubid);
            sp_name += ",@LocationId=" + _dao.FilterString(locationId);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonRequest.ActionUser);
            sp_name += ",@ActionIp=" + _dao.FilterString(commonRequest.ActionIP);
            sp_name += ",@DisplayId=" + _dao.FilterString(displayid);
            sp_name += ",@GroupId=" + _dao.FilterString(groupId);
            sp_name += ",@RecommendationId=" + _dao.FilterString(recommendationid);
            return _dao.ParseCommonDbResponse(sp_name);
        }
        #endregion

        #region "Host List"
        public List<HostListModelCommon> GetSelectedHostList(string recommendationId, string clubId, string Locationid, string groupId)
        {
            List<HostListModelCommon> responseInfo = new List<HostListModelCommon>();
            string sp_name = "EXEC sproc_admin_mainpage_recommendation_management_v2 @Flag = 'gmprh'";
            sp_name += ",@RecommendationId=" + _dao.FilterString(recommendationId);
            sp_name += ",@ClubId=" + _dao.FilterString(clubId);
            sp_name += ",@LocationId=" + _dao.FilterString(Locationid);
            sp_name += ",@GroupId=" + _dao.FilterString(groupId);
            var dbResponse = _dao.ExecuteDataTable(sp_name);
            if (dbResponse != null)
            {
                foreach (DataRow row in dbResponse.Rows)
                {
                    responseInfo.Add(new HostListModelCommon()
                    {
                        ClubId = row["ClubId"].ToString(),
                        HostId = row["HostId"].ToString(),
                        RecommendationId = row["RecommendationId"].ToString(),
                        RecommendationHostId = row["RecommendationHostId"].ToString(),
                        HostImage = row["HostImage"].ToString(),
                        HostName = row["HostName"].ToString(),
                        CreatedOn = row["CreatedOn"].ToString() != "-"  ? DateTime.Parse(row["CreatedOn"].ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : row["CreatedOn"].ToString(),
                        UpdatedOn = row["UpdatedOn"].ToString() != "-" ? DateTime.Parse(row["UpdatedOn"].ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : row["UpdatedOn"].ToString() ,
                        DisplayOrder = row["DisplayOrder"].ToString(),
                    });
                }
            }
            return responseInfo;
        }

        public CommonDbResponse DeleteSelectedHost(string clubId, string recommendationHostId, string recommendationId, string hostId, Common commonRequest)
        {
            string sp_name = "EXEC sproc_admin_mainpage_recommendation_management_v2 @Flag= 'rmprh'";
            sp_name += ",@ClubId=" + _dao.FilterString(clubId);
            sp_name += ",@RecommendationHostId=" + _dao.FilterString(recommendationHostId);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonRequest.ActionUser);
            sp_name += ",@ActionIp=" + _dao.FilterString(commonRequest.ActionIP);
            sp_name += ",@RecommendationId=" + _dao.FilterString(recommendationId);
            sp_name += ",@HostId=" + _dao.FilterString(hostId);
            return _dao.ParseCommonDbResponse(sp_name);
        }


        #endregion

        #region "Club Recommendation Request List"
        public List<ClubRecommendationManagementListModelCommon> GetClubRecommendationReqList()
        {
            List<ClubRecommendationManagementListModelCommon> responseInfo = new List<ClubRecommendationManagementListModelCommon>();
            string sp_name = "EXEC sproc_admin_recommendation_request_management_v2 @Flag= 'grrl'";
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new ClubRecommendationManagementListModelCommon()
                    {
                        RecommendationHoldId = row["RecommendationHoldId"].ToString(),
                        ClubId = row["ClubId"].ToString(),
                        ClubName = row["ClubName"].ToString(),
                        ClubCategory = row["ClubCategory"].ToString(),
                        ClubLogo = row["ClubLogo"].ToString(),
                        DisplayPage = row["DisplayPage"].ToString(),
                        DisplayOrder = row["DisplayOrder"].ToString(),
                        RequestedDate = !string.IsNullOrEmpty(row["RequestedDate"].ToString()) ? DateTime.Parse(row["RequestedDate"].ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : row["RequestedDate"].ToString(),
                        UpdatedDate = !string.IsNullOrEmpty(row["UpdatedDate"].ToString()) ? DateTime.Parse(row["UpdatedDate"].ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : row["UpdatedDate"].ToString(),
                        Status = row["Status"].ToString(),
                        DisplayId = row["DisplayId"].ToString(),
                        LocationId = row["LocationId"].ToString(),
                    });
                }
            }
            return responseInfo;
        }
        #endregion

        #region "Home and Search Page Recommendation Request Host List"
        public List<SearchAndHomePageRecommendationReqHostListModelCommon> GetSearchAndHomePageClubRecommendationReqHostList(string recommendationHoldId, string clubId, string locationId, string displayId)
        {
            List<SearchAndHomePageRecommendationReqHostListModelCommon> responseInfo = new List<SearchAndHomePageRecommendationReqHostListModelCommon>();
            string sp_name = "EXEC sproc_admin_recommendation_request_management_v2 @Flag= 'grrshhl'";
            sp_name += ",@RecommendationHoldId=" + _dao.FilterString(recommendationHoldId);
            sp_name += ",ClubId=" + _dao.FilterString(clubId);
            sp_name += ",@LocationId=" + _dao.FilterString(locationId);
            sp_name += ",@Displayid=" + _dao.FilterString(displayId);
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new SearchAndHomePageRecommendationReqHostListModelCommon()
                    {
                        ClubId = row["ClubId"].ToString(),
                        DisplayId = row["DisplayId"].ToString(),
                        RecommendationHoldId = row["RecommendationHoldId"].ToString(),
                        RecommendationHostHoldId = row["RecommendationHostHoldId"].ToString(),
                        HostName = row["HostName"].ToString(),
                        HostImage = row["HostImage"].ToString(),
                        CreatedDate = row["CreatedDate"].ToString(),
                        UpdatedDate = row["UpdatedDate"].ToString()
                    });
                }
            }
            return responseInfo;
        }
        #endregion

        #region "Main Page Club Recommendation Request Host List"
        public List<MainPageClubRecommendationReqHostListModelCommon> GetMainPageClubRecommendationReqHostList(string recommendationHoldId, string clubId, string locationId, string displayId)
        {
            List<MainPageClubRecommendationReqHostListModelCommon> responseInfo = new List<MainPageClubRecommendationReqHostListModelCommon>();
            string sp_name = "EXEC sproc_admin_recommendation_request_management_v2 @Flag= 'grrmhl'";
            sp_name += ",@RecommendationHoldId=" + _dao.FilterString(recommendationHoldId);
            sp_name += ",ClubId=" + _dao.FilterString(clubId);
            sp_name += ",@LocationId=" + _dao.FilterString(locationId);
            sp_name += ",@Displayid=" + _dao.FilterString(displayId);
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new MainPageClubRecommendationReqHostListModelCommon()
                    {
                        ClubId = row["ClubId"].ToString(),
                        DisplayId = row["DisplayId"].ToString(),
                        RecommendationHoldId = row["RecommendationHoldId"].ToString(),
                        RecommendationHostHoldId = row["RecommendationHostHoldId"].ToString(),
                        HostName = row["HostName"].ToString(),
                        HostImage = row["HostImage"].ToString(),
                        CreatedDate = row["CreatedDate"].ToString(),
                        UpdatedDate = row["UpdatedDate"].ToString()
                    });
                }
            }
            return responseInfo;
        }
        #endregion

        #region "Reject Home And Search Page Recommendation Request"
        public CommonDbResponse RejectHomeAndSearchPageRecommendationRequest(string recommendationHoldId, string clubId, string locationId, string displayId, Common commonRequest)
        {
            string sp_name = "EXEC sproc_admin_recommendation_request_management_v2 @Flag= 'rhsrr'";
            sp_name += ",@RecommendationHoldId=" + _dao.FilterString(recommendationHoldId);
            sp_name += ",@ClubId=" + _dao.FilterString(clubId);
            sp_name += ",@DisplayId=" + _dao.FilterString(displayId);
            sp_name += ",@LocationId=" + _dao.FilterString(locationId);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonRequest.ActionUser);
            sp_name += ",@ActionIp=" + _dao.FilterString(commonRequest.ActionIP);
            return _dao.ParseCommonDbResponse(sp_name);
        }
        #endregion

        #region "Reject Main Page Recommendation Request"
        public CommonDbResponse RejectMainPageRecommendationRequest(string recommendationHoldId, string clubId, string locationId, string displayId, Common commonRequest)
        {
            string sp_name = "EXEC sproc_admin_recommendation_request_management_v2 @Flag= 'rmprr'";
            sp_name += ",@RecommendationHoldId=" + _dao.FilterString(recommendationHoldId);
            sp_name += ",@ClubId=" + _dao.FilterString(clubId);
            sp_name += ",@DisplayId=" + _dao.FilterString(displayId);
            sp_name += ",@LocationId=" + _dao.FilterString(locationId);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonRequest.ActionUser);
            sp_name += ",@ActionIp=" + _dao.FilterString(commonRequest.ActionIP);
            return _dao.ParseCommonDbResponse(sp_name);
        }
        #endregion

        #region "Remove Main Page Single Host Recommendation Requests"
        public CommonDbResponse RemoveMainPageSingleHostRecommendationReq(string recommendationHoldId, string clubId, string recommendationHoldHostId, Common commonRequest)
        {
            string sp_name = "EXEC sproc_admin_recommendation_request_management_v2 @Flag= 'rmphrr'";
            sp_name += ",@RecommendationHoldId=" + _dao.FilterString(recommendationHoldId);
            sp_name += ",@ClubId=" + _dao.FilterString(clubId);
            sp_name += ",@RecommendationHoldHostId=" + _dao.FilterString(recommendationHoldHostId);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonRequest.ActionUser);
            sp_name += ",@ActionIp=" + _dao.FilterString(commonRequest.ActionIP);
            return _dao.ParseCommonDbResponse(sp_name);
        }
        #endregion

        #region "Remove Home & Search Page Single Host Recommendation Requests"
        public CommonDbResponse RemoveHomeAndSearchPageSingleHostRecommendationReq(string recommendationHoldId, string clubId, string recommendationHoldHostId, Common commonRequest)
        {
            string sp_name = "EXEC sproc_admin_recommendation_request_management_v2 @Flag= 'rhsphrr'";
            sp_name += ",@RecommendationHoldId=" + _dao.FilterString(recommendationHoldId);
            sp_name += ",@ClubId=" + _dao.FilterString(clubId);
            sp_name += ",@RecommendationHoldHostId=" + _dao.FilterString(recommendationHoldHostId);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonRequest.ActionUser);
            sp_name += ",@ActionIp=" + _dao.FilterString(commonRequest.ActionIP);
            return _dao.ParseCommonDbResponse(sp_name);
        }

        #endregion

        #region "APPROVE HOME AND SEARCH PAGE RECOMMENDATION REQUEST"

        public CommonDbResponse ApproveHomeAndSearchPageRecommendationReq(string recommendationHoldId, string clubId, string displayIdHold, string locationId, Common commonRequest)
        {
            string sp_name = "EXEC sproc_admin_accept_recommendation_request_Management @Flag= 'ahsrr'";
            sp_name += ",@RecommendationHoldId=" + _dao.FilterString(recommendationHoldId);
            sp_name += ",@ClubId=" + _dao.FilterString(clubId);
            sp_name += ",@DisplayIdHold=" + _dao.FilterString(displayIdHold);
            sp_name += ",@LocationId=" + _dao.FilterString(locationId);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonRequest.ActionUser);
            sp_name += ",@ActionIp=" + _dao.FilterString(commonRequest.ActionIP);
            return _dao.ParseCommonDbResponse(sp_name);
        }
        #endregion

        #region "MAIN PAGE RECOMMENDATION REQUEST FOR UPDATE"
        public ManageClubRecommendationRequestCommon MainPageRecommendationReqForUpdate(string clubId, string displayIdHold, string locationId)
        {
            ManageClubRecommendationRequestCommon responseinfo = new ManageClubRecommendationRequestCommon();
            string sp_name = "EXEC sproc_admin_accept_recommendation_request_Management @Flag= 'gmprrd'";
            sp_name += ",@ClubId=" + _dao.FilterString(clubId);
            sp_name += ",@DisplayIdHold=" + _dao.FilterString(displayIdHold);
            sp_name += ",@LocationId=" + _dao.FilterString(locationId);
            var dbresponseInfo = _dao.ExecuteDataRow(sp_name);
            if (dbresponseInfo != null)
            {
                return new ManageClubRecommendationRequestCommon()
                {
                    ClubId = _dao.ParseColumnValue(dbresponseInfo, "ClubId").ToString(),
                    DisplayOrderId = _dao.ParseColumnValue(dbresponseInfo, "DisplayOrderId").ToString(),
                    DisplayId = _dao.ParseColumnValue(dbresponseInfo, "DisplayIdHold").ToString(),
                    RecommendationId = _dao.ParseColumnValue(dbresponseInfo, "RecommendationHoldId").ToString(),
                    LocationId = _dao.ParseColumnValue(dbresponseInfo, "LocationId").ToString()

                };
            }
            return new ManageClubRecommendationRequestCommon();
        }
        #endregion

        #region "MAIN PAGE HOST RECOMMENDATION DETAIL FOR UPDATE"
        public List<MainPageHostListForUpdateCommon> MainPageHostRecommendationReqForUpdate(string recommendationHoldId, string clubId, string displayIdHold, string locationId)
        {
            List<MainPageHostListForUpdateCommon> responseInfo = new List<MainPageHostListForUpdateCommon>();
            string sp_name = "EXEC sproc_admin_accept_recommendation_request_Management @Flag= 'gmphrd'";
            sp_name += ",@RecommendationHoldId=" + _dao.FilterString(recommendationHoldId);
            sp_name += ",@ClubId=" + _dao.FilterString(clubId);
            sp_name += ",@DisplayIdHold=" + _dao.FilterString(displayIdHold);
            sp_name += ",@LocationId=" + _dao.FilterString(locationId);
            var dbResponse = _dao.ExecuteDataTable(sp_name);
            if (dbResponse != null)
            {
                foreach (DataRow row in dbResponse.Rows)
                {
                    responseInfo.Add(new MainPageHostListForUpdateCommon()
                    {
                        HostDisplayOrderHoldId = row["HostDisplayOrderHoldId"].ToString(),
                        HostId = row["HostId"].ToString(),
                        RecommendationHoldId = row["RecommendationHoldId"].ToString(),
                        RecommendationHostHoldId = row["RecommendationHostHoldId"].ToString(),
                        HostName = row["HostName"].ToString()
                    });
                }
            }
            return responseInfo;
        }
        #endregion

        #region "'
        public CommonDbResponse ApproveMainPageRecommendationReq(string recommendationHoldId, string clubId, string displayIdHold, string locationId, string groupId, Common commonRequest)
        {
            string sp_name = "EXEC sproc_admin_accept_recommendation_request_Management @Flag= 'amprd'";
            sp_name += ",@RecommendationHoldId=" + _dao.FilterString(recommendationHoldId);
            sp_name += ",@ClubId=" + _dao.FilterString(clubId);
            sp_name += ",@DisplayIdHold=" + _dao.FilterString(displayIdHold);
            sp_name += ",@LocationId=" + _dao.FilterString(locationId);
            sp_name += ",@GroupId =" + _dao.FilterString(groupId);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonRequest.ActionUser);
            sp_name += ",@ActionIp=" + _dao.FilterString(commonRequest.ActionIP);
            return _dao.ParseCommonDbResponse(sp_name);
        }
        #endregion
    }
}
