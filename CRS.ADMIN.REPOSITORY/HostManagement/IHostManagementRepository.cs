using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.HostManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.HostManagement
{
    public interface IHostManagementRepository
    {
        List<HostListCommon> GetHostList(string AgentId, PaginationFilterCommon Request);
        ManageHostCommon GetHostDetail(string AgentId, string HostId);
        CommonDbResponse ManageHost(ManageHostCommon Request);
        CommonDbResponse ManageHostStatus(string AgentId, string HostId, string Status, Common Request);
        #region Manage gallery
        List<HostGalleryManagementCommon> GetGalleryImage(string AgentId, string HostId, string GalleryId = "", string SearchFilter = "");
        CommonDbResponse ManageGalleryImage(HostManageGalleryImageCommon Request);
        CommonDbResponse ManageGalleryImageStatus(string AgentId, string HostId, string GalleryId, Common Request);
        #endregion
        #region Host Identity Detail Management 
        List<HostIdentityDataCommon> GetHostIdentityDetail(string AgentId = "", string HostId = "");
        List<StaticDataCommon> GetSkillsDLL();
        #endregion
    }
}
