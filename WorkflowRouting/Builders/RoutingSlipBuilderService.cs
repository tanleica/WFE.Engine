using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WFE.Engine.DTOs;
using WFE.Engine.Domain.Workflow;
using WFE.Engine.Persistence;
using WFE.Engine.WorkflowRouting.Activities;
using WFE.Engine.WorkflowRouting.Helpers;
using WFE.Engine.WorkflowTemplates;

namespace WFE.Engine.WorkflowRouting.Builders
{
    public class RoutingSlipBuilderService(
        SagaDbContext db,
        IWorkflowTemplateBuilderService workflowTemplateBuilderService,
        IRuleTreeEvaluator ruleEvaluator,
        IBranchRuleParser ruleParser,
        ILogger<RoutingSlipBuilderService> logger
    ) : IRoutingSlipBuilderService
    {
        private readonly Uri _evaluateStepConditionUri = new("queue:evaluate-step-condition_execute");
        private readonly Uri _waitForVoteUri = new("queue:wait-for-actor-vote_execute");

        private readonly SagaDbContext _db = db;
        private readonly IWorkflowTemplateBuilderService _workflowTemplateBuilderService = workflowTemplateBuilderService;
        private readonly IRuleTreeEvaluator _ruleEvaluator = ruleEvaluator;
        private readonly IBranchRuleParser _ruleParser = ruleParser;
        private readonly ILogger<RoutingSlipBuilderService> _logger = logger;

        public async Task<RoutingSlip> BuildAsync(KickoffRequestDto request)
        {
            _logger.LogInformation("⚙️ RoutingSlipBuilderService.BuildAsync() called");

            var correlationId = NewId.NextGuid(); // always use a fresh ID
            _logger.LogWarning("🔄 Overriding incoming CorrelationId with new: {CorrelationId}", correlationId);
            request.CorrelationId = correlationId;

            var workflowId = request.WorkflowId;

            Workflow? workflow = await _db.Workflows
                .Include(w => w.Branches)
                    .ThenInclude(b => b.Steps)
                .Include(w => w.Branches)
                    .ThenInclude(b => b.Rules)
                .FirstOrDefaultAsync(w => w.Id == workflowId);

            if (workflow == null)
            {
                _logger.LogWarning("🆕 Workflow not found in replica. Creating one from first-time kickoff request...");
                workflow = await _workflowTemplateBuilderService.BuildFromFirstRequestAsync(request);
            }

            var selectedBranch = workflow.Branches
                .OrderBy(b => b.Priority)
                .FirstOrDefault(); // EntryConditionJson is null by design here

            if (selectedBranch == null || selectedBranch.Steps == null || !selectedBranch.Steps.Any())
                throw new InvalidOperationException("❌ Selected branch contains no steps.");

            var firstStep = selectedBranch.Steps.OrderBy(s => s.StepOrder).First();

            var instance = new WorkflowInstance
            {
                CorrelationId = correlationId,
                WorkflowId = workflowId,
                CurrentStep = firstStep.StepName,
                LastActionAt = DateTime.UtcNow,
                IsApproved = false,

                DbType = request.DbType,
                EncryptedConnectionString = request.EncryptedConnectionString,

                LastActorUsername = request.Actor.Username,
                LastActorFullName = request.Actor.FullName,
                LastActorEmail = request.Actor.Email,
                LastActorEmployeeCode = request.Actor.EmployeeCode
            };

            await _db.WorkflowInstances.AddAsync(instance);
            await _db.SaveChangesAsync();

            var builder = new RoutingSlipBuilder(correlationId);

            foreach (var step in selectedBranch.Steps.OrderBy(s => s.StepOrder))
            {
                var rule = selectedBranch.Rules.FirstOrDefault(r => r.WorkflowStepId == step.Id);

                builder.AddActivity(
                    $"EvaluateStep:{step.StepName}",
                    _evaluateStepConditionUri,
                    new EvaluateStepConditionArguments
                    {
                        CorrelationId = correlationId,
                        WorkflowStepId = step.Id,
                        StepName = step.StepName,
                        StepOrder = step.StepOrder,
                        Actor = request.Actor,
                        RuleTree = rule?.Node,
                        DynamicSqlParameters = request.Attributes?.ToDictionary(a => a.Key, a => a.Value)
                    });

                builder.AddActivity(
                    $"WaitForVote:{step.StepName}",
                    _waitForVoteUri,
                    new WaitForActorVoteArguments
                    {
                        CorrelationId = correlationId,
                        WorkflowStepId = step.Id,
                        StepName = step.StepName,
                        StepOrder = step.StepOrder,
                        Actor = request.Actor
                    });
            }

            // 🔧 Common routing slip variables
            builder.AddVariable("WorkflowId", workflowId);
            builder.AddVariable("EncryptedConnectionString", request.EncryptedConnectionString);
            builder.AddVariable("DbType", request.DbType);
            builder.AddVariable("RequestedByUserId", request.Actor.Id);
            builder.AddVariable("RequestedByUsername", request.Actor.Username);
            builder.AddVariable("RequestedByFullName", request.Actor.FullName);
            builder.AddVariable("RequestedByEmail", request.Actor.Email);
            builder.AddVariable("RequestedByEmployeeCode", request.Actor.EmployeeCode);
            builder.AddVariable("RequestedAt", request.RequestedAt);

            if (request.Attributes != null)
            {
                foreach (var attr in request.Attributes)
                {
                    builder.AddVariable(attr.Key, attr.Value);
                    builder.AddVariable($"__type_{attr.Key}", attr.ValueClrType);
                }
            }

            var routingSlip = builder.Build();
            _logger.LogInformation("✅ Routing slip built with CorrelationId = {CorrelationId}", correlationId);

            return routingSlip;
        }


    }
}
