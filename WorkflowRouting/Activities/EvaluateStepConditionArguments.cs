using WFE.Engine.Contracts;
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
    public Actor Actor {get; set;} = new();

    // 🔥 Optional dynamic parameters for SQL eval
    public Dictionary<string, string>? DynamicSqlParameters { get; set; }
}
