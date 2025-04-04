namespace WFE.Engine.Contracts
{
    public interface IWorkflowEvent
    {
        Guid CorrelationId { get; }
        DateTime OccurredAt { get; }
        bool CanGoFurther { get; }
        string? Reason { get; }
    }
}