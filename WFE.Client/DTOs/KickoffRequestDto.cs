namespace WFE.Client.DTOs
{
    public class KickoffRequestDto
    {
        public string RequestedByUsername { get; set; } = string.Empty;
        public string RequestedByFullName { get; set; } = string.Empty;
        public string RequestedByEmail { get; set; } = string.Empty;
        public string RequestedByEmployeeCode { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }

        public string EncryptedConnectionString { get; set; } = string.Empty;
        public string DbType { get; set; } = "SqlServer"; // Or use enum if preferred
        public List<PlannedStepDto> Steps { get; set; } = [];
        public List<RequestAttributeDto> Attributes { get; set; } = [];
    }

    public class PlannedStepDto
    {
        public string StepName { get; set; } = string.Empty;
        public int StepOrder { get; set; }

        public string ApprovalType { get; set; } = string.Empty;
        public List<PlannedActorDto> Actors { get; set; } = [];
        public RuleNodeDto? RuleTree { get; set; }
    }

    public class PlannedActorDto
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
    }

    public class RequestAttributeDto
    {
        public string Key { get; set; } = default!;
        public string Value { get; set; } = default!;
        public string ValueClrType { get; set; } = default!;
    }

    public class RuleNodeDto
    {
        public string? RuleName { get; set; }
        public string? ConditionScript { get; set; }
        public string? LogicalOperator { get; set; } = "Leaf"; // "And", "Or", "Leaf"
        public string? FilterMode { get; set; } = "SoftWarn"; // "SoftWarn" (default) or "HardBlock"
        public List<RuleNodeDto>? Children { get; set; }
    }

}