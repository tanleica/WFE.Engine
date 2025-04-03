namespace WFE.Engine.WorkflowRouting.Activities;

public class WaitForActorVoteArguments
{
    public Guid WorkflowStepId { get; set; }
    public Guid CorrelationId { get; set; }
    public string StepName { get; set; } = string.Empty;
    public int StepOrder { get; set; }
    public string ActorUsername { get; set; } = string.Empty;
    public string ActorFullName { get; set; } = string.Empty;
    public string ActorEmail { get; set; } = string.Empty;
    public string ActorEmployeeCode { get; set; } = string.Empty;
}
