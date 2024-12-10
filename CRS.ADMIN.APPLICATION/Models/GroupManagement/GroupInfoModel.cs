using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.APPLICATION.Models.GroupManagement
{
    public class GroupOverviewModel
    {
        public string SearchFilter { get; set; }
        public ManageGroupModel ManageGroup { get; set; }
        public GroupAnalyticModel GetGroupAnalytic { get; set; } = new GroupAnalyticModel();
        public List<GroupInfoModel> GetGroupList { get; set; } = new List<GroupInfoModel>();
    }
    public class GroupInfoModel
    {
        public string sno { get; set; }
        public string totalRecords { get; set; }
        public string groupId { get; set; }
        public string groupName { get; set; }
        public string groupNameKatakana { get; set; }
        public string groupImage { get; set; }
        public string status { get; set; }
        public string subGroupCount { get; set; }
        public string clubCount { get; set; }
        public string createdDate { get; set; }
    }
    public class GroupAnalyticModel
    {
        public string TotalGroup { get; set; }
        public string TotalClub { get; set; }
        public string AssignedClub { get; set; }
        public string UnAssignedClub { get; set; }
    }
    public class ManageGroupModel : PaginationFilterCommon
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupNameKatakana { get; set; }
        public string GroupCoverPhoto { get; set; }
        public string GroupDescription { get; set; }
    }

}