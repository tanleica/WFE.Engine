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
        public bool? IsRejected { get; set; }
        public string? FinalApprovedByUsername { get; set; }
        public string? FinalApprovedByFullName { get; set; }
        public string? FinalApprovedByEmail { get; set; }
        public string? FinalApprovedByEmployeeCode { get; set; }
        public DateTime LastActionAt { get; set; } = DateTime.UtcNow;

        // ✅ FIELDS FOR AUDIT
        public string? DbType { get; set; }
        public string? EncryptedConnectionString { get; set; }
    }
}
