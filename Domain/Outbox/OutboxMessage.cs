using System.ComponentModel.DataAnnotations;

namespace WFE.Engine.Domain.Outbox
{
    public class OutboxMessage
    {
        [Key]
        public Guid MessageId { get; set; }

        public string Destination { get; set; } = string.Empty;

        public string Payload { get; set; } = string.Empty;

        public string MessageType { get; set; } = string.Empty;

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public bool Processed { get; set; } = false;

        // Optional: one-to-one navigation to OutboxState
        public OutboxState? State { get; set; }
    }
}