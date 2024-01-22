using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.RecommendationManagement;
using System.Collections.Generic;
using System.Data;

namespace CRS.ADMIN.REPOSITORY.RecommendationManagement
{
    public class RecommendationManagementRepository : IRecommendationManagementRepository
    {
        private readonly RepositoryDao _dao;
        public RecommendationManagementRepository()
        {
            _dao = new RepositoryDao();
        }

        public CommonDbResponse DeleteClubRecommendatioRequest(string recommendationId, Common commonRequest)
        {
            string sp_name = "EXEC sproc_admin_recommendation_management @Flag='dcr'";
            sp_name += ",@RecommendationId=" + _dao.FilterString(recommendationId);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonRequest.ActionUser);
            sp_name += ",@ActionIp=" + _dao.FilterString(commonRequest.ActionIP);
            var dbResponseInfo = _dao.ParseCommonDbResponse(sp_name);
            return dbResponseInfo;
        }

        public List<ClubRequestListModelCommon> GetClubRequestList(string ClubLocationID)
        {
            List<ClubRequestListModelCommon> responseInfo = new List<ClubRequestListModelCommon>();
            string sp_name = "EXEC sproc_admin_recommendation_management @Flag='grgcl'";
            sp_name += ",@LocationId=" + _dao.FilterString(ClubLocationID);
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new ClubRequestListModelCommon()
                    {
                        RecommendationId = row["RecommendationId"].ToString(),
                        ClubId = row["ClubId"].ToString(),
                        ClubName = row["ClubName"].ToString(),
                        ClubCategory = row["ClubCategory"].ToString(),
                        ClubLogo = row["ClubLogo"].ToString(),
                        DisplayOrder = row["DisplayOrder"].ToString(),
                        ActionDate = row["ActionDate"].ToString(),
                        DisplayPage = row["DisplayPage"].ToString()
                    });
                }
            }
            return responseInfo;
        }

        public List<DisplayPageOrderDetailModelCommon> GetDisplayPageDetail(string recommendationId)
        {
            List<DisplayPageOrderDetailModelCommon> responseinfo = new List<DisplayPageOrderDetailModelCommon>();
            string sp_name = "EXEC sproc_admin_recommendation_management @Flag='gdpd2'";
            sp_name += ",@RecommendationId=" + _dao.FilterString(recommendationId);
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseinfo.Add(new DisplayPageOrderDetailModelCommon()
                    {
                        RecommendationId = row["RecommendationId"].ToString(),
                        DisplayPageId = row["DisplayPageId"].ToString()
                    });
                }
            }
            return responseinfo;
        }

        public List<RecommendationGroupListModelCommon> GetGroupList(string locationId)
        {
            List<RecommendationGroupListModelCommon> responseInfo = new List<RecommendationGroupListModelCommon>();
            string sp_name = "sproc_recommendation_group_management @Flag='grgl'";
            sp_name += ",@LocationId=" + _dao.FilterParameter(locationId);
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new RecommendationGroupListModelCommon()
                    {
                        GroupId = row["GroupId"].ToString(),
                        GroupName = row["GroupName"].ToString(),
                        Descriptions = row["Description"].ToString(),
                        RequestedDate = row["CreatedDate"].ToString(),
                        TotalClubs = row["TotalClubs"].ToString()
                    });
                }
            }
            return responseInfo;
        }

        public List<HostRecommendationDetailModelCommon> GetHostRecommendationDetail(string recommendationId)
        {
            List<HostRecommendationDetailModelCommon> responseInfo = new List<HostRecommendationDetailModelCommon>();
            string sp_name = "EXEC sproc_admin_recommendation_management @Flag='ghrd3'";
            sp_name += ",@RecommendationId=" + _dao.FilterString(recommendationId);
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new HostRecommendationDetailModelCommon()
                    {
                        ClubId = row["ClubId"].ToString(),
                        HostId = row["HostId"].ToString(),
                        OrderId = row["OrderId"].ToString(),
                        RecommendationHostId = row["RecommendationHostId"].ToString(),
                        RecommendationId = row["RecommendationId"].ToString()
                    });
                }
            }
            return responseInfo;
        }

        public List<LocationListModelCommon> GetLocationList()
        {
            List<LocationListModelCommon> responseInfo = new List<LocationListModelCommon>();
            string sp_name = " EXEC sproc_recommendation_group_management @Flag = 'gll'";
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new LocationListModelCommon()
                    {
                        LocationId = row["LocationId"].ToString(),
                        LocationName = row["LocationName"].ToString(),
                        LocationImage = row["LocationImage"].ToString(),
                    });
                }
            }
            return responseInfo;
        }

        public List<RecommendationAnalyticListModelCommon> GetRecommendationAnalyticList()
        {
            List<RecommendationAnalyticListModelCommon> responseInfo = new List<RecommendationAnalyticListModelCommon>();
            string sp_name = "EXEC sproc_admin_recommendation_request_management @Flag='gra'";
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new RecommendationAnalyticListModelCommon()
                    {
                        StatusCount = row["StatusCount"].ToString(),
                        StatusLabel = row["StatusLabel"].ToString()
                    });
                }
            }
            return responseInfo;
        }

        public ManageClubRecommendationRequestCommon GetRecommendationDetail(string recommendationID, string groupId, string locationId)
        {
            string sp_name = "EXEC sproc_admin_recommendation_management @Flag='grds1'";
            sp_name += ",@RecommendationId=" + _dao.FilterString(recommendationID);
            sp_name += ",@GroupId=" + _dao.FilterString(groupId);
            sp_name += ",@LocationId=" + _dao.FilterString(locationId);
            var dbResponseInfo = _dao.ExecuteDataRow(sp_name);
            if (dbResponseInfo != null)
            {
                return new ManageClubRecommendationRequestCommon()
                {
                    RecommendationId = _dao.ParseColumnValue(dbResponseInfo, "RecommendationId").ToString(),
                    LocationId = _dao.ParseColumnValue(dbResponseInfo, "LocationId").ToString(),
                    ClubId = _dao.ParseColumnValue(dbResponseInfo, "ClubId").ToString(),
                    GroupId = _dao.ParseColumnValue(dbResponseInfo, "GroupId").ToString(),
                    DisplayOrderId = _dao.ParseColumnValue(dbResponseInfo, "DisplayOrderId").ToString()

                };
            }
            return new ManageClubRecommendationRequestCommon();
        }

        public List<RecommendationRequestListModelCommon> GetRecommendationRequestList()
        {
            List<RecommendationRequestListModelCommon> responseInfo = new List<RecommendationRequestListModelCommon>();
            string sp_name = "EXEC sproc_admin_recommendation_management @Flag='grgcl'";
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new RecommendationRequestListModelCommon()
                    {
                        RecommendationId = row["RecommendationId"].ToString(),
                        ClubId = row["ClubId"].ToString(),
                        ClubName = row["ClubName"].ToString(),
                        ClubCategory = row["ClubCategory"].ToString(),
                        ClubLogo = row["ClubLogo"].ToString(),
                        DisplayOrder = row["DisplayOrder"].ToString(),
                        ActionDate = row["ActionDate"].ToString(),
                        DisplayPage = row["DisplayPage"].ToString()
                    });
                }
            }
            return responseInfo;
        }

        public List<ShufflingTimeListModelCommon> GetShufflingTimeList()
        {
            List<ShufflingTimeListModelCommon> responseInfo = new List<ShufflingTimeListModelCommon>();
            string sp_name = "EXEC sproc_admin_recommendation_management @Flag='gstl'";
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new ShufflingTimeListModelCommon()
                    {
                        DisplayId = row["DisplayId"].ToString(),
                        DisplayName = row["DisplayName"].ToString(),
                        DisplayTime = row["DisplayTime"].ToString()
                    });
                }
            }
            return responseInfo;
        }

        public CommonDbResponse ManageClubRecommendationRequest(ManageClubRecommendationRequestCommon commonModel)
        {
            string sp_name = "";
            if (!string.IsNullOrEmpty(commonModel.RecommendationId))
            {
                sp_name = "EXEC sproc_admin_recommendation_management @Flag='ucrr'";
                sp_name += ",@RecommendationId=" + _dao.FilterParameter(commonModel.RecommendationId);
            }
            else
            {
                sp_name = "EXEC sproc_admin_recommendation_management @Flag='ccrr'";
            }
            sp_name += ",@ClubId=" + _dao.FilterString(commonModel.ClubId);
            sp_name += ",@GroupId=" + _dao.FilterString(commonModel.GroupId);
            sp_name += ",@DisplayPageId=" + _dao.FilterString(commonModel.DisplayPageId);
            sp_name += ",@OrderId=" + _dao.FilterString(commonModel.DisplayOrderId);
            sp_name += ",@LocationId=" + _dao.FilterString(commonModel.LocationId);
            sp_name += ",@HostRecommendationCSValue=" + _dao.FilterString(commonModel.HostRecommendationCSValue);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonModel.ActionUser);
            sp_name += ",@ActionIp=" + _dao.FilterString(commonModel.ActionIP);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(commonModel.ActionPlatform);
            return _dao.ParseCommonDbResponse(sp_name);
        }

        public CommonDbResponse ManageGroup(ManageGroupCommon commonModel)
        {
            string sp_name = "sproc_recommendation_group_management @Flag='cg'";
            sp_name += ",@GroupName=N" + _dao.FilterString(commonModel.GroupName);
            sp_name += ",@Description=N" + _dao.FilterString(commonModel.Description);
            sp_name += ",@DisplayOrderId=" + _dao.FilterString(commonModel.DisplayOrderId);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonModel.ActionUser);
            sp_name += ",@ActionIP=" + _dao.FilterString(commonModel.ActionIP);
            return _dao.ParseCommonDbResponse(sp_name);
        }

        public CommonDbResponse ManageShufflingTime(ManageShufflingTimeCommon commonModel)
        {
            string sp_name = "EXEC sproc_admin_recommendation_management @Flag='mst'";
            sp_name += ",@ShufflingTimeCSValue=" + _dao.FilterString(commonModel.ShufflingTimeCSValue);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonModel.ActionUser);
            return _dao.ParseCommonDbResponse(sp_name);
        }
    }
}
