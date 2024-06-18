using CRS.ADMIN.APPLICATION.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.AffiliateManagement
{
    public class ReferalCommonModel
    {
        public string ListType { get; set; }
        public string value { get; set; }
        public AffiliatePageAnalyticModel AffiliatePageAnalyticModel { get; set; } = new AffiliatePageAnalyticModel();
        public ManageAffiliateModel ManageAffiliate { get; set; } = new ManageAffiliateModel();
        public List<ReferralConvertedCustomerListModel> GetReferalConvertedCustomerList { get; set; } = new List<ReferralConvertedCustomerListModel>();
        public List<AffiliateManagementModel> GetAffiliateList { get; set; } = new List<AffiliateManagementModel>();
    }
    public class AffiliateManagementModel
    {
        public string SNO { get; set; }
        public string AffiliateId { get; set; }
        public string HoldAffiliateId { get; set; }
        public string AffiliateImage { get; set; }
        public string AffiliateFullName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public string RequestedDate { get; set; }
        public string Status { get; set; }
        public string ApprovedStatus { get; set; }
        public string SnsURLLink { get; set; }
        public string AffiliateType { get; set; }
    }

    public class ReferralConvertedCustomerListModel
    {
        public string SNO { get; set; }
        public string CustomerId { get; set; }
        public string ReferCode { get; set; }
        public string CustomerImage { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerUserName { get; set; }
        public string CustomerConvertedDate { get; set; }
        public string AffiliateId { get; set; }
        public string AffiliateFullName { get; set; }
        public string AffiliateAmount { get; set; }
        public string AffiliateImaeg { get; set; }
    }
    public class AffiliatePageAnalyticModel
    {
        public string TotalClick { get; set; }
        public string TotalAffiliates { get; set; }
        public string TotalConvertedCustomer { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Facebook { get; set; }
        public string Tiktok { get; set; }
        public string Line { get; set; }
        public string TwitterPercentage { get; set; }
        public string InstagramPercentage { get; set; }
        public string FacebookPercentage { get; set; }
        public string TiktokPercentage { get; set; }
        public string LinePercentage { get; set; }
    }
    public class ManageAffiliateModel
    {
        public string AffiliateId { get; set; }
        public string ListType { get; set; }
        public string SearchFilter { get; set; }
        public string StartIndex { get; set; }
        public string PageSize { get; set; }
        //[MinLength(3, ErrorMessage = "Minimum 3 characters required")]
        //[MaxLength(16, ErrorMessage = "Maximum 16 characters allowed")]
        public string UserName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string LoginId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string FullName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression("^[0-9]{11}$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_11_digit_number")]
        public string MobileNumber { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(75, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_75_characters_allowed")]
        public string EmailAddress { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression("^\\d{3}[0-9]$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_4_digit_number")]
        public string BirthDateYear { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression("^(0?[1-9]|1[0-2])$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_2_digit_number")]
        public string BirthDateMonth { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression("^(0?[1-9]|[12]\\d|3[01]|32)$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_2_digit_number")]
        public string BirthDateDay { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Gender { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^\d{3} \d{4}$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Invalid_Postal_Code")]
        public string PostalCode { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string FullNameFurigana { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Prefecture { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string City { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Street { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string BuildingRoomNo { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string BusinessType { get; set; }
        public string CEOName { get; set; }
        public string CEONameFurigana { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
    }
}