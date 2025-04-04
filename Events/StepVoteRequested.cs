using WFE.Engine.Contracts;

namespace WFE.Engine.Events
{
    public record StepVoteRequested : WorkflowEventBase, IStepVoteRequested
    {
    }
}
