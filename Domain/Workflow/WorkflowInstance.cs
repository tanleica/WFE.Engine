using System.ComponentModel.DataAnnotations;

namespace WFE.Engine.Domain.Workflow
{
    public class WorkflowInstance
    {
        public Guid CorrelationId { get; set; }

        [Required]
        public Guid WorkflowId { get; set; }

        public string CurrentStep { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;
        public bool IsRejected { get; set; } = false;

        // ✅ Final actor (either approved or rejected)
        public string? LastActorUsername { get; set; }
        public string? LastActorFullName { get; set; }
        public string? LastActorEmail { get; set; }
        public string? LastActorEmployeeCode { get; set; }

        public DateTime LastActionAt { get; set; } = DateTime.UtcNow;

        // ✅ Fields for auditing
        public string? DbType { get; set; }
        public string? EncryptedConnectionString { get; set; }
    }

}
