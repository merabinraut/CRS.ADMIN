using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.RecommendationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.RecommendationManagement
{
    public interface IRecommendationManagementRepository
    {
        CommonDbResponse DeleteClubRecommendatioRequest(string recommendationId, Common commonRequest);
        List<ClubRequestListModelCommon> GetClubRequestList(string ClubLocationID);
        List<DisplayPageOrderDetailModelCommon> GetDisplayPageDetail(string recommendationId);
        List<RecommendationGroupListModelCommon> GetGroupList(string locationId);
        List<HostRecommendationDetailModelCommon> GetHostRecommendationDetail(string recommendationId);
        List<LocationListModelCommon> GetLocationList();
        List<RecommendationAnalyticListModelCommon> GetRecommendationAnalyticList();
        ManageClubRecommendationRequestCommon GetRecommendationDetail(string recommendationID, string groupId, string locationId);
        List<RecommendationRequestListModelCommon> GetRecommendationRequestList();
        List<ShufflingTimeListModelCommon> GetShufflingTimeList();
        CommonDbResponse ManageClubRecommendationRequest(ManageClubRecommendationRequestCommon commonModel);
        CommonDbResponse ManageGroup(ManageGroupCommon commonModel);
        CommonDbResponse ManageShufflingTime(ManageShufflingTimeCommon commonModel);
    }
}
