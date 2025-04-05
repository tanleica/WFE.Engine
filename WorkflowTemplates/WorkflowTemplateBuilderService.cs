using WFE.Engine.Persistence;
using WFE.Engine.DTOs;
using WFE.Engine.Domain.Workflow;
using WFE.Engine.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using WFE.Engine.Contracts;

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

        public async Task<Workflow> BuildFromFirstRequestAsync(KickoffRequestDto dto)
        {
            if (dto.RuleTree == null)
                throw new InvalidOperationException("❌ KickoffRequestDto.RuleTree is required for first-time workflow registration.");

            ValidateStepNamesUnique(dto.RuleTree);

            var workflow = new Workflow
            {
                Id = dto.WorkflowId,
                Name = $"Workflow_{dto.WorkflowId.ToString("N").Substring(0, 8)}", // Optional fallback
                Description = dto.Reason,
                IsActive = true,
                Branches = []
            };

            var branch = new WorkflowBranch
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                Priority = 1,
                EntryConditionJson = null, // This is the entry branch
                Steps = [],
                Rules = []
            };

            workflow.Branches.Add(branch);

            var stepIndex = 0;

            void TraverseTree(RuleNodeDto node)
            {
                // Create WorkflowStep
                var step = new WorkflowStep
                {
                    Id = Guid.NewGuid(),
                    WorkflowId = workflow.Id,
                    BranchId = branch.Id,
                    StepOrder = stepIndex++,
                    StepName = node.StepName,
                    ApprovalType = ApprovalTypes.Sequential,
                    ConditionScript = null,
                    Actors = [] // Empty, resolved at runtime
                };

                branch.Steps.Add(step);

                // Create associated WorkflowRule (for audit/traceability)
                var rule = new WorkflowRule
                {
                    Id = Guid.NewGuid(),
                    WorkflowStepId = step.Id,
                    RuleName = node.RuleName ?? $"Rule_{step.StepName}",
                    Node = node
                };

                branch.Rules.Add(rule);

                if (node.Children != null)
                {
                    foreach (var child in node.Children)
                        TraverseTree(child);
                }
            }

            TraverseTree(dto.RuleTree);

            await _db.Workflows.AddAsync(workflow);
            await _db.SaveChangesAsync();

            _logger.LogInformation("✅ First-time workflow created from kickoff request with Id = {WorkflowId}", dto.WorkflowId);

            return workflow;
        }

        public async Task<Workflow> ForceUpdateFromRequestAsync(KickoffRequestDto dto)
        {
            _logger.LogWarning("🔁 Forcing workflow re-import from kickoff DTO: {WorkflowId}", dto.WorkflowId);

            // 1. Remove old workflow if exists (including cascade)
            var existing = await _db.Workflows
                .Include(w => w.Branches)
                    .ThenInclude(b => b.Steps)
                .FirstOrDefaultAsync(w => w.Id == dto.WorkflowId);

            if (existing != null)
            {
                _db.Workflows.Remove(existing);
                await _db.SaveChangesAsync();
                _logger.LogInformation("🗑️ Removed existing workflow: {WorkflowId}", dto.WorkflowId);
            }

            // 2. Rebuild using the same first-time method
            var workflow = await BuildFromFirstRequestAsync(dto);
            _logger.LogInformation("✅ Rebuilt workflow from kickoff DTO: {WorkflowId}", dto.WorkflowId);

            return workflow;

        }

        private static void FlattenRuleTree(RuleNodeDto node, List<WorkflowStep> steps, ref int order, Actor actor)
        {
            if (node.LogicalOperator == null || node.LogicalOperator == "Leaf")
            {
                if (string.IsNullOrWhiteSpace(node.StepName))
                    throw new InvalidOperationException("❌ Leaf node must contain StepName.");

                var step = new WorkflowStep
                {
                    Id = Guid.NewGuid(),
                    StepName = node.StepName,
                    StepOrder = order++,
                    ApprovalType = "single", // Default, later enhanced
                    Actors = new List<WorkflowActor>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Username = actor.Username,
                    FullName = actor.FullName,
                    Email = actor.Email,
                    EmployeeCode = actor.EmployeeCode,
                    IsMandatory = true,
                    Order = 1
                }
            }
                };

                steps.Add(step);
            }
            else if (node.Children != null)
            {
                foreach (var child in node.Children)
                    FlattenRuleTree(child, steps, ref order, actor);
            }
        }

        private static List<(string StepName, RuleNodeDto Rule)> FlattenRuleTreeToSteps(RuleNodeDto node)
        {
            var steps = new List<(string StepName, RuleNodeDto Rule)>();

            if (node.LogicalOperator is null or "Leaf")
            {
                if (string.IsNullOrWhiteSpace(node.StepName))
                    throw new InvalidOperationException("StepName is required for leaf rule nodes.");

                steps.Add((node.StepName, node));
                return steps;
            }

            foreach (var child in node.Children ?? [])
                steps.AddRange(FlattenRuleTreeToSteps(child));

            return steps;
        }

        private static void ValidateStepNamesUnique(RuleNodeDto rootNode)
        {
            var stepNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            void Traverse(RuleNodeDto node)
            {
                bool isLeaf = node.Children == null || node.Children.Count == 0;

                if (isLeaf)
                {
                    if (string.IsNullOrWhiteSpace(node.StepName))
                        throw new InvalidOperationException("StepName is required on every leaf node.");

                    if (!stepNames.Add(node.StepName))
                        throw new InvalidOperationException($"❌ Duplicate StepName detected: '{node.StepName}'");
                }
                else
                {
                    // For logical (non-leaf) nodes: StepName is allowed to be empty
                    if (node.Children != null)
                    {
                        foreach (var child in node.Children)
                            Traverse(child);
                    }
                }
            }

            Traverse(rootNode);
        }

    }
}