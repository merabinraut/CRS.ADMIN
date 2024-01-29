using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CRS.ADMIN.REPOSITORY.ClubManagement
{
    public class ClubManagementRepository : IClubManagementRepository
    {
        RepositoryDao _DAO;
        public ClubManagementRepository()
        {
            _DAO = new RepositoryDao();
        }

        #region Club Management
        public List<ClubListCommon> GetClubList(PaginationFilterCommon Request)
        {
            var response = new List<ClubListCommon>();
            string SQL = "EXEC sproc_club_management @Flag='gclist'";
            SQL += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _DAO.FilterString(Request.SearchFilter) : null;
            SQL += ",@Skip=" + Request.Skip;
            SQL += ",@Take=" + Request.Take;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new ClubListCommon()
                    {
                        LoginId = _DAO.ParseColumnValue(item, "LoginId").ToString(),
                        AgentId = _DAO.ParseColumnValue(item, "AgentId").ToString(),
                        Status = _DAO.ParseColumnValue(item, "Status").ToString(),
                        ClubNameEng = _DAO.ParseColumnValue(item, "ClubNameEng").ToString(),
                        ClubNameJap = _DAO.ParseColumnValue(item, "ClubNameJap").ToString(),
                        MobileNumber = _DAO.ParseColumnValue(item, "MobileNumber").ToString(),
                        Location = _DAO.ParseColumnValue(item, "Location").ToString(),
                        CreatedDate = _DAO.ParseColumnValue(item, "CreatedDate").ToString(),
                        UpdatedDate = _DAO.ParseColumnValue(item, "UpdatedDate").ToString(),
                        Rank = _DAO.ParseColumnValue(item, "Rank").ToString(),
                        Ratings = _DAO.ParseColumnValue(item, "Ratings").ToString(),
                        ClubLogo = _DAO.ParseColumnValue(item, "ClubLogo").ToString(),
                        ClubCategory = _DAO.ParseColumnValue(item, "ClubCategory").ToString(),
                        TotalRecords = Convert.ToInt32(_DAO.ParseColumnValue(item, "TotalRecords").ToString()),
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "SNO").ToString())
                    });
                }
            }
            return response;
        }

        public ClubDetailCommon GetClubDetails(string AgentId)
        {
            string SQL = "EXEC sproc_club_management @Flag='gcd'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            var dbResponse = _DAO.ExecuteDataRow(SQL);
            if (dbResponse != null)
            {
                return new ClubDetailCommon()
                {
                    AgentId = _DAO.ParseColumnValue(dbResponse, "AgentId").ToString(),
                    UserId = _DAO.ParseColumnValue(dbResponse, "UserId").ToString(),
                    LoginId = _DAO.ParseColumnValue(dbResponse, "LoginId").ToString(),
                    FirstName = _DAO.ParseColumnValue(dbResponse, "FirstName").ToString(),
                    MiddleName = _DAO.ParseColumnValue(dbResponse, "MiddleName").ToString(),
                    LastName = _DAO.ParseColumnValue(dbResponse, "LastName").ToString(),
                    Email = _DAO.ParseColumnValue(dbResponse, "Email").ToString(),
                    MobileNumber = _DAO.ParseColumnValue(dbResponse, "MobileNumber").ToString(),
                    ClubName1 = _DAO.ParseColumnValue(dbResponse, "ClubName1").ToString(),
                    ClubName2 = _DAO.ParseColumnValue(dbResponse, "ClubName2").ToString(),
                    BusinessType = _DAO.ParseColumnValue(dbResponse, "BusinessType").ToString(),
                    GroupName = _DAO.ParseColumnValue(dbResponse, "GroupName").ToString(),
                    Description = _DAO.ParseColumnValue(dbResponse, "Description").ToString(),
                    LocationURL = _DAO.ParseColumnValue(dbResponse, "LocationURL").ToString(),
                    Longitude = _DAO.ParseColumnValue(dbResponse, "Longitude").ToString(),
                    Latitude = _DAO.ParseColumnValue(dbResponse, "Latitude").ToString(),
                    Status = _DAO.ParseColumnValue(dbResponse, "Status").ToString(),
                    Logo = _DAO.ParseColumnValue(dbResponse, "Logo").ToString(),
                    CoverPhoto = _DAO.ParseColumnValue(dbResponse, "CoverPhoto").ToString(),
                    BusinessCertificate = _DAO.ParseColumnValue(dbResponse, "BusinessCertificate").ToString(),
                    Gallery = _DAO.ParseColumnValue(dbResponse, "Gallery").ToString(),
                    WebsiteLink = _DAO.ParseColumnValue(dbResponse, "WebsiteLink").ToString(),
                    TiktokLink = _DAO.ParseColumnValue(dbResponse, "TiktokLink").ToString(),
                    TwitterLink = _DAO.ParseColumnValue(dbResponse, "TwitterLink").ToString(),
                    InstagramLink = _DAO.ParseColumnValue(dbResponse, "InstagramLink").ToString(),
                    LocationId = _DAO.ParseColumnValue(dbResponse, "LocationId").ToString(),
                    CompanyName = _DAO.ParseColumnValue(dbResponse, "CompanyName").ToString(),
                };
            }
            return new ClubDetailCommon();
        }

        public CommonDbResponse ManageClub(ManageClubCommon Request)
        {
            string SQL = "EXEC sproc_club_management ";
            SQL += string.IsNullOrEmpty(Request.AgentId) ? "@Flag='rc'" : "@Flag='mc'";
            if (string.IsNullOrEmpty(Request.AgentId))
            {
                SQL += ",@LoginId=N" + _DAO.FilterString(Request.LoginId);
                SQL += ",@Email=" + _DAO.FilterString(Request.Email);
                SQL += ",@MobileNumber=" + _DAO.FilterString(Request.MobileNumber);
            }
            else
            {
                SQL += ",@AgentId=" + _DAO.FilterString(Request.AgentId);
            }
            SQL += ",@FirstName=N" + _DAO.FilterString(Request.FirstName);
            SQL += ",@MiddleName=N" + _DAO.FilterString(Request.MiddleName);
            SQL += ",@LastName=N" + _DAO.FilterString(Request.LastName);
            SQL += ",@ClubName1=N" + _DAO.FilterString(Request.ClubName1);
            SQL += ",@ClubName2=N" + _DAO.FilterString(Request.ClubName2);
            SQL += ",@BusinessType=" + _DAO.FilterString(Request.BusinessType);
            SQL += ",@GroupName=N" + _DAO.FilterString(Request.GroupName);
            SQL += ",@Description=N" + _DAO.FilterString(Request.Description);
            SQL += ",@LocationURL=" + _DAO.FilterString(Request.LocationURL);
            SQL += ",@Longitude=" + _DAO.FilterString(Request.Longitude);
            SQL += ",@Latitude=" + _DAO.FilterString(Request.Latitude);
            SQL += ",@Logo=" + _DAO.FilterString(Request.Logo);
            SQL += ",@CoverPhoto=" + _DAO.FilterString(Request.CoverPhoto);
            SQL += ",@BusinessCertificate=" + _DAO.FilterString(Request.BusinessCertificate);
            SQL += ",@Gallery=" + _DAO.FilterString(Request.Gallery);
            SQL += ",@WebsiteLink=" + _DAO.FilterString(Request.WebsiteLink);
            SQL += ",@TiktokLink=" + _DAO.FilterString(Request.TiktokLink);
            SQL += ",@TwitterLink=" + _DAO.FilterString(Request.TwitterLink);
            SQL += ",@InstagramLink=" + _DAO.FilterString(Request.InstagramLink);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            SQL += ",@LocationId=" + _DAO.FilterString(Request.LocationId);
            SQL += ",@CompanyName=N" + _DAO.FilterString(Request.CompanyName);
            return _DAO.ParseCommonDbResponse(SQL);
        }

        public CommonDbResponse ManageClubStatus(string AgentId, string Status, Common Request)
        {
            string SQL = "EXEC sproc_club_management @Flag='ucs'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += ",@Status=" + _DAO.FilterString(Status);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@ActionPlatform =" + _DAO.FilterString(Request.ActionPlatform);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        #endregion
        #region Club User Management
        public CommonDbResponse ResetClubUserPassword(string AgentId, string UserId, Common Request)
        {
            string SQL = "EXEC sproc_club_management @Flag='rcup'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            //SQL += ",@UserId=" + _DAO.FilterString(UserId);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        #endregion

        #region "Manage Tag"
        public CommonDbResponse ManageTag(ManageTagCommon Request)
        {
            string sp_name = "sproc_tag_management ";
            sp_name += string.IsNullOrEmpty(Request.TagId) ? "@Flag='it'" : "@Flag='ut'";
            sp_name += ",@ClubId=" + _DAO.FilterString(Request.ClubId);
            sp_name += ",@TagId=" + _DAO.FilterString(Request.TagId);
            sp_name += ",@Tag1Location=" + _DAO.FilterString(Request.Tag1Location);
            sp_name += ",@Tag1Status=" + _DAO.FilterString(Request.Tag1Status);
            sp_name += ",@Tag2RankName=" + _DAO.FilterString(Request.Tag2RankName);
            sp_name += ",@Tag2RankDescription=" + _DAO.FilterString(Request.Tag2RankDescription);
            sp_name += ",@Tag2Status=" + _DAO.FilterString(Request.Tag2Status);
            sp_name += ",@Tag3CategoryName=" + _DAO.FilterString(Request.Tag3CategoryName);
            sp_name += ",@Tag3CategoryDescription=" + _DAO.FilterString(Request.Tag3CategoryDescription);
            sp_name += ",@Tag3Status=" + _DAO.FilterString(Request.Tag3Status);
            sp_name += ",@Tag4ExcellencyName=" + _DAO.FilterString(Request.Tag4ExcellencyName);
            sp_name += ",@Tag4ExcellencyDescription=" + _DAO.FilterString(Request.Tag4ExcellencyDescription);
            sp_name += ",@Tag4Status=" + _DAO.FilterString(Request.Tag4Status);
            sp_name += ",@Tag5StoreName=" + _DAO.FilterString(Request.Tag5StoreName);
            sp_name += ",@Tag5StoreDescription=" + _DAO.FilterString(Request.Tag5StoreDescription);
            sp_name += ",@Tag5Status=" + _DAO.FilterString(Request.Tag5Status);
            sp_name += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            sp_name += ",@ActionIp=" + _DAO.FilterString(Request.ActionIP);

            return _DAO.ParseCommonDbResponse(sp_name);

        }
        public List<LocationListCommon> GetLocationDDL(string clubID)
        {
            List<LocationListCommon> response = new List<LocationListCommon>();
            string sp_name = "sproc_tag_management @Flag='glddl'";
            sp_name += ",ClubId" + _DAO.FilterString(clubID);
            var dbResponseInfo = _DAO.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow item in dbResponseInfo.Rows)
                {
                    response.Add(new LocationListCommon
                    {

                        LocationID = item["Location"].ToString(),
                        LocationName = item["LocationName"].ToString()
                    });

                }
            }
            return response;
        }
        public ManageTagCommon GetTagDetails(string clubid)
        {
            string sp_name = "sproc_tag_management @Flag = 'gtd'";
            sp_name += ",@ClubId=" + _DAO.FilterString(clubid);
            var responseInfo = _DAO.ExecuteDataRow(sp_name);
            if ((responseInfo != null))
            {
                var code = _DAO.ParseColumnValue(responseInfo, "Code").ToString();
                var message = _DAO.ParseColumnValue(responseInfo, "Message").ToString();
                if (!string.IsNullOrEmpty(code) && code == "0")
                {
                    return new ManageTagCommon()
                    {
                        Code = code,
                        Message = message,
                        TagId = _DAO.ParseColumnValue(responseInfo, "TagId").ToString(),
                        Tag1Location = _DAO.ParseColumnValue(responseInfo, "Tag1Location").ToString(),
                        Tag1Status = _DAO.ParseColumnValue(responseInfo, "Tag1Status").ToString(),
                        Tag2RankName = _DAO.ParseColumnValue(responseInfo, "Tag2RankName").ToString(),
                        Tag2RankDescription = _DAO.ParseColumnValue(responseInfo, "Tag2RankDescription").ToString(),
                        Tag2Status = _DAO.ParseColumnValue(responseInfo, "Tag2Status").ToString(),
                        Tag3CategoryName = _DAO.ParseColumnValue(responseInfo, "Tag3CategoryName").ToString(),
                        Tag3CategoryDescription = _DAO.ParseColumnValue(responseInfo, "Tag3CategoryDescription").ToString(),
                        Tag3Status = _DAO.ParseColumnValue(responseInfo, "Tag3Status").ToString(),
                        Tag4ExcellencyName = _DAO.ParseColumnValue(responseInfo, "Tag4ExcellencyName").ToString(),
                        Tag4ExcellencyDescription = _DAO.ParseColumnValue(responseInfo, "Tag4ExcellencyDescription").ToString(),
                        Tag4Status = _DAO.ParseColumnValue(responseInfo, "Tag4Status").ToString(),
                        Tag5StoreName = _DAO.ParseColumnValue(responseInfo, "Tag5StoreName").ToString(),
                        Tag5StoreDescription = _DAO.ParseColumnValue(responseInfo, "Tag5StoreDescription").ToString(),
                        Tag5Status = _DAO.ParseColumnValue(responseInfo, "Tag5Status").ToString()


                    };
                }
                return new ManageTagCommon()
                {
                    Code = code,
                    Message = message
                };
            }
            return new ManageTagCommon();
        }
        #endregion

        #region Manage gallery
        public List<GalleryManagementCommon> GetGalleryImage(string AgentId, PaginationFilterCommon request, string GalleryId = "")
        {
            string SQL = "EXEC dbo.sproc_club_gallery_management @Flag='gcgl'";
            SQL += ", @AgentId =" + _DAO.FilterString(AgentId);
            SQL += !string.IsNullOrEmpty(GalleryId) ? ", @GalleryId =" + _DAO.FilterString(GalleryId) : "";
            SQL += !string.IsNullOrEmpty(request.SearchFilter) ? ", @SearchFilter =N" + _DAO.FilterString(request.SearchFilter) : "";
            SQL += ",@Skip=" + request.Skip;
            SQL += ",@Take=" + request.Take;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) return _DAO.DataTableToListObject<GalleryManagementCommon>(dbResponse).ToList();
            return new List<GalleryManagementCommon>();
        }

        public CommonDbResponse ManageGalleryImage(ManageGalleryImageCommon Request)
        {
            string SQL = "EXEC dbo.sproc_club_gallery_management ";
            SQL += !string.IsNullOrEmpty(Request.GalleryId) ? "@Flag='mcgi'" : "@Flag='icgi'";
            SQL += !string.IsNullOrEmpty(Request.GalleryId) ? ", @GalleryId =" + _DAO.FilterString(Request.GalleryId) : "";
            SQL += ",@AgentId=" + _DAO.FilterString(Request.AgentId);
            SQL += ",@ImageTitle=N" + _DAO.FilterString(Request.ImageTitle);
            SQL += ",@ImagePath=" + _DAO.FilterString(Request.ImagePath);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }

        public CommonDbResponse ManageGalleryImageStatus(string AgentId, string GalleryId, Common Request)
        {
            string SQL = "EXEC dbo.sproc_club_gallery_management @Flag='mcgis'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += ",@GalleryId=" + _DAO.FilterString(GalleryId);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        #endregion
    }
}
