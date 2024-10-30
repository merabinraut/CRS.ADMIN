using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.SHARED.HostManagement
{
    public class HostListCommon : PaginationResponseCommon
    {
        public string AgentId { get; set; }
        public string HostId { get; set; }
        public string HostName { get; set; }
        public string Position { get; set; }
        public string Rank { get; set; }
        public string Age { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string ClubName { get; set; }
        public string Ratings { get; set; }
        public string TotalVisitors { get; set; }
        public string HostImage { get; set; }
        public string Address { get; set; }
        public string Height { get; set; }
      
    }

    public class ManageHostCommon : Common
    {
        public string AgentId { get; set; }
        public string HostId { get; set; }
        public string HostName { get; set; }
        public string Position { get; set; }
        public string Rank { get; set; }
        public string DOB { get; set; }
        public string ConstellationGroup { get; set; }
        public string Height { get; set; }
        public string BloodType { get; set; }
        public string PreviousOccupation { get; set; }
        public string LiquorStrength { get; set; }
        public string WebsiteLink { get; set; }
        public string TiktokLink { get; set; }
        public string TwitterLink { get; set; }
        public string InstagramLink { get; set; }
        public string ImagePath { get; set; }
        public string IconImagePath { get; set; }
        public string Line { get; set; }
        public string Address { get; set; }
        public string HostNameJapanese { get; set; }
        public string HostIntroduction { get; set; }
        public string OtherPositionRemark { get; set; }
        public List<HostIdentityDataCommon> HostIdentityDataModel { get; set; } = new List<HostIdentityDataCommon>();
    }
    public class HostIdentityDataCommon
    {
        public string IdentityLabelJapanese { get; set; }
        public string IdentityLabelEnglish { get; set; }
        public string IdentityType { get; set; }
        public string IdentityValue { get; set; }
        public string IdentityDDLType { get; set; }
        public string IdentityDescription { get; set; }
        public string InputType { get; set; }
    }

    public class InquiryListCommon
    {
        public string Sno {  get; set; }
        public string EmailAddress { get; set; }
        public string Message { get; set; }
        public string ActionDate { get; set; }
        public string ActionBy { get; set; }
    }
}
