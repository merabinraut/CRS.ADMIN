using CRS.ADMIN.APPLICATION.Resources;
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
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string ManagerName { get; set; }
        [DisplayName("Email Address")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(75, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_75_characters_allowed")]
        public string Email { get; set; }
        [DisplayName("Mobile Number")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression("^[0-9]{11}$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_11_digit_number")]
        public string MobileNumber { get; set; }
    }
}