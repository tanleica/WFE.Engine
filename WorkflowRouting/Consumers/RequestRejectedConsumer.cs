using MassTransit;
using Microsoft.EntityFrameworkCore;
using WFE.Engine.Contracts;
using WFE.Engine.Persistence;

namespace WFE.Engine.WorkflowRouting.Consumers;

public class RequestRejectedConsumer(SagaDbContext db) : IConsumer<IRequestRejected>
{
    private readonly SagaDbContext _db = db;

    public async Task Consume(ConsumeContext<IRequestRejected> context)
    {
        var message = context.Message;
        Console.WriteLine($"❌ Request Rejected: {message.CorrelationId} — Step: {message.StepName}");

        // ✅ Enforce: WorkflowInstance must exist
        var instance = await _db.WorkflowInstances
            .FirstOrDefaultAsync(i => i.CorrelationId == message.CorrelationId);

        if (instance is null)
        {
            Console.WriteLine($"❌ WorkflowInstance not found for CorrelationId = {message.CorrelationId}");
            return;
        }

        // 🔄 Update rejection state
        instance.IsRejected = true;
        instance.IsApproved = false;
        instance.CurrentStep = message.StepName;
        instance.LastActorUsername = message.Actor.Username;
        instance.LastActorFullName = message.Actor.FullName;
        instance.LastActorEmail = message.Actor.Email;
        instance.LastActorEmployeeCode = message.Actor.EmployeeCode;
        instance.LastActionAt = message.OccurredAt;

        await _db.SaveChangesAsync();
        Console.WriteLine("✏️ Updated workflow instance after rejection.");

        // ✅ Push notification
        await context.Publish<IPushNotificationRequested>(new
        {
            message.CorrelationId,
            message.StepName,
            message.Actor,
            Title = $"❌ Request Rejected",
            Message = $"Request {message.CorrelationId} was rejected at step: {message.StepName}",
            UserId = Guid.Parse("b2e05ec4-6022-4f35-baea-ceb7fa2ee9dd")
        });
    }
}