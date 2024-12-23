using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.APPLICATION.Models.GroupManagement
{
    #region GROUP MODEL
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
    #endregion
    #region SUB-GROUP MODEL
    public class CommonSubGroupModel
    {
        public ManageSubGroupModel ManageSubGroup { get; set; } = new ManageSubGroupModel();
        public ManageSubGroupClubModel ManageSubGroupClub { get; set; } = new ManageSubGroupClubModel();
        public List<SubGroupInfoModel> SubGroupInfoList { get; set; } = new List<SubGroupInfoModel>();
    }

    public class SubGroupInfoModel
    {
        public string SubGroupName { get; set; }
        public string GroupName { get; set; }
        public string GroupNameKatakana { get; set; }
        public string SubGroupId { get; set; }
        public string GroupId { get; set; }
        public string Status { get; set; }
        public string CreatedOn { get; set; }
        public string TotalClubCount { get; set; }
        public int TotalRecords { get; set; }
        public List<SubGroupClubInfo> ClubShortInfo { get; set; } = new List<SubGroupClubInfo>();
    }
    public class SubGroupClubInfo
    {
        public string ClubId { get; set; }
        public string Id { get; set; }
        public string ClubName { get; set; }
        public string ClubNameJp { get; set; }
        public string ClubLogo { get; set; }
        public string LocationName { get; set; }
        public string CreatedOn { get; set; }
    }
    public class ManageSubGroupModel : PaginationFilterCommon
    {
        public string SubGroupName { get; set; }
        public string GroupName { get; set; }
        public string GroupNameKatakana { get; set; }
        public string SubGroupId { get; set; }
        public string GroupId { get; set; }
    }
    public class AssignedClubInfo
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public AssignedClubInfo(string value, string text)
        {
            Value = value;
            Text = text;
        }
    }
    public class ManageSubGroupClubModel : PaginationFilterCommon
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupNameKatakana { get; set; }
        public string SubGroupId { get; set; }
        public string LocationId { get; set; }
        public string ClubId { get; set; }
        public string TotalClubCount { get; set; }
        public string xmlInput { get; set; }
    }

    public class LocationClubMapping
    {
        public string locationId { get; set; }
        public int clubCount { get; set; } = 0;
        public List<string> clubs { get; set; }
    }
    #endregion

    #region GROUP GALLERY 
    public class CommonGroupGalleryModel
    {
        public string SearchFilter { get; set; }
        public List<GroupGalleryInfoModel> GroupGalleryList { get; set; }
        public ManageGroupGalleryModel ManageGroupGallery { get; set; }
    }
    public class GroupGalleryInfoModel
    {
        public string ImageId { get; set; }
        public string GroupId { get; set; }
        public string ImageTitle { get; set; }
        public string ImagePath { get; set; }
        public string UpdatedDate { get; set; }
        public string ImageCount { get; set; }
        public int TotalRecords { get; set; }
    }
    public class ManageGroupGalleryModel : PaginationFilterCommon
    {
        public string ImageId { get; set; }
        public string GroupId { get; set; }
        public string ImageTitle { get; set; }
        public string GalleryImage { get; set; }
    }
    #endregion
}