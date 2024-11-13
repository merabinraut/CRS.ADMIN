using CRS.ADMIN.REPOSITORY.RecommendationManagement_V2;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.RecommendationManagement_V2;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.RecommendationManagement_V2
{
    public class RecommendationManagementV2Business : IRecommendationManagementV2Business
    {
        private readonly IRecommnedationManagementV2Repository _repo;
        public RecommendationManagementV2Business(RecommendationManagementV2Repository repo)
        {
            _repo = repo;
        }

        public CommonDbResponse ApproveHomeAndSearchPageRecommendationReq(string recommendationHoldId, string clubId, string displayIdHold, string locationId, Common commonRequest)
        {
            return _repo.ApproveHomeAndSearchPageRecommendationReq(recommendationHoldId, clubId, displayIdHold, locationId, commonRequest);
        }

        public CommonDbResponse ApproveMainPageRecommendationReq(string recommendationHoldId, string clubId, string displayIdHold, string locationId, string groupId, Common commonRequest)
        {
            return _repo.ApproveMainPageRecommendationReq(recommendationHoldId, clubId, displayIdHold, locationId, groupId, commonRequest);
        }

        public CommonDbResponse DeleteHomePageRequest(string clubid, string locationid, string displayid, string recommendationid, Common commonRequest)
        {
            return _repo.DeleteHomePageRequest(clubid, locationid, displayid, recommendationid, commonRequest);
        }

        public CommonDbResponse DeleteMainPageRequest(string clubid, string locationId, string displayid, string recommendationid, Common commonRequest, string groupId)
        {
            return _repo.DeleteMainPageRequest(clubid, locationId, displayid, recommendationid, commonRequest, groupId);
        }

        public CommonDbResponse DeleteSearchPageRequest(string clubid, string locationId, string displayid, string recommendationid, Common commonRequest)
        {
            return _repo.DeleteSearchPageRequest(clubid, locationId, displayid, recommendationid, commonRequest);
        }

        public CommonDbResponse DeleteSelectedHost(string clubId, string recommendationHostId, string recommendationId, string hostId, Common commonRequest)
        {
            return _repo.DeleteSelectedHost(clubId, recommendationHostId, recommendationId, hostId, commonRequest);
        }

        public List<ClubRecommendationManagementListModelCommon> GetClubRecommendationReqList()
        {
            return _repo.GetClubRecommendationReqList();
        }

        public List<HomePageClubRequestListModelCommon> GetClubRequestListByHomePage(string LocationId, string SearchFilter = "")
        {
            return _repo.GetClubRequestListByHomePage(LocationId, SearchFilter);
        }

        public List<MainPageClubRequestListModelCommon> GetClubRequestListByMainPage(string locationId, string groupId, string SearchFilter = "")
        {
            return _repo.GetClubRequestListByMainPage(locationId, groupId, SearchFilter);
        }

        public List<SearchPageClubRequestListModelCommon> GetClubRequestListBySearchPage(string locationId,PaginationFilterCommon objPaginationFilterCommon )
        {
            return _repo.GetClubRequestListBySearchPage(locationId, objPaginationFilterCommon);
        }

        public List<DisplayPageListModelCommon> GetDisplayPageList()
        {
            return _repo.GetDisplayPageList();
        }

        public List<GroupListModelCommon> GetGroupList(string LocationId, string SearchFilter = "")
        {
            return _repo.GetGroupList(LocationId, SearchFilter);
        }

        public List<LocationListModelCommon> GetLocationList()
        {
            return _repo.GetLocationList();
        }

        public List<MainPageClubRecommendationReqHostListModelCommon> GetMainPageClubRecommendationReqHostList(string recommendationHoldId, string clubId, string locationId, string displayId)
        {
            return _repo.GetMainPageClubRecommendationReqHostList(recommendationHoldId, clubId, locationId, displayId);
        }

        public List<HostRecommendationDetailModelCommon> GetMPageHostRecommendationDetail(string recommendationId, string groupid, string locationId, string displayId, string clubId)
        {
            return _repo.GetMPageHostRecommendationDetail(recommendationId, groupid, locationId, displayId, clubId);
        }

        public ManageClubRecommendationRequestCommon GetMPageRecommendationDetail(string recommendationId, string groupid, string locationId, string displayId, string clubId)
        {
            return _repo.GetMPageRecommendationDetail(recommendationId, groupid, locationId, displayId, clubId);
        }

        public List<SearchAndHomePageRecommendationReqHostListModelCommon> GetSearchAndHomePageClubRecommendationReqHostList(string recommendationHoldId, string clubId, string locationId, string displayId)
        {
            return _repo.GetSearchAndHomePageClubRecommendationReqHostList(recommendationHoldId, clubId, locationId, displayId);
        }

        public List<HostListModelCommon> GetSelectedHostList(string recommendationId, string clubId, string locationId, string groupId)
        {
            return _repo.GetSelectedHostList(recommendationId, clubId, locationId, groupId);
        }

        public List<ShufflingTimeListModelCommon> GetShufflingTimeList()
        {
            return _repo.GetShufflingTimeList();
        }

        public List<MainPageHostListForUpdateCommon> MainPageHostRecommendationReqForUpdate(string RecommendationHoldId, string clubId, string displayIdHold, string locationId)
        {
            return _repo.MainPageHostRecommendationReqForUpdate(RecommendationHoldId, clubId, displayIdHold, locationId);
        }

        public ManageClubRecommendationRequestCommon MainPageRecommendationReqForUpdate(string clubId, string displayIdHold, string locationId)
        {
            return _repo.MainPageRecommendationReqForUpdate(clubId, displayIdHold, locationId);
        }

        public CommonDbResponse ManageGroup(ManageGroupCommon commonModel)
        {
            return _repo.ManageGroup(commonModel);
        }
        public CommonDbResponse DeleteGroup(string groupid, string locationid, Common commonRequest)
        {
            return _repo.DeleteGroup(groupid, locationid, commonRequest);
        }

        public CommonDbResponse ManageHomePageRequest(ManageHomePageRequestCommon commonModel)
        {
            return _repo.ManageHomePageRequest(commonModel);
        }

        public CommonDbResponse ManageMainPageRequest(ManageClubRecommendationRequestCommon commonModel)
        {
            return _repo.ManageMainPageRequest(commonModel);
        }

        public CommonDbResponse ManageSearchPageRequest(ManageSearchPageRequestCommon commonModel)
        {
            return _repo.ManageSearchPageRequest(commonModel);
        }

        public CommonDbResponse ManageShufflingTime(ManageShufflingTimeCommon commonModel)
        {
            return _repo.ManageshufflingTime(commonModel);
        }

        public CommonDbResponse RejectHomeAndSearchPageRecommendationRequest(string recommendationHoldId, string clubId, string locationId, string displayId, Common commonRequest)
        {
            return _repo.RejectHomeAndSearchPageRecommendationRequest(recommendationHoldId, clubId, locationId, displayId, commonRequest);
        }

        public CommonDbResponse RejectMainPageRecommendationRequest(string recommendationHoldId, string clubId, string locationId, string displayId, Common commonRequest)
        {
            return _repo.RejectMainPageRecommendationRequest(recommendationHoldId, clubId, locationId, displayId, commonRequest);
        }

        public CommonDbResponse RemoveHomeAndSearchPageSingleHostRecommendationReq(string recommendationHoldId, string clubId, string recommendationHoldHostId, Common commonRequest)
        {
            return _repo.RemoveHomeAndSearchPageSingleHostRecommendationReq(recommendationHoldId, clubId, recommendationHoldHostId, commonRequest);
        }

        public CommonDbResponse RemoveMainPageSingleHostRecommendationReq(string recommendationHoldId, string clubId, string recommendationHoldHostId, Common commonRequest)
        {
            return _repo.RemoveMainPageSingleHostRecommendationReq(recommendationHoldId, clubId, recommendationHoldHostId, commonRequest);
        }
    }
}
