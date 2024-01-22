namespace CRS.ADMIN.SHARED.RoleManagement
{
    public class GetCurrentAssignedRoleCommon
    {
        public ResponseCode Code { get; set; } = ResponseCode.Exception;
        public string Message { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public class ManageAgentRoleCommon : Common
    {
        public string RoleId { get; set; }
        public string AgentType { get; set; }
        public string AgentId { get; set; }
    }
}
