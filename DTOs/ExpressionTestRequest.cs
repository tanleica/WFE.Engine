namespace FamilySagaWorkflow.DTOs
{
    public class ExpressionTestRequest
    {
        public string Expression { get; set; } = default!;
        public int Duration { get; set; }
        public bool IsApproved { get; set; }  // Maps to StepProgress.IsCompleted
        public string? Reason { get; set; }   // Optional for extended testing
    }
}