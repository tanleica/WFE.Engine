namespace WFE.Engine.WorkflowRouting.Activities
{
    public class EvaluateStepConditionLog
    {
        public string EvaluatedStep { get; set; } = default!;
        public bool IsAllowed { get; set; }

        public string? Reason { get; set; }
    }
}