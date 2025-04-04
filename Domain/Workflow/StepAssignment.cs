using System.ComponentModel.DataAnnotations.Schema;

namespace WFE.Engine.Domain.Workflow
{
    public class StepAssignment
    {
        public Guid Id { get; set; }

        public Guid CorrelationId { get; set; }
        public Guid WorkflowStepId { get; set; }

        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;

        public bool IsCurrent { get; set; } = true;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReplacedAt { get; set; }

        // 🔗 Navigation properties (optional for traceability)
        [NotMapped]
        public WorkflowStep Step { get; set; } = null!;
    }
}