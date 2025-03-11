﻿using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CRS.ADMIN.REPOSITORY.ClubManagement
{
    public interface IClubManagementRepository
    {
        List<ClubListCommon> GetClubList(PaginationFilterCommon Request);
        List<ClubListCommon> GetClubPendingList(PaginationFilterCommon Request);
        List<ClubListCommon> GetClubRejectedList(PaginationFilterCommon Request);
        ClubDetailCommon GetClubPendingDetails(string AgentId, String holdId = "", String culture = "");
        ClubDetailCommon GetplanPendingDetails(string AgentId, String holdId = "", String culture = "");
        CommonDbResponse ManageApproveReject(string holdId, string flag, string AgentId, String culture = "", ManageClubCommon Request = null, SqlConnection connection = null, SqlTransaction transaction = null);
        ClubDetailCommon GetClubDetails(string AgentId, String culture = "");
        CommonDbResponse ManageClub(ManageClubCommon Request);
        CommonDbResponse ManageClubStatus(string AgentId, string Status, Common Request, SqlConnection connection = null, SqlTransaction transaction = null);
        CommonDbResponse ResetClubUserPassword(string AgentId, string UserId, Common Request, SqlConnection connection = null, SqlTransaction transaction = null);
        List<PlanListCommon> GetClubPlanIdentityList(string culture);
        List<planIdentityDataCommon> GetClubPlanIdentityListAddable(string culture);

        #region "Manage Tag"
        CommonDbResponse ManageTag(ManageTagCommon request);
        List<LocationListCommon> GetLocationDDL(string clubID);
        ManageTagCommon GetTagDetails(string clubid);
        List<AvailabilityTagModelCommon> GetAvailabilityList(string cId, string culture);
        CommonDbResponse ManageClubAvailability(AvailabilityTagModelCommon request, ManageTagCommon dbRequest, string[] updatedValues);
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
        #region club Manager
        ManageManagerCommon GetManagerDetails(string AgentId);
        CommonDbResponse ManageManager(ManageManagerCommon request);
        #endregion
        CommonDbResponse ManageClubCognitoDetail(string clubId, string loginId, string cognitoUserId, SqlConnection connection = null, SqlTransaction transaction = null);
    }
}
