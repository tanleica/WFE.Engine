using WFE.Engine.Contracts;

namespace WFE.Engine.Events
{
    public record RequestApproved : WorkflowEventBase, IRequestApproved
    {
    }
}
