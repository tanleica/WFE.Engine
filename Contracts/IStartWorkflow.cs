using WFE.Engine.DTOs;

namespace WFE.Engine.Contracts
{
    public interface IStartWorkflow : IWorkflowEvent, IActorCarrier
    {
        IDictionary<string, string> Attributes { get; }
        IEnumerable<PlannedStepDto> Steps { get; }
        string DbType { get; }
        string EncryptedConnectionString { get; }
    }
}
