using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.CustomerManagement
{
    public class ManageCustomerModel
    {
        public string AgentId { get; set; }
        [DisplayName("Nick Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MinLength(3, ErrorMessage = "Minimum 3 characters required")]
        [MaxLength(16, ErrorMessage = "Maximum 50 characters allowed")]
        public string NickName { get; set; }
        [DisplayName("First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MinLength(3, ErrorMessage = "Minimum 3 characters required")]
        [MaxLength(16, ErrorMessage = "Maximum 50 characters allowed")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MinLength(3, ErrorMessage = "Minimum 3 characters required")]
        [MaxLength(16, ErrorMessage = "Maximum 50 characters allowed")]
        public string LastName { get; set; }
        [DisplayName("Contact Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [RegularExpression("^[9][0-9]*$", ErrorMessage = "Invalid Contact number")]
        [MaxLength(11, ErrorMessage = "Contact number max length is invalid"), MinLength(11, ErrorMessage = "Invalid contact numner")]
        public string MobileNumber { get; set; }
        [DisplayName("Email Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(75, ErrorMessage = "Maximum 75 characters allowed")]
        public string EmailAddress { get; set; }
        [DisplayName("Date of Birth")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string DOB { get; set; }
        [DisplayName("Gender")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Gender { get; set; }
        [DisplayName("Preferred Location")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string PreferredLocation { get; set; }
        [DisplayName("Postal Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(50, ErrorMessage = "Postal code length is invalid")]
        public string PostalCode { get; set; }
        [DisplayName("Prefecture")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(50, ErrorMessage = "Prefecture length is invalid")]
        public string Prefecture { get; set; }
        [DisplayName("City")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(50, ErrorMessage = "City length is invalid")]
        public string City { get; set; }
        [DisplayName("Street")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(50, ErrorMessage = "Street length is invalid")]
        public string Street { get; set; }
        [DisplayName("Residence Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(50, ErrorMessage = "Residence number length is invalid")]
        public string ResidenceNumber { get; set; }
    }
    public class CustomerListCommonModel
    {
        public string SearchFilter { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Status { get; set; }
        public string MobileNumber { get; set; }
        public List<CustomerListModel> CustomerListModel { get; set; } = new List<CustomerListModel>();
    }
    public class CustomerListModel
    {
        public string AgentId { get; set; }
        public string ProfileImage { get; set; }
        public string NickName { get; set; }
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Age { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Referer { get; set; }
        public string Location { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }
}