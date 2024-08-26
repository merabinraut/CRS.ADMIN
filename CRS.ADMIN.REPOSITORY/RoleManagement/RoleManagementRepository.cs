using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.RoleManagement;
using System;
using System.Collections.Generic;
using System.Data;

namespace CRS.ADMIN.REPOSITORY.RoleManagement
{
    public class RoleManagementRepository : IRoleManagementRepository
    {
        RepositoryDao _DAO;
        public RoleManagementRepository()
        {
            _DAO = new RepositoryDao();
        }
        #region Manage Role
        public List<RoleListCommon> GetRoleList()
        {
            List<RoleListCommon> response = new List<RoleListCommon>();
            string SQL = "EXEC sproc_role_management @flag='grl';";
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new RoleListCommon
                    {
                        RoleId = item["RoleId"].ToString(),
                        RoleName = item["RoleName"].ToString(),
                        RoleDescription = item["RoleDescription"].ToString(),
                        ActionDate = !string.IsNullOrEmpty(item["ActionDate"].ToString()) ? DateTime.Parse(item["ActionDate"].ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : item["ActionDate"].ToString() ,
                        ActionUser = item["ActionUser"].ToString()
                    });
                }
            }
            return response;
        }

        public List<RoleTypeListCommon> GetRoleTypeList(string RoleType, string SearchFilter = "")
        {
            List<RoleTypeListCommon> response = new List<RoleTypeListCommon>();
            string SQL = "EXEC sproc_role_management @flag='grtl'";
            SQL += ", @RoleType=" + _DAO.FilterString(@RoleType);
            SQL += !string.IsNullOrEmpty(SearchFilter) ? ", @SearchFilter=N" + _DAO.FilterString(SearchFilter) : null;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new RoleTypeListCommon
                    {
                        RoleId = item["RoleId"].ToString(),
                        RoleType = item["RoleType"].ToString(),
                        RoleName = item["RoleName"].ToString(),
                        RoleDescription = item["RoleDescription"].ToString(),
                        ActionDate = !string.IsNullOrEmpty(item["ActionDate"].ToString()) ? DateTime.Parse(item["ActionDate"].ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : item["ActionDate"].ToString(),
                        ActionUser = item["ActionUser"].ToString()
                    });
                }
            }
            return response;
        }

        public CommonDbResponse AddRoleType(ManageRoleCommon Request)
        {
            string SQL = "EXEC sproc_role_management @Flag='irt'";
            SQL += ",@RoleType=" + _DAO.FilterString(Request.RoleType);
            SQL += ",@RoleName=N" + _DAO.FilterString(Request.RoleName);
            SQL += ",@RoleDescription=N" + _DAO.FilterString(Request.Description);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        #endregion

        #region Menus Management
        public List<MenuManagementListCommon> GetMenus(string RoleId)
        {
            var response = new List<MenuManagementListCommon>();
            string SQL = "EXEC sproc_menu_management @Flag='gm'";
            SQL += ",@RoleId=" + _DAO.FilterString(RoleId);
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new MenuManagementListCommon()
                    {
                        MenuId = _DAO.ParseColumnValue(item, "MenuId").ToString(),
                        MenuName = _DAO.ParseColumnValue(item, "MenuName").ToString(),
                        Status = _DAO.ParseColumnValue(item, "Status").ToString().ToUpper() == "Y" ? true : false
                    });
                }
            }
            return response;
        }

        public CommonDbResponse AssignMenus(string RoleId, string Roles, string ActionUser, string ActionIP)
        {
            string SQL = "EXEC sproc_menu_management @Flag='am'";
            SQL += ",@RoleId=" + _DAO.FilterString(RoleId);
            SQL += ",@Roles=" + _DAO.FilterString(Roles);
            SQL += ",@ActionUser=" + _DAO.FilterString(ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        #endregion

        #region Functions Management
        public List<FunctionManagementListCommon> GetFunctions(string RoleId)
        {
            var Response = new List<FunctionManagementListCommon>();
            string SQL = "EXEC sproc_function_management @Flag='gf'";
            SQL += ",@RoleId=" + _DAO.FilterString(RoleId);
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    Response.Add(new FunctionManagementListCommon()
                    {
                        FunctionId = _DAO.ParseColumnValue(item, "FunctionId").ToString(),
                        FunctionName = _DAO.ParseColumnValue(item, "FunctionName").ToString(),
                        MenuName = _DAO.ParseColumnValue(item, "MenuName").ToString(),
                        Status = _DAO.ParseColumnValue(item, "Status").ToString().ToUpper() == "Y" ? true : false
                    });
                }
            }
            return Response;
        }

        public CommonDbResponse AssignFunctions(string RoleId, string Functions, string ActionUser, string ActionIP)
        {
            string SQL = "EXEC sproc_function_management @Flag='af'";
            SQL += ",@RoleId=" + _DAO.FilterString(RoleId);
            SQL += ",@Functions=" + _DAO.FilterString(Functions);
            SQL += ",@ActionUser=" + _DAO.FilterString(ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        #endregion

        #region Assign Role Management
        public GetCurrentAssignedRoleCommon GetCurrentAssignedRole(string AgentId, string AgentType)
        {
            string SQL = "EXEC sproc_assign_role_management @Flag = 'gcar'";
            SQL += ",@AgentId =" + _DAO.FilterString(AgentId);
            SQL += ",@AgentType =" + _DAO.FilterString(AgentType);
            var dbResponse = _DAO.ExecuteDataRow(SQL);
            if (dbResponse != null)
            {
                var Code = _DAO.ParseColumnValue(dbResponse, "Code").ToString();
                var Message = _DAO.ParseColumnValue(dbResponse, "Message").ToString();
                if (!string.IsNullOrEmpty(Code) && Code == "0")
                {
                    return new GetCurrentAssignedRoleCommon()
                    {
                        Code = ResponseCode.Success,
                        RoleId = _DAO.ParseColumnValue(dbResponse, "RoleId").ToString(),
                        RoleName = _DAO.ParseColumnValue(dbResponse, "RoleName").ToString(),
                    };
                }
                return new GetCurrentAssignedRoleCommon()
                {
                    Code = ResponseCode.Failed,
                    Message = Message ?? "Failed"
                };
            }
            return new GetCurrentAssignedRoleCommon()
            {
                Code = ResponseCode.Failed,
                Message = "Failed"
            };
        }

        public CommonDbResponse ManageAgentRole(ManageAgentRoleCommon Request)
        {
            string SQL = "EXEC sproc_assign_role_management @Flag = 'mur'";
            SQL += ",@AgentId =" + _DAO.FilterString(Request.AgentId);
            SQL += ",@RoleId =" + _DAO.FilterString(Request.RoleId);
            SQL += ",@AgentType =" + _DAO.FilterString(Request.AgentType);
            SQL += ",@ActionUser =" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP =" + _DAO.FilterString(Request.ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        #endregion
    }
}
