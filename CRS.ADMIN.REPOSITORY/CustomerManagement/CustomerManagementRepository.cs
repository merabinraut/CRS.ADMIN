using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.CustomerManagement;
using System;
using System.Collections.Generic;
using System.Data;

namespace CRS.ADMIN.REPOSITORY.CustomerManagement
{
    public class CustomerManagementRepository : ICustomerManagementRepository
    {
        RepositoryDao _DAO;
        public CustomerManagementRepository()
        {
            _DAO = new RepositoryDao();
        }
        public List<CustomerListCommon> GetCustomerList(CustomerSearchFilterCommon Request)
        {
            var response = new List<CustomerListCommon>();
            string SQL = "EXEC sproc_customer_management @Flag='gcl'";
            SQL += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _DAO.FilterString(Request.SearchFilter) : null;
            SQL += !string.IsNullOrEmpty(Request.FromDate) ? ",@FromDate=" + _DAO.FilterString(Request.FromDate) : null;
            SQL += !string.IsNullOrEmpty(Request.ToDate) ? ",@ToDate=" + _DAO.FilterString(Request.ToDate) : null;
            SQL += !string.IsNullOrEmpty(Request.Status) ? ",@Status=" + _DAO.FilterString(Request.Status) : null;
            SQL += !string.IsNullOrEmpty(Request.MobileNumber) ? ",@MobileNumber=" + _DAO.FilterString(Request.MobileNumber) : null;
            SQL += !string.IsNullOrEmpty(Request.MobileNumber) ? ",@MobileNumber=" + _DAO.FilterString(Request.MobileNumber) : null;
            SQL += ",@Skip=" + Request.Skip;
            SQL += ",@Take=" + Request.Take;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new CustomerListCommon()
                    {
                        AgentId = _DAO.ParseColumnValue(item, "AgentId").ToString(),
                        NickName = _DAO.ParseColumnValue(item, "NickName").ToString(),
                        FullName = _DAO.ParseColumnValue(item, "FullName").ToString(),
                        ProfileImage = _DAO.ParseColumnValue(item, "ProfileImage").ToString(),
                        Referer = _DAO.ParseColumnValue(item, "Referer").ToString(),
                        Type = _DAO.ParseColumnValue(item, "Type").ToString(),
                        MobileNumber = _DAO.ParseColumnValue(item, "MobileNumber").ToString(),
                        EmailAddress = _DAO.ParseColumnValue(item, "EmailAddress").ToString(),
                        Age = _DAO.ParseColumnValue(item, "Age").ToString(),
                        Status = _DAO.ParseColumnValue(item, "Status").ToString(),
                        Location = _DAO.ParseColumnValue(item, "Location").ToString(),
                        CreatedDate = _DAO.ParseColumnValue(item, "CreatedDate").ToString(),
                        UpdatedDate = _DAO.ParseColumnValue(item, "UpdatedDate").ToString(),
                        TotalRecords = Convert.ToInt32(_DAO.ParseColumnValue(item, "TotalRecords").ToString()),
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "SNO").ToString())
                    });
                }
            }
            return response;
        }

        public CommonDbResponse ManageCustomer(ManageCustomerCommon Request)
        {
            var SQL = "EXEC sproc_customer_management ";
            SQL += string.IsNullOrEmpty(Request.AgentId) ? "@Flag='icd'" : "@Flag='ucd'";
            if (string.IsNullOrEmpty(Request.AgentId))
            {
                SQL += ",@MobileNumber=" + _DAO.FilterString(Request.MobileNumber);
                SQL += ",@DOB=" + _DAO.FilterString(Request.DOB);
                SQL += ",@EmailAddress=" + _DAO.FilterString(Request.EmailAddress);
            }
            SQL += ",@AgentId=" + _DAO.FilterString(Request.AgentId);
            SQL += ",@NickName=" + _DAO.FilterString(Request.NickName);
            SQL += ",@FirstName=" + _DAO.FilterString(Request.FirstName);
            SQL += ",@LastName=" + _DAO.FilterString(Request.LastName);
            SQL += ",@Gender=" + _DAO.FilterString(Request.Gender);
            SQL += ",@PreferredLocation=" + _DAO.FilterString(Request.PreferredLocation);
            SQL += ",@PostalCode=" + _DAO.FilterString(Request.PostalCode);
            SQL += ",@Prefecture=" + _DAO.FilterString(Request.Prefecture);
            SQL += ",@City=" + _DAO.FilterString(Request.City);
            SQL += ",@Street=" + _DAO.FilterString(Request.Street);
            SQL += ",@ResidenceNumber=" + _DAO.FilterString(Request.ResidenceNumber);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            return _DAO.ParseCommonDbResponse(SQL);
        }

        public ManageCustomerCommon GetCustomerDetail(string AgentId)
        {
            string SQL = "EXEC sproc_customer_management @Flag='gcd'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            var dbResonse = _DAO.ExecuteDataRow(SQL);
            if (dbResonse != null)
            {
                return new ManageCustomerCommon()
                {
                    NickName = _DAO.ParseColumnValue(dbResonse, "NickName").ToString(),
                    FirstName = _DAO.ParseColumnValue(dbResonse, "FirstName").ToString(),
                    LastName = _DAO.ParseColumnValue(dbResonse, "LastName").ToString(),
                    MobileNumber = _DAO.ParseColumnValue(dbResonse, "MobileNumber").ToString(),
                    DOB = _DAO.ParseColumnValue(dbResonse, "DOB").ToString(),
                    EmailAddress = _DAO.ParseColumnValue(dbResonse, "EmailAddress").ToString(),
                    Gender = _DAO.ParseColumnValue(dbResonse, "Gender").ToString(),
                    PreferredLocation = _DAO.ParseColumnValue(dbResonse, "PreferredLocation").ToString(),
                    PostalCode = _DAO.ParseColumnValue(dbResonse, "PostalCode").ToString(),
                    Prefecture = _DAO.ParseColumnValue(dbResonse, "Prefecture").ToString(),
                    City = _DAO.ParseColumnValue(dbResonse, "City").ToString(),
                    Street = _DAO.ParseColumnValue(dbResonse, "Street").ToString(),
                    ResidenceNumber = _DAO.ParseColumnValue(dbResonse, "ResidenceNumber").ToString()
                };
            }
            return new ManageCustomerCommon();
        }

        public CommonDbResponse ManageCustomerStatus(string AgentId, string Status, Common Request)
        {
            string SQL = "EXEC sproc_customer_management @Flag='ucs'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += ",@Status=" + _DAO.FilterString(Status);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            return _DAO.ParseCommonDbResponse(SQL);
        }

        public CommonDbResponse ResetCustomerPassword(string AgentId, Common Request)
        {
            string SQL = "EXEC sproc_customer_management @Flag='rcp'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            return _DAO.ParseCommonDbResponse(SQL);
        }
    }
}
