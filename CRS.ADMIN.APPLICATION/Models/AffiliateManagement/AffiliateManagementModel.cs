using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.AffiliateManagement
{
    public class ReferalCommonModel
    {
        public string ListType { get; set; }
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
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MinLength(3, ErrorMessage = "Minimum 3 characters required")]
        [MaxLength(16, ErrorMessage = "Maximum 16 characters allowed")]
        public string UserName { get; set; }
        public string LoginId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string FullName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "The field must be a 11-digit number.")]
        public string MobileNumber { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(75, ErrorMessage = "Maximum 75 characters allowed")]
        public string EmailAddress { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [RegularExpression("^\\d{3}[0-9]$", ErrorMessage = "The field must be a 4-digit number.")]
        public string BirthDateYear { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [RegularExpression("^(0?[1-9]|1[0-2])$", ErrorMessage = "The field must be a 2-digit number.")]
        public string BirthDateMonth { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [RegularExpression("^(0?[1-9]|[12]\\d|3[01]|32)$", ErrorMessage = "The field must be a 2-digit number.")]
        public string BirthDateDay { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Gender { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [RegularExpression(@"^\d{3} \d{4}$", ErrorMessage = "Invalid Postal Code")]
        public string PostalCode { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Address { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Prefecture { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string City { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Street { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string BuildingRoomNo { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string BusinessType { get; set; }
        public string CEOName { get; set; }
        public string CEONameFurigana { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
    }
}