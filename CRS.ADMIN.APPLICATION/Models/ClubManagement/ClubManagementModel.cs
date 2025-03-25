using CRS.ADMIN.APPLICATION.Models.ClubManagerModel;
using CRS.ADMIN.APPLICATION.Models.TagManagement;
using CRS.ADMIN.APPLICATION.Resources;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.ClubManagement
{
    public class ClubManagementCommonModel
    {
        public string SearchFilter { get; set; }
        public string SearchFilterPending { get; set; }
        public string SearchFilterReject { get; set; }
        public string value { get; set; }
        public string TabValue { get; set; }
        public string ListType { get; set; }
        public List<ClubListModel> ClubListModel { get; set; }
        public List<ClubListModel> ClubPendingListModel { get; set; }
        public List<ClubListModel> ClubRejectedListModel { get; set; } = new List<ClubListModel>();
        public ManageClubModel ManageClubModel { get; set; }
        public ManageClubModel ClubHoldModel { get; set; }
        public ManageTag ManageTag { get; set; }
        public ManageManagerModel ManageManager { get; set; }
        public LineGroupModel LineGroupModel { get; set; }
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
        public string holdStatus { get; set; }
        public string ActionPlatform { get; set; }
        public string LandLineCode { get; set; }
        public string LineGroupId { get; set; }
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
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MinLength(3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Minimum_3_characters_required")]
        [MaxLength(16, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_16_characters_allowed")]
        public string LoginId { get; set; }
        [DisplayName("Email Address")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(75, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_75_characters_allowed")]
        public string Email { get; set; }
        [DisplayName("Contact Number")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression("^[0-9]{11}$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_11_digit_number")]
        public string MobileNumber { get; set; }
        [DisplayName("Landline Number")]
        [RegularExpression("^[0-9]{10,11}$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_either_10_digit_or_11_digit_number")]
        public string LandlineNumber { get; set; }
        [DisplayName("Club Name (English)")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_50_characters_allowed")]
        public string ClubName1 { get; set; }
        [DisplayName("Club Name (Katakana)")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_50_characters_allowed")]
        public string ClubName2 { get; set; }
        [DisplayName("Business Type")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string BusinessType { get; set; }
        [DisplayName("Group Name (English)")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_50_characters_allowed")]
        public string GroupName { get; set; }
        [DisplayName("Group Name (Katakana)")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_50_characters_allowed")]
        public string GroupName2 { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(200, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_200_characters_allowed")]
        public string Description { get; set; }
        [DisplayName("Location")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string LocationURL { get; set; }
        [MaxLength(30, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_30_characters_allowed")]
        public string Longitude { get; set; }
        [MaxLength(30, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_30_characters_allowed")]
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
        public string WebsiteLink { get; set; }
        [DisplayName("Tiktok")]
        public string TiktokLink { get; set; }
        [DisplayName("Twitter")]
        public string TwitterLink { get; set; }
        [DisplayName("Instagram")]
        public string InstagramLink { get; set; }
        [DisplayName("Location")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string LocationId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string CompanyName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string ceoFullName { get; set; }
        public string Line { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string GoogleMap { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string WorkingHrFrom { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string WorkingHrTo { get; set; }
        public string Holiday { get; set; }
        public string[] HolidayStr { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string LastOrderTime { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string LastEntryTime { get; set; }
        [RegularExpression(@"^(?:100|\d{1,2})$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Invalid_Tax")]
        public string Tax { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^\d{3} \d{4}$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Invalid_Postal_Code")]
        public string PostalCode { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Prefecture { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string City { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Street { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string BuildingRoomNo { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Invalid_amount")]
        public string RegularFee { get; set; }
        [RegularExpression("^[0-9]+$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Invalid_amount")]
        public string DesignationFee { get; set; }
        [RegularExpression("^[0-9]+$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Invalid_amount")]
        public string CompanionFee { get; set; }
        [RegularExpression("^[0-9]+$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Invalid_amount")]
        public string ExtensionFee { get; set; }
        //[RegularExpression("^[0-9]+$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Invalid_amount")]
        [RegularExpression(@"^(?:100|\d{1,2})$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Invalid_Tax")]
        public string Drink { get; set; }
        public string Pref { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string LocationDDL { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string BusinessTypeDDL { get; set; }
        public List<PlanList> PlanDetailList { get; set; } = new List<PlanList>();
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string CompanyAddress { get; set; }
        public string KYCDocument { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string BusinessLicenseNumber { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string LicenseIssuedDate { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Representative1_ContactName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression("^[0-9]{11}$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_11_digit_number")]
        public string Representative1_MobileNo { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Representative1_Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Representative1_Furigana { get; set; }
        public string Representative2_ContactName { get; set; }
        [RegularExpression("^[0-9]{11}$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_11_digit_number")]
        public string Representative2_MobileNo { get; set; }
        public string Representative2_Email { get; set; }
        public string Representative2_Furigana { get; set; }
        public string ClosingDate { get; set; }
        public string holdId { get; set; }
        public string LandLineCode { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string ClubName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string CeoFurigana { get; set; }

        public string CompanyNameFurigana { get; set; }
        public string CorporateRegistryDocument { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string IdentificationType { get; set; }
        public string IdentificationTypeName { get; set; }
        public string DocumentType { get; set; }
        public string KYCDocumentBack { get; set; }
        public string PassportPhoto { get; set; }
        public string InsurancePhoto { get; set; }
        public string SearchFilter { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public string OthersHoliday { get; set; }
        public string[] OthersHolidayStr { get; set; }
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
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string StaticDataValue { get; set; }
        public string japanese { get; set; }
        public string inputtype { get; set; }
        public string name { get; set; }
        public string IdentityLabel { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName  = "Required")]
        public string IdentityDescription { get; set; }
        public string PlanListId { get; set; }
        public string Id { get; set; }
        public string PlanId { get; set; }

    }
    public class ManageClubPlanCommonModel
    {
        public string SearchFilter { get; set; }
        public string ClubId { get; set; }
        public ManageClubPlanModel ManageClubPlanModel { get; set; }
        public List<ClubplanListModel> planList { get; set; } = new List<ClubplanListModel>();
    }
    public class ManageClubPlanModel
    {
        public string ClubId { get; set; }
        public List<PlanList> ClubPlanDetailList { get; set; } = new List<PlanList>();
    }
    public class ClubplanListModel
    {
        public string Id { get; set; }
        public string PlanId { get; set; }
        public string PlanListId { get; set; }
        public string PlanName { get; set; }
        public string LastEntryTime { get; set; }
        public string LastOrderTime { get; set; }
        public string NoofPeople { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string ClubId { get; set; }
    }

}