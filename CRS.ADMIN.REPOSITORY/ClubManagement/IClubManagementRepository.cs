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

        #region "Manage Tag"
        CommonDbResponse ManageTag(ManageTagCommon request);
        List<LocationListCommon> GetLocationDDL(string clubID);
        ManageTagCommon GetTagDetails(string clubid);
        List<AvailabilityTagModelCommon> GetAvailabilityList(string cId);
        CommonDbResponse ManageClubAvailability(AvailabilityTagModelCommon request, ManageTagCommon dbRequest, string[] updatedValues);
        #endregion

        #region Manage gallery
        List<GalleryManagementCommon> GetGalleryImage(string AgentId, PaginationFilterCommon request, string GalleryId = "");
        CommonDbResponse ManageGalleryImage(ManageGalleryImageCommon Request);
        CommonDbResponse ManageGalleryImageStatus(string AgentId, string GalleryId, Common Request);
        

        #endregion
    }
}
