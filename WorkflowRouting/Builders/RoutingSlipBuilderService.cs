using MassTransit;
using MassTransit.Courier.Contracts;
using WFE.Engine.DTOs;
using WFE.Engine.WorkflowRouting.Activities;
using WFE.Engine.Persistence;
using WFE.Engine.WorkflowTemplates;
using Microsoft.EntityFrameworkCore;
using WFE.Engine.Domain.Workflow;

namespace WFE.Engine.WorkflowRouting.Builders
{
    public class RoutingSlipBuilderService(
        SagaDbContext db,
        IWorkflowTemplateBuilderService workflowTemplateBuilderService,
        ILogger<RoutingSlipBuilderService> logger
    ) : IRoutingSlipBuilderService
    {
        private readonly Uri _evaluateStepConditionUri = new("queue:evaluate-step-condition_execute");
        private readonly Uri _waitForVoteUri = new("queue:wait-for-actor-vote_execute");

        private readonly SagaDbContext _db = db;
        private readonly IWorkflowTemplateBuilderService _workflowTemplateBuilderService = workflowTemplateBuilderService;
        private readonly ILogger<RoutingSlipBuilderService> _logger = logger;

        public async Task<RoutingSlip> BuildAsync(KickoffRequestDto request)
        {
            _logger.LogInformation("⚙️ RoutingSlipBuilderService BuildAsync method called");

            try
            {
                var correlationId = NewId.NextGuid();
                var workflowId = request.WorkflowId;

                if (workflowId == Guid.Empty)
                    throw new InvalidOperationException("🚨 WorkflowId must not be empty. Check KickoffRequestDto or upstream caller.");

                // ✅ Ensure the Workflow exists
                if (!await _db.Workflows.AnyAsync(w => w.Id == workflowId))
                {
                    _db.Workflows.Add(new Workflow
                    {
                        Id = workflowId,
                        Name = $"Injected-{DateTime.UtcNow:yyyyMMdd_HHmmss}",
                        IsActive = true
                    });

                    await _db.SaveChangesAsync();
                }

                var firstStepName = request.Steps.OrderBy(x => x.StepOrder).FirstOrDefault()?.StepName;
                if (firstStepName == null)
                {
                    _logger.LogCritical("❌ {RequestedByFullName} kicked off a request with no step", request.RequestedByFullName);
                    throw new InvalidOperationException("Workflow must contain at least one step.");
                }

                var instance = new WorkflowInstance
                {
                    CorrelationId = correlationId,
                    WorkflowId = workflowId,
                    CurrentStep = firstStepName,
                    LastActionAt = DateTime.UtcNow,
                    IsApproved = false,

                    DbType = request.DbType,
                    EncryptedConnectionString = request.EncryptedConnectionString,

                    FinalApprovedByUsername = request.RequestedByUsername,
                    FinalApprovedByFullName = request.RequestedByFullName,
                    FinalApprovedByEmail = request.RequestedByEmail,
                    FinalApprovedByEmployeeCode = request.RequestedByEmployeeCode
                };

                var workflowSteps = request.Steps.Select(step => new WorkflowStep
                {
                    Id = step.WorkflowStepId,
                    WorkflowId = workflowId,
                    StepName = step.StepName,
                    StepOrder = step.StepOrder,
                    ApprovalType = step.ApprovalType
                }).ToList();

                await _db.WorkflowInstances.AddAsync(instance);
                await _db.WorkflowSteps.AddRangeAsync(workflowSteps);
                await _db.SaveChangesAsync();

                _logger.LogInformation("🧾 Transaction committed: WorkflowInstance + {correlationId}", correlationId);

                var builder = new RoutingSlipBuilder(correlationId);

                foreach (var step in request.Steps.OrderBy(s => s.StepOrder))
                {
                    var activityArgs = new EvaluateStepConditionArguments
                    {
                        CorrelationId = correlationId,
                        WorkflowStepId = step.WorkflowStepId,
                        StepName = step.StepName,
                        StepOrder = step.StepOrder,

                        ActorUsername = request.RequestedByUsername,
                        ActorFullName = request.RequestedByFullName,
                        ActorEmail = request.RequestedByEmail,
                        ActorEmployeeCode = request.RequestedByEmployeeCode,

                        RuleTree = step.RuleTree,
                        DynamicSqlParameters = request.Attributes.ToDictionary(a => a.Key, a => a.Value)
                    };

                    _logger.LogInformation("👉 Adding EvaluateStepCondition activity for step name {StepName}", step.StepName);

                    builder.AddActivity(
                        $"EvaluateStep:{step.StepName}",
                        _evaluateStepConditionUri,
                        activityArgs
                    );

                    var voteArgs = new WaitForActorVoteArguments
                    {
                        CorrelationId = correlationId,
                        WorkflowStepId = step.WorkflowStepId,
                        StepName = step.StepName,
                        StepOrder = step.StepOrder,

                        ActorUsername = request.RequestedByUsername,
                        ActorFullName = request.RequestedByFullName,
                        ActorEmail = request.RequestedByEmail,
                        ActorEmployeeCode = request.RequestedByEmployeeCode
                    };

                    _logger.LogInformation("👉 Adding WaitForActorVote activity for step name {StepName}", step.StepName);

                    builder.AddActivity(
                        $"WaitForVote:{step.StepName}",
                        new Uri("queue:wait-for-actor-vote_execute"),
                        voteArgs
                    );
                }

                // ✨ Add all required variables
                builder.AddVariable("WorkflowId", workflowId);
                builder.AddVariable("EncryptedConnectionString", request.EncryptedConnectionString);
                builder.AddVariable("DbType", request.DbType);
                builder.AddVariable("RequestedByUsername", request.RequestedByUsername);
                builder.AddVariable("RequestedByFullName", request.RequestedByFullName);
                builder.AddVariable("RequestedByEmail", request.RequestedByEmail);
                builder.AddVariable("RequestedByEmployeeCode", request.RequestedByEmployeeCode);
                builder.AddVariable("RequestedAt", request.RequestedAt);

                if (request.Attributes is not null)
                {
                    foreach (var attr in request.Attributes)
                    {
                        builder.AddVariable(attr.Key, attr.Value);
                        builder.AddVariable($"__type_{attr.Key}", attr.ValueClrType);
                    }
                }

                var routingSlip = builder.Build();
                _logger.LogInformation("✅ A routing slip has been built with correlation Id = {correlationId}", correlationId);
                return routingSlip;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("🟥 Build failed: {Message}", ex.Message);
                throw;
            }
        }


    }
}
