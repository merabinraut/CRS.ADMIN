using CRS.ADMIN.REPOSITORY.GroupManagement;
using CRS.ADMIN.SHARED;
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
        #region GROUP SECTION
        public CommonDbResponse BlockGroup(string groupId, Common request)
        {
            return _repo.BlockGroup(groupId, request);
        }

        public CommonDbResponse DeleteGroup(string groupId, Common request)
        {
            return _repo.DeleteGroup(groupId, request);
        }

        public GroupAnalyticModelCommon GetGroupAnalytic()
        {
            return _repo.GetGroupAnalytic();
        }

        public ManageGroupModelCommon GetGroupDetail(string groupId)
        {
            return _repo.GetGroupDetail(groupId);
        }

        public List<GroupInfoModelCommon> GetGroupList(PaginationFilterCommon paginationFilter)
        {
            return _repo.GetGroupList(paginationFilter);
        }

        public CommonDbResponse ManageGroup(ManageGroupModelCommon commonModel)
        {
            return _repo.ManageGroup(commonModel);
        }

        public CommonDbResponse UnblockGroup(string groupId, Common request)
        {
            return _repo.UnblockGroup(groupId, request);
        }
        #endregion

        #region Sub Group Section
        public SubGroupModelCommon GetSubGroupByGroupId(string groupId, PaginationFilterCommon paginationFilter, string locationid, string clubName)
        {
            return _repo.GetSubGroupByGroupId(groupId, paginationFilter, locationid, clubName);
        }

        public CommonDbResponse ManageSubGroup(ManageSubGroupModelCommon commonRequest)
        {
            return _repo.ManageSubGroup(commonRequest);
        }

        public ManageSubGroupModelCommon GetSubGroupDetail(string subGroupId)
        {
            return _repo.GetSubGroupDetail(subGroupId);
        }

        public CommonDbResponse DeleteSubGroup(string subGroupId, Common request)
        {
            return _repo.DeleteSubGroup(subGroupId, request);
        }

        public CommonDbResponse ManageSubGroupClub(ManageSubGroupClubModelCommon commonModel)
        {
            return _repo.ManageSubGroupClub(commonModel);
        }

        public ManageSubGroupClubModelCommon GetSubGroupClubDetailById(string subGroupId)
        {
            return _repo.GetSubGroupClubDetailById(subGroupId);
        }
        public CommonDbResponse DeleteSubGroupClub(string id, string subgroupid, string clubid, Common request)
        {
            return _repo.DeleteSubGroupClub(id, subgroupid, clubid, request);
        }
        public List<AssignedClubInfoCommon> GetAssignedClubList()
        {
            return _repo.GetAssignedClubList();
        }
        #endregion

        #region GROUP GALLERY
        public List<GroupGalleryInfoModelCommon> GetGalleryListById(string groupId, string SearchFilter)
        {
            return _repo.GetGalleryListById(groupId, SearchFilter);
        }

        public CommonDbResponse ManageGroupGallery(ManageGroupGalleryModelCommon commonModel)
        {
            return _repo.ManageGroupGallery(commonModel);
        }

        public ManageGroupGalleryModelCommon GetGroupGalleryDetail(string imageid)
        {
            return _repo.GetGroupGalleryDetail(imageid);
        }

        public CommonDbResponse DeleteImage(string imageid, string groupid, Common request)
        {
            return _repo.DeleteImage(imageid, groupid, request);
        }



        #endregion

    }
}
