using WFE.Engine.Contracts;

namespace WFE.Engine.Events
{
    public record RequestRejected : WorkflowEventBase, IRequestRejected
    {
    }
}
