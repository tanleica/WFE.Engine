using WFE.Engine.Domain.Constants;

namespace WFE.Engine.Domain.Workflow
{
    public class WorkflowRule
    {
        public Guid Id { get; set; }
        public Guid WorkflowStepId { get; set; }
        public string RuleName { get; set; } = default!;
        public string ConditionScript { get; set; } = default!;
        public string LogicalOperator { get; set; } = LogicalOperators.Leaf;
        public WorkflowStep Step { get; set; } = default!;
    }
}