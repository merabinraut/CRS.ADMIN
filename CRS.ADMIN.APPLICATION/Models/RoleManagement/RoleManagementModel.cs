using CRS.ADMIN.APPLICATION.Resources;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.RoleManagement
{
    public class RoleManagementCommonModel
    {
        public List<RoleListModel> RoleListModel { get; set; } = new List<RoleListModel>();
        public ManageAgentRoleModel ManageAgentRoleModel { get; set; } = new ManageAgentRoleModel();

    }
    public class RoleListModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public string ActionUser { get; set; }
        public string ActionDate { get; set; }
    }

    public class RoleTypeManagementCommonModel
    {
        public List<RoleTypeListModel> RoleTypeListModel { get; set; } = new List<RoleTypeListModel>();
        public ManageRole ManageRole { get; set; } = new ManageRole();
    }
    public class RoleTypeListModel
    {
        public string RoleId { get; set; }
        public string RoleType { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public string ActionDate { get; set; }
        public string ActionUser { get; set; }
        public string Action { get; set; }
    }
    public class ManageRole
    {
        public string RoleId { get; set; }
        public string RoleType { get; set; }
        [Required]
        [DisplayName("Role Type")]
        public string RoleTypeName { get; set; }
        [Required]
        [DisplayName("Role Name")]
        public string RoleName { get; set; }
        [Required]
        [DisplayName("Description")]
        public string Description { get; set; }
    }

    public class AssignRoleModel
    {
        public string AgentId { get; set; }
        public string AgentName { get; set; }
        public string AgentType { get; set; }
        public string CurrentRole { get; set; }
        public string AssignedRole { get; set; }
    }

    public class ManageAgentRoleModel
    {
        public string RoleId { get; set; }
        public string AgentType { get; set; }
        public string AgentId { get; set; }
        [Display(Name = "Current_Role", ResourceType = typeof(Resource))]
        public string CurrentRole { get; set; }
    }
}