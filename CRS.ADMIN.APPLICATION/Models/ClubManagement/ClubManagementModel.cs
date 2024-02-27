using CRS.ADMIN.APPLICATION.Models.TagManagement;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.ClubManagement
{
    public class ClubManagementCommonModel
    {
        public string SearchFilter { get; set; }
        public List<ClubListModel> ClubListModel { get; set; }
        public ManageClubModel ManageClubModel { get; set; }
        public ManageTag ManageTag { get; set; }
        public List<AvailabilityTagModel> GetAvailabilityList { get; set; }
    }
    public class ClubListModel
    {
        public string SNO { get; set; }
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
        public string Action { get; set; }
        public string ClubLogo { get; set; }
        public string Sno { get; set; }
        public string ClubCategory { get; set; }
    }

    public class ClubDetailModel
    {
        public string AgentId { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Username")]
        public string LoginId { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        [Display(Name = "Club Name (English)")]
        public string ClubName1 { get; set; }
        [Display(Name = "Club Name (Katakana)")]
        public string ClubName2 { get; set; }
        [Display(Name = "Business Type")]
        public string BusinessType { get; set; }
        [Display(Name = "Group Name")]
        public string GroupName { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Location")]
        public string LocationURL { get; set; }
        [Display(Name = "Longitude")]
        public string Longitude { get; set; }
        [Display(Name = "Latitude")]
        public string Latitude { get; set; }
        public string Status { get; set; }
        [Display(Name = "Logo")]
        public string Logo { get; set; }
        [Display(Name = "Cover Photo")]
        public string CoverPhoto { get; set; }
        [Display(Name = "Business Certificate")]
        public string BusinessCertificate { get; set; }
        public string Gallery { get; set; }
        [Display(Name = "Website")]
        public string WebsiteLink { get; set; }
        [Display(Name = "Tiktok")]
        public string TiktokLink { get; set; }
        [Display(Name = "Twitter")]
        public string TwitterLink { get; set; }
        [Display(Name = "Instagram")]
        public string InstagramLink { get; set; }


    }

    public class ManageClubModel
    {
        public string AgentId { get; set; }
        [DisplayName("Username")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MinLength(3, ErrorMessage = "Minimum 3 characters required")]
        [MaxLength(16, ErrorMessage = "Maximum 16 characters allowed")]
        public string LoginId { get; set; }
        //[DisplayName("First Name")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        //[MaxLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        //public string FirstName { get; set; }
        //[DisplayName("Middle Name")]
        //[MaxLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        //public string MiddleName { get; set; }
        //[DisplayName("Last Name")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        //[MaxLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        //public string LastName { get; set; }
        [DisplayName("Email Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(75, ErrorMessage = "Maximum 75 characters allowed")]
        public string Email { get; set; }
        [DisplayName("Contact Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "The field must be a 11-digit number.")]
        public string MobileNumber { get; set; }    
        [DisplayName("Landline Number")]      
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "The field must be a 10-digit number.")]
        public string LandlineNumber { get; set; }
        [DisplayName("Club Name (English)")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        public string ClubName1 { get; set; }
        [DisplayName("Club Name (Katakana)")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        public string ClubName2 { get; set; }
        [DisplayName("Business Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string BusinessType { get; set; }
        [DisplayName("Group Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        public string GroupName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(200, ErrorMessage = "Maximum 200 characters allowed")]
        public string Description { get; set; }
        [DisplayName("Location")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string LocationURL { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(30, ErrorMessage = "Maximum 30 characters allowed")]
        public string Longitude { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(30, ErrorMessage = "Maximum 30 characters allowed")]
        public string Latitude { get; set; }
        [DisplayName("Logo")]
        public string Logo { get; set; }
        [DisplayName("Cover Photo")]
        public string CoverPhoto { get; set; }
        [DisplayName("Business Certificate")]
        public string BusinessCertificate { get; set; }
        [DisplayName("Gallery")]
        public string Gallery { get; set; }
        [DisplayName("Website")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string WebsiteLink { get; set; }
        [DisplayName("Tiktok")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string TiktokLink { get; set; }
        [DisplayName("Twitter")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string TwitterLink { get; set; }
        [DisplayName("Instagram")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string InstagramLink { get; set; }
        [DisplayName("Location")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string LocationId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string CompanyName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string ceoFullName { get; set; }
       
        public string Line { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string GoogleMap { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string WorkingHrFrom { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string WorkingHrTo { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Holiday { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string LastOrderTime { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string LastEntryTime { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [RegularExpression(@"^(?:100|\d{1,2})$", ErrorMessage = "Tax must be a number between 0 and 100.")]
        public string Tax { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [RegularExpression(@"^\d{3}-\d{4}$", ErrorMessage = "Postal Code contain 3 number before dash and 4 number after dash.")]
        public string PostalCode { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Prefecture { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string City { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Street { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string BuildingRoomNo { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string RegularFee { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string DesignationFee { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string CompanionFee { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string ExtensionFee { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Drink { get; set; }

        public string Pref { get; set; }
        public string LocationDDL { get; set; }
        public string BusinessTypeDDL { get; set; }

        public List<PlanList> PlanDetailList { get; set; } = new List<PlanList>();
        // public List<planIdentityDataModel> PlanList { get; set; }
    }

    public class plan
    {
        public string PlanId { get; set; }
        public string LastorderTime { get; set; }
        public string LastEntryTime { get; set; }
        public string MaximumReservation { get; set; }
        public string Drink { get; set; }
    }

    public class PlanList
    {
        public List<planIdentityDataModel> PlanIdentityList { get; set; }
       = new List<planIdentityDataModel>();
    }
    public class planIdentityDataModel
    {
        public string English { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string StaticDataValue { get; set; }
        public string japanese { get; set; }
        public string inputtype { get; set; }
        public string name { get; set; } 
        public string IdentityLabel { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string IdentityDescription { get; set; }
        public string PlanListId { get; set; }
        public string Id { get; set; }
        public string PlanId { get; set; }

    }

}