using WFE.Engine.Contracts;

namespace WFE.Engine.DTOs
{
    public class KickoffRequestDto
    {
        public Guid WorkflowId { get; set; }
        public Actor Actor { get; set; } = new();
        public string Reason { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public string? EncryptedConnectionString { get; set; } = string.Empty;
        public string? DbType { get; set; }
        public RuleNodeDto? RuleTree { get; set; }
        public List<RequestAttributeDto>? Attributes { get; set; } = [];
        public List<PlannedStepDto>? FlatSteps { get; set; } = [];
    }
}