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

        public GroupAnalyticModelCommon GetGroupAnalytic()
        {
            string sp_name = "";
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
    }
}
