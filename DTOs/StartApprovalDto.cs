namespace WFE.Engine.DTOs
{
    public class StartApprovalDto
    {
        public string ActorFullName { get; set; } = string.Empty;
        public string ActorEmail { get; set; } = string.Empty;
        public string ActorEmployeeCode { get; set; } = string.Empty;
        public string ConditionScript { get; set; } = string.Empty;
    }
}