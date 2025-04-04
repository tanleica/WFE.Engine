namespace WFE.Engine.Domain.Workflow
{
    public class StepProgress
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CorrelationId { get; set; } // Link to RequestState

        public Guid WorkflowStepId { get; set; } // FK to static WorkflowStep

        public WorkflowStep WorkflowStep { get; set; } = default!;

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }

        public string? ActorUserId { get; set; }
        public string? ActorUsername { get; set; }
        public string? ActorFullName { get; set; }
        public string? ActorEmail { get; set; }
        public string? ActorEmployeeCode { get; set; }
        public string? Reason { get; set; }
        public string? FilterMode { get; set; }
        public bool ConditionPassed { get; set; } = false;
        public bool CanActorVote { get; set; } = false; // Useful for analytics
    }
}