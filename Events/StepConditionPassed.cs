using WFE.Engine.Contracts;

namespace WFE.Engine.Events
{
    public record StepConditionPassed : WorkflowEventBase, IStepConditionPassed
    {
    }
}
