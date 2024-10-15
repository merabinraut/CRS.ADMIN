using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.RecommendationManagement_V2;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.RecommendationManagement_V2
{
    public interface IRecommnedationManagementV2Repository
    {
        CommonDbResponse ApproveHomeAndSearchPageRecommendationReq(string recommendationHoldId, string clubId, string displayIdHold, string locationId, Common commonRequest);
        CommonDbResponse ApproveMainPageRecommendationReq(string recommendationHoldId, string clubId, string displayIdHold, string locationId, string groupId, Common commonRequest);
        CommonDbResponse DeleteHomePageRequest(string clubid, string locationid, string displayid, string recommendationid, Common commonRequest);
        CommonDbResponse DeleteMainPageRequest(string clubid, string locationId, string displayid, string recommendationid, Common commonRequest, string groupId);
        CommonDbResponse DeleteSearchPageRequest(string clubid, string locationId, string displayid, string recommendationid, Common commonRequest);
        CommonDbResponse DeleteSelectedHost(string clubId, string recommendationHostId, string recommendationId, string hostId, Common commonRequest);
        List<ClubRecommendationManagementListModelCommon> GetClubRecommendationReqList();
        List<HomePageClubRequestListModelCommon> GetClubRequestListByHomePage(string locationId, string SearchFilter = "");
        List<MainPageClubRequestListModelCommon> GetClubRequestListByMainPage(string locationId, string groupId, string SearchFilter = "");
        List<SearchPageClubRequestListModelCommon> GetClubRequestListBySearchPage(string locationId, string SearchFilter = "");
        List<DisplayPageListModelCommon> GetDisplayPageList();
        List<GroupListModelCommon> GetGroupList(string LocationId, string SearchFilter = "");
        List<LocationListModelCommon> GetLocationList();
        List<MainPageClubRecommendationReqHostListModelCommon> GetMainPageClubRecommendationReqHostList(string recommendationHoldId, string clubId, string locationId, string displayId);
        List<HostRecommendationDetailModelCommon> GetMPageHostRecommendationDetail(string recommendationId, string groupid, string locationId, string displayId, string clubId);
        ManageClubRecommendationRequestCommon GetMPageRecommendationDetail(string recommendationId, string groupid, string locationId, string displayId, string clubId);
        List<SearchAndHomePageRecommendationReqHostListModelCommon> GetSearchAndHomePageClubRecommendationReqHostList(string recommendationHoldId, string clubId, string locationId, string displayId);
        List<HostListModelCommon> GetSelectedHostList(string recommendationId, string clubId, string locationId, string groupId);
        List<ShufflingTimeListModelCommon> GetShufflingTimeList();
        List<MainPageHostListForUpdateCommon> MainPageHostRecommendationReqForUpdate(string recommendationHoldId, string clubId, string displayIdHold, string locationId);
        ManageClubRecommendationRequestCommon MainPageRecommendationReqForUpdate(string clubId, string displayIdHold, string locationId);
        CommonDbResponse ManageGroup(ManageGroupCommon commonModel);
        CommonDbResponse DeleteGroup(string groupid, string locationid, Common commonRequest);
        CommonDbResponse ManageHomePageRequest(ManageHomePageRequestCommon commonModel);
        CommonDbResponse ManageMainPageRequest(ManageClubRecommendationRequestCommon commonModel);
        CommonDbResponse ManageSearchPageRequest(ManageSearchPageRequestCommon commonModel);
        CommonDbResponse ManageshufflingTime(ManageShufflingTimeCommon commonModel);
        CommonDbResponse RejectHomeAndSearchPageRecommendationRequest(string recommendationHoldId, string clubId, string locationId, string displayId, Common commonRequest);
        CommonDbResponse RejectMainPageRecommendationRequest(string recommendationHoldId, string clubId, string locationId, string displayId, Common commonRequest);
        CommonDbResponse RemoveHomeAndSearchPageSingleHostRecommendationReq(string recommendationHoldId, string clubId, string recommendationHoldHostId, Common commonRequest);
        CommonDbResponse RemoveMainPageSingleHostRecommendationReq(string recommendationHoldId, string clubId, string recommendationHoldHostId, Common commonRequest);
    }
}
