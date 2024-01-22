using CRS.ADMIN.SHARED.HostManagement;
using CRS.ADMIN.SHARED;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.HostManagement
{
    public interface IHostManagementBusiness
    {
        List<HostListCommon> GetHostList(string AgentId, string SearchFilter = "");
        ManageHostCommon GetHostDetail(string AgentId, string HostId);
        CommonDbResponse ManageHost(ManageHostCommon Request);
        CommonDbResponse ManageHostStatus(string AgentId, string HostId, string Status, Common Request);
        #region Manage gallery
        List<HostGalleryManagementCommon> GetGalleryImage(string AgentId, string HostId, string GalleryId = "", string SearchFilter = "");
        CommonDbResponse ManageGalleryImage(HostManageGalleryImageCommon Request);
        CommonDbResponse ManageGalleryImageStatus(string AgentId, string HostId, string GalleryId, Common Request);
        #endregion
    }
}
