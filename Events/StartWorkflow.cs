using WFE.Engine.Contracts;
using WFE.Engine.DTOs;

namespace WFE.Engine.Events
{
    public record StartWorkflow : WorkflowEventBase, IStartWorkflow
    {
        public IDictionary<string, string> Attributes { get; init; } = new Dictionary<string, string>();
        public IEnumerable<PlannedStepDto> Steps { get; init; } = new List<PlannedStepDto>();
        public string DbType { get; init; } = string.Empty;
        public string EncryptedConnectionString { get; init; } = string.Empty;
    }
}
