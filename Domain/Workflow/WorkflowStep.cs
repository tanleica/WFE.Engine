using System.ComponentModel.DataAnnotations.Schema;
using WFE.Engine.Domain.Constants;

namespace WFE.Engine.Domain.Workflow
{
    public class WorkflowStep
    {
        public Guid Id { get; set; }
        public Guid WorkflowId { get; set; }
        public string StepName { get; set; } = default!;
        public int StepOrder { get; set; }
        public string ApprovalType { get; set; } = ApprovalTypes.Sequential;

        public string? ConditionScript { get; set; } = string.Empty;

        public Guid BranchId { get; set; }         // Now primary owner

        [NotMapped]
        public WorkflowBranch? Branch { get; set; }

        public ICollection<WorkflowActor> Actors { get; set; } = [];
        public ICollection<WorkflowRule> Rules { get; set; } = [];
    }
}
