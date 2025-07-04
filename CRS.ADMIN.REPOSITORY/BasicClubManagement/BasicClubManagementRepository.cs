using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.BasicClubManagement;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CRS.ADMIN.REPOSITORY.BasicClubManagement
{
    public class BasicClubManagementRepository : IBasicClubManagementRepository
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
            if (!string.IsNullOrEmpty(Request.AgentId))
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
            SQL += ",@OtherHoliday=" + _DAO.FilterString(Request.OthersHoliday);
            SQL += ",@lastOrderTime=" + _DAO.FilterString(Request.LastOrderTime);
            SQL += ",@lastEntryTime=" + _DAO.FilterString(Request.LastEntryTime);
            SQL += ",@postalcode=" + _DAO.FilterString(Request.PostalCode);
            SQL += ",@prefecture=" + _DAO.FilterString(Request.LocationId);
            SQL += ",@city=N" + _DAO.FilterString(Request.City);
            SQL += ",@closingDate=" + _DAO.FilterString(Request.ClosingDate);
            SQL += ",@street=N" + _DAO.FilterString(Request.Street);
            SQL += ",@roomNumber=" + (!string.IsNullOrEmpty(Request.BuildingRoomNo) ? "N" + _DAO.FilterString(Request.BuildingRoomNo) : _DAO.FilterString(Request.BuildingRoomNo));
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
                    OthersHoliday = _DAO.ParseColumnValue(dbResponse, "OtherHoliday").ToString(),
                    //ClosingDate = _DAO.ParseColumnValue(dbResponse, "ClosingDate").ToString(),
                    GoogleMap = _DAO.ParseColumnValue(dbResponse, "LocationURL").ToString(),

                };
            }
            return new ManageBasicClubCommon();
        }
        public CommonDbResponse DeleteBasicClubStatus(string AgentId, Common Request)
        {
            string SQL = "EXEC sproc_delete_club_basic_details ";
            SQL += " @agentId=" + _DAO.FilterString(AgentId);
            SQL += ",@actionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@actionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@actionPlatform =" + _DAO.FilterString(Request.ActionPlatform);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        public CommonDbResponse BlockBasicClubStatus(string AgentId, Common Request)
        {
            string SQL = "EXEC sproc_block_club_basic_details ";
            SQL += " @agentId=" + _DAO.FilterString(AgentId);
            SQL += ",@actionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@actionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@actionPlatform =" + _DAO.FilterString(Request.ActionPlatform);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        public CommonDbResponse UnBlockBasicClubStatus(string AgentId, Common Request)
        {
            string SQL = "EXEC sproc_unblock_club_basic_details ";
            SQL += " @agentId=" + _DAO.FilterString(AgentId);
            SQL += ",@actionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@actionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@actionPlatform =" + _DAO.FilterString(Request.ActionPlatform);
            return _DAO.ParseCommonDbResponse(SQL);
        }

        public ClubDetailCommon GetBasicConversionClubDetails(string AgentId, String culture = "")
        {
            var plan = new List<planIdentityDataCommon>();
            var PlanListCommon = new List<PlanListCommon>();
            ClubDetailCommon ClubDetail = new ClubDetailCommon();
            string SQL = "EXEC sproc_club_management_approvalrejection @Flag='gcd' ";
            SQL += " ,@AgentId=" + _DAO.FilterString(AgentId);
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
                    LandLineNumber = _DAO.ParseColumnValue(dbResponse, "LandLineNumber").ToString(),
                    //LandLineCode = _DAO.ParseColumnValue(dbResponse, "LandLineCode").ToString(),
                    Line = _DAO.ParseColumnValue(dbResponse, "Line").ToString(),
                    ceoFullName = _DAO.ParseColumnValue(dbResponse, "ceoFullName").ToString(),
                    WorkingHrTo = _DAO.ParseColumnValue(dbResponse, "ClubClosingTime").ToString(),
                    WorkingHrFrom = _DAO.ParseColumnValue(dbResponse, "ClubOpeningTime").ToString(),
                    PostalCode = _DAO.ParseColumnValue(dbResponse, "InputZip").ToString(),
                    Drink = _DAO.ParseColumnValue(dbResponse, "VariousDrinksFee").ToString(),
                    ExtensionFee = _DAO.ParseColumnValue(dbResponse, "OnSiteNominationFee").ToString(),
                    CompanionFee = _DAO.ParseColumnValue(dbResponse, "AccompanyingFee").ToString(),
                    DesignationFee = _DAO.ParseColumnValue(dbResponse, "NominationFee").ToString(),
                    RegularFee = _DAO.ParseColumnValue(dbResponse, "RegularPrice").ToString(),
                    BuildingRoomNo = _DAO.ParseColumnValue(dbResponse, "InputHouseNo").ToString(),
                    Street = _DAO.ParseColumnValue(dbResponse, "InputStreet").ToString(),
                    City = _DAO.ParseColumnValue(dbResponse, "InputCity").ToString(),
                    Prefecture = _DAO.ParseColumnValue(dbResponse, "InputPrefecture").ToString(),
                    Tax = _DAO.ParseColumnValue(dbResponse, "Tax").ToString(),
                    LastEntryTime = _DAO.ParseColumnValue(dbResponse, "LastEntrySyokai").ToString(),
                    LastOrderTime = _DAO.ParseColumnValue(dbResponse, "LastOrderTime").ToString(),
                    Holiday = _DAO.ParseColumnValue(dbResponse, "Holiday").ToString(),
                    OthersHoliday = _DAO.ParseColumnValue(dbResponse, "OtherHoliday").ToString(),
                    GroupName2 = _DAO.ParseColumnValue(dbResponse, "GroupNamekatakana").ToString(),
                    CompanyAddress = _DAO.ParseColumnValue(dbResponse, "CompanyAddress").ToString(),
                    BusinessLicenseNumber = _DAO.ParseColumnValue(dbResponse, "BusinessLicenseNumber").ToString(),
                    LicenseIssuedDate = !string.IsNullOrEmpty(_DAO.ParseColumnValue(dbResponse, "LicenseIssuedDate").ToString()) ? Convert.ToDateTime(_DAO.ParseColumnValue(dbResponse, "LicenseIssuedDate")).ToString("yyyy/MM/dd") : _DAO.ParseColumnValue(dbResponse, "LicenseIssuedDate").ToString(),
                    //ClosingDate = _DAO.ParseColumnValue(dbResponse, "ClosingDate").ToString(),
                    Representative1_ContactName = _DAO.ParseColumnValue(dbResponse, "Representative1_ContactName").ToString(),
                    Representative1_Email = _DAO.ParseColumnValue(dbResponse, "Representative1_Email").ToString(),
                    Representative1_MobileNo = _DAO.ParseColumnValue(dbResponse, "Representative1_MobileNo").ToString(),
                    Representative2_ContactName = _DAO.ParseColumnValue(dbResponse, "Representative2_ContactName").ToString(),
                    Representative2_Email = _DAO.ParseColumnValue(dbResponse, "Representative2_Email").ToString(),
                    Representative2_MobileNo = _DAO.ParseColumnValue(dbResponse, "Representative2_MobileNo").ToString(),
                    GoogleMap = _DAO.ParseColumnValue(dbResponse, "LocationURL").ToString(),
                    Representative1_Furigana = _DAO.ParseColumnValue(dbResponse, "Representative1_Furigana").ToString(),
                    Representative2_Furigana = _DAO.ParseColumnValue(dbResponse, "Representative2_Furigana").ToString(),
                    ClubName = _DAO.ParseColumnValue(dbResponse, "English").ToString(),
                    CompanyNameFurigana = _DAO.ParseColumnValue(dbResponse, "CompanyNameKatakana").ToString(),
                    CeoFurigana = _DAO.ParseColumnValue(dbResponse, "CeoNameKatakana").ToString(),
                    CorporateRegistryDocument = _DAO.ParseColumnValue(dbResponse, "CompanyRegistry").ToString(),
                    PassportPhoto = (_DAO.ParseColumnValue(dbResponse, "DocumentType").ToString() == "2")
                                         ? _DAO.ParseColumnValue(dbResponse, "KYCDocument").ToString()
                                         : null,
                    InsurancePhoto = (_DAO.ParseColumnValue(dbResponse, "DocumentType").ToString() == "2")
                                         ? _DAO.ParseColumnValue(dbResponse, "KYCDocumentBack").ToString()
                                         : null,

                    KYCDocument = (_DAO.ParseColumnValue(dbResponse, "DocumentType").ToString() == "2")
                                         ? null
                                         : _DAO.ParseColumnValue(dbResponse, "KYCDocument").ToString(),
                    KYCDocumentBack = (_DAO.ParseColumnValue(dbResponse, "DocumentType").ToString() == "2")
                                         ? null
                                         : _DAO.ParseColumnValue(dbResponse, "KYCDocumentBack").ToString(),
                    IdentificationType = _DAO.ParseColumnValue(dbResponse, "DocumentType").ToString()

                };
            }
            return new ClubDetailCommon();
        }

        public CommonDbResponse ManageConversionClub(ManageClubCommon Request, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            var Response = new CommonDbResponse();
            string SQL = "EXEC sproc_switch_club_basic_to_premium_details ";
            SQL += " @email=" + _DAO.FilterString(Request.Email);
            SQL += ",@mobileNumber=" + _DAO.FilterString(Request.MobileNumber);
            SQL += ",@agentId=" + _DAO.FilterString(Request.AgentId);
            SQL += ",@LandLineNumber=" + _DAO.FilterString(Request.LandLineNumber);
            SQL += ",@ClubName=" + _DAO.FilterString(Request.ClubName);
            SQL += ",@ClubName2=" + (!string.IsNullOrEmpty(Request.ClubName2) ? "N" + _DAO.FilterString(Request.ClubName2) : _DAO.FilterString(Request.ClubName2));
            SQL += ",@ClubName1=" + (!string.IsNullOrEmpty(Request.ClubName1) ? "N" + _DAO.FilterString(Request.ClubName1) : _DAO.FilterString(Request.ClubName1));
            SQL += ",@businessType=" + _DAO.FilterString(Request.BusinessType);
            SQL += ",@groupName=" + (!string.IsNullOrEmpty(Request.GroupName) ? "N" + _DAO.FilterString(Request.GroupName) : _DAO.FilterString(Request.GroupName));
            SQL += ",@description=" + (!string.IsNullOrEmpty(Request.Description) ? "N" + _DAO.FilterString(Request.Description) : _DAO.FilterString(Request.Description));
            SQL += ",@locationUrl=" + _DAO.FilterString(Request.GoogleMap);
            SQL += ",@longitude=" + _DAO.FilterString(Request.Longitude);
            SQL += ",@latitude=" + _DAO.FilterString(Request.Latitude);
            SQL += ",@logo=" + _DAO.FilterString(Request.Logo);
            SQL += ",@coverPhoto=" + _DAO.FilterString(Request.CoverPhoto);
            SQL += ",@businessCertificate=" + _DAO.FilterString(Request.BusinessCertificate);
            SQL += ",@gallery=" + _DAO.FilterString(Request.Gallery);
            SQL += ",@webSite=" + _DAO.FilterString(Request.WebsiteLink);
            SQL += ",@tiktok=" + _DAO.FilterString(Request.TiktokLink);
            SQL += ",@twitter=" + _DAO.FilterString(Request.TwitterLink);
            SQL += ",@instagram=" + _DAO.FilterString(Request.InstagramLink);
            SQL += ",@actionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@actionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@actionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            SQL += ",@locationId=" + _DAO.FilterString(Request.LocationId);
            SQL += ",@ceoFullName=N" + _DAO.FilterString(Request.ceoFullName);
            SQL += ",@line=" + _DAO.FilterString(Request.Line);
            SQL += ",@clubOpeningTime=" + _DAO.FilterString(Request.WorkingHrFrom);
            SQL += ",@clubClosingTime=" + _DAO.FilterString(Request.WorkingHrTo);
            SQL += ",@holiday=" + _DAO.FilterString(Request.Holiday);
            SQL += ",@lastOrderTime=" + _DAO.FilterString(Request.LastOrderTime);
            SQL += ",@lastEntryTime=" + _DAO.FilterString(Request.LastEntryTime);
            SQL += ",@tax=" + _DAO.FilterString(Request.Tax);
            SQL += ",@postalCode=" + _DAO.FilterString(Request.PostalCode);
            SQL += ",@prefecture=" + _DAO.FilterString(Request.Prefecture);
            SQL += ",@city=N" + _DAO.FilterString(Request.City);
            SQL += ",@inputStreet=N" + _DAO.FilterString(Request.Street);
            SQL += ",@buildingRoomNo=" + (!string.IsNullOrEmpty(Request.BuildingRoomNo) ? "N" + _DAO.FilterString(Request.BuildingRoomNo) : _DAO.FilterString(Request.BuildingRoomNo));
            SQL += ",@regularFee=" + _DAO.FilterString(Request.RegularFee);
            SQL += ",@DesignationFee=" + _DAO.FilterString(Request.DesignationFee);
            SQL += ",@accompanyingFee=" + _DAO.FilterString(Request.CompanionFee);
            SQL += ",@extensionFee=" + _DAO.FilterString(Request.ExtensionFee);
            SQL += ",@variousDrinksFee=" + (!string.IsNullOrEmpty(Request.Drink) ? "N" + _DAO.FilterString(Request.Drink) : _DAO.FilterString(Request.Drink));
            SQL += ",@groupNameKatakana=" + (!string.IsNullOrEmpty(Request.GroupName2) ? "N" + _DAO.FilterString(Request.GroupName2) : _DAO.FilterString(Request.GroupName2));
            SQL += ",@companyAddress=" + (!string.IsNullOrEmpty(Request.CompanyAddress) ? "N" + _DAO.FilterString(Request.CompanyAddress) : _DAO.FilterString(Request.CompanyAddress));
            SQL += ",@businessLicenseNumber=" + _DAO.FilterString(Request.BusinessLicenseNumber);
            SQL += ",@licenseIssuedDate=" + _DAO.FilterString(Request.LicenseIssuedDate);
            //SQL += ",@closingDate=" + _DAO.FilterString(Request.ClosingDate);
            SQL += ",@ceoNameKatakana=" + (!string.IsNullOrEmpty(Request.CeoFurigana) ? "N" + _DAO.FilterString(Request.CeoFurigana) : _DAO.FilterString(Request.CeoFurigana));
            SQL += ",@companyRegistry=" + _DAO.FilterString(Request.CorporateRegistryDocument);
            SQL += ",@documentType=" + _DAO.FilterString(Request.IdentificationType);
            SQL += ",@OtherHoliday=" + _DAO.FilterString(Request.OthersHoliday);
            if (Request.IdentificationType == "2")
            {
                SQL += ",@kycDocument=" + _DAO.FilterString(Request.PassportPhoto);
                SQL += ",@KYCDocumentBack=" + _DAO.FilterString(Request.InsurancePhoto);
            }
            else
            {
                SQL += ",@kycDocument=" + _DAO.FilterString(Request.KYCDocument);
                SQL += ",@kycDocumentBack=" + _DAO.FilterString(Request.KYCDocumentBack);
            }
            if (Request.BusinessType == "1")
            {
                SQL += ",@companyName=N" + _DAO.FilterString(Request.CompanyName);
                SQL += ",@companyNameKatakana=" + (!string.IsNullOrEmpty(Request.CompanyNameFurigana) ? "N" + _DAO.FilterString(Request.CompanyNameFurigana) : _DAO.FilterString(Request.CompanyNameFurigana));
                SQL += ",@representative1_ContactName=" + (!string.IsNullOrEmpty(Request.Representative1_ContactName) ? "N" + _DAO.FilterString(Request.Representative1_ContactName) : _DAO.FilterString(Request.Representative1_ContactName));
                SQL += ",@representative1_MobileNo=" + _DAO.FilterString(Request.Representative1_MobileNo);
                SQL += ",@representative1_Email=" + _DAO.FilterString(Request.Representative1_Email);
                SQL += ",@representative1_Furigana=" + (!string.IsNullOrEmpty(Request.Representative1_Furigana) ? "N" + _DAO.FilterString(Request.Representative1_Furigana) : _DAO.FilterString(Request.Representative1_Furigana));
                SQL += ",@representative2_ContactName=" + (!string.IsNullOrEmpty(Request.Representative2_ContactName) ? "N" + _DAO.FilterString(Request.Representative2_ContactName) : _DAO.FilterString(Request.Representative2_ContactName));
                SQL += ",@representative2_MobileNo=" + (!string.IsNullOrEmpty(Request.Representative2_MobileNo) ? "N" + _DAO.FilterString(Request.Representative2_MobileNo) : _DAO.FilterString(Request.Representative2_MobileNo));
                SQL += ",@representative2_Email=" + (!string.IsNullOrEmpty(Request.Representative2_Email) ? "N" + _DAO.FilterString(Request.Representative2_Email) : _DAO.FilterString(Request.Representative2_Email));
                SQL += ",@representative2_Furigana=" + (!string.IsNullOrEmpty(Request.Representative2_Furigana) ? "N" + _DAO.FilterString(Request.Representative2_Furigana) : _DAO.FilterString(Request.Representative2_Furigana));
            }
            var _sqlTransactionHandler = new RepositoryDaoWithTransaction(connection, transaction);
            Response = _sqlTransactionHandler.ParseCommonDbResponse(SQL);
            return Response;
        }


    }
}
