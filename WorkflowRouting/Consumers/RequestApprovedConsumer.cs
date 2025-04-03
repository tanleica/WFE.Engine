using MassTransit;
using WFE.Engine.Contracts;
using WFE.Engine.Domain.Workflow;
using WFE.Engine.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WFE.Engine.WorkflowRouting.Consumers;

public class RequestApprovedConsumer(SagaDbContext db, HttpClient httpClient) : IConsumer<IRequestApproved>
{
    private readonly SagaDbContext _db = db;
    private readonly HttpClient _httpClient = httpClient;
    public async Task Consume(ConsumeContext<IRequestApproved> context)
    {
        var message = context.Message;
        Console.WriteLine($"🎉 Request Approved: {message.CorrelationId} — Final Step: {message.FinalStepName}");

        // First: Try to find existing instance (no tracking for read-only)
        var existing = await _db.WorkflowInstances
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.CorrelationId == message.CorrelationId);

        if (existing == null)
        {
            // Create new instance
            var newInstance = new WorkflowInstance
            {
                CorrelationId = message.CorrelationId,
                WorkflowId = Guid.NewGuid(),
                CurrentStep = message.FinalStepName,
                IsApproved = true,
                FinalApprovedByUsername = message.FinalApprovedByUsername,
                FinalApprovedByFullName = message.FinalApprovedByFullName,
                FinalApprovedByEmail = message.FinalApprovedByEmail,
                FinalApprovedByEmployeeCode = message.FinalApprovedByEmployeeCode,
                LastActionAt = message.ApprovedAt
            };

            await _db.WorkflowInstances.AddAsync(newInstance);
            Console.WriteLine("🆕 Created new workflow instance.");
        }
        else
        {
            // Update existing instance
            var updatedInstance = new WorkflowInstance
            {
                WorkflowId = existing.WorkflowId,
                CorrelationId = existing.CorrelationId,
                CurrentStep = message.FinalStepName,
                IsApproved = true,
                FinalApprovedByUsername = message.FinalApprovedByUsername,
                FinalApprovedByFullName = message.FinalApprovedByFullName,
                FinalApprovedByEmail = message.FinalApprovedByEmail,
                FinalApprovedByEmployeeCode = message.FinalApprovedByEmployeeCode,
                LastActionAt = message.ApprovedAt
            };

            _db.WorkflowInstances.Update(updatedInstance);
            Console.WriteLine("✏️ Updated existing workflow instance.");
        }

        await _db.SaveChangesAsync();

        // ✅ Publish push notification request
        await context.Publish<IPushNotificationRequested>(new
        {
            RecipientUsername = "b2e05ec4-6022-4f35-baea-ceb7fa2ee9dd",
            Title = $"🎉 Request Approved: {message.CorrelationId} — Final Step: {message.FinalStepName}",
            Body = "No reason provided"
        });
    }
}
