using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.SHARED.GroupManagement
{
    public class GroupInfoModelCommon
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
    public class GroupAnalyticModelCommon
    {
        public string TotalGroup { get; set; }
        public string TotalClub { get; set; }
        public string AssignedClub { get; set; }
        public string UnAssignedClub { get; set; }
    }
    public class ManageGroupModelCommon : PaginationFilterCommon
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupNameKatakana { get; set; }
        public string GroupCoverPhoto { get; set; }
        public string GroupDescription { get; set; }
        public string ActionUser { get; set; }
        public string ActionPlatform { get; set; } = "ADMIN";
        public string ActionIP { get; set; }
    }
}
