using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.ClubManagement
{
    public interface IClubManagementRepository
    {
        List<ClubListCommon> GetClubList(PaginationFilterCommon Request);
        ClubDetailCommon GetClubDetails(string AgentId);
        CommonDbResponse ManageClub(ManageClubCommon Request);
        CommonDbResponse ManageClubStatus(string AgentId, string Status, Common Request);
        CommonDbResponse ResetClubUserPassword(string AgentId, string UserId, Common Request);
        List<planIdentityDataCommon> GetClubPlanIdentityList();

        #region "Manage Tag"
        CommonDbResponse ManageTag(ManageTagCommon request);
        List<LocationListCommon> GetLocationDDL(string clubID);
        ManageTagCommon GetTagDetails(string clubid);
        #endregion

        #region Manage gallery
        List<GalleryManagementCommon> GetGalleryImage(string AgentId, PaginationFilterCommon request, string GalleryId = "");
        CommonDbResponse ManageGalleryImage(ManageGalleryImageCommon Request);
        CommonDbResponse ManageGalleryImageStatus(string AgentId, string GalleryId, Common Request);
        #endregion

        #region Event Management

        List<EventListCommon> GetEventList(PaginationFilterCommon Request, string AgentId);
        CommonDbResponse ManageEvent(EventCommon Request);
        EventCommon GetEventDetails(string AgentId, string EventId);
        #endregion
    }
}
