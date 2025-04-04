using System.Text.Json.Serialization;

namespace WFE.Engine.Domain.Workflow
{
    public class WorkflowActor
    {
        public Guid Id { get; set; } = Guid.Empty;
        public Guid WorkflowStepId { get; set; }
        public string Username { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string EmployeeCode { get; set; } = default!;

        public bool IsMandatory { get; set; }
        public int Order { get; set; }

        [JsonIgnore]
        public WorkflowStep Step { get; set; } = default!;
    }
}