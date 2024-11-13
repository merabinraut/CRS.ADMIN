using CRS.ADMIN.APPLICATION.Models.BasicClubManagement;
using CRS.ADMIN.APPLICATION.Models.ClubManagement;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using CRS.ADMIN.APPLICATION.Resources;

namespace CRS.ADMIN.APPLICATION.Models.BasicClubManagement
{
    public class BasicClubManagementModel
    {
        public string SearchFilter { get; set; }
        public List<BasicClubListModel> BasicClubList { get; set; }=new List<BasicClubListModel>();
        public ManageBasicClubModel ManageBasicClub { get; set; } 
        public ManagePremiumClubModel ManagePremiumClub { get; set; }
    }
    public class BasicClubListModel
    {
        public string SNO { get; set; }       
        public string AgentId { get; set; }
        public string Status { get; set; }
        public string ClubNameEng { get; set; }
        public string ClubNameJap { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public string ClubLogo { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string Ratings { get; set; }
        public string Action { get; set; }
        public string LandLineCode { get; set; }
    }
    public class ManageBasicClubModel
    {
        public string AgentId { get; set; }
        [DisplayName("Username")]
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MinLength(3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Minimum_3_characters_required")]
        [MaxLength(16, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_16_characters_allowed")]
        public string LoginId { get; set; }

        [DisplayName("Email Address")]
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(75, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_75_characters_allowed")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
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
        public string Line { get; set; }
        public string GoogleMap { get; set; }
        public string WorkingHrFrom { get; set; }
        public string WorkingHrTo { get; set; }
        public string Holiday { get; set; }
        public string[] HolidayStr { get; set; }
        public string LastOrderTime { get; set; }
        public string LastEntryTime { get; set; }
       
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^\d{3} \d{4}$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Invalid_Postal_Code")]
        public string PostalCode { get; set; }
      
        public string Prefecture { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string City { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Street { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string BuildingRoomNo { get; set; }              
        public string Pref { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string LocationDDL { get; set; }
        [DisplayName("Group Name (English)")]
        //[MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_50_characters_allowed")]
        public string GroupName { get; set; }
        public string ClosingDate { get; set; }      
        public string LandLineCode { get; set; }
        public string ClubName { get; set; }        
        public string SearchFilter { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class ManagePremiumClubModel
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
       
        [DisplayName("Landline Number")]
        [RegularExpression("^[0-9]{10,11}$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_either_10_digit_or_11_digit_number")]
        public string LandlineNumber { get; set; }
        [DisplayName("Contact Number")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression("^[0-9]{11}$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_11_digit_number")]
        public string MobileNumber { get; set; }
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
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_50_characters_allowed")]
        public string GroupName { get; set; }
        [DisplayName("Group Name (Katakana)")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_50_characters_allowed")]
        public string GroupName2 { get; set; }
        [MaxLength(200, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_200_characters_allowed")]
        public string Description { get; set; }
        [DisplayName("Location")]
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
        [RegularExpression(@"^(?:100|\d{1,2})$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Invalid_Tax")]
        public string Drink { get; set; }
        public string Pref { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string LocationDDL { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string BusinessTypeDDL { get; set; }
        public List<PlanList> PlanDetailList { get; set; } = new List<PlanList>();
        public string CompanyAddress { get; set; }
        public string KYCDocument { get; set; }
        public string BusinessLicenseNumber { get; set; }
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

    }
}