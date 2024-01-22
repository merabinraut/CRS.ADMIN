using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CRS.ADMIN.SHARED.RoleManagement
{
    public class RoleListCommon : Common
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
    }

    public class RoleTypeListCommon : Common
    {
        public string RoleId { get; set; }
        public string RoleType { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
    }

    public class ManageRoleCommon : Common
    {
        public string RoleId { get; set; }
        public string RoleType { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
}
