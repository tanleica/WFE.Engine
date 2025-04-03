using WFE.Engine.DTOs;

namespace WFE.Engine.WorkflowTemplates
{
    public interface IWorkflowTemplateBuilderService
    {
        Task<Guid> CreateAdHocWorkflowAsync(string name, List<PlannedStepDto> steps);
    }
}