using CRS.ADMIN.SHARED.HostManagement;
using CRS.ADMIN.SHARED;
using System.Collections.Generic;
using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.BUSINESS.HostManagement
{
    public interface IHostManagementBusiness
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
        List<InquiryListCommon> GetInquiryListAsync(string SearchFilter,int StartIndex,int PageSize);
        #endregion
    }
}
