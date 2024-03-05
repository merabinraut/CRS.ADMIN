using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.HostManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using DocumentFormat.OpenXml.Office2016.Excel;
using System;
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
        public List<HostListCommon> GetHostList(string AgentId, PaginationFilterCommon Request)
        {
            var response = new List<HostListCommon>();
            string SQL = "EXEC sproc_host_management @Flag='ghl'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _DAO.FilterString(Request.SearchFilter) : null;
            SQL += ",@Skip=" + Request.Skip;
            SQL += ",@Take=" + Request.Take;
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
                        HostImage = _DAO.ParseColumnValue(item, "HostImage").ToString(),
                        TotalRecords = Convert.ToInt32(_DAO.ParseColumnValue(item, "TotalRecords").ToString()),
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "SNO").ToString())
                    });
                }
            }
            return response;
        }

        public ManageHostCommon GetHostDetail(string AgentId, string HostId)
        {
            var Response = new ManageHostCommon();
            string SQL = "EXEC sproc_host_management @Flag='ghd'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += ",@HostId=" + _DAO.FilterString(HostId);
            var dbResponse = _DAO.ExecuteDataRow(SQL);
            if (dbResponse != null)
            {
                Response = new ManageHostCommon()
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
                    InstagramLink = _DAO.ParseColumnValue(dbResponse, "InstagramLink").ToString(),
                    TiktokLink = _DAO.ParseColumnValue(dbResponse, "TiktokLink").ToString(),
                    TwitterLink = _DAO.ParseColumnValue(dbResponse, "TwitterLink").ToString(),
                    Rank = _DAO.ParseColumnValue(dbResponse, "Rank").ToString(),
                    Line = _DAO.ParseColumnValue(dbResponse, "Line").ToString(),
                    ImagePath = _DAO.ParseColumnValue(dbResponse, "ImagePath").ToString(),
                    Address = _DAO.ParseColumnValue(dbResponse, "Address").ToString(),
                    HostNameJapanese = _DAO.ParseColumnValue(dbResponse, "HostNameJapanese").ToString(),
                    HostIntroduction = _DAO.ParseColumnValue(dbResponse, "HostIntroduction").ToString(),
                };

                string SQL2 = "EXEC sproc_host_identity_detail_management @Flag = 'ghid'";
                SQL2 += ",@ClubId=" + _DAO.FilterString(AgentId);
                SQL2 += ",@HostId=" + _DAO.FilterString(HostId);
                var dbResponse2 = _DAO.ExecuteDataTable(SQL2);
                if (dbResponse2 != null && dbResponse2.Rows.Count > 0) Response.HostIdentityDataModel = _DAO.DataTableToListObject<HostIdentityDataCommon>(dbResponse2).ToList();
            }
            return Response;
        }

        public CommonDbResponse ManageHost(ManageHostCommon Request)
        {
            var Response = new CommonDbResponse();
            string SQL = "EXEC sproc_host_management ";
            SQL += !string.IsNullOrEmpty(Request.HostId) ? "@Flag='uh'" : "@Flag='rh'";
            SQL += ",@AgentId=" + _DAO.FilterString(Request.AgentId);
            SQL += !string.IsNullOrEmpty(Request.HostId) ? ",@HostId=" + _DAO.FilterString(Request.HostId) : "";
            SQL += ",@HostName=" + _DAO.FilterString(Request.HostName);
            SQL += ",@HostNameJapanese=N" + _DAO.FilterString(Request.HostNameJapanese);
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
            SQL += string.IsNullOrEmpty(Request.Address) ? ",@Address=" + _DAO.FilterString(Request.Address) : ",@Address=N" + _DAO.FilterString(Request.Address); 
            SQL += string.IsNullOrEmpty(Request.HostIntroduction) ? ",@HostIntroduction=" + _DAO.FilterString(Request.HostIntroduction) : ",@HostIntroduction=N" + _DAO.FilterString(Request.HostIntroduction);  
            Response = _DAO.ParseCommonDbResponse(SQL);

            foreach (var item in Request.HostIdentityDataModel)
            {
                var SQL2 = "EXEC sproc_host_identity_detail_management @Flag = 'mhid'";
                SQL2 += ",@ClubId=" + _DAO.FilterString(Request.AgentId);
                SQL2 += !string.IsNullOrEmpty(Request.HostId) ? ",@HostId=" + _DAO.FilterString(Request.HostId) : ",@HostId=" + _DAO.FilterString(Response.Extra1);
                SQL2 += ",@IdentityType=" + _DAO.FilterString(item.IdentityType);
                SQL2 += ",@IdentityValue=" + _DAO.FilterString(item.IdentityValue);
                SQL2 += !string.IsNullOrEmpty(item.IdentityDDLType) ? ",@IdentityDDLType=" + _DAO.FilterString(item.IdentityDDLType) : null;
                SQL2 += string.IsNullOrEmpty(item.IdentityDescription) ? ",@IdentityDescription=" + _DAO.FilterString(item.IdentityDescription) : ",@IdentityDescription=N" + _DAO.FilterString(item.IdentityDescription); 
                SQL2 += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
                SQL2 += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
                _DAO.ParseCommonDbResponse(SQL2);
            }
            return Response;
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

        #region Host Identity Detail Management 
        public List<HostIdentityDataCommon> GetHostIdentityDetail(string AgentId = "", string HostId = "")
        {
            var Response = new List<HostIdentityDataCommon>();
            string SQL = "EXEC sproc_host_identity_detail_management @Flag = 'ghid'";
            SQL += !string.IsNullOrEmpty(AgentId) ? ",@ClubId=" + _DAO.FilterString(AgentId) : null;
            SQL += !string.IsNullOrEmpty(HostId) ? ",@HostId=" + _DAO.FilterString(HostId) : null;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) Response = _DAO.DataTableToListObject<HostIdentityDataCommon>(dbResponse).ToList();
            return Response;
        }
        public List<StaticDataCommon> GetSkillsDLL()
        {
            string SQL = "EXEC sproc_host_identity_detail_management @Flag = 'gsddl'";
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) return _DAO.DataTableToListObject<StaticDataCommon>(dbResponse).ToList();
            return new List<StaticDataCommon>();
        }
        #endregion
    }
}
