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
            string SQL = "EXEC sproc_get_club_basic_details";
            SQL += " @Skip=" + Request.Skip;
            SQL += ",@Take=" + Request.Take;
            SQL += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _DAO.FilterString(Request.SearchFilter) : null;
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
                        MobileNumber = _DAO.ParseColumnValue(item, "LandLineNumber").ToString(),
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
            
            string SQL = "";
            if (!string.IsNullOrEmpty(Request.AgentId) )
            {
                 SQL = "EXEC sproc_update_club_basic_details ";
               
            }
            else
            {
                 SQL = "EXEC sproc_manage_club_basic_details ";
            }
            SQL += " @AgentId=" + _DAO.FilterString(Request.AgentId);
            SQL += ", @landLine=" + _DAO.FilterString(Request.LandlineNumber);
            SQL += ",@furigana=" + (!string.IsNullOrEmpty(Request.ClubName2) ? "N" + _DAO.FilterString(Request.ClubName2) : _DAO.FilterString(Request.ClubName2));
            SQL += ",@storeName=" + (!string.IsNullOrEmpty(Request.ClubName1) ? "N" + _DAO.FilterString(Request.ClubName1) : _DAO.FilterString(Request.ClubName1));
            SQL += ",@GroupName=" + (!string.IsNullOrEmpty(Request.GroupName) ? "N" + _DAO.FilterString(Request.GroupName) : _DAO.FilterString(Request.GroupName));
            SQL += ",@googleMap=" + _DAO.FilterString(Request.GoogleMap);
            SQL += ",@longitude=" + _DAO.FilterString(Request.Longitude);
            SQL += ",@latitude=" + _DAO.FilterString(Request.Latitude);
            SQL += ",@logoImage=" + _DAO.FilterString(Request.Logo);
            SQL += ",@coverImage=" + _DAO.FilterString(Request.CoverPhoto);
            SQL += ",@webSite=" + _DAO.FilterString(Request.WebsiteLink);
            SQL += ",@tiktok=" + _DAO.FilterString(Request.TiktokLink);
            SQL += ",@twitter=" + _DAO.FilterString(Request.TwitterLink);
            SQL += ",@instagram=" + _DAO.FilterString(Request.InstagramLink);
            SQL += ",@actionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@actionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@actionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            SQL += ",@line=" + _DAO.FilterString(Request.Line);
            SQL += ",@workingHourFrom=" + _DAO.FilterString(Request.WorkingHrFrom);
            SQL += ",@workingHoursTo=" + _DAO.FilterString(Request.WorkingHrTo);
            SQL += ",@holiday=" + _DAO.FilterString(Request.Holiday);
            SQL += ",@lastOrderTime=" + _DAO.FilterString(Request.LastOrderTime);
            SQL += ",@lastEntryTime=" + _DAO.FilterString(Request.LastEntryTime);
            SQL += ",@postalcode=" + _DAO.FilterString(Request.PostalCode);
            SQL += ",@prefecture=" + _DAO.FilterString(Request.LocationId);
            SQL += ",@city=N" + _DAO.FilterString(Request.City);
            SQL += ",@closingDate=" + _DAO.FilterString(Request.ClosingDate);
            SQL += ",@street=N" + _DAO.FilterString(Request.Street);
            SQL += ",@roomNumber=N" + _DAO.FilterString(Request.BuildingRoomNo);
           
            Response = _DAO.ParseCommonDbResponse(SQL);
            return Response;
        }

        public ManageBasicClubCommon GetBasicClubDetails(string AgentId, String culture = "")
        {

            ManageBasicClubCommon ClubDetail = new ManageBasicClubCommon();
            string SQL = "EXEC sproc_get_club_basic_edit_details ";
            SQL += " @AgentId=" + _DAO.FilterString(AgentId);
            var dbResponse = _DAO.ExecuteDataRow(SQL);
            if (dbResponse != null)
            {
                return new ManageBasicClubCommon()
                {
                    AgentId = _DAO.ParseColumnValue(dbResponse, "AgentId").ToString(),                    
                    //LoginId = _DAO.ParseColumnValue(dbResponse, "LoginId").ToString(),                    
                    //Email = _DAO.ParseColumnValue(dbResponse, "Email").ToString(),
                    ClubName1 = _DAO.ParseColumnValue(dbResponse, "ClubName1").ToString(),
                    ClubName2 = _DAO.ParseColumnValue(dbResponse, "ClubName2").ToString(),              
                    GroupName = _DAO.ParseColumnValue(dbResponse, "GroupName").ToString(),
                    Longitude = _DAO.ParseColumnValue(dbResponse, "Longitude").ToString(),
                    Latitude = _DAO.ParseColumnValue(dbResponse, "Latitude").ToString(),
                    Status = _DAO.ParseColumnValue(dbResponse, "Status").ToString(),
                    Logo = _DAO.ParseColumnValue(dbResponse, "Logo").ToString(),
                    CoverPhoto = _DAO.ParseColumnValue(dbResponse, "CoverPhoto").ToString(),                   
                    WebsiteLink = _DAO.ParseColumnValue(dbResponse, "WebsiteLink").ToString(),
                    TiktokLink = _DAO.ParseColumnValue(dbResponse, "TiktokLink").ToString(),
                    TwitterLink = _DAO.ParseColumnValue(dbResponse, "TwitterLink").ToString(),
                    InstagramLink = _DAO.ParseColumnValue(dbResponse, "InstagramLink").ToString(),
                    LocationId = _DAO.ParseColumnValue(dbResponse, "InputPrefecture").ToString(),                  
                    LandlineNumber = _DAO.ParseColumnValue(dbResponse, "LandLineNumber").ToString(),
                    Line = _DAO.ParseColumnValue(dbResponse, "Line").ToString(),
                    WorkingHrTo = _DAO.ParseColumnValue(dbResponse, "ClubClosingTime").ToString(),
                    WorkingHrFrom = _DAO.ParseColumnValue(dbResponse, "ClubOpeningTime").ToString(),
                    PostalCode = _DAO.ParseColumnValue(dbResponse, "InputZip").ToString(),                  
                    BuildingRoomNo = _DAO.ParseColumnValue(dbResponse, "InputHouseNo").ToString(),
                    Street = _DAO.ParseColumnValue(dbResponse, "InputStreet").ToString(),
                    City = _DAO.ParseColumnValue(dbResponse, "InputCity").ToString(),
                                       
                    LastEntryTime = _DAO.ParseColumnValue(dbResponse, "LastEntrySyokai").ToString(),
                    LastOrderTime = _DAO.ParseColumnValue(dbResponse, "LastOrderTime").ToString(),
                    Holiday = _DAO.ParseColumnValue(dbResponse, "Holiday").ToString(),
                    ClosingDate = _DAO.ParseColumnValue(dbResponse, "ClosingDate").ToString(),                   
                    GoogleMap = _DAO.ParseColumnValue(dbResponse, "LocationURL").ToString(),                    
                                              
                };
            }
            return new ManageBasicClubCommon();
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
