using MassTransit;
using WFE.Engine.Contracts;

namespace WFE.Engine.WorkflowRouting.Consumers;

public class RequestRejectedConsumer(ILogger<RequestRejectedConsumer> logger) : IConsumer<IRequestRejected>
{
    private readonly ILogger<RequestRejectedConsumer> _logger = logger;

    public Task Consume(ConsumeContext<IRequestRejected> context)
    {
        /*
        var msg = context.Message;

        _logger.LogWarning("❌ REQUEST REJECTED");
        _logger.LogWarning("↪️ Step: {Step}", msg.StepName);
        _logger.LogWarning("🧑‍💼 By: {User} ({Email})", msg.RejectedByFullName, msg.RejectedByEmail);
        _logger.LogWarning("💬 Reason: {Reason}", msg.Reason ?? "(none)");
        _logger.LogWarning("🕒 At: {Time}", msg.RejectedAt);

        // ✅ Publish push notification request
        await context.Publish<IPushNotificationRequested>(new
        {
            RecipientUsername = "b2e05ec4-6022-4f35-baea-ceb7fa2ee9dd",
            Title = $"Request Rejected at Step: {msg.StepName}",
            Body = msg.Reason ?? "No reason provided"
        });
        */
        return Task.CompletedTask;
    }
}
