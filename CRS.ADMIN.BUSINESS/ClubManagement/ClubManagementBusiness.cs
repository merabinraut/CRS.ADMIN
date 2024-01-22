using CRS.ADMIN.REPOSITORY.ClubManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.ClubManagement
{
    public class ClubManagementBusiness : IClubManagementBusiness
    {
        IClubManagementRepository _REPO;
        public ClubManagementBusiness(ClubManagementRepository REPO)
        {
            _REPO = REPO;
        }

        public ClubDetailCommon GetClubDetails(string AgentId)
        {
            return _REPO.GetClubDetails(AgentId);
        }

        public List<ClubListCommon> GetClubList(string SearchFilter = "")
        {
            return _REPO.GetClubList(SearchFilter);
        }

        public CommonDbResponse ManageClub(ManageClubCommon Request)
        {
            return _REPO.ManageClub(Request);
        }

        public CommonDbResponse ManageClubStatus(string AgentId, string Status, Common Request)
        {
            return _REPO.ManageClubStatus(AgentId, Status, Request);
        }

        public CommonDbResponse ResetClubUserPassword(string AgentId, string UserId, Common Request)
        {
            return _REPO.ResetClubUserPassword(AgentId, UserId, Request);
        }

        #region "Manage Tag
        public CommonDbResponse ManageTag(ManageTagCommon Request)
        {
            return _REPO.ManageTag(Request);
        }
        public List<LocationListCommon> GetLocationDDL(string clubID)
        {
            return _REPO.GetLocationDDL(clubID);
        }
        public ManageTagCommon GetTagDetails(string clubid)
        {
            return _REPO.GetTagDetails(clubid);
        }
        #endregion

        #region Manage gallery
        public List<GalleryManagementCommon> GetGalleryImage(string AgentId, string GalleryId = "", string SearchFilter = "")
        {
            return _REPO.GetGalleryImage(AgentId, GalleryId, SearchFilter);
        }
        public CommonDbResponse ManageGalleryImage(ManageGalleryImageCommon Request)
        {
            return _REPO.ManageGalleryImage(Request);
        }
        public CommonDbResponse ManageGalleryImageStatus(string AgentId, string GalleryId, Common Request)
        {
            return _REPO.ManageGalleryImageStatus(AgentId, GalleryId, Request);
        }
        #endregion
    }
}
