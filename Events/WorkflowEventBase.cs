using WFE.Engine.Contracts;

namespace WFE.Engine.Events;

public abstract record WorkflowEventBase: IWorkflowEvent, IActorCarrier, IStepAware
{
    public Guid CorrelationId { get; init; }
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public bool CanGoFurther { get; init; } = true;
    public string? Reason { get; init; }
    public Actor Actor { get; init; } = null!;
    public string StepName { get; init; } = null!;
}
