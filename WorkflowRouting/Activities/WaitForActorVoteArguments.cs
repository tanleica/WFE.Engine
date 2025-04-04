using WFE.Engine.Contracts;

namespace WFE.Engine.WorkflowRouting.Activities;

public class WaitForActorVoteArguments
{
    public Guid WorkflowStepId { get; set; }
    public Guid CorrelationId { get; set; }
    public string StepName { get; set; } = string.Empty;
    public int StepOrder { get; set; }
    public Actor Actor { get; set; } = new();
}
