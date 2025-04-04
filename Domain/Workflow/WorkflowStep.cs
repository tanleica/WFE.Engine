using System.Text.Json.Serialization;
using WFE.Engine.Domain.Constants;

namespace WFE.Engine.Domain.Workflow
{
    public class WorkflowStep
    {
        public Guid Id { get; set; }
        public Guid WorkflowId { get; set; }
        public string StepName { get; set; } = default!;
        public int StepOrder { get; set; }
        public string ApprovalType { get; set; } = ApprovalTypes.Sequential;

        // For condition checking
        public string? ConditionScript { get; set; } = string.Empty;

        [JsonIgnore]
        public Workflow Workflow { get; set; } = default!;
        public ICollection<WorkflowActor> Actors { get; set; } = [];

        public ICollection<WorkflowRule> Rules { get; set; } = [];
    }
}