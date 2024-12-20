using CRS.ADMIN.SHARED;
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
        #region GROUP SECTION
        public CommonDbResponse BlockGroup(string groupId, Common request)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_block_group]";
            sp_name += "@GroupId=" + _dao.FilterString(groupId);
            sp_name += ",@ActionUser=" + _dao.FilterString(request.ActionUser);
            sp_name += ",@ActionIP=" + _dao.FilterString(request.ActionIP);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(request.ActionPlatform);

            var dbResponse = _dao.ParseCommonDbResponse(sp_name);
            return dbResponse;
        }

        public CommonDbResponse DeleteGroup(string groupId, Common request)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_delete_group]";
            sp_name += "@GroupId=" + _dao.FilterString(groupId);
            sp_name += ",@ActionUser=" + _dao.FilterString(request.ActionUser);
            sp_name += ",@ActionIP=" + _dao.FilterString(request.ActionIP);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(request.ActionPlatform);

            var dbResponse = _dao.ParseCommonDbResponse(sp_name);
            return dbResponse;
        }

        public GroupAnalyticModelCommon GetGroupAnalytic()
        {
            string sp_name = "[dbo].[sproc_admin_group_analytic_overview]";
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

        public ManageGroupModelCommon GetGroupDetail(string groupId)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_group_detail]";
            sp_name += "@GroupId=" + _dao.FilterString(groupId);
            var dbReponse = _dao.ExecuteDataRow(sp_name);
            if (dbReponse != null)
            {
                return new ManageGroupModelCommon()
                {
                    GroupId = _dao.ParseColumnValue(dbReponse, "GroupId").ToString(),
                    GroupName = _dao.ParseColumnValue(dbReponse, "GroupName").ToString(),
                    GroupNameKatakana = _dao.ParseColumnValue(dbReponse, "GroupNameKatakana").ToString(),
                    GroupDescription = _dao.ParseColumnValue(dbReponse, "GroupDescription").ToString(),
                    GroupCoverPhoto = _dao.ParseColumnValue(dbReponse, "GroupCoverPhoto").ToString(),
                };
            }
            else
            {
                return new ManageGroupModelCommon();
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


        public CommonDbResponse ManageGroup(ManageGroupModelCommon commonModel)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_add_update_group]";
            sp_name += "@GroupId=" + _dao.FilterString(commonModel.GroupId);
            sp_name += ",@GroupName=" + _dao.FilterString(commonModel.GroupName);
            sp_name += ",@GroupNameKatakana=" + (!string.IsNullOrEmpty(commonModel.GroupNameKatakana) ? "N" + _dao.FilterString(commonModel.GroupNameKatakana) : _dao.FilterString(commonModel.GroupNameKatakana));
            sp_name += ",@GroupDescription=" + _dao.FilterString(commonModel.GroupDescription);
            sp_name += ",@GroupCoverPhoto=" + _dao.FilterString(commonModel.GroupCoverPhoto);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonModel.ActionUser);
            sp_name += ",@ActionIP=" + _dao.FilterString(commonModel.ActionIP);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(commonModel.ActionPlatform);
            var dbResponse = _dao.ParseCommonDbResponse(sp_name);
            return dbResponse;

        }

        public CommonDbResponse UnblockGroup(string groupId, Common request)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_unblock_group]";
            sp_name += "@GroupId=" + _dao.FilterString(groupId);
            sp_name += ",@ActionUser=" + _dao.FilterString(request.ActionUser);
            sp_name += ",@ActionIP=" + _dao.FilterString(request.ActionIP);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(request.ActionPlatform);

            var dbResponse = _dao.ParseCommonDbResponse(sp_name);
            return dbResponse;
        }
        #endregion

        #region SUB GROUP SECTION
        public SubGroupModelCommon GetSubGroupByGroupId(string groupId, PaginationFilterCommon paginationFilter)
        {
            var response = new SubGroupModelCommon
            {
                SubGroupInfoList = new List<SubGroupInfoModelCommon>()
            };

            string sp_name = "EXEC [dbo].[sproc_admin_get_subgroup]";
            sp_name += "@GroupId=" + _dao.FilterString(groupId);
            sp_name += ",@SearchFilter=" + _dao.FilterString(paginationFilter.SearchFilter);
            sp_name += ",@Skip=" + paginationFilter.Skip;
            sp_name += ",@Take=" + paginationFilter.Take;

            var dbResponse = _dao.ExecuteDataTable(sp_name);
            if (dbResponse.Rows.Count > 0 && dbResponse != null)
            {
                response.SubGroupInfoList = _dao.DataTableToListObject<SubGroupInfoModelCommon>(dbResponse).ToList();
                foreach (var item in response.SubGroupInfoList)
                {
                    string sp_name_1 = "EXEC [dbo].[sproc_admin_subgroup_club_info]";
                    sp_name_1 += "@SubGroupId=" + _dao.FilterString(item.SubGroupId);
                    var dbResponse1 = _dao.ExecuteDataTable(sp_name_1);
                    if (dbResponse1 != null)
                    {
                        item.ClubShortInfo.AddRange(_dao.DataTableToListObject<SubGroupClubInfoCommon>(dbResponse1));
                    }
                }
                return response;
            }
            else
            {
                return new SubGroupModelCommon
                {
                    SubGroupInfoList = new List<SubGroupInfoModelCommon>()
                };
            }
        }


        public CommonDbResponse ManageSubGroup(ManageSubGroupModelCommon commonRequest)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_subgroup_addupdated]";
            sp_name += "@GroupId=" + _dao.FilterString(commonRequest.GroupId);
            sp_name += ",@SubGroupId=" + _dao.FilterString(commonRequest.SubGroupId);
            sp_name += ",@SubGroupName=" + (!string.IsNullOrEmpty(commonRequest.SubGroupName) ? "N" + _dao.FilterString(commonRequest.SubGroupName) : _dao.FilterString(commonRequest.SubGroupName));
            sp_name += ",@SubGroupNameKatakana=" + (!string.IsNullOrEmpty(commonRequest.SubGroupNameKatakana) ? "N" + _dao.FilterString(commonRequest.SubGroupNameKatakana) : _dao.FilterString(commonRequest.SubGroupNameKatakana));
            sp_name += ",@ActionUser=" + _dao.FilterString(commonRequest.ActionUser);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(commonRequest.ActionPlatform);
            sp_name += ",@ActionIP=" + _dao.FilterString(commonRequest.ActionIP);

            var dbResponse = _dao.ParseCommonDbResponse(sp_name);
            return dbResponse;
        }

        public ManageSubGroupModelCommon GetSubGroupDetail(string subGroupId)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_subgroup_detail]";
            sp_name += "@SubGroupId=" + _dao.FilterString(subGroupId);
            var dbResponse = _dao.ExecuteDataRow(sp_name);

            if (dbResponse != null)
            {
                return new ManageSubGroupModelCommon()
                {
                    GroupId = _dao.ParseColumnValue(dbResponse, "GroupId").ToString(),
                    SubGroupId = _dao.ParseColumnValue(dbResponse, "SubGroupId").ToString(),
                    SubGroupName = _dao.ParseColumnValue(dbResponse, "SubGroupName").ToString(),
                    GroupName = _dao.ParseColumnValue(dbResponse, "GroupName").ToString(),
                    GroupNameKatakana = _dao.ParseColumnValue(dbResponse, "GroupNameKatakana").ToString()
                };
            }
            else
            {
                return new ManageSubGroupModelCommon();
            }
        }

        public CommonDbResponse DeleteSubGroup(string subGroupId, Common request)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_subgroup_delete]";
            sp_name += "@SubGroupId=" + _dao.FilterString(subGroupId);
            sp_name += ",@ActionIP=" + _dao.FilterString(request.ActionIP);
            sp_name += ",@ActionUser=" + _dao.FilterString(request.ActionUser);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(request.ActionPlatform);

            var dbResponse = _dao.ParseCommonDbResponse(sp_name);
            return dbResponse;
        }

        public CommonDbResponse ManageSubGroupClub(ManageSubGroupClubModelCommon commonModel)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_subgroup_club_addupdate]";
            sp_name += "@XMLInput=" + _dao.FilterString(commonModel.xmlInput);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonModel.ActionUser);
            sp_name += ",@ActionIP=" + _dao.FilterString(commonModel.ActionIP);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(commonModel.ActionPlatform);

            var dbResponse = _dao.ParseCommonDbResponse(sp_name);
            return dbResponse;
        }

        public ManageSubGroupClubModelCommon GetSubGroupClubDetailById(string subGroupId)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_subgroup_club_detail]";
            sp_name += "@SubGroupId=" + _dao.FilterString(subGroupId);

            var dbResponse = _dao.ExecuteDataRow(sp_name);
            if (dbResponse != null)
            {
                return new ManageSubGroupClubModelCommon()
                {
                    SubGroupId = _dao.ParseColumnValue(dbResponse, "SubGroupId").ToString(),
                    ClubId = _dao.ParseColumnValue(dbResponse, "ClubId").ToString(),
                    LocationId = _dao.ParseColumnValue(dbResponse, "LocationId").ToString(),
                    TotalClubCount = _dao.ParseColumnValue(dbResponse, "TotalClubCount").ToString()
                };
            }
            else
            {
                return new ManageSubGroupClubModelCommon();
            }
        }
        public CommonDbResponse DeleteSubGroupClub(string id, string subgroupid, string clubid, Common request)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_subgroup_club_delete]";
            sp_name += "@Id=" + _dao.FilterString(id);
            sp_name += "@SubGroupId=" + _dao.FilterString(subgroupid);
            sp_name += "@ClubId=" + _dao.FilterString(clubid);

            var dbResponse = _dao.ParseCommonDbResponse(sp_name);
            return dbResponse;
        }

        #endregion

        #region GROUP GALLERY
        public List<GroupGalleryInfoModelCommon> GetGalleryListById(string groupId)
        {

            string sp_name = "EXEC [dbo].[sproc_admin_group_gallery_list]";
            sp_name += "@GroupId=" + _dao.FilterString(groupId);


            var dbResponse = _dao.ExecuteDataTable(sp_name);
            if (dbResponse != null && dbResponse.Rows.Count > 0)
                return _dao.DataTableToListObject<GroupGalleryInfoModelCommon>(dbResponse).ToList();
            else
                return new List<GroupGalleryInfoModelCommon>();
        }

        public CommonDbResponse ManageGroupGallery(ManageGroupGalleryModelCommon commonModel)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_group_gallery_addupdate]";
            sp_name += "@ImageId=" + _dao.FilterString(commonModel.ImageId);
            sp_name += ",@GroupId=" + _dao.FilterString(commonModel.GroupId);
            sp_name += ",@ImageTitle=" + _dao.FilterString(commonModel.ImageTitle);
            sp_name += ",@GalleryImage=" + _dao.FilterString(commonModel.GalleryImage);
            sp_name += ",@ActionUser=" + _dao.FilterString(commonModel.ActionUser);
            sp_name += ",@ActionIP=" + _dao.FilterString(commonModel.ActionIP);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(commonModel.ActionPlatform);

            var dbResponse = _dao.ParseCommonDbResponse(sp_name);
            return dbResponse;
        }

        public ManageGroupGalleryModelCommon GetGroupGalleryDetail(string imageid)
        {
            string sp_name = "[dbo].[sproc_admin_group_gallery_detail]";
            sp_name += "@ImageId=" + _dao.FilterString(imageid);

            var dbResponse = _dao.ExecuteDataRow(sp_name);
            if (dbResponse != null)
            {
                return new ManageGroupGalleryModelCommon()
                {
                    ImageId = _dao.ParseColumnValue(dbResponse, "ImageId").ToString(),
                    GalleryImage = _dao.ParseColumnValue(dbResponse, "GalleryImage").ToString(),
                    ImageTitle = _dao.ParseColumnValue(dbResponse, "ImageTitle").ToString(),
                };
            }
            else
                return new ManageGroupGalleryModelCommon();
        }

        public CommonDbResponse DeleteImage(string imageid, string groupid, Common request)
        {
            string sp_name = "EXEC [dbo].[sproc_admin_group_delete_image]";
            sp_name += "@ImageId=" + _dao.FilterString(imageid);
            sp_name += ",@GroupId=" + _dao.FilterString(groupid);
            sp_name += ",@ActionUser=" + _dao.FilterString(request.ActionUser);
            sp_name += ",@ActionIP=" + _dao.FilterString(request.ActionIP);
            sp_name += ",@ActionPlatform=" + _dao.FilterString(request.ActionPlatform);

            return _dao.ParseCommonDbResponse(sp_name);
        }
        #endregion
    }
}
