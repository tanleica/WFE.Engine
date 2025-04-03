
namespace WFE.Engine.DTOs
{
    public class KickoffRequestDto
    {
        public Guid WorkflowId { get; set; }
        public string RequestedByUsername { get; set; } = string.Empty;
        public string RequestedByFullName { get; set; } = string.Empty;
        public string RequestedByEmail { get; set; } = string.Empty;
        public string RequestedByEmployeeCode { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }

        // ✅ Encrypt this on Alpha side
        public string EncryptedConnectionString { get; set; } = string.Empty;
        public string? DbType { get; set; }

        public List<PlannedStepDto> Steps { get; set; } = [];
        public List<RequestAttributeDto> Attributes { get; set; } = [];
    }

}