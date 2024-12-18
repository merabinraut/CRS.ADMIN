﻿using CRS.ADMIN.REPOSITORY.HostManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.HostManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.HostManagement
{
    public class HostManagementBusiness : IHostManagementBusiness
    {
        IHostManagementRepository _REPO;
        public HostManagementBusiness(HostManagementRepository REPO)
        {
            _REPO = REPO;
        }
        public ManageHostCommon GetHostDetail(string AgentId, string HostId)
        {
            return _REPO.GetHostDetail(AgentId, HostId);
        }

        public List<HostListCommon> GetHostList(string AgentId, PaginationFilterCommon Request)
        {
            return _REPO.GetHostList(AgentId, Request);
        }

        public CommonDbResponse ManageHost(ManageHostCommon Request)
        {
            return _REPO.ManageHost(Request);
        }

        public CommonDbResponse ManageHostStatus(string AgentId, string HostId, string Status, Common Request)
        {
            return _REPO.ManageHostStatus(AgentId, HostId, Status, Request);
        }
        #region Manage gallery
        public List<HostGalleryManagementCommon> GetGalleryImage(string AgentId, string HostId, string GalleryId = "", string SearchFilter = "")
        {
            return _REPO.GetGalleryImage(AgentId, HostId, GalleryId, SearchFilter);
        }
        public CommonDbResponse ManageGalleryImage(HostManageGalleryImageCommon Request)
        {
            return _REPO.ManageGalleryImage(Request);
        }
        public CommonDbResponse ManageGalleryImageStatus(string AgentId, string HostId, string GalleryId, Common Request)
        {
            return _REPO.ManageGalleryImageStatus(AgentId, HostId, GalleryId, Request);
        }
        #endregion
        #region Host Identity Detail Management 
        public List<HostIdentityDataCommon> GetHostIdentityDetail(string AgentId = "", string HostId = "")
        {
            return _REPO.GetHostIdentityDetail(AgentId, HostId);
        }
        public List<StaticDataCommon> GetSkillsDLL()
        {
            return _REPO.GetSkillsDLL();
        }

        public List<InquiryListCommon> GetInquiryListAsync(string SearchFilter, int StartIndex, int PageSize)
        {
            return _REPO.GetInquiryListAsync(SearchFilter,StartIndex,PageSize);
        }
        #endregion

        public CommonDbResponse UploadHostImage(string ClubName, string LocationId, string HostName, string ImagePath)
        {
            return _REPO.UploadHostImage(ClubName, LocationId, HostName, ImagePath);
        }
    }
}
