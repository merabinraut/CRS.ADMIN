using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.GroupManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;
using System.Linq;

namespace CRS.ADMIN.REPOSITORY.GroupManagement
{
    public class GroupManagementRepository : IGroupManagementRepository
    {
        RepositoryDao _dao;
        public GroupManagementRepository()
        {
            _dao = new RepositoryDao();
        }
        #region GROUP SECTION
        public CommonDbResponse BlockGroup(string groupId, Common request)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_block_group]";
            sp_name += "@GroupId=" + _dao.FilterString(groupId);
            sp_name += ",@ActionUser=" + _dao.FilterString(request.ActionUser);
            sp_name += ",@ActionIP=" + _dao.FilterString(request.ActionIP);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(request.ActionPlatform);

            var dbResponse = _dao.ParseCommonDbResponse(sp_name);
            return dbResponse;
        }

        public CommonDbResponse DeleteGroup(string groupId, Common request)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_delete_group]";
            sp_name += "@GroupId=" + _dao.FilterString(groupId);
            sp_name += ",@ActionUser=" + _dao.FilterString(request.ActionUser);
            sp_name += ",@ActionIP=" + _dao.FilterString(request.ActionIP);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(request.ActionPlatform);

            var dbResponse = _dao.ParseCommonDbResponse(sp_name);
            return dbResponse;
        }

        public GroupAnalyticModelCommon GetGroupAnalytic()
        {
            string sp_name = "[dbo].[sproc_admin_group_analytic_overview]";
            var dbResponse = _dao.ExecuteDataRow(sp_name);
            if (dbResponse != null)
            {
                return new GroupAnalyticModelCommon()
                {
                    AssignedClub = _dao.ParseColumnValue(dbResponse, "AssignedClub").ToString(),
                    TotalClub = _dao.ParseColumnValue(dbResponse, "TotalClub").ToString(),
                    TotalGroup = _dao.ParseColumnValue(dbResponse, "TotalGroup").ToString(),
                    UnAssignedClub = _dao.ParseColumnValue(dbResponse, "UnAssignedClub").ToString()
                };
            }
            else
            {
                return new GroupAnalyticModelCommon();
            }

        }

        public ManageGroupModelCommon GetGroupDetail(string groupId)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_group_detail]";
            sp_name += "@GroupId=" + _dao.FilterString(groupId);
            var dbReponse = _dao.ExecuteDataRow(sp_name);
            if (dbReponse != null)
            {
                return new ManageGroupModelCommon()
                {
                    GroupId = _dao.ParseColumnValue(dbReponse, "GroupId").ToString(),
                    GroupName = _dao.ParseColumnValue(dbReponse, "GroupName").ToString(),
                    GroupNameKatakana = _dao.ParseColumnValue(dbReponse, "GroupNameKatakana").ToString(),
                    GroupDescription = _dao.ParseColumnValue(dbReponse, "GroupDescription").ToString(),
                    GroupCoverPhoto = _dao.ParseColumnValue(dbReponse, "GroupCoverPhoto").ToString(),
                };
            }
            else
            {
                return new ManageGroupModelCommon();
            }
        }

        public List<GroupInfoModelCommon> GetGroupList(PaginationFilterCommon paginationFilter)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_group_list]";
            sp_name += "@Skip=" + paginationFilter.Skip;
            sp_name += ",@Take=" + paginationFilter.Take;
            sp_name += !string.IsNullOrEmpty(paginationFilter.SearchFilter) ? ", @SearchFilter =N" + _dao.FilterString(paginationFilter.SearchFilter) : "";
            var dbResponse = _dao.ExecuteDataTable(sp_name);
            if (dbResponse != null && dbResponse.Rows.Count > 0)
                return _dao.DataTableToListObject<GroupInfoModelCommon>(dbResponse).ToList();

            return new List<GroupInfoModelCommon>();
        }


        public CommonDbResponse ManageGroup(ManageGroupModelCommon commonModel)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_add_update_group]";
            sp_name += "@GroupId=" + _dao.FilterString(commonModel.GroupId);
            sp_name += ",@GroupName=" + _dao.FilterString(commonModel.GroupName);
            sp_name += ",@GroupNameKatakana=" + (!string.IsNullOrEmpty(commonModel.GroupNameKatakana) ? "N" + _dao.FilterString(commonModel.GroupNameKatakana) : _dao.FilterString(commonModel.GroupNameKatakana));
            sp_name += ",@GroupDescription=" + _dao.FilterString(commonModel.GroupDescription);
            sp_name += ",@GroupCoverPhoto=" + _dao.FilterString(commonModel.GroupCoverPhoto);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonModel.ActionUser);
            sp_name += ",@ActionIP=" + _dao.FilterString(commonModel.ActionIP);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(commonModel.ActionPlatform);
            var dbResponse = _dao.ParseCommonDbResponse(sp_name);
            return dbResponse;

        }

        public CommonDbResponse UnblockGroup(string groupId, Common request)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_unblock_group]";
            sp_name += "@GroupId=" + _dao.FilterString(groupId);
            sp_name += ",@ActionUser=" + _dao.FilterString(request.ActionUser);
            sp_name += ",@ActionIP=" + _dao.FilterString(request.ActionIP);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(request.ActionPlatform);

            var dbResponse = _dao.ParseCommonDbResponse(sp_name);
            return dbResponse;
        }
        #endregion

        #region SUB GROUP SECTION
        public List<SubGroupInfoModelCommon> GetSubGroupByGroupId(string groupId, PaginationFilterCommon paginationFilter)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_get_subgroup]";
            sp_name += "@GroupId=" + _dao.FilterString(groupId);
            sp_name += ",@SearchFilter=" + _dao.FilterString(paginationFilter.SearchFilter);
            sp_name += ",@Skip=" + paginationFilter.Skip;
            sp_name += ",@Take=" + paginationFilter.Take;

            var dbResponse = _dao.ExecuteDataTable(sp_name);
            if (dbResponse.Rows.Count > 0 && dbResponse != null)
                return _dao.DataTableToListObject<SubGroupInfoModelCommon>(dbResponse).ToList();
            else return new List<SubGroupInfoModelCommon>();
        }

        public CommonDbResponse ManageSubGroup(ManageSubGroupModelCommon commonRequest)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_subgroup_addupdated]";
            sp_name += "@GroupId=" + _dao.FilterString(commonRequest.GroupId);
            sp_name += ",@SubGroupId=" + _dao.FilterString(commonRequest.SubGroupId);
            sp_name += ",@SubGroupName=" + (!string.IsNullOrEmpty(commonRequest.SubGroupName) ? "N" + _dao.FilterString(commonRequest.SubGroupName) : _dao.FilterString(commonRequest.SubGroupName));
            sp_name += ",@SubGroupNameKatakana=" + (!string.IsNullOrEmpty(commonRequest.SubGroupNameKatakana) ? "N" + _dao.FilterString(commonRequest.SubGroupNameKatakana) : _dao.FilterString(commonRequest.SubGroupNameKatakana));
            sp_name += ",@ActionUser=" + _dao.FilterString(commonRequest.ActionUser);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(commonRequest.ActionPlatform);
            sp_name += ",@ActionIP=" + _dao.FilterString(commonRequest.ActionIP);

            var dbResponse = _dao.ParseCommonDbResponse(sp_name);
            return dbResponse;
        }

        public ManageSubGroupModelCommon GetSubGroupDetail(string subGroupId)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_subgroup_detail]";
            sp_name += "@SubGroupId=" + _dao.FilterString(subGroupId);
            var dbResponse = _dao.ExecuteDataRow(sp_name);

            if (dbResponse != null)
            {
                return new ManageSubGroupModelCommon()
                {
                    GroupId = _dao.ParseColumnValue(dbResponse, "GroupId").ToString(),
                    SubGroupId = _dao.ParseColumnValue(dbResponse, "SubGroupId").ToString(),
                    SubGroupName = _dao.ParseColumnValue(dbResponse, "SubGroupName").ToString(),
                    GroupName = _dao.ParseColumnValue(dbResponse, "GroupName").ToString(),
                    GroupNameKatakana = _dao.ParseColumnValue(dbResponse, "GroupNameKatakana").ToString()
                };
            }
            else
            {
                return new ManageSubGroupModelCommon();
            }
        }
        #endregion
    }
}
