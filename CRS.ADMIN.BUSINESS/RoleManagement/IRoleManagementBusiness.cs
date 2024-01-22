using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.RoleManagement;
using System;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.RoleManagement
{
    public interface IRoleManagementBusiness
    {
        #region Manage Role
        List<RoleListCommon> GetRoleList();
        List<RoleTypeListCommon> GetRoleTypeList(string RoleType, string SearchFilter = "");
        CommonDbResponse AddRoleType(ManageRoleCommon Request);
        #endregion
        #region Menus Management
        List<MenuManagementListCommon> GetMenus(string RoleId);
        CommonDbResponse AssignMenus(string RoleId, string Roles, string ActionUser, string ActionIP);
        #endregion
        #region Functions Management
        List<FunctionManagementListCommon> GetFunctions(string RoleId);
        CommonDbResponse AssignFunctions(string RoleId, string Functions, string ActionUser, string ActionIP);
        #endregion
        #region Assign Role Management
        GetCurrentAssignedRoleCommon GetCurrentAssignedRole(string AgentId, string AgentType);
        CommonDbResponse ManageAgentRole(ManageAgentRoleCommon Request);
        #endregion
    }
}
