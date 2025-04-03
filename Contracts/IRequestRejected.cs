namespace WFE.Engine.Contracts;

public interface IRequestRejected
{
    Guid CorrelationId { get; }
    string RejectedByUsername { get; }
    string RejectedByFullName { get; }
    string RejectedByEmail { get; }
    string RejectedByEmployeeCode { get; }
    string StepName { get; }
    string Reason { get; }
    DateTime RejectedAt { get; }
}
