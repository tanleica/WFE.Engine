using WFE.Engine.Contracts;
using WFE.Engine.DTOs;

namespace WFE.Engine.Events
{
    public record StartWorkflow : WorkflowEventBase, IStartWorkflow
    {
        public List<RequestAttributeDto> Attributes { get; set; } = [];
        public IEnumerable<PlannedStepDto> Steps { get; init; } = [];
        public string DbType { get; init; } = string.Empty;
        public string EncryptedConnectionString { get; init; } = string.Empty;
    }
}
