using MassTransit;
using WFE.Engine.Contracts;

namespace WFE.Engine.WorkflowRouting.Consumers
{
    public class PushNotificationRequestedConsumer(ILogger<PushNotificationRequestedConsumer> logger, HttpClient httpClient) : IConsumer<IPushNotificationRequested>
    {
        private readonly ILogger<PushNotificationRequestedConsumer> _logger = logger;
        private readonly HttpClient _httpClient = httpClient;

        public async Task Consume(ConsumeContext<IPushNotificationRequested> context)
        {
            var msg = context.Message;

            _logger.LogInformation("PushNotificationConsumer >> Title: {Title}, To: {Recipient}, Msg: {Message}",
                msg.Title, msg.UserId, msg.Message);

            var payload = new
            {
                msg.Message,
                UserId = "b2e05ec4-6022-4f35-baea-ceb7fa2ee9dd"
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    "https://alpha.histaff.vn/api/WebPush/SendSimpleWebPushNotification",
                    payload,
                    context.CancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Push notification failed: {StatusCode}", response.StatusCode);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PushNotificationActivity failed.");
            }
        }
    }
}
