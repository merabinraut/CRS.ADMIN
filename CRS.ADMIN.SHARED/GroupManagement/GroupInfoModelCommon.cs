using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.SHARED.GroupManagement
{
    #region GROUP MODEL
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
    #endregion

    #region SUB GROUP MODEL

    public class SubGroupModelCommon
    {
        public List<SubGroupInfoModelCommon> SubGroupInfoList { get; set; }
        public List<SubGroupClubInfoCommon> ClubShortInfo { get; set; }
    }
    public class SubGroupInfoModelCommon
    {
        public string SubGroupName { get; set; }
        public string GroupName { get; set; }
        public string GroupNameKatakana { get; set; }
        public string SubGroupId { get; set; }
        public string GroupId { get; set; }
        public string Status { get; set; }
        public string CreatedOn { get; set; }
        public string TotalClubCount { get; set; }
    }
    public class SubGroupClubInfoCommon
    {
        public string ClubId { get; set; }
        public string ClubName { get; set; }
        public string ClubNameJp { get; set; }
        public string ClubLogo { get; set; }
        public string LocationName { get; set; }
        public string CreatedOn { get; set; }
    }
    public class ManageSubGroupModelCommon : PaginationFilterCommon
    {
        public string SubGroupName { get; set; }
        public string SubGroupNameKatakana { get; set; }
        public string GroupName { get; set; }
        public string GroupNameKatakana { get; set; }
        public string SubGroupId { get; set; }
        public string GroupId { get; set; }
        public string ActionUser { get; set; }
        public string ActionPlatform { get; set; } = "ADMIN";
        public string ActionIP { get; set; }
    }

    public class ManageSubGroupClubModelCommon
    {
        public string GroupName { get; set; }
        public string GroupNameKatakana { get; set; }
        public string SubGroupId { get; set; }
        public string LocationId { get; set; }
        public string ClubId { get; set; }
        public string TotalClubCount { get; set; }
        public string ActionUser { get; set; }
        public string ActionPlatform { get; set; } = "ADMIN";
        public string ActionIP { get; set; }
        public string xmlInput { get; set; }
    }
    #endregion

    #region GROUP GALLERY
    public class GroupGalleryInfoModelCommon
    {
        public string ImageId { get; set; }
        public string GroupId { get; set; }
        public string ImageTitle { get; set; }
        public string ImagePath { get; set; }
        public string UpdatedDate { get; set; }
        public string ImageCount { get; set; }
    }
    public class ManageGroupGalleryModelCommon
    {
        public string ImageId { get; set; }
        public string GroupId { get; set; }
        public string ImageTitle { get; set; }
        public string GalleryImage { get; set; }
        public string ActionUser { get; set; }
        public string ActionPlatform { get; set; } = "ADMIN";
        public string ActionIP { get; set; }
    }

    #endregion
}


