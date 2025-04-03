using System.Text.Json.Serialization;

namespace WFE.Engine.Domain.Workflow
{
    public class WorkflowActor
    {
        public Guid Id { get; set; }
        public Guid WorkflowStepId { get; set; }
        public string ActorName { get; set; } = default!;
        public string? Email { get; set; }
        public bool IsMandatory { get; set; }
        public int Order { get; set; }

        [JsonIgnore]
        public WorkflowStep Step { get; set; } = default!;
    }
}