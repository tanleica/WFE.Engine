using WFE.Engine.Domain.Workflow;
using WFE.Engine.DTOs;

namespace WFE.Engine.WorkflowTemplates
{
    public interface IWorkflowTemplateBuilderService
    {
        Task<Guid> CreateAdHocWorkflowAsync(string name, List<PlannedStepDto> steps);
        Task<Workflow> BuildFromFirstRequestAsync(KickoffRequestDto dto);
        Task<Workflow> ForceUpdateFromRequestAsync(KickoffRequestDto dto);
    }
}