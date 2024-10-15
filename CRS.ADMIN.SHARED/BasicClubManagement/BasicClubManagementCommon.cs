using CRS.ADMIN.SHARED.PaginationManagement;
using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.SHARED.BasicClubManagement
{
    public class BasicClubManagementCommon : PaginationResponseCommon
    {       
      public string Status { get; set; }
      public string ClubNameEng { get; set; }
      public string ClubNameJap { get; set; }
      public string MobileNumber { get; set; }
      public string Location { get; set; }
      public string CreatedDate { get; set; }
      public string UpdatedDate { get; set; }          
      public string Sno { get; set; }
      public string ClubLogo { get; set; }
      public string LandLineCode { get; set; }
      public string AgentId { get; set; }

    }
    public class ManageBasicClubCommon
    {
        public string AgentId { get; set; }
        public string LoginId { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string LandlineNumber { get; set; }
        public string ClubName1 { get; set; }       
        public string ClubName2 { get; set; }     
        public string LocationURL { get; set; }      
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Logo { get; set; }
        public string Status { get; set; }
        public string CoverPhoto { get; set; }
        public string WebsiteLink { get; set; }
        public string TiktokLink { get; set; }
        public string TwitterLink { get; set; }
        public string InstagramLink { get; set; }
        public string LocationId { get; set; }
        public string Line { get; set; }
        public string GoogleMap { get; set; }
        public string WorkingHrFrom { get; set; }
        public string WorkingHrTo { get; set; }
        public string Holiday { get; set; }
        public string[] HolidayStr { get; set; }
        public string LastOrderTime { get; set; }
        public string LastEntryTime { get; set; }
        public string PostalCode { get; set; }
        public string Prefecture { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string BuildingRoomNo { get; set; }
        public string Pref { get; set; }
        public string LocationDDL { get; set; }
        public string GroupName { get; set; }
        public string ClosingDate { get; set; }
        public string LandLineCode { get; set; }
        public string ClubName { get; set; }
        public string SearchFilter { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public string ActionUser { get; set; }
        public string ActionPlatform { get; set; }
        public string ActionIP { get; set; }
    }
}
