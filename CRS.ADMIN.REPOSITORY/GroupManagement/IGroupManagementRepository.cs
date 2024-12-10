using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.GroupManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.GroupManagement
{
    public interface IGroupManagementRepository
    {
        CommonDbResponse BlockGroup(string groupId, Common request);
        CommonDbResponse DeleteGroup(string groupId, Common request);
        GroupAnalyticModelCommon GetGroupAnalytic();
        ManageGroupModelCommon GetGroupDetail(string groupId);
        List<GroupInfoModelCommon> GetGroupList(PaginationFilterCommon paginationFilter);
        CommonDbResponse ManageGroup(ManageGroupModelCommon commonModel);
        CommonDbResponse UnblockGroup(string groupId, Common request);
    }
}
