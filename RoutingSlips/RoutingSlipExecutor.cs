using MassTransit;
using WFE.Engine.DTOs;
using WFE.Engine.Contracts;
using WFE.Engine.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using WFE.Engine.RoutingSlips;
using Microsoft.Extensions.Logging;
using WFE.Engine.Events;
using WFE.Engine.WorkflowRouting.Builders;

namespace WFE.Engine.RoutingSlips;

public class RoutingSlipExecutor(
    IBus bus,
    IPublishEndpoint publishEndpoint,
    ILogger<RoutingSlipExecutor> logger,
    SagaDbContext db,
    IRoutingSlipBuilderService builder)
{
    private IBus _bus = bus;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly ILogger<RoutingSlipExecutor> _logger = logger;
    private readonly SagaDbContext _db = db;
    private readonly IRoutingSlipBuilderService _builder = builder;

    public async Task StartAsync(KickoffRequestDto dto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🍺 RoutingSlipExecutor StartAsync method called");

        // Step 1: Build the routing slip
        var routingSlip = await _builder.BuildAsync(dto);
        _logger.LogInformation("🧱 Routing slip built with {ActivityCount} activities for CorrelationId: {CorrelationId}",
            routingSlip.Itinerary.Count, dto.CorrelationId);

        // Step 2: Schedule it for execution
        await _bus.Execute(routingSlip, cancellationToken);
        _logger.LogInformation("🚀 Routing slip execution scheduled.");

        // Step 3: Publish the kickoff event
        await _publishEndpoint.Publish<IStartWorkflow>(new StartWorkflow
        {
            CorrelationId = dto.CorrelationId,
            Actor = dto.Actor,
            OccurredAt = dto.RequestedAt,
            CanGoFurther = true,
            Reason = dto.Reason,
            DbType = dto.DbType,
            EncryptedConnectionString = dto.EncryptedConnectionString,
            StepName = string.Empty, // start has no step yet
            Attributes = dto.Attributes ?? []
        }, cancellationToken);

        _logger.LogInformation("📣 StartWorkflow event published for CorrelationId = {CorrelationId}", dto.CorrelationId);
    }

    public async Task FinalizeStepAsync(
        Guid correlationId,
        string stepName,
        bool isApproved,
        Actor actor,
        string? reason = null)
    {
        _logger.LogInformation("🚩 FinalizeStepAsync called for CorrelationId: {CorrelationId}, Step: {StepName}, Approved: {Approved}", correlationId, stepName, isApproved);

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var instance = await _db.WorkflowInstances.FirstOrDefaultAsync(x => x.CorrelationId == correlationId);

        if (instance == null)
        {
            _logger.LogWarning("🚨 WorkflowInstance not found for CorrelationId: {CorrelationId}", correlationId);
            return;
        }

        if (instance.IsApproved || instance.IsRejected == true)
        {
            _logger.LogWarning("🚨 Workflow already finalized. Skipping. CorrelationId: {CorrelationId}", correlationId);
            return;
        }

        if (isApproved)
        {
            await _publishEndpoint.Publish<IRequestApproved>(new RequestApproved
            {
                CorrelationId = correlationId,
                Actor = actor,
                StepName = stepName,
                OccurredAt = DateTime.UtcNow,
                CanGoFurther = true,
                Reason = reason
            });
        }
        else
        {
            await _publishEndpoint.Publish<IRequestRejected>(new RequestRejected
            {
                CorrelationId = correlationId,
                Actor = actor,
                StepName = stepName,
                OccurredAt = DateTime.UtcNow,
                CanGoFurther = false,
                Reason = reason ?? "No reason provided"
            });
        }

        await _publishEndpoint.Publish<IPushNotificationRequested>(new PushNotificationRequested
        {
            CorrelationId = correlationId,
            Actor = actor,
            StepName = stepName,
            OccurredAt = DateTime.UtcNow,
            CanGoFurther = true,
            Reason = reason,
            Title = isApproved
                ? $"🎉 Request Approved by {actor.FullName}"
                : $"❌ Request Rejected by {actor.FullName}",
            Message = reason ?? "No reason provided",
            UserId = actor.Id
        });

        _logger.LogInformation("✅ Finalization events published for CorrelationId: {CorrelationId}", correlationId);

        scope.Complete();
    }
}
