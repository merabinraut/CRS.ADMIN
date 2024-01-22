using CRS.ADMIN.REPOSITORY.RecommendationManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.RecommendationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.RecommendationManagement
{
    public class RecommendationManagementBusiness : IRecommendationManagementBusiness
    {
        private readonly IRecommendationManagementRepository _repository;
        public RecommendationManagementBusiness(RecommendationManagementRepository repository)
        {
            _repository = repository;
        }

        public CommonDbResponse DeleteClubRecommendatioRequest(string recommendationId, Common commonRequest)
        {
            return _repository.DeleteClubRecommendatioRequest(recommendationId,commonRequest);
        }

        public List<ClubRequestListModelCommon> GetClubRequestList(string ClubLocationID)
        {
            return _repository.GetClubRequestList(ClubLocationID);
        }

        public List<DisplayPageOrderDetailModelCommon> GetDisplayPageDetail(string recommendationId)
        {
            return _repository.GetDisplayPageDetail(recommendationId);
        }

        public List<RecommendationGroupListModelCommon> GetGroupList(string locationId)
        {
            return _repository.GetGroupList(locationId);
        }

        public List<HostRecommendationDetailModelCommon> GetHostRecommendationDetail(string recommendationId)
        {
            return _repository.GetHostRecommendationDetail(recommendationId);
        }

        public List<LocationListModelCommon> GetLocationList()
        {
            return _repository.GetLocationList();
        }

        public List<RecommendationAnalyticListModelCommon> GetRecommendationAnalyticList()
        {
            return _repository.GetRecommendationAnalyticList();
        }

        public ManageClubRecommendationRequestCommon GetRecommendationDetail(string recommendationID, string groupId, string locationId)
        {
            return _repository.GetRecommendationDetail(recommendationID, groupId, locationId);
        }

        public List<RecommendationRequestListModelCommon> GetRecommendationRequestList()
        {
            return _repository.GetRecommendationRequestList();
        }

        public List<ShufflingTimeListModelCommon> GetShufflingTimeList()
        {
            return _repository.GetShufflingTimeList();
        }

        public CommonDbResponse ManageClubRecommendationRequest(ManageClubRecommendationRequestCommon commonModel)
        {
            return _repository.ManageClubRecommendationRequest(commonModel);
        }

        public CommonDbResponse ManageGroup(ManageGroupCommon commonModel)
        {
            return _repository.ManageGroup(commonModel);
        }

        public CommonDbResponse ManageShufflingTime(ManageShufflingTimeCommon commonModel)
        {
            return _repository.ManageShufflingTime(commonModel);
        }
    }
}
