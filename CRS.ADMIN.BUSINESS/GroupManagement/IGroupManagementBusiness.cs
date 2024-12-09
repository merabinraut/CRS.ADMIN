using CRS.ADMIN.SHARED.GroupManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.GroupManagement
{
    public interface IGroupManagementBusiness
    {
        GroupAnalyticModelCommon GetGroupAnalytic();
        List<GroupInfoModelCommon> GetGroupList(PaginationFilterCommon paginationFilter);
    }
}
