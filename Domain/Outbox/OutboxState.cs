using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFE.Engine.Domain.Outbox
{
    public class OutboxState
    {
        [Key]
        public Guid StateId { get; set; }
        public Guid MessageId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Delivered { get; set; }
        public string Status { get; set; } = "Pending";
        public int RetryCount { get; set; } = 0;

        [NotMapped]
        public OutboxMessage? Message { get; set; }
    }
}