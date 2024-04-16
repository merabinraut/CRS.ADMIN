using CRS.ADMIN.SHARED;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRS.ADMIN.APPLICATION.Models.ClubManagerModel
{
    public class ManageManagerModel: Common
    {
        public string ManagerId { get; set; }
        public string ClubId { get; set; }
        [DisplayName("Manager Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string ManagerName { get; set; }
        [DisplayName("Email Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(75, ErrorMessage = "Maximum 75 characters allowed")]
        public string Email { get; set; }
        [DisplayName("Mobile Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "The field must be a 11-digit number.")]
        public string MobileNumber { get; set; }
    }
}