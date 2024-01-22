using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.HostManagement;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CRS.ADMIN.REPOSITORY.HostManagement
{
    public class HostManagementRepository : IHostManagementRepository
    {
        RepositoryDao _DAO;
        public HostManagementRepository()
        {
            _DAO = new RepositoryDao();
        }
        public List<HostListCommon> GetHostList(string AgentId, string SearchFilter = "")
        {
            var response = new List<HostListCommon>();
            string SQL = "EXEC sproc_host_management @Flag='ghl'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += !string.IsNullOrEmpty(SearchFilter) ? ",@SearchFilter=N" + _DAO.FilterString(SearchFilter) : null;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new HostListCommon()
                    {
                        AgentId = _DAO.ParseColumnValue(item, "AgentId").ToString(),
                        HostId = _DAO.ParseColumnValue(item, "HostId").ToString(),
                        HostName = _DAO.ParseColumnValue(item, "HostName").ToString(),
                        Position = _DAO.ParseColumnValue(item, "Position").ToString(),
                        Rank = _DAO.ParseColumnValue(item, "Rank").ToString(),
                        Age = _DAO.ParseColumnValue(item, "Age").ToString(),
                        Status = _DAO.ParseColumnValue(item, "Status").ToString(),
                        CreatedDate = _DAO.ParseColumnValue(item, "CreatedDate").ToString(),
                        UpdatedDate = _DAO.ParseColumnValue(item, "UpdatedDate").ToString(),
                        ClubName = _DAO.ParseColumnValue(item, "ClubName").ToString(),
                        Ratings = _DAO.ParseColumnValue(item, "Ratings").ToString(),
                        TotalVisitors = _DAO.ParseColumnValue(item, "TotalVisitors").ToString(),
                        HostImage = _DAO.ParseColumnValue(item, "HostImage").ToString()
                    });
                }
            }
            return response;
        }

        public ManageHostCommon GetHostDetail(string AgentId, string HostId)
        {
            string SQL = "EXEC sproc_host_management @Flag='ghd'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += ",@HostId=" + _DAO.FilterString(HostId);
            var dbResponse = _DAO.ExecuteDataRow(SQL);
            if (dbResponse != null)
            {
                return new ManageHostCommon()
                {
                    AgentId = _DAO.ParseColumnValue(dbResponse, "AgentId").ToString(),
                    HostId = _DAO.ParseColumnValue(dbResponse, "HostId").ToString(),
                    HostName = _DAO.ParseColumnValue(dbResponse, "HostName").ToString(),
                    Position = _DAO.ParseColumnValue(dbResponse, "Position").ToString(),
                    DOB = _DAO.ParseColumnValue(dbResponse, "DOB").ToString(),
                    ConstellationGroup = _DAO.ParseColumnValue(dbResponse, "ConstellationGroup").ToString(),
                    Height = _DAO.ParseColumnValue(dbResponse, "Height").ToString(),
                    BloodType = _DAO.ParseColumnValue(dbResponse, "BloodType").ToString(),
                    PreviousOccupation = _DAO.ParseColumnValue(dbResponse, "PreviousOccupation").ToString(),
                    LiquorStrength = _DAO.ParseColumnValue(dbResponse, "LiquorStrength").ToString(),
                    //WebsiteLink = _DAO.ParseColumnValue(dbResponse, "WebsiteLink").ToString(),
                    InstagramLink = _DAO.ParseColumnValue(dbResponse, "InstagramLink").ToString(),
                    TiktokLink = _DAO.ParseColumnValue(dbResponse, "TiktokLink").ToString(),
                    TwitterLink = _DAO.ParseColumnValue(dbResponse, "TwitterLink").ToString(),
                    Rank = _DAO.ParseColumnValue(dbResponse, "Rank").ToString(),
                    Line = _DAO.ParseColumnValue(dbResponse, "Line").ToString(),
                    ImagePath = _DAO.ParseColumnValue(dbResponse, "ImagePath").ToString(),
                    Address = _DAO.ParseColumnValue(dbResponse, "Address").ToString()
                };
            }
            return new ManageHostCommon();
        }

        public CommonDbResponse ManageHost(ManageHostCommon Request)
        {
            string SQL = "EXEC sproc_host_management ";
            SQL += !string.IsNullOrEmpty(Request.HostId) ? "@Flag='uh'" : "@Flag='rh'";
            SQL += ",@AgentId=" + _DAO.FilterString(Request.AgentId);
            SQL += !string.IsNullOrEmpty(Request.HostId) ? ",@HostId=" + _DAO.FilterString(Request.HostId) : "";
            SQL += ",@HostName=N" + _DAO.FilterString(Request.HostName);
            SQL += ",@Position=N" + _DAO.FilterString(Request.Position);
            SQL += !string.IsNullOrEmpty(Request.Rank?.ToString()) ? ",@Rank=" + Request.Rank : "";
            SQL += ",@DOB=" + _DAO.FilterString(Request.DOB);
            SQL += ",@ConstellationGroup=" + _DAO.FilterString(Request.ConstellationGroup);
            SQL += ",@Height=" + _DAO.FilterString(Request.Height);
            SQL += ",@BloodType=" + _DAO.FilterString(Request.BloodType);
            SQL += ",@PreviousOccupation=" + _DAO.FilterString(Request.PreviousOccupation);
            SQL += ",@LiquorStrength=" + _DAO.FilterString(Request.LiquorStrength);
            //SQL += ",@WebsiteLink=" + _DAO.FilterString(Request.WebsiteLink);
            SQL += ",@TiktokLink=" + _DAO.FilterString(Request.TiktokLink);
            SQL += ",@TwitterLink=" + _DAO.FilterString(Request.TwitterLink);
            SQL += ",@InstagramLink=" + _DAO.FilterString(Request.InstagramLink);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            SQL += ",@ImagePath=" + _DAO.FilterString(Request.ImagePath);
            SQL += ",@Line=" + _DAO.FilterString(Request.Line);
            SQL += ",@Address=N" + _DAO.FilterString(Request.Address);
            return _DAO.ParseCommonDbResponse(SQL);
        }

        public CommonDbResponse ManageHostStatus(string AgentId, string HostId, string Status, Common Request)
        {
            string SQL = "EXEC sproc_host_management @Flag='uhs'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += ",@HostId=" + _DAO.FilterString(HostId);
            SQL += ",@Status=" + _DAO.FilterString(Status);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        #region Manage gallery
        public List<HostGalleryManagementCommon> GetGalleryImage(string AgentId, string HostId, string GalleryId = "", string SearchFilter = "")
        {
            string SQL = "EXEC dbo.sproc_host_gallery_management @Flag='ghgl'";
            SQL += ", @AgentId =" + _DAO.FilterString(AgentId);
            SQL += ", @HostId =" + _DAO.FilterString(HostId);
            SQL += !string.IsNullOrEmpty(GalleryId) ? ", @GalleryId =" + _DAO.FilterString(GalleryId) : "";
            SQL += !string.IsNullOrEmpty(SearchFilter) ? ", @SearchFilter =N" + _DAO.FilterString(SearchFilter) : "";
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) return _DAO.DataTableToListObject<HostGalleryManagementCommon>(dbResponse).ToList();
            return new List<HostGalleryManagementCommon>();
        }

        public CommonDbResponse ManageGalleryImage(HostManageGalleryImageCommon Request)
        {
            string SQL = "EXEC dbo.sproc_host_gallery_management ";
            SQL += !string.IsNullOrEmpty(Request.GalleryId) ? "@Flag='mhgi'" : "@Flag='ihgi'";
            SQL += !string.IsNullOrEmpty(Request.GalleryId) ? ", @GalleryId =" + _DAO.FilterString(Request.GalleryId) : "";
            SQL += ",@AgentId=" + _DAO.FilterString(Request.AgentId);
            SQL += ", @HostId =" + _DAO.FilterString(Request.HostId);
            SQL += ",@ImageTitle=N" + _DAO.FilterString(Request.ImageTitle);
            SQL += ",@ImagePath=" + _DAO.FilterString(Request.ImagePath);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }

        public CommonDbResponse ManageGalleryImageStatus(string AgentId, string HostId, string GalleryId, Common Request)
        {
            string SQL = "EXEC dbo.sproc_host_gallery_management @Flag='mhgis'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += ",@HostId=" + _DAO.FilterString(HostId);
            SQL += ",@GalleryId=" + _DAO.FilterString(GalleryId);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        #endregion
    }
}
