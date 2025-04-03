using WFE.Engine.Domain.Constants;

namespace WFE.Engine.DTOs
{
    public class PlannedStepDto
    {
        public Guid WorkflowStepId { get; set; }
        public string StepName { get; set; } = string.Empty;

        public string ApprovalType { get; set; } = ApprovalTypes.Sequential;
        public int StepOrder { get; set; }

        public List<PlannedActorDto> Actors { get; set; } = [];
        public RuleNodeDto? RuleTree { get; set; } // Replaces flat `Rules` array
    }
}