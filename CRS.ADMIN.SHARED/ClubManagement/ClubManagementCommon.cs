using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.SHARED.ClubManagement
{
    public class ClubListCommon : PaginationResponseCommon
    {
        public string LoginId { get; set; }
        public string AgentId { get; set; }
        public string Status { get; set; }
        public string ClubNameEng { get; set; }
        public string ClubNameJap { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string Rank { get; set; }
        public string Ratings { get; set; }
        public string Sno { get; set; }
        public string ClubLogo { get; set; }
        public string ClubCategory { get; set; }
    }

    public class ClubDetailCommon
    {
        public string AgentId { get; set; }
        public string UserId { get; set; }
        public string LoginId { get; set; }
        public string LocationId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string ClubName1 { get; set; }
        public string ClubName2 { get; set; }
        public string BusinessType { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string LocationURL { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Status { get; set; }
        public string Logo { get; set; }
        public string CoverPhoto { get; set; }
        public string BusinessCertificate { get; set; }
        public string Gallery { get; set; }
        public string WebsiteLink { get; set; }
        public string TiktokLink { get; set; }
        public string TwitterLink { get; set; }
        public string InstagramLink { get; set; }
        public string CompanyName { get; set; }
        public string GoogleMap { get; set; }
        public string WorkingHrFrom { get; set; }
        public string WorkingHrTo { get; set; }
        public string Holiday { get; set; }
        public string LastOrderTime { get; set; }
        public string LastEntryTime { get; set; }
        public string Tax { get; set; }
        public string PostalCode { get; set; }
        public string Prefecture { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string BuildingRoomNo { get; set; }
        public string RegularFee { get; set; }
        public string DesignationFee { get; set; }
        public string CompanionFee { get; set; }
        public string ExtensionFee { get; set; }
        public string Drink { get; set; }
        public string Pref { get; set; }
        public string LandLineNumber { get; set; } 
        public string Line { get; set; }
        public string ceoFullName { get; set; }
        
        public List<PlanListCommon> PlanDetailList { get; set; } = new List<PlanListCommon>();
    }

    public class ManageClubCommon : Common
    {
        public string AgentId { get; set; }
        public string LoginId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string ClubName1 { get; set; }
        public string ClubName2 { get; set; }
        public string BusinessType { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string LocationURL { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Logo { get; set; }
        public string CoverPhoto { get; set; }
        public string BusinessCertificate { get; set; }
        public string Gallery { get; set; }
        public string WebsiteLink { get; set; }
        public string TiktokLink { get; set; }
        public string TwitterLink { get; set; }
        public string InstagramLink { get; set; }
        public string LocationId { get; set; }
        public string CompanyName { get; set; }
        public string ceoFullName { get; set; }
        public string Line { get; set; }

        public string GoogleMap { get; set; }
        public string LandLineNumber { get; set; }
        public string WorkingHrFrom { get; set; }
        public string WorkingHrTo { get; set; }
        public string Holiday { get; set; }
        public string LastOrderTime { get; set; }
        public string LastEntryTime { get; set; }
        public string Tax { get; set; }
        public string PostalCode { get; set; }
        public string Prefecture { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string BuildingRoomNo { get; set; }
        public string RegularFee { get; set; }
        public string DesignationFee { get; set; }
        public string CompanionFee { get; set; }
        public string ExtensionFee { get; set; }
        public string Drink { get; set; }
        public string Pref { get; set; }
        public object data { get; set; }
        public List<PlanListCommon> PlanDetailList { get; set; } = new List<PlanListCommon>();
    }


    public class PlanListCommon
    {
        public List<planIdentityDataCommon> PlanIdentityList { get; set; }
       = new List<planIdentityDataCommon>();
    }
    public class planIdentityDataCommon
    {
        public string English { get; set; }
        public string StaticDataValue { get; set; }
        public string japanese { get; set; }
        public string inputtype { get; set; }
        public string name { get; set; }
        public string IdentityLabel { get; set; }
        public string IdentityDescription { get; set; }
        public string PlanListId { get; set; }
        public string Id { get; set; }
        public string PlanId { get; set; }
    }
}
