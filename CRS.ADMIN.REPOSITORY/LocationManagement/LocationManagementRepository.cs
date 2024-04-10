using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.LocationManagement;
using System.Collections.Generic;
using System.Data;

namespace CRS.ADMIN.REPOSITORY.LocationManagement
{
    public class LocationManagementRepository : ILocationManagementRepository
    {
        private readonly RepositoryDao _dao;

        public LocationManagementRepository(RepositoryDao dao) => this._dao = dao;

        public List<LocationCommon> GetLocations(string SearchFilter = "")
        {
            var locationList = new List<LocationCommon>();
            var sql = "Exec sproc_admin_location_management @Flag='s'";
            sql += !string.IsNullOrEmpty(SearchFilter) ? ",@SearchFilter=N" + _dao.FilterString(SearchFilter) : "";
            var dt = _dao.ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    locationList.Add(new LocationCommon()
                    {
                        LocationId = item["LocationID"].ToString(),
                        LocationName = item["LocationName"].ToString(),
                        LocationSubtitle = item["LocationSubtitle"].ToString(),
                        LocationImage = item["LocationImage"].ToString(),
                        LocationStatus = item["LocationStatus"].ToString(),
                        TotalClubs = item["ClubCount"].ToString()
                    });
                }
            }
            return locationList;
        }

        public LocationCommon GetLocation(LocationCommon locationCommon)
        {
            string sql = "Exec sproc_admin_location_management";
            sql += " @Flag='sd'";
            sql += ", @LocationId=" + _dao.FilterString(locationCommon.LocationId);
            sql += ", @ActionUser=" + _dao.FilterString(locationCommon.ActionUser);
            var dataTable = _dao.ExecuteDataTable(sql);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return new LocationCommon()
                {
                    LocationId = dataTable.Rows[0]["LocationId"].ToString(),
                    LocationName = dataTable.Rows[0]["LocationName"].ToString(),
                    LocationSubtitle = dataTable.Rows[0]["LocationSubtitle"].ToString(),
                    LocationImage = dataTable.Rows[0]["LocationImage"].ToString(),
                    LocationURL = dataTable.Rows[0]["LocationURL"].ToString(),
                    Latitude = dataTable.Rows[0]["Latitude"].ToString(),
                    Longitude = dataTable.Rows[0]["Longitude"].ToString(),
                    LocationStatus = dataTable.Rows[0]["STATUS"].ToString(),
                    ActionUser = dataTable.Rows[0]["ActionUser"].ToString(),
                    ActionIP = dataTable.Rows[0]["ActionIp"].ToString(),
                    ActionDate = dataTable.Rows[0]["ActionDate"].ToString(),
                };
            }
            return new LocationCommon();
        }

        public CommonDbResponse ManageLocation(LocationCommon locationCommon)
        {
            string sql = "Exec sproc_admin_location_management";
            string flag = locationCommon.LocationId is null ? "i" : "u";
            sql += $" @Flag='{flag}'";
            sql += ", @LocationId=" + _dao.FilterString(locationCommon.LocationId);
            sql += ", @LocationName=N" + _dao.FilterString(locationCommon.LocationName);
            sql += ", @LocationSubtitle=N" + _dao.FilterString(locationCommon.LocationSubtitle);
            sql += ", @LocationImage=" + _dao.FilterString(locationCommon.LocationImage);
            sql += ", @LocationURL=" + _dao.FilterString(locationCommon.LocationURL);
            sql += ", @Latitude=" + _dao.FilterString(locationCommon.Latitude);
            sql += ", @Longitude=" + _dao.FilterString(locationCommon.Longitude);
            sql += ", @ActionIp=" + _dao.FilterString(locationCommon.ActionIP);
            sql += ", @ActionPlatform=" + _dao.FilterString(locationCommon.ActionPlatform);
            sql += ", @ActionUser=" + _dao.FilterString(locationCommon.ActionUser);
            var commondbResp = _dao.ParseCommonDbResponse(sql);
            return commondbResp;
        }
        public CommonDbResponse EnableDisableLocation(LocationCommon locationCommon)
        {
            string sql = "Exec sproc_admin_location_management";
            sql += " @Flag='bu'";
            sql += ", @LocationId=" + _dao.FilterString(locationCommon.LocationId);
            sql += ", @ActionUser=" + _dao.FilterString(locationCommon.ActionUser);
            sql += ", @ActionIp=" + _dao.FilterString(locationCommon.ActionIP);
            var commondbResp = _dao.ParseCommonDbResponse(sql);
            return commondbResp;
        }
    }
}