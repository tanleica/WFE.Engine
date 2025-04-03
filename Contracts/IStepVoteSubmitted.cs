namespace WFE.Engine.Contracts;

public interface IStepVoteSubmitted
{
    Guid CorrelationId { get; }
    string StepName { get; }

    string PerformedByUsername { get; }
    string PerformedByFullName { get; }
    string PerformedByEmail { get; }
    string PerformedByEmployeeCode { get; }

    bool IsApproved { get; }
    string? Reason { get; }
    DateTime PerformedAt { get; }
}
