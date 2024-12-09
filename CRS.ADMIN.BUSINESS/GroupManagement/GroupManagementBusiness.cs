using CRS.ADMIN.REPOSITORY.GroupManagement;
using CRS.ADMIN.SHARED.GroupManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.GroupManagement
{
    public class GroupManagementBusiness : IGroupManagementBusiness
    {
        private readonly IGroupManagementRepository _repo;
        public GroupManagementBusiness(GroupManagementRepository repo)
        {
            _repo = repo;
        }

        public GroupAnalyticModelCommon GetGroupAnalytic()
        {
            return _repo.GetGroupAnalytic();
        }

        public List<GroupInfoModelCommon> GetGroupList(PaginationFilterCommon paginationFilter)
        {
            return _repo.GetGroupList(paginationFilter);
        }
    }
}
