namespace WFE.Engine.Contracts
{
    public interface IWorkflowEvent
    {
        Guid CorrelationId { get; }
        DateTime OccurredAt { get; }
        string? Reason { get; }
    }
}