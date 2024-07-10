using CRS.ADMIN.APPLICATION.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.StaffManagement
{
    public class StaffManagementCommonModel
    {
        public string SearchFilter { get; set; }
        public List<StaffManagementModel> StaffListModel { get; set; }
        public ManageStaff ManageStaffModel { get; set; }
    }
    public class StaffManagementModel
    {
        public string SNO { get; set; }
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string ProfileImage { get; set; }
        public string ActionDate { get; set; }
        public string RoleName { get; set; }
    }
    public class ManageStaff
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(30, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_length_is_30_characters")]
        public string UserName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string EmailAddress { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string MobileNumber { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string RoleId { get; set; }
    }
}