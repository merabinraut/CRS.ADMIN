using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.BasicClubManagement;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.BasicClubManagement
{
    public class BasicClubManagementRepository: IBasicClubManagementRepository
    {
        RepositoryDao _DAO;
        public BasicClubManagementRepository()
        {
            _DAO = new RepositoryDao();
        }
        public List<BasicClubManagementCommon> GetBasicClubList(PaginationFilterCommon Request)
        {
            var response = new List<BasicClubManagementCommon>();
            string SQL = "EXEC sproc_club_management_approvalrejection @Flag='gclist'";
            SQL += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _DAO.FilterString(Request.SearchFilter) : null;
            SQL += ",@Skip=" + Request.Skip;
            SQL += ",@Take=" + Request.Take;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new BasicClubManagementCommon()
                    {
                        AgentId = _DAO.ParseColumnValue(item, "AgentId").ToString(),
                        Status = _DAO.ParseColumnValue(item, "Status").ToString(),
                        ClubNameEng = _DAO.ParseColumnValue(item, "ClubNameEng").ToString(),
                        ClubNameJap = _DAO.ParseColumnValue(item, "ClubNameJap").ToString(),
                        MobileNumber = _DAO.ParseColumnValue(item, "MobileNumber").ToString(),
                        Location = _DAO.ParseColumnValue(item, "Location").ToString(),
                        CreatedDate = _DAO.ParseColumnValue(item, "CreatedDate").ToString(),
                        UpdatedDate = _DAO.ParseColumnValue(item, "UpdatedDate").ToString(),                       
                        ClubLogo = _DAO.ParseColumnValue(item, "ClubLogo").ToString(),                     
                        TotalRecords = Convert.ToInt32(_DAO.ParseColumnValue(item, "TotalRecords").ToString()),
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "SNO").ToString())
                    });
                }
            }
            return response;
        }

        public CommonDbResponse ManageBasicClub(ManageBasicClubCommon Request)
        {
            var Response = new CommonDbResponse();
            string SQL = "EXEC sproc_club_management_approvalrejection ";
            if (!string.IsNullOrEmpty(Request.AgentId))
            {

                SQL += "@Flag='r_cmth'";
            }
            else
            {
                SQL += "@Flag='r_cmth'";
            }
            
            SQL += ",@AgentId=" + _DAO.FilterString(Request.AgentId);
            SQL += ",@LandLineNumber=" + _DAO.FilterString(Request.LandlineNumber);
            SQL += ",@ClubName=" + _DAO.FilterString(Request.ClubName);
            SQL += ",@ClubName2=" + (!string.IsNullOrEmpty(Request.ClubName2) ? "N" + _DAO.FilterString(Request.ClubName2) : _DAO.FilterString(Request.ClubName2));
            SQL += ",@ClubName1=" + (!string.IsNullOrEmpty(Request.ClubName1) ? "N" + _DAO.FilterString(Request.ClubName1) : _DAO.FilterString(Request.ClubName1));
            SQL += ",@GroupName=" + (!string.IsNullOrEmpty(Request.GroupName) ? "N" + _DAO.FilterString(Request.GroupName) : _DAO.FilterString(Request.GroupName));
            SQL += ",@LocationURL=" + _DAO.FilterString(Request.GoogleMap);
            SQL += ",@Longitude=" + _DAO.FilterString(Request.Longitude);
            SQL += ",@Latitude=" + _DAO.FilterString(Request.Latitude);
            SQL += ",@Logo=" + _DAO.FilterString(Request.Logo);
            SQL += ",@CoverPhoto=" + _DAO.FilterString(Request.CoverPhoto);
            SQL += ",@WebsiteLink=" + _DAO.FilterString(Request.WebsiteLink);
            SQL += ",@TiktokLink=" + _DAO.FilterString(Request.TiktokLink);
            SQL += ",@TwitterLink=" + _DAO.FilterString(Request.TwitterLink);
            SQL += ",@InstagramLink=" + _DAO.FilterString(Request.InstagramLink);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            SQL += ",@LocationId=" + _DAO.FilterString(Request.LocationId);
            SQL += ",@Line=" + _DAO.FilterString(Request.Line);
            SQL += ",@ClubOpeningTime=" + _DAO.FilterString(Request.WorkingHrFrom);
            SQL += ",@ClubClosingTime=" + _DAO.FilterString(Request.WorkingHrTo);
            SQL += ",@Holiday=" + _DAO.FilterString(Request.Holiday);
            SQL += ",@LastOrderTime=" + _DAO.FilterString(Request.LastOrderTime);
            SQL += ",@LastEntryTime=" + _DAO.FilterString(Request.LastEntryTime);
            SQL += ",@PostalCode=" + _DAO.FilterString(Request.PostalCode);
            SQL += ",@Prefecture=" + _DAO.FilterString(Request.Prefecture);
            SQL += ",@City=N" + _DAO.FilterString(Request.City);
            SQL += ",@InputStreet=N" + _DAO.FilterString(Request.Street);
            SQL += ",@BuildingRoomNo=N" + _DAO.FilterString(Request.BuildingRoomNo);
             
            Response = _DAO.ParseCommonDbResponse(SQL);
            return Response;
        }
        public CommonDbResponse ManageBasicClubStatus(string AgentId, string Status, Common Request)
        {
            string SQL = "EXEC sproc_club_management @Flag='ucs'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += ",@Status=" + _DAO.FilterString(Status);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@ActionPlatform =" + _DAO.FilterString(Request.ActionPlatform);
            return _DAO.ParseCommonDbResponse(SQL);
        }

    }
}
