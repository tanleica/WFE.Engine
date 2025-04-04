using MassTransit;
using Microsoft.EntityFrameworkCore;
using WFE.Engine.Contracts;
using WFE.Engine.Persistence;
using WFE.Engine.Domain.Workflow;
using WFE.Engine.RoutingSlips;

namespace WFE.Engine.WorkflowRouting.Consumers;

public class StepVoteSubmittedConsumer(
    SagaDbContext db,
    ILogger<StepVoteSubmittedConsumer> logger,
    VoteTallyingService tallyingService,
    StepVoteEvaluator evaluator,
    RoutingSlipExecutor executor
) : IConsumer<IStepVoteSubmitted>
{
    private readonly SagaDbContext _db = db;
    private readonly ILogger<StepVoteSubmittedConsumer> _logger = logger;
    private readonly VoteTallyingService _tallyingService = tallyingService;
    private readonly StepVoteEvaluator _evaluator = evaluator;
    private readonly RoutingSlipExecutor _executor = executor;

    public async Task Consume(ConsumeContext<IStepVoteSubmitted> context)
    {
        var message = context.Message;

        _logger.LogInformation("🔔 [StepVoteSubmittedConsumer Consume]:");
        _logger.LogInformation("   → Step: {Step}", message.StepName);
        _logger.LogInformation("   → Username: {Username}", message.Actor.Username);
        _logger.LogInformation("   → FullName: {FullName}", message.Actor.FullName);
        _logger.LogInformation("   → Email: {Email}", message.Actor.Email);
        _logger.LogInformation("   → EmployeeCode: {Code}", message.Actor.EmployeeCode);

        // Find the workflow instance first (must include WorkflowId)
        var workflowInstance = await _db.WorkflowInstances
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.CorrelationId == message.CorrelationId);

        if (workflowInstance is null)
        {
            _logger.LogCritical("❌ WorkflowInstance not found for CorrelationId = {CorrelationId}", message.CorrelationId);
            return;
        }

        // 🧠 Now find the correct WorkflowStep via WorkflowId + StepName
        _logger.LogInformation("🧩 Looking for Step '{StepName}' in WorkflowId = {WorkflowId}", message.StepName, workflowInstance.WorkflowId);
        var step = await _db.WorkflowSteps
            .Include(s => s.Actors)
            .FirstOrDefaultAsync(s =>
                s.WorkflowId == workflowInstance.WorkflowId &&
                s.StepName == message.StepName);

        if (step is null)
        {
            _logger.LogCritical("❌ Step not found: {StepName}", message.StepName);
            return;
        }

        _logger.LogDebug("📊 Step '{StepName}' has {ActorCount} actor(s)", step.StepName, step.Actors.Count);

        // ✅ Save StepProgress
        await _db.StepProgresses.AddAsync(new StepProgress
        {
            CorrelationId = message.CorrelationId,
            WorkflowStepId = step.Id,
            ActorUsername = message.Actor.Username,
            ActorFullName = message.Actor.FullName,
            ActorEmail = message.Actor.Email,
            ActorEmployeeCode = message.Actor.EmployeeCode,
            CompletedAt = message.OccurredAt,
            IsCompleted = true,
            Reason = message.Reason,
            CanActorVote = true
        });

        await _db.SaveChangesAsync();

        // 🧠 Count votes & evaluate
        var (isStepApproved, isStepRejected) = await _evaluator.EvaluateStepAsync(
            step.ApprovalType,
            message.CorrelationId,
            step.Id,
            totalActors: step.Actors.Count
        );

        _logger.LogInformation("🧮 Evaluation result → Approved: {Approved}, Rejected: {Rejected}", isStepApproved, isStepRejected);

        if ((isStepApproved && workflowInstance.IsApproved) ||
            (isStepRejected && workflowInstance.IsRejected == true))
        {
            _logger.LogWarning("⚠️ Finalization already done for {CorrelationId}. Skipping second FinalizeStepAsync call.", message.CorrelationId);
            return;
        }

        /* EARLY FINALIZE (TMP for TEST) */
        await _executor.FinalizeStepAsync(
            message.CorrelationId,
            message.StepName,
            isStepApproved,
            message.Actor,
            message.Reason
        );
        return;

        /* UNCOMMENT THIS BLOCK WHEN WE DO NOT TEST
        if (isStepApproved || isStepRejected)
        {
            await _executor.FinalizeStepAsync(
                message.CorrelationId,
                message.StepName,
                isStepApproved,
                message.Actor,
                message.Reason
            );
        }
        else
        {
            _logger.LogInformation("🔄 Step '{StepName}' is waiting for more votes...", message.StepName);
        }
        */
    }
}
