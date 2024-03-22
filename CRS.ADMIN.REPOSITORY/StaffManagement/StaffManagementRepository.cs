using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.StaffManagement;
using System;
using System.Collections.Generic;
using System.Data;

namespace CRS.ADMIN.REPOSITORY.StaffManagement
{
    public class StaffManagementRepository : IStaffManagementRepository
    {
        RepositoryDao _dao;
        public StaffManagementRepository()
        {
            _dao = new RepositoryDao();
        }

        public CommonDbResponse DeleteStaff(string id, Common commonRequest)
        {
            string sp_name = "EXEC sproc_superadmin_staffmanagement @Flag='ds'";
            sp_name += ",@Id=" + _dao.FilterString(id);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonRequest.ActionUser);
            sp_name += ",@ActionIp=" + _dao.FilterString(commonRequest.ActionIP);

            return _dao.ParseCommonDbResponse(sp_name);
        }

        public StaffDetailsCommon GetStaffDetails(string id)
        {
            string sp_name = "EXEC sproc_superadmin_staffmanagement @Flag='gsd'";
            sp_name += ",@Id=" + _dao.FilterString(id);

            var dbResponseInfo = _dao.ExecuteDataRow(sp_name);
            if (dbResponseInfo != null)
            {
                return new StaffDetailsCommon()
                {
                    Id = _dao.ParseColumnValue(dbResponseInfo, "Id").ToString(),
                    UserName = _dao.ParseColumnValue(dbResponseInfo, "UserName").ToString(),
                    EmailAddress = _dao.ParseColumnValue(dbResponseInfo, "EmailAddress").ToString(),
                    MobileNumber = _dao.ParseColumnValue(dbResponseInfo, "MobileNumber").ToString(),
                    RoleId = _dao.ParseColumnValue(dbResponseInfo, "RoleId").ToString(),
                };
            }
            return new StaffDetailsCommon();
        }

        public List<StaffManagementListModelCommon> GetStaffList(PaginationFilterCommon Request)
        {
            var responseInfo = new List<StaffManagementListModelCommon>();
            string sp_name = "EXEC sproc_superadmin_staffmanagement @Flag='gsl'";
            sp_name += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _dao.FilterString(Request.SearchFilter) : null;
            sp_name += ",@Skip=" + Request.Skip;
            sp_name += ",@Take=" + Request.Take;
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow dataRow in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new StaffManagementListModelCommon()
                    {
                        Id = dataRow["Id"].ToString(),
                        FullName = dataRow["FullName"].ToString(),
                        UserName = dataRow["UserName"].ToString(),
                        EmailAddress = dataRow["EmailAddress"].ToString(),
                        MobileNumber = dataRow["MobileNumber"].ToString(),
                        ProfileImage = dataRow["ProfileImage"].ToString(),
                        RoleName = dataRow["RoleName"].ToString(),
                        ActionDate = dataRow["ActionDate"].ToString(),
                        TotalRecords = Convert.ToInt32(_dao.ParseColumnValue(dataRow, "TotalRecords").ToString()),
                        SNO = Convert.ToInt32(_dao.ParseColumnValue(dataRow, "SNO").ToString())
                    });
                }
            }
            return responseInfo;
        }

        public CommonDbResponse ManageStaff(ManagerStaffCommon commonModel)
        {
            string sp_name = "EXEC sproc_superadmin_staffmanagement ";
            sp_name += string.IsNullOrEmpty(commonModel.Id) ? "@Flag='ms'" : "@Flag='us'";
            sp_name += ",@Id=" + _dao.FilterString(commonModel.Id);
            sp_name += ",@UserName=N" + _dao.FilterString(commonModel.UserName);
            sp_name += ",@EmailAddress=" + _dao.FilterString(commonModel.EmailAddress);
            sp_name += ",@MobileNumber=" + _dao.FilterString(commonModel.MobileNumber);
            sp_name += ",@RoleId=" + _dao.FilterString(commonModel.RoleId);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonModel.ActionUser);
            sp_name += ",@ActionIp=" + _dao.FilterString(commonModel.ActionIP);

            return _dao.ParseCommonDbResponse(sp_name);
        }
    }
}
