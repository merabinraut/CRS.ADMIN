using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.SHARED
{
    public class UserCommon
    {
        //[Display(Name = "User Id")]
        public string UserId { get; set; }
        //[Display(Name = "Agent Id")]
        public string AgentId { get; set; }
        public string ParentId { get; set; }
        //[Display(Name = "Name")]
        public string FullName { get; set; }
        public string Email { get; set; }
        // [Display(Name = "Mobile Number")]
        public string MobileNo { get; set; }
        public string Status { get; set; }
        //[Display(Name = "User Id")]
        public string UserName { get; set; }
        //[Display(Name = "Member From")]
        public string CreatedLocalDate { get; set; }
        public string Balance { get; set; }
        public string PPImage { get; set; }
        public string AgentStatus { get; set; }
        public string KycStatus { get; set; }
        public string Action { get; set; }
        //[Display(Name = "Amount To Add")]
        public string BalanceToAdd { get; set; }
        public string Remarks { get; set; }
        public string ActionUser { get; set; }
        public string ActionIP { get; set; }
        public string ActionBrowser { get; set; }
        public string RoleName { get; set; }
        public string Gender { get; set; }
        public string CardNo { get; set; }
        public string CardExpiryDate { get; set; }
        public string OTPCode { get; set; }
        public string Session { get; set; }
        public string IpAddress { get; set; }
        public string QRCheck { get; set; }
        public string CustomerId { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedIp { get; set; }
        public string OperationType { get; set; }
        public string AgentName { get; set; }
        public string TotalRecords { get; set; }
        public string TotalRecord { get; set; }
        public string AgentEmail { get; set; }
        public string AgentMobileNumber { get; set; }
        public string AgentBalance { get; set; }
        public string AgentCreditLimit { get; set; }
        public string AgentOperationType { get; set; }
        public string HoldStatus { get; set; }
        public string CommissionCategory { get; set; }
        public string ChargeCategory { get; set; }
        public string CreateDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string HoldType { get; set; }
    }
    public class UserFilterCommon
    {
        [RegularExpression("^[9][0-9]*$", ErrorMessage = "Phone Number Start With 9")]
        [MaxLength(10, ErrorMessage = "Mobile Number Max Length is Invalid"), MinLength(10, ErrorMessage = "Mobile Number Minimum Length is Invalid")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        [Display(Name = "Email Address")]
        //[EmailAddress]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Email Format Invalid")]
        public string Email { get; set; }
        //[Display(Name = "Status")]
        //public string KycStatus { get; set; }
        [Display(Name = "From Date")]
        public string FromDate { get; set; }
        [Display(Name = "To Date")]
        public string ToDate { get; set; }

        public string UpdatedFromDate { get; set; }

        public string UpdatedToDate { get; set; }

        public string MerchantType { get; set; }
        public string UserType { get; set; }
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        public string HoldType { get; set; }

        [Display(Name = "Charge Category")]
        public string ChargeCategory { get; set; }

        [Display(Name = "Commission Category")]
        public string CommissionCategory { get; set; }

        [Display(Name = "Role")]
        public string RoleId { get; set; }
        public string Status { get; set; }
        public string FName { get; set; }
        public string AgentName { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
