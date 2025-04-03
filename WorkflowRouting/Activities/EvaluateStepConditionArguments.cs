using WFE.Engine.DTOs;

namespace WFE.Engine.WorkflowRouting.Activities;

public class EvaluateStepConditionArguments
{
    public Guid WorkflowStepId { get; set; }
    public Guid CorrelationId { get; set; }
    public string StepName { get; set; } = string.Empty;
    public int StepOrder { get; set; }
    public RuleNodeDto? RuleTree { get; set; }

    // Person performing the step
    public string ActorUsername { get; set; } = string.Empty;
    public string ActorFullName { get; set; } = string.Empty;
    public string ActorEmail { get; set; } = string.Empty;
    public string ActorEmployeeCode { get; set; } = string.Empty;

    // 🔥 Optional dynamic parameters for SQL eval
    public Dictionary<string, string>? DynamicSqlParameters { get; set; }
}
