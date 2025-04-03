using MassTransit;
using WFE.Engine.DTOs;
using WFE.Engine.Contracts;
using WFE.Engine.Persistence;
using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace WFE.Engine.RoutingSlips;

public class RoutingSlipExecutor(IPublishEndpoint publishEndpoint, ILogger<RoutingSlipExecutor> logger, SagaDbContext db)
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly ILogger<RoutingSlipExecutor> _logger = logger;

    private readonly SagaDbContext _db = db;

    public async Task StartAsync(KickoffRequestDto dto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🍺 RoutingSlipExecutor StartAsync method called");
        await _publishEndpoint.Publish<IStartWorkflow>(new
        {
            CorrelationId = Guid.NewGuid(),
            dto.RequestedByUsername,
            dto.RequestedByFullName,
            dto.RequestedByEmail,
            dto.RequestedByEmployeeCode,
            dto.Reason,
            dto.RequestedAt,
            dto.EncryptedConnectionString,
            dto.DbType,
            dto.Steps,
            dto.Attributes
        }, cancellationToken);
        _logger.LogInformation("🍻 RoutingSlipExecutor StartAsync Publish<T> passed");
    }

    public async Task FinalizeStepAsync(
        Guid correlationId,
        string stepName,
        bool isApproved,
        string actorUsername,
        string actorFullName,
        string actorEmail,
        string actorEmployeeCode,
        string? reason = null)
    {
        _logger.LogInformation("🚩 RoutingSlipExecutor FinalizeStepAsync method called");
        _logger.LogInformation("🚩 isApproved = {isApproved}", isApproved);

        // 👇 Fetch workflow instance
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var instance = await _db.WorkflowInstances.FirstOrDefaultAsync(x => x.CorrelationId == correlationId);

        if (instance == null)
        {
            _logger.LogWarning("🚨 WorkflowInstance not found for correlation ID: {CorrelationId}", correlationId);
            return;
        }

        if (instance.IsApproved || instance.IsRejected == true)
        {
            _logger.LogWarning("🚨 Workflow already finalized. Skipping push. [CorrelationId: {CorrelationId}]", correlationId);
            return;
        }

        _logger.LogInformation("🚩 IRequestApproved is being published");
        await _publishEndpoint.Publish<IRequestApproved>(new
        {
            CorrelationId = correlationId,
            FinalStepName = stepName,
            FinalApprovedByUsername = actorUsername,
            FinalApprovedByFullName = actorFullName,
            FinalApprovedByEmail = actorEmail,
            FinalApprovedByEmployeeCode = actorEmployeeCode,
            ApprovedAt = DateTime.UtcNow
        });
        await _publishEndpoint.Publish<IPushNotificationRequested>(new
        {
            CorrelationId = correlationId,
            Title = isApproved
                ? $"🎉 Request Approved by {actorFullName}"
                : $"❌ Request Rejected by {actorFullName}",
            Message = isApproved
                ? $"🎉 Request Approved by {actorFullName}"
                : $"❌ Request Rejected by {actorFullName}",
            RecipientUsername = "tannv",
            SentAt = DateTime.UtcNow
        });

        _logger.LogInformation("🚩 IRequestApproved was published");

    }
}