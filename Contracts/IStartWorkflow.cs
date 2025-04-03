namespace WFE.Engine.Contracts;

public interface IStartWorkflow
{
    Guid CorrelationId { get; }
    string WorkflowName { get; }
    string RequestedBy { get; }
}
