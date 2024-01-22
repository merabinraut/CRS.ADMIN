using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRS.ADMIN.APPLICATION.Models.StaffManagement
{
    public class StaffManagementCommonModel
    {
        public List<StaffManagementModel> StaffListModel { get; set; }
        public ManageStaff ManageStaffModel { get; set; }
    }
    public class StaffManagementModel
    {
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
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string RoleId { get; set; }
    }
    
}