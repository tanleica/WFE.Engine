using WFE.Engine.Contracts;

namespace WFE.Engine.WorkflowRouting.Activities;

public class WaitForActorVoteLog
{
    public string StepName { get; set; } = string.Empty;
    public Actor Actor { get; set; } = new();

}
