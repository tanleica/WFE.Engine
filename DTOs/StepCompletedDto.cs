namespace FamilySagaWorkflow.DTOs
{
    public class StepCompletedDto
    {
        public Guid CorrelationId { get; set; }
        public string StepName { get; set; } = default!;
        public string PerformedByEmail { get; set; } = default!;
        public DateTime PerformedAt { get; set; }
        public bool IsApproved { get; set; }
        public string? Reason { get; set; }
    }
}