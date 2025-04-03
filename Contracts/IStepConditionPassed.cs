namespace WFE.Engine.Contracts;

public interface IStepConditionPassed
{
    Guid CorrelationId { get; }
    string StepName { get; }
    bool IsApproved { get; }
    string PerformedByUsername { get; }
    string PerformedByFullName { get; }
    string PerformedByEmail { get; }
    string PerformedByEmployeeCode { get; }
    string? Reason { get; }
    DateTime PerformedAt { get; }
}
