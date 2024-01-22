using CRS.ADMIN.REPOSITORY.RoleManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.RoleManagement;
using System;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.RoleManagement
{
    public class RoleManagementBusiness : IRoleManagementBusiness
    {
        IRoleManagementRepository _REPO;
        public RoleManagementBusiness(RoleManagementRepository REPO)
        {
            _REPO = REPO;
        }
        #region Manage Role
        public List<RoleListCommon> GetRoleList()
        {
            return _REPO.GetRoleList();
        }

        public List<RoleTypeListCommon> GetRoleTypeList(string RoleType, string SearchFilter = "")
        {
            return _REPO.GetRoleTypeList(RoleType, SearchFilter);
        }

        public CommonDbResponse AddRoleType(ManageRoleCommon Request)
        {
            return _REPO.AddRoleType(Request);
        }
        #endregion

        #region Menus Management
        public List<MenuManagementListCommon> GetMenus(string RoleId)
        {
            return _REPO.GetMenus(RoleId);
        }

        public CommonDbResponse AssignMenus(string RoleId, string Roles, string ActionUser, string ActionIP)
        {
            return _REPO.AssignMenus(RoleId, Roles, ActionUser, ActionIP);
        }
        #endregion

        #region Functions Management
        public List<FunctionManagementListCommon> GetFunctions(string RoleId)
        {
            return _REPO.GetFunctions(RoleId);
        }

        public CommonDbResponse AssignFunctions(string RoleId, string Functions, string ActionUser, string ActionIP)
        {
            return _REPO.AssignFunctions(RoleId, Functions, ActionUser, ActionIP);
        }
        #endregion

        #region Assign Role Management
        public GetCurrentAssignedRoleCommon GetCurrentAssignedRole(string AgentId, string AgentType)
        {
            return _REPO.GetCurrentAssignedRole(AgentId, AgentType);
        }
        public CommonDbResponse ManageAgentRole(ManageAgentRoleCommon Request)
        {
            return _REPO.ManageAgentRole(Request);
        }
        #endregion
    }
}
