namespace WFE.Engine.Contracts;

public interface IVoteRequested
{
    Guid CorrelationId { get; }
    string StepName { get; }

    string ActorUsername { get; }
    string ActorFullName { get; }
    string ActorEmail { get; }
    string ActorEmployeeCode { get; }

    DateTime RequestedAt { get; }
}
