using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.SHARED.StaffManagement
{
    public class StaffManagementListModelCommon : Common
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string ProfileImage { get; set; }
        public string RoleName { get; set; }
    }
    public class ManagerStaffCommon : Common
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string RoleId { get; set; }
    }
    public class StaffDetailsCommon
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string RoleId { get; set; }
    }
}
