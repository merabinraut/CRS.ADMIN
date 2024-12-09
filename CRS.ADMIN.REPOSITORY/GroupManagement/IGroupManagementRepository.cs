using CRS.ADMIN.SHARED.GroupManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.GroupManagement
{
    public interface IGroupManagementRepository
    {
        GroupAnalyticModelCommon GetGroupAnalytic();
        List<GroupInfoModelCommon> GetGroupList(PaginationFilterCommon paginationFilter);
    }
}
