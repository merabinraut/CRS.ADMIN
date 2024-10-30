﻿using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using DocumentFormat.OpenXml.Office2016.Excel;
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
            string SQL = "EXEC sproc_club_management_approvalrejection @Flag='gclist'";
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
                        Rank = !string.IsNullOrEmpty(_DAO.ParseColumnValue(item, "Rank").ToString()) ? _DAO.ParseColumnValue(item, "Rank").ToString() : "",
                        Ratings = _DAO.ParseColumnValue(item, "Ratings").ToString(),
                        ClubLogo = _DAO.ParseColumnValue(item, "ClubLogo").ToString(),
                        ClubCategory = _DAO.ParseColumnValue(item, "ClubCategory").ToString(),
                        TotalRecords = Convert.ToInt32(_DAO.ParseColumnValue(item, "TotalRecords").ToString()),
                        holdStatus = _DAO.ParseColumnValue(item, "holdStatus").ToString(),
                        //LandLineCode = _DAO.ParseColumnValue(item, "LandLineCode").ToString(),
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "SNO").ToString())
                    });
                }
            }
            return response;
        }

        public List<ClubListCommon> GetClubPendingList(PaginationFilterCommon Request)
        {
            var response = new List<ClubListCommon>();
            string SQL = "EXEC sproc_club_management_approvalrejection @Flag='gcplist'";
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
                        //Status = _DAO.ParseColumnValue(item, "Status").ToString(),
                        ClubNameEng = _DAO.ParseColumnValue(item, "ClubNameEng").ToString(),
                        ClubNameJap = _DAO.ParseColumnValue(item, "ClubNameJap").ToString(),
                        MobileNumber = _DAO.ParseColumnValue(item, "MobileNumber").ToString(),
                        Location = _DAO.ParseColumnValue(item, "LocationName").ToString(),
                        CreatedDate = _DAO.ParseColumnValue(item, "CreatedDate").ToString(),
                        UpdatedDate = _DAO.ParseColumnValue(item, "UpdatedDate").ToString(),
                        ClubLogo = _DAO.ParseColumnValue(item, "ClubLogo").ToString(),
                        ActionPlatform = _DAO.ParseColumnValue(item, "ActionPlatform").ToString(),
                        //LandLineCode = _DAO.ParseColumnValue(item, "LandLineCode").ToString(),
                        TotalRecords = Convert.ToInt32(_DAO.ParseColumnValue(item, "TotalRecords").ToString()),
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "holdId").ToString()),
                        Sno = _DAO.ParseColumnValue(item, "SNO").ToString(),

                        AgentId = _DAO.ParseColumnValue(item, "AgentId").ToString()
                    });
                }
            }
            return response;
        }
        public List<ClubListCommon> GetClubRejectedList(PaginationFilterCommon Request)
        {
            var response = new List<ClubListCommon>();
            string SQL = "EXEC sproc_club_management_approvalrejection @Flag='gcrlist'";
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
                        ClubNameEng = _DAO.ParseColumnValue(item, "ClubNameEng").ToString(),
                        ClubNameJap = _DAO.ParseColumnValue(item, "ClubNameJap").ToString(),
                        MobileNumber = _DAO.ParseColumnValue(item, "MobileNumber").ToString(),
                        Location = _DAO.ParseColumnValue(item, "LocationName").ToString(),
                        CreatedDate = _DAO.ParseColumnValue(item, "CreatedDate").ToString(),
                        UpdatedDate = _DAO.ParseColumnValue(item, "UpdatedDate").ToString(),
                        ClubLogo = _DAO.ParseColumnValue(item, "ClubLogo").ToString(),
                        //LandLineCode = _DAO.ParseColumnValue(item, "LandLineCode").ToString(),
                        TotalRecords = Convert.ToInt32(_DAO.ParseColumnValue(item, "TotalRecords").ToString()),
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "holdId").ToString()),
                        Sno = _DAO.ParseColumnValue(item, "SNO").ToString(),
                    });
                }
            }
            return response;
        }
        public ClubDetailCommon GetClubPendingDetails(string AgentId, String holdId = "", String culture = "")
        {
            var plan = new List<planIdentityDataCommon>();
            var PlanListCommon = new List<PlanListCommon>();
            ClubDetailCommon ClubDetail = new ClubDetailCommon();          

            string SQL = "EXEC sproc_club_management_approvalrejection @Flag='g_chpd'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += ",@holdId=" + _DAO.FilterString(holdId);
            var dbResponse = _DAO.ExecuteDataRow(SQL);
            if (dbResponse != null)
            {
                return new ClubDetailCommon()
                {
                    AgentId = _DAO.ParseColumnValue(dbResponse, "AgentId").ToString(),
                    holdId = _DAO.ParseColumnValue(dbResponse, "Id").ToString(),
                    // UserId = _DAO.ParseColumnValue(dbResponse, "UserId").ToString(),
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
                    GoogleMap = _DAO.ParseColumnValue(dbResponse, "LocationURL").ToString(),
                    GroupName2 = _DAO.ParseColumnValue(dbResponse, "GroupNamekatakana").ToString(),
                    CompanyAddress = _DAO.ParseColumnValue(dbResponse, "CompanyAddress").ToString(),
                    BusinessLicenseNumber = _DAO.ParseColumnValue(dbResponse, "BusinessLicenseNumber").ToString(),
                    LicenseIssuedDate = !string.IsNullOrEmpty(_DAO.ParseColumnValue(dbResponse, "LicenseIssuedDate").ToString()) ? Convert.ToDateTime(_DAO.ParseColumnValue(dbResponse, "LicenseIssuedDate")).ToString("yyyy/MM/dd") : _DAO.ParseColumnValue(dbResponse, "LicenseIssuedDate").ToString(),
                    ClosingDate = _DAO.ParseColumnValue(dbResponse, "ClosingDate").ToString(),                   
                    Representative1_ContactName = _DAO.ParseColumnValue(dbResponse, "Representative1_ContactName").ToString(),
                    Representative1_Email = _DAO.ParseColumnValue(dbResponse, "Representative1_Email").ToString(),
                    Representative1_MobileNo = _DAO.ParseColumnValue(dbResponse, "Representative1_MobileNo").ToString(),
                    Representative2_ContactName = _DAO.ParseColumnValue(dbResponse, "Representative2_ContactName").ToString(),
                    Representative2_Email = _DAO.ParseColumnValue(dbResponse, "Representative2_Email").ToString(),
                    Representative2_MobileNo = _DAO.ParseColumnValue(dbResponse, "Representative2_MobileNo").ToString(),
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
        public ClubDetailCommon GetplanPendingDetails(string AgentId, String holdId = "", String culture = "")
        {
            var plan = new List<planIdentityDataCommon>();
            var PlanListCommon = new List<PlanListCommon>();
            ClubDetailCommon ClubDetail = new ClubDetailCommon();            
            string SQL = "EXEC sproc_club_management_approvalrejection @Flag='g_chpd'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += ",@holdId=" + _DAO.FilterString(holdId);
            var dbResponse = _DAO.ExecuteDataRow(SQL);
            if (dbResponse != null)
            {
                return new ClubDetailCommon()
                {
                    AgentId = _DAO.ParseColumnValue(dbResponse, "AgentId").ToString(),
                    holdId = _DAO.ParseColumnValue(dbResponse, "Id").ToString(),
                    // UserId = _DAO.ParseColumnValue(dbResponse, "UserId").ToString(),
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
                    GoogleMap = _DAO.ParseColumnValue(dbResponse, "LocationURL").ToString(),
                    GroupName2 = _DAO.ParseColumnValue(dbResponse, "GroupNamekatakana").ToString(),
                    CompanyAddress = _DAO.ParseColumnValue(dbResponse, "CompanyAddress").ToString(),
                    BusinessLicenseNumber = _DAO.ParseColumnValue(dbResponse, "BusinessLicenseNumber").ToString(),
                    LicenseIssuedDate = !string.IsNullOrEmpty(_DAO.ParseColumnValue(dbResponse, "LicenseIssuedDate").ToString()) ? Convert.ToDateTime(_DAO.ParseColumnValue(dbResponse, "LicenseIssuedDate")).ToString("yyyy/MM/dd") : _DAO.ParseColumnValue(dbResponse, "LicenseIssuedDate").ToString(),
                    ClosingDate = _DAO.ParseColumnValue(dbResponse, "ClosingDate").ToString(),
                   
                    Representative1_ContactName = _DAO.ParseColumnValue(dbResponse, "Representative1_ContactName").ToString(),
                    Representative1_Email = _DAO.ParseColumnValue(dbResponse, "Representative1_Email").ToString(),
                    Representative1_MobileNo = _DAO.ParseColumnValue(dbResponse, "Representative1_MobileNo").ToString(),
                    Representative1_Furigana = _DAO.ParseColumnValue(dbResponse, "Representative1_Furigana").ToString(),
                    Representative2_ContactName = _DAO.ParseColumnValue(dbResponse, "Representative2_ContactName").ToString(),
                    Representative2_Email = _DAO.ParseColumnValue(dbResponse, "Representative2_Email").ToString(),
                    Representative2_MobileNo = _DAO.ParseColumnValue(dbResponse, "Representative2_MobileNo").ToString(),
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

        public CommonDbResponse ManageClub(ManageClubCommon Request)
        {
            var Response = new CommonDbResponse();
            string SQL = "EXEC sproc_club_management_approvalrejection ";
            if (!string.IsNullOrEmpty(Request.AgentId) && string.IsNullOrEmpty(Request.holdId))
            {

                SQL += "@Flag='r_cmth'";
            }
            else
            {
                SQL += string.IsNullOrEmpty(Request.holdId) ? "@Flag='r_ch'" : "@Flag='m_ch'";
            }
            if (string.IsNullOrEmpty(Request.holdId))
            {
                SQL += ",@LoginId=" + (!string.IsNullOrEmpty(Request.LoginId) ? "N" + _DAO.FilterString(Request.LoginId) : _DAO.FilterString(Request.LoginId));  
                SQL += ",@Email=" + _DAO.FilterString(Request.Email);
                SQL += ",@MobileNumber=" + _DAO.FilterString(Request.MobileNumber);

            }
            else
            {
                SQL += ",@holdId=" + _DAO.FilterString(Request.holdId);
            }
            SQL += ",@AgentId=" + _DAO.FilterString(Request.AgentId);
            //SQL += ",@FirstName=N" +   _DAO.FilterString(Request.FirstName);
            SQL += ",@LandLineNumber=" + _DAO.FilterString(Request.LandLineNumber);
            //SQL += ",@LandLineCode=" + _DAO.FilterString(Request.LandLineCode);
            //SQL += ",@MiddleName=N" + _DAO.FilterString(Request.MiddleName);
            //SQL += ",@LastName=N" + _DAO.FilterString(Request.LastName);
            SQL += ",@ClubName=" + _DAO.FilterString(Request.ClubName);  
            SQL += ",@ClubName2=" + (!string.IsNullOrEmpty(Request.ClubName2) ? "N" + _DAO.FilterString(Request.ClubName2) : _DAO.FilterString(Request.ClubName2)); 
            SQL += ",@ClubName1=" + (!string.IsNullOrEmpty(Request.ClubName1) ? "N" + _DAO.FilterString(Request.ClubName1) : _DAO.FilterString(Request.ClubName1));
            SQL += ",@BusinessType=" + _DAO.FilterString(Request.BusinessType);
            SQL += ",@GroupName=" + (!string.IsNullOrEmpty(Request.GroupName) ? "N" + _DAO.FilterString(Request.GroupName) : _DAO.FilterString(Request.GroupName));
            SQL += ",@Description="  +(!string.IsNullOrEmpty(Request.Description) ? "N" + _DAO.FilterString(Request.Description) : _DAO.FilterString(Request.Description));
            SQL += ",@LocationURL=" + _DAO.FilterString(Request.GoogleMap);
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
            SQL += ",@ceoFullName=N" + _DAO.FilterString(Request.ceoFullName);
            SQL += ",@Line=" + _DAO.FilterString(Request.Line);
            SQL += ",@ClubOpeningTime=" + _DAO.FilterString(Request.WorkingHrFrom);
            SQL += ",@ClubClosingTime=" + _DAO.FilterString(Request.WorkingHrTo);
            SQL += ",@Holiday=" + _DAO.FilterString(Request.Holiday);
            SQL += ",@LastOrderTime=" + _DAO.FilterString(Request.LastOrderTime);
            SQL += ",@LastEntryTime=" + _DAO.FilterString(Request.LastEntryTime);
            SQL += ",@Tax=" + _DAO.FilterString(Request.Tax);
            SQL += ",@PostalCode=" + _DAO.FilterString(Request.PostalCode);
            SQL += ",@Prefecture=" + _DAO.FilterString(Request.Prefecture);
            SQL += ",@City=N" + _DAO.FilterString(Request.City);
            SQL += ",@InputStreet=N" + _DAO.FilterString(Request.Street);
            SQL += ",@BuildingRoomNo=" + (!string.IsNullOrEmpty(Request.BuildingRoomNo) ? "N" + _DAO.FilterString(Request.BuildingRoomNo) : _DAO.FilterString(Request.BuildingRoomNo));
            SQL += ",@RegularFee=" + _DAO.FilterString(Request.RegularFee);
            SQL += ",@DesignationFee=" + _DAO.FilterString(Request.DesignationFee);
            SQL += ",@CompanionFee=" + _DAO.FilterString(Request.CompanionFee);
            SQL += ",@ExtensionFee=" + _DAO.FilterString(Request.ExtensionFee);
            SQL += ",@VariousDrinks=" +(!string.IsNullOrEmpty(Request.Drink) ? "N" + _DAO.FilterString(Request.Drink) : _DAO.FilterString(Request.Drink));
            SQL += ",@GroupNamekatakana=" + (!string.IsNullOrEmpty(Request.GroupName2) ? "N" + _DAO.FilterString(Request.GroupName2) : _DAO.FilterString(Request.GroupName2));
            SQL += ",@CompanyAddress=" + (!string.IsNullOrEmpty(Request.CompanyAddress) ? "N" + _DAO.FilterString(Request.CompanyAddress) : _DAO.FilterString(Request.CompanyAddress));
            SQL += ",@BusinessLicenseNumber=" + _DAO.FilterString(Request.BusinessLicenseNumber);
            SQL += ",@LicenseIssuedDate=" + _DAO.FilterString(Request.LicenseIssuedDate);        
            SQL += ",@ClosingDate=" + _DAO.FilterString(Request.ClosingDate);
            SQL += ",@CeoNameKatakana=" + (!string.IsNullOrEmpty(Request.CeoFurigana) ? "N" + _DAO.FilterString(Request.CeoFurigana) : _DAO.FilterString(Request.CeoFurigana));           
            SQL += ",@CompanyRegistry=" + _DAO.FilterString(Request.CorporateRegistryDocument);
            SQL += ",@DocumentType=" + _DAO.FilterString(Request.IdentificationType);
           
            if (Request.IdentificationType=="2")
            {
                SQL += ",@KYCDocument=" + _DAO.FilterString(Request.PassportPhoto);
                SQL += ",@KYCDocumentBack=" + _DAO.FilterString(Request.InsurancePhoto);
            }
            else
            {
                SQL += ",@KYCDocument=" + _DAO.FilterString(Request.KYCDocument);
                SQL += ",@KYCDocumentBack=" + _DAO.FilterString(Request.KYCDocumentBack);
            }
           
            //SQL += ",@VariousDrinks=" + (!string.IsNullOrEmpty(Request.Drink) ? "N" + _DAO.FilterString(Request.Drink) : _DAO.FilterString(Request.Drink));
            if (Request.BusinessType == "1")
            {
                SQL += ",@CompanyName=N" + _DAO.FilterString(Request.CompanyName);
                SQL += ",@CompanyNameKatakana=" + (!string.IsNullOrEmpty(Request.CompanyNameFurigana) ? "N" + _DAO.FilterString(Request.CompanyNameFurigana) : _DAO.FilterString(Request.CompanyNameFurigana));
                SQL += ",@Representative1_ContactName=" + (!string.IsNullOrEmpty(Request.Representative1_ContactName) ? "N" + _DAO.FilterString(Request.Representative1_ContactName) : _DAO.FilterString(Request.Representative1_ContactName));
                SQL += ",@Representative1_MobileNo=" + _DAO.FilterString(Request.Representative1_MobileNo);
                SQL += ",@Representative1_Email=" + _DAO.FilterString(Request.Representative1_Email);
                SQL += ",@Representative1_Furigana=" + (!string.IsNullOrEmpty(Request.Representative1_Furigana) ? "N" + _DAO.FilterString(Request.Representative1_Furigana) : _DAO.FilterString(Request.Representative1_Furigana)); 
                SQL += ",@Representative2_ContactName=" + (!string.IsNullOrEmpty(Request.Representative2_ContactName) ? "N" + _DAO.FilterString(Request.Representative2_ContactName) : _DAO.FilterString(Request.Representative2_ContactName));
                SQL += ",@Representative2_MobileNo=" + (!string.IsNullOrEmpty(Request.Representative2_MobileNo) ? "N" + _DAO.FilterString(Request.Representative2_MobileNo) : _DAO.FilterString(Request.Representative2_MobileNo));  
                SQL += ",@Representative2_Email=" + (!string.IsNullOrEmpty(Request.Representative2_Email) ? "N" + _DAO.FilterString(Request.Representative2_Email) : _DAO.FilterString(Request.Representative2_Email));
                SQL += ",@Representative2_Furigana=" + (!string.IsNullOrEmpty(Request.Representative2_Furigana) ? "N" + _DAO.FilterString(Request.Representative2_Furigana) : _DAO.FilterString(Request.Representative2_Furigana));

            }
            Response = _DAO.ParseCommonDbResponse(SQL);
            return Response;           
        }

        public CommonDbResponse ManageApproveReject(string holdId, string flag, string AgentId, String culture = "", ManageClubCommon Request = null)
        {
            var Response = new CommonDbResponse();
            string sp_name = "sproc_club_management_approvalrejection ";
            sp_name += " @flag=" + _DAO.FilterString(flag);
            sp_name += ",@holdId =" + _DAO.FilterString(holdId);
            sp_name += ",@AgentId =" + _DAO.FilterString(AgentId);
            Response = _DAO.ParseCommonDbResponse(sp_name);            
            return Response;
        }


        public List<PlanListCommon> GetClubPlanIdentityList(string culture)
        
        {
            var plan = new List<planIdentityDataCommon>();
            var PlanListCommon = new List<PlanListCommon>();
            string SQL1 = "EXEC sproc_club_management_approvalrejection @Flag='planlst'";
            string SQL2 = "EXEC sproc_club_management_approvalrejection @Flag='cpi'";
            var dbResponse1 = _DAO.ExecuteDataTable(SQL1);
            var dbResponse2 = _DAO.ExecuteDataTable(SQL2);
            var response = new List<planIdentityDataCommon>();
            List<PlanListCommon> listcomm = new List<PlanListCommon>();
            int i = 0;
            if (dbResponse1.Rows.Count > 0)
            {

                foreach (DataRow row in dbResponse1.Rows)
                {
                    List<planIdentityDataCommon> filteredPlan = new List<planIdentityDataCommon>();
                    foreach (DataRow rows in dbResponse2.Rows)
                    {
                        filteredPlan.Add(new planIdentityDataCommon()
                        {
                            StaticDataValue = _DAO.ParseColumnValue(rows, "StaticDataValue").ToString(),
                            English = _DAO.ParseColumnValue(rows, "English").ToString(),
                            PlanListId = Convert.ToString(i),
                            japanese = _DAO.ParseColumnValue(rows, "japanese").ToString(),
                            inputtype = _DAO.ParseColumnValue(rows, "inputtype").ToString(),
                            IdentityDescription = (_DAO.ParseColumnValue(rows, "StaticDataValue").ToString() == "1") ? _DAO.ParseColumnValue(row, "planId").ToString() : "",
                            name = _DAO.ParseColumnValue(rows, "name").ToString(),
                            IdentityLabel = culture.ToLower() == "en" ? _DAO.ParseColumnValue(rows, "English").ToString() : _DAO.ParseColumnValue(rows, "japanese").ToString(),
                        }); ; ;

                    }
                    response.AddRange(filteredPlan);
                    listcomm.Add(new PlanListCommon { PlanIdentityList = filteredPlan });
                    i++;
                }
                  
                
            }
            return listcomm;
        }
        public List<planIdentityDataCommon> GetClubPlanIdentityListAddable(string culture)
        {

            var response = new List<planIdentityDataCommon>();
            string SQL = "EXEC sproc_club_management @Flag='cpi'";

            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new planIdentityDataCommon()
                    {
                        StaticDataValue = _DAO.ParseColumnValue(item, "StaticDataValue").ToString(),
                        English = _DAO.ParseColumnValue(item, "English").ToString(),
                        japanese = _DAO.ParseColumnValue(item, "japanese").ToString(),
                        inputtype = _DAO.ParseColumnValue(item, "inputtype").ToString(),
                        name = _DAO.ParseColumnValue(item, "name").ToString(),
                        IdentityLabel = culture == "en" ? _DAO.ParseColumnValue(item, "English").ToString() : _DAO.ParseColumnValue(item, "japanese").ToString(),
                    });
                    response.AddRange(response);
                }




            }
            return response;
        }
        public ClubDetailCommon GetClubDetails(string AgentId, String culture = "")
        {
            var plan = new List<planIdentityDataCommon>();
            var PlanListCommon = new List<PlanListCommon>();
            ClubDetailCommon ClubDetail = new ClubDetailCommon();           
            string SQL = "EXEC sproc_club_management_approvalrejection @Flag='gcd'";
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
                    GroupName2 = _DAO.ParseColumnValue(dbResponse, "GroupNamekatakana").ToString(),
                    CompanyAddress = _DAO.ParseColumnValue(dbResponse, "CompanyAddress").ToString(),
                    BusinessLicenseNumber = _DAO.ParseColumnValue(dbResponse, "BusinessLicenseNumber").ToString(),
                    LicenseIssuedDate = !string.IsNullOrEmpty(_DAO.ParseColumnValue(dbResponse, "LicenseIssuedDate").ToString()) ? Convert.ToDateTime(_DAO.ParseColumnValue(dbResponse, "LicenseIssuedDate")).ToString("yyyy/MM/dd") : _DAO.ParseColumnValue(dbResponse, "LicenseIssuedDate").ToString(),
                    ClosingDate = _DAO.ParseColumnValue(dbResponse, "ClosingDate").ToString(),                   
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
        public ManageClubCommon GetPlanListByClub(string culture, string ClubId)
        {

            var response = new ManageClubCommon();
            //List<planIdentityDataCommon> filteredPlan = new List<planIdentityDataCommon>(); = new List<planIdentityDataCommon>();
            string SQL = "EXEC sproc_club_management @Flag='gcpd'";

            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {               
                int i = 0;
                if (dbResponse.Rows.Count > 0)
                {

                    foreach (DataRow item in dbResponse.Rows)
                    {
                        List<planIdentityDataCommon> filteredPlan = new List<planIdentityDataCommon>();
                        if (_DAO.ParseColumnValue(item, "PlanListId").ToString() == Convert.ToString(i))
                        {


                            // Iterate through the filtered rows and add them to the list
                            foreach (DataRow row in dbResponse.Rows.Cast<DataRow>()
                                .Where(row => _DAO.ParseColumnValue(row, "PlanListId").ToString() == Convert.ToString(i)))
                            {

                                filteredPlan.Add(new planIdentityDataCommon()
                                {
                                    StaticDataValue = _DAO.ParseColumnValue(row, "StaticDataValue").ToString(),
                                    English = _DAO.ParseColumnValue(row, "English").ToString(),
                                    PlanListId = _DAO.ParseColumnValue(row, "PlanListId").ToString(),
                                    japanese = _DAO.ParseColumnValue(row, "japanese").ToString(),
                                    inputtype = _DAO.ParseColumnValue(row, "inputtype").ToString(),
                                    name = _DAO.ParseColumnValue(row, "name").ToString(),
                                    IdentityLabel = culture == "en" ? _DAO.ParseColumnValue(row, "English").ToString() : _DAO.ParseColumnValue(row, "japanese").ToString(),
                                });

                            }
                            response.PlanDetailList[i].PlanIdentityList.AddRange(filteredPlan);
                            i++;
                        }
                    }

                    // Add all items from the filtered list to PlanIdentityList

                }

            }
            return response;
        }
        //public CommonDbResponse ManageClub(ManageClubCommon Request)
        //{
        //    var Response=new CommonDbResponse();    
        //    string SQL = "EXEC sproc_club_management ";
        //    SQL += string.IsNullOrEmpty(Request.AgentId) ? "@Flag='rc'" : "@Flag='mc'";
        //    if (string.IsNullOrEmpty(Request.AgentId))
        //    {
        //        SQL += ",@LoginId=N" + _DAO.FilterString(Request.LoginId);
        //        SQL += ",@Email=" + _DAO.FilterString(Request.Email);
        //        SQL += ",@MobileNumber=" + _DAO.FilterString(Request.MobileNumber);
        //    }
        //    else
        //    {
        //        SQL += ",@AgentId=" + _DAO.FilterString(Request.AgentId);
        //    }
        //    //SQL += ",@FirstName=N" +   _DAO.FilterString(Request.FirstName);
        //    SQL += ",@LandLineNumber=" + _DAO.FilterString(Request.LandLineNumber);
        //    //SQL += ",@MiddleName=N" + _DAO.FilterString(Request.MiddleName);
        //    //SQL += ",@LastName=N" + _DAO.FilterString(Request.LastName);
        //    SQL += ",@ClubName1=N" + _DAO.FilterString(Request.ClubName1);
        //    SQL += ",@ClubName2=N" + _DAO.FilterString(Request.ClubName2);
        //    SQL += ",@BusinessType=" + _DAO.FilterString(Request.BusinessType);
        //    SQL += ",@GroupName=N" + _DAO.FilterString(Request.GroupName);
        //    SQL += ",@Description=N" + _DAO.FilterString(Request.Description);
        //    SQL += ",@LocationURL=" + _DAO.FilterString(Request.GoogleMap);
        //    SQL += ",@Longitude=" + _DAO.FilterString(Request.Longitude);
        //    SQL += ",@Latitude=" + _DAO.FilterString(Request.Latitude);
        //    SQL += ",@Logo=" + _DAO.FilterString(Request.Logo);
        //    SQL += ",@CoverPhoto=" + _DAO.FilterString(Request.CoverPhoto);
        //    SQL += ",@BusinessCertificate=" + _DAO.FilterString(Request.BusinessCertificate);
        //    SQL += ",@Gallery=" + _DAO.FilterString(Request.Gallery);
        //    SQL += ",@WebsiteLink=" + _DAO.FilterString(Request.WebsiteLink);
        //    SQL += ",@TiktokLink=" + _DAO.FilterString(Request.TiktokLink);
        //    SQL += ",@TwitterLink=" + _DAO.FilterString(Request.TwitterLink);
        //    SQL += ",@InstagramLink=" + _DAO.FilterString(Request.InstagramLink);
        //    SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
        //    SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
        //    SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
        //    SQL += ",@LocationId=" + _DAO.FilterString(Request.LocationId);
        //    SQL += ",@CompanyName=N" + _DAO.FilterString(Request.CompanyName);
        //    SQL += ",@ceoFullName=N" + _DAO.FilterString(Request.ceoFullName);
        //    SQL += ",@Line=" + _DAO.FilterString(Request.Line);      
        //    SQL += ",@ClubOpeningTime=" + _DAO.FilterString(Request.WorkingHrFrom);
        //    SQL += ",@ClubClosingTime=" + _DAO.FilterString(Request.WorkingHrTo);
        //    SQL += ",@Holiday=" + _DAO.FilterString(Request.Holiday);
        //    SQL += ",@LastOrderTime=" + _DAO.FilterString(Request.LastOrderTime);
        //    SQL += ",@LastEntryTime=" + _DAO.FilterString(Request.LastEntryTime);
        //    SQL += ",@Tax=" + _DAO.FilterString(Request.Tax);
        //    SQL += ",@PostalCode=" + _DAO.FilterString(Request.PostalCode);
        //    SQL += ",@Prefecture=" + _DAO.FilterString(Request.Prefecture);
        //    SQL += ",@City=N" + _DAO.FilterString(Request.City); 
        //    SQL += ",@InputStreet=N" + _DAO.FilterString(Request.Street); 
        //    SQL += ",@BuildingRoomNo=N" + _DAO.FilterString(Request.BuildingRoomNo);
        //    SQL += ",@RegularFee=" + _DAO.FilterString(Request.RegularFee);
        //    SQL += ",@DesignationFee=" + _DAO.FilterString(Request.DesignationFee); 
        //    SQL += ",@CompanionFee=" + _DAO.FilterString(Request.CompanionFee); 
        //    SQL += ",@ExtensionFee=" + _DAO.FilterString(Request.ExtensionFee);
        //    SQL += ",@VariousDrinks=N" + _DAO.FilterString(Request.Drink);
        //    SQL += ",@GroupNamekatakana=" + _DAO.FilterString(Request.GroupName2);
        //    SQL += ",@CompanyAddress=" + _DAO.FilterString(Request.CompanyAddress);
        //    SQL += ",@BusinessLicenseNumber=" + _DAO.FilterString(Request.BusinessLicenseNumber);
        //    SQL += ",@LicenseIssuedDate=" + _DAO.FilterString(Request.LicenseIssuedDate);
        //    SQL += ",@KYCDocument=" + _DAO.FilterString(Request.KYCDocument);
        //    SQL += ",@ClosingDate=N" + _DAO.FilterString(Request.ClosingDate);
        //    SQL += ",@Representative1_ContactName=" + _DAO.FilterString(Request.Representative1_ContactName);
        //    SQL += ",@Representative1_MobileNo=" + _DAO.FilterString(Request.Representative1_MobileNo);
        //    SQL += ",@Representative1_Email=" + _DAO.FilterString(Request.Representative1_Email);
        //    SQL += ",@Representative2_ContactName=" + _DAO.FilterString(Request.Representative2_ContactName);
        //    SQL += ",@Representative2_MobileNo=" + _DAO.FilterString(Request.Representative2_MobileNo);
        //    SQL += ",@Representative2_Email=" + _DAO.FilterString(Request.Representative2_Email);
        //    Response = _DAO.ParseCommonDbResponse(SQL);
        //    var i = 0;
        //    foreach (var planList in Request.PlanDetailList)
        //    {

        //        foreach (var planIdentity in planList.PlanIdentityList)
        //        {
        //            string SQL2 = "EXEC sproc_club_management ";
        //            SQL2 += "@Flag='icph'";
        //            SQL2 += ",@ClubPlanTypeId=" + _DAO.FilterString(planIdentity.StaticDataValue);
        //            SQL2 += ",@Description=N" + _DAO.FilterString(planIdentity.IdentityDescription);
        //            SQL2 += ",@PlanListId=" + _DAO.FilterString(Convert.ToString( i));
        //            SQL2 += ",@ClubId=" + _DAO.FilterString(string.IsNullOrEmpty(Request.AgentId)?   Response.Extra1: Request.AgentId);
        //            SQL2 += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
        //            SQL2 += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
        //            SQL2 += ",@Id=" + _DAO.FilterString(  planIdentity.Id);
        //            _DAO.ParseCommonDbResponse(SQL2);
        //       }
        //        i++;
        //    }

        //    return Response;
        //    // Create a DataTable to hold the PlanIdentity data
        //    //DataTable table = new DataTable();

        //    //table.Columns.Add("ClubPlanTypeId", typeof(long));
        //    //table.Columns.Add("Description", typeof(string));
        //    //table.Columns.Add("PlanListId", typeof(long));


        //    //// Populate the DataTable with PlanIdentity data from the model
        //    //foreach (var planList in Request.PlanDetailList)
        //    //{
        //    //    var i = 0;
        //    //    foreach (var planIdentity in planList.PlanIdentityList)
        //    //    {
        //    //        table.Rows.Add(planIdentity.StaticDataValue, planIdentity.IdentityDescription, i);

        //    //    }
        //    //    i++;
        //    //}

        //    //using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnString"] != null ? ConfigurationManager.ConnectionStrings["DBConnString"].ConnectionString : ""))
        //    //{
        //    //    using (SqlCommand command = new SqlCommand(SQL, connection))
        //    //    {
        //    //        // Create and add parameters
        //    //        // Your existing parameter creation code...

        //    //        // Add the @ClubPlanDetails parameter
        //    //        SqlParameter parameter = new SqlParameter("@ClubPlanDetails", SqlDbType.Structured);
        //    //        parameter.Value = table;
        //    //        parameter.TypeName = "ClubPlanDetailsType";
        //    //        command.Parameters.Add(parameter);

        //    //        // Add output parameter to capture message and code
        //    //        SqlParameter returnMessageParam = new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 1000);
        //    //        returnMessageParam.Direction = ParameterDirection.Output;
        //    //        command.Parameters.Add(returnMessageParam);

        //    //        SqlParameter returnCodeParam = new SqlParameter("@ReturnCode", SqlDbType.Int);
        //    //        returnCodeParam.Direction = ParameterDirection.Output;
        //    //        command.Parameters.Add(returnCodeParam);

        //    //        // Open connection, execute the command, and capture output parameters
        //    //        connection.Open();
        //    //        command.ExecuteNonQuery();

        //    //        // Retrieve message and code from output parameters
        //    //        string returnMessage = returnMessageParam.Value != DBNull.Value ? returnMessageParam.Value.ToString() : "";
        //    //        int Code = (int)returnCodeParam.Value;

        //    //        // Construct CommonDbResponse object with message and code
        //    //        CommonDbResponse response = new CommonDbResponse();
        //    //        response.Message = returnMessage;
        //    //        response.Extra1 =Convert.ToString( Code);

        //    //        return response;
        //    //    }
        //    //}
        //    //return _DAO.ParseCommonDbResponse(SQL);
        //}


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
        public List<AvailabilityTagModelCommon> GetAvailabilityList(string cId,string culture)
        {
            List<AvailabilityTagModelCommon> responseInfo = new List<AvailabilityTagModelCommon>();
            string sp_name = "exec sproc_ap_club_tag_management @Flag='gtl'";
            sp_name += ",@ClubId=" + _DAO.FilterString(cId);
            var dbResponseInfo = _DAO.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow data in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new AvailabilityTagModelCommon
                    {
                        StaticType = data["StaticType"].ToString(),
                        StaticLabel = culture?.ToLower()=="ja"? data["StaticLabelJapanese"].ToString() : data["StaticLabel"].ToString(),
                        StaticVaue = data["StaticValue"].ToString(),
                        StaticDescription = data["StaticDescription"].ToString(),
                        StaticStatus = data["StaticStatus"].ToString(),
                        StaticLabelJapanese = data["StaticLabelJapanese"].ToString(),
                    });
                }
            }
            return responseInfo;
        }
        public CommonDbResponse ManageClubAvailability(AvailabilityTagModelCommon request, ManageTagCommon dbRequest, string[] updatedValues)
        {
            string originalSpName = "EXEC sproc_ap_club_tag_management @Flag='mt'";
            var response = new CommonDbResponse();

            foreach (var item in updatedValues)
            {
                string sp_name = originalSpName;
                string[] parts = item.Split(',');
                string tagid = parts[0];
                string tagstatus = parts[1];
                sp_name += ", @ClubId=" + _DAO.FilterString(dbRequest.ClubId);
                sp_name += ", @TagType=" + _DAO.FilterString(request.StaticType);
                sp_name += ", @TagId=" + _DAO.FilterString(tagid);
                sp_name += ", @TagDescription=" + _DAO.FilterString("");
                sp_name += ", @TagStatus=" + _DAO.FilterString(tagstatus);
                sp_name += ", @ActionUser=" + _DAO.FilterString(dbRequest.ActionUser);
                sp_name += ", @ActionIP=" + _DAO.FilterString(dbRequest.ActionIP);
                sp_name += ", @ActionPlatform=" + _DAO.FilterString(dbRequest.ActionPlatform);
                response= _DAO.ParseCommonDbResponse(sp_name);
            }
            return response;
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

        #region Event Management

        public List<EventListCommon> GetEventList(PaginationFilterCommon Request, string AgentId)
        {
            var response = new List<EventListCommon>();
            string SQL = "EXEC sproc_event_management @Flag='gel'";
            SQL += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _DAO.FilterString(Request.SearchFilter) : null;
            SQL += !string.IsNullOrEmpty(AgentId) ? ",@AgentId=N" + _DAO.FilterString(AgentId) : null;
            SQL += ",@Skip=" + Request.Skip;
            SQL += ",@Take=" + Request.Take;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new EventListCommon()
                    {
                        LoginId = _DAO.ParseColumnValue(item, "LoginId").ToString(),
                        AgentId = _DAO.ParseColumnValue(item, "AgentId").ToString(),
                        Status = _DAO.ParseColumnValue(item, "Status").ToString(),
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "Sno").ToString()),
                        EventId = _DAO.ParseColumnValue(item, "EventId").ToString(),
                        Title = _DAO.ParseColumnValue(item, "EventTitle").ToString(),
                        Description = _DAO.ParseColumnValue(item, "Description").ToString(),
                        ActionUser = _DAO.ParseColumnValue(item, "ActionUser").ToString(),
                        CreatedDate = _DAO.ParseColumnValue(item, "ActionDate").ToString(),
                        ActionPlatform = _DAO.ParseColumnValue(item, "ActionPlatform").ToString(),
                        ActionDate = _DAO.ParseColumnValue(item, "ActionDate").ToString(),
                        UpdatedDate = _DAO.ParseColumnValue(item, "UpdatedDate").ToString(),
                        EventDate = _DAO.ParseColumnValue(item, "EventDate").ToString(),
                        EventType = _DAO.ParseColumnValue(item, "EventTypeName").ToString(),
                        Image = _DAO.ParseColumnValue(item, "Image").ToString(),
                        TotalRecords = Convert.ToInt32(_DAO.ParseColumnValue(item, "TotalRecords").ToString()),
                    });
                }
            }
            return response;
        }


        public CommonDbResponse ManageEvent(EventCommon Request)
        {
            string SQL = "EXEC sproc_event_management ";
            //SQL += "@Flag='I'";
            if (Request.flag == "del")
            {
                SQL += "@Flag='del'";
            }
            else
            {
                SQL += string.IsNullOrEmpty(Request.EventId) ? "@Flag='rc'" : "@Flag='me'";
            }
            SQL += ",@EventDate=" + _DAO.FilterString(Request.EventDate);
            SQL += ",@EventId=" + _DAO.FilterString(Request.EventId);
            SQL += ",@EventType=" + _DAO.FilterString(Request.EventType);
            SQL += ",@Description=N" + _DAO.FilterString(Request.Description);
            SQL += string.IsNullOrEmpty(Request.Title) ? ",@EventTitle=" + _DAO.FilterString(Request.Title) : ",@EventTitle=N" + _DAO.FilterString(Request.Title);
            SQL += string.IsNullOrEmpty(Request.Image) ? ",@ImagePath=" + _DAO.FilterString(Request.Image) : ",@ImagePath=N" + _DAO.FilterString(Request.Image);
            SQL += ",@AgentId=" + _DAO.FilterString(Request.AgentId);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            return _DAO.ParseCommonDbResponse(SQL);
        }

        public EventCommon GetEventDetails(string AgentId, string EventId)
        {
            string SQL = "EXEC sproc_event_management @Flag='ged'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += ",@EventId=" + _DAO.FilterString(EventId);
            var dbResponse = _DAO.ExecuteDataRow(SQL);
            if (dbResponse != null)
            {
                return new EventCommon()
                {
                    EventDate = _DAO.ParseColumnValue(dbResponse, "Date").ToString(),
                    EventType = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "EventType")),
                    EventTypeName = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "EventTypeName")),
                    Description = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "Description")),
                    Title = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "EventTitle")),
                    Image = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "Image")),
                    AgentId = _DAO.ParseColumnValue(dbResponse, "AgentId").ToString(),
                    EventId = _DAO.ParseColumnValue(dbResponse, "EventId").ToString(),

                };
            }
            return new EventCommon();
        }
        #endregion

        #region
        public ManageManagerCommon GetManagerDetails(string AgentId)
        {
            string SQL = "EXEC sproc_admin_club_manager_management @Flag='s'";
            SQL += ",@clubId=" + _DAO.FilterString(AgentId);
            var dbResponse = _DAO.ExecuteDataRow(SQL);
            if (dbResponse != null)
            {
                return new ManageManagerCommon()
                {
                    ManagerName = _DAO.ParseColumnValue(dbResponse, "FullName").ToString(),
                    Email = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "EmailAddress")),
                    MobileNumber = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "MobileNumber")),
                    ClubId = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "ClubId")),
                    ManagerId = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "ManagerId")),
                };
            }
            return new ManageManagerCommon();
        }
        public CommonDbResponse ManageManager(ManageManagerCommon Request)
        {
            string sp_name = "sproc_admin_club_manager_management ";
            sp_name += string.IsNullOrEmpty(Request.ManagerId) ? "@Flag='i'" : "@Flag='u'";
            sp_name += ",@ClubId=" + _DAO.FilterString(Request.ClubId);
            sp_name += ",@ManagerName=N" + _DAO.FilterString(Request.ManagerName);
            sp_name += ",@MobileNumber=" + _DAO.FilterString(Request.MobileNumber);
            sp_name += ",@Email=" + _DAO.FilterString(Request.Email);
            sp_name += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            sp_name += ",@ActionIp=" + _DAO.FilterString(Request.ActionIP);
            sp_name += ",@ManagerId=" + _DAO.FilterString(Request.ManagerId);
            return _DAO.ParseCommonDbResponse(sp_name);

        }
        #endregion
    }
}
