using CRS.ADMIN.REPOSITORY.ClubManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CRS.ADMIN.BUSINESS.ClubManagement
{
    public class ClubManagementBusiness : IClubManagementBusiness
    {
        IClubManagementRepository _REPO;
        public ClubManagementBusiness(ClubManagementRepository REPO)
        {
            _REPO = REPO;
        }

        public ClubDetailCommon GetClubDetails(string AgentId, String culture = "")
        {
            return _REPO.GetClubDetails(AgentId, culture);
        }

        public List<ClubListCommon> GetClubList(PaginationFilterCommon Request)
        {
            return _REPO.GetClubList(Request);
        }
        public List<ClubListCommon> GetClubPendingList(PaginationFilterCommon Request)
        {
            return _REPO.GetClubPendingList(Request);
        }
        public List<ClubListCommon> GetClubRejectedList(PaginationFilterCommon Request)
        {
            return _REPO.GetClubRejectedList(Request);
        }
        public ClubDetailCommon GetClubPendingDetails(string AgentId, String holdId = "", String culture = "")
        {
            return _REPO.GetClubPendingDetails(AgentId, holdId, culture);
        }
        public ClubDetailCommon GetplanPendingDetails(string AgentId, String holdId = "", String culture = "")
        {
            return _REPO.GetplanPendingDetails(AgentId, holdId, culture);
        }
        public CommonDbResponse ManageApproveReject(string holdId, string flag, string AgentId, String culture = "", ManageClubCommon Request = null, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            return _REPO.ManageApproveReject(holdId, flag, AgentId, culture, Request, connection, transaction);
        }
        public List<PlanListCommon> GetClubPlanIdentityList(string culture)
        {
            return _REPO.GetClubPlanIdentityList(culture);
        }
        public List<planIdentityDataCommon> GetClubPlanIdentityListAddable(string culture)
        {
            return _REPO.GetClubPlanIdentityListAddable(culture);
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
        public List<AvailabilityTagModelCommon> GetAvailabilityList(string cId, string culture)
        {
            return _REPO.GetAvailabilityList(cId, culture);
        }
        public CommonDbResponse ManageClubAvailability(AvailabilityTagModelCommon request, ManageTagCommon dbRequest, string[] updatedValues)
        {
            return _REPO.ManageClubAvailability(request, dbRequest, updatedValues);
        }
        #endregion

        #region Manage gallery
        public List<GalleryManagementCommon> GetGalleryImage(string AgentId, PaginationFilterCommon request, string GalleryId = "")
        {
            return _REPO.GetGalleryImage(AgentId, request, GalleryId);
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
        #region Event Management

        public List<EventListCommon> GetEventList(PaginationFilterCommon Request, string AgentId)
        {
            return _REPO.GetEventList(Request, AgentId);
        }
        public CommonDbResponse ManageEvent(EventCommon Request)
        {
            return _REPO.ManageEvent(Request);
        }
        public EventCommon GetEventDetails(string AgentId, string EventId)
        {
            return _REPO.GetEventDetails(AgentId, EventId);
        }
        #endregion
        #region club Manager
        public ManageManagerCommon GetManagerDetails(string AgentId)
        {
            return _REPO.GetManagerDetails(AgentId);
        }
        public CommonDbResponse ManageManager(ManageManagerCommon request)
        {
            return _REPO.ManageManager(request);
        }
        #endregion
    }
}
