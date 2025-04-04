using WFE.Engine.Domain.Constants;

namespace WFE.Engine.DTOs
{
    public class StepNodeDto
    {
        public string StepName { get; set; } = string.Empty;
        public string ApprovalType { get; set; } = ApprovalTypes.Sequential;
        public List<PlannedActorDto> Actors { get; set; } = [];
    }
}