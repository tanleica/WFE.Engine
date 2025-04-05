using WFE.Engine.Contracts;
using WFE.Engine.Domain.Constants;

namespace WFE.Engine.DTOs
{
    public class KickoffRequestDto : IActorCarrier
    {
        public Guid CorrelationId { get; set; } = Guid.NewGuid();

        public Actor Actor { get; set; } = new();

        public Guid WorkflowId { get; set; }

        public List<RequestAttributeDto> Attributes { get; set; } = [];

        public string DbType { get; set; } = DbTypes.SqlServer;

        public string EncryptedConnectionString { get; set; } = string.Empty;

        public RuleNodeDto? RuleTree { get; set; } // ✅ Must exist

        public DateTime RequestedAt { get; set;} = DateTime.UtcNow;

        public string? Scenario { get; set; }
        public string? Reason {get; set;}
    }
}