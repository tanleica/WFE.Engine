using MassTransit;
using WFE.Engine.Contracts;
using WFE.Engine.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WFE.Engine.WorkflowRouting.Consumers;

public class RequestApprovedConsumer(SagaDbContext db) : IConsumer<IRequestApproved>
{
    private readonly SagaDbContext _db = db;
    public async Task Consume(ConsumeContext<IRequestApproved> context)
    {
        var message = context.Message;
        Console.WriteLine($"🎉 Request Approved: {message.CorrelationId} — Final Step: {message.StepName}");

        // ✅ Enforce: WorkflowInstance must exist
        var instance = await _db.WorkflowInstances
            .FirstOrDefaultAsync(i => i.CorrelationId == message.CorrelationId);

        if (instance is null)
        {
            Console.WriteLine($"❌ WorkflowInstance not found for CorrelationId = {message.CorrelationId}");
            return;
        }

        // 🔄 Update final approval state
        instance.IsApproved = true;
        instance.CurrentStep = message.StepName;
        instance.LastActorUsername = message.Actor.Username;
        instance.LastActorFullName = message.Actor.FullName;
        instance.LastActorEmail = message.Actor.Email;
        instance.LastActorEmployeeCode = message.Actor.EmployeeCode;
        instance.LastActionAt = message.OccurredAt;

        await _db.SaveChangesAsync();
        Console.WriteLine("✏️ Updated existing workflow instance.");

        // ✅ Push Notification
        await context.Publish<IPushNotificationRequested>(new
        {
            message.CorrelationId,
            message.StepName,
            message.Actor,
            Title = $"🎉 Request Approved",
            Message = $"Request {message.CorrelationId} was approved at step: {message.StepName}",
            UserId = Guid.Parse("b2e05ec4-6022-4f35-baea-ceb7fa2ee9dd")  // Example fallback
        });
    }
}