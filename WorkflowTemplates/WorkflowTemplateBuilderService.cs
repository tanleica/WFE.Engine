using WFE.Engine.Persistence;
using WFE.Engine.DTOs;
using WFE.Engine.Domain.Workflow;
using WFE.Engine.Domain.Constants;

namespace WFE.Engine.WorkflowTemplates
{
    public class WorkflowTemplateBuilderService(SagaDbContext db, ILogger<WorkflowTemplateBuilderService> logger) : IWorkflowTemplateBuilderService
    {

        private readonly SagaDbContext _db = db;
        private readonly ILogger<WorkflowTemplateBuilderService> _logger = logger;

        public async Task<Guid> CreateAdHocWorkflowAsync(string name, List<PlannedStepDto> steps)
        {
            var workflow = new Workflow
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = "Ad-hoc generated at runtime",
                IsActive = true
            };

            _db.Workflows.Add(workflow);

            var workflowSteps = new List<WorkflowStep>();
            var workflowActors = new List<WorkflowActor>();

            foreach (var stepDto in steps.OrderBy(s => s.StepOrder))
            {
                var stepId = Guid.NewGuid();
                _logger.LogInformation("🛠 Created WorkflowStep: {StepName} with ID = {Id}", stepDto.StepName, stepId);

                // Assign back the generated ID for later use in RoutingSlip
                stepDto.WorkflowStepId = stepId;

                var step = new WorkflowStep
                {
                    Id = stepId,
                    WorkflowId = workflow.Id,
                    StepOrder = stepDto.StepOrder,
                    StepName = stepDto.StepName,
                    ConditionScript = stepDto.RuleTree?.PredicateScript,
                    ApprovalType = stepDto.ApprovalType ?? ApprovalTypes.Sequential,
                };

                workflowSteps.Add(step);

                int actorOrder = 1;
                foreach (var actorDto in stepDto.Actors)
                {
                    var actor = new WorkflowActor
                    {
                        Id = Guid.NewGuid(),
                        WorkflowStepId = stepId,
                        Username = actorDto.Username,
                        FullName = actorDto.FullName,
                        Email = actorDto.Email,
                        EmployeeCode = actorDto.EmployeeCode,
                        IsMandatory = true,
                        Order = actorOrder++
                    };
                    workflowActors.Add(actor);
                    _logger.LogInformation("👤 Assigned actor {Email} to step {StepName}", actorDto.Email, stepDto.StepName);
                }
            }

            _db.WorkflowSteps.AddRange(workflowSteps);
            _db.WorkflowActors.AddRange(workflowActors);
            await _db.SaveChangesAsync(); // ✅ One transaction

            return workflow.Id;
        }


    }
}