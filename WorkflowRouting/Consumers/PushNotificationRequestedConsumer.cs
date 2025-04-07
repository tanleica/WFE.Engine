using MassTransit;
using WFE.Engine.Contracts;
using WFE.Engine.Notifications;

namespace WFE.Engine.WorkflowRouting.Consumers;

public class PushNotificationRequestedConsumer(
    IPushNotificationDispatcher dispatcher,
    ILogger<PushNotificationRequestedConsumer> logger) : IConsumer<IPushNotificationRequested>
{
    public async Task Consume(ConsumeContext<IPushNotificationRequested> context)
    {
        var message = context.Message;

        logger.LogInformation("📨 [PushNotificationRequestedConsumer] Received push request: {Title} => {UserId}",
            message.Title, message.UserId);

        await dispatcher.SendAsync(
            title: message.Title,
            message: message.Message,
            userId: message.UserId,
            correlationId: message.CorrelationId,
            actor: message.Actor,
            cancellationToken: context.CancellationToken);
    }
}
