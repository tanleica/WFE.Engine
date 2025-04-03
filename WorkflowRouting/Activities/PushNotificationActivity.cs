using MassTransit;

namespace WFE.Engine.WorkflowRouting.Activities
{
    public class PushNotificationActivity(ILogger<PushNotificationActivity> logger, HttpClient httpClient) : IExecuteActivity<PushNotificationArguments>
    {
        private readonly ILogger<PushNotificationActivity> _logger = logger;
        private readonly HttpClient _httpClient = httpClient;

        public async Task<ExecutionResult> Execute(ExecuteContext<PushNotificationArguments> context)
        {
            var args = context.Arguments;

            var payload = new
            {
                args.Title,
                args.Message,
                RecipientUsername = "b2e05ec4-6022-4f35-baea-ceb7fa2ee9dd"
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

                _logger.LogInformation("[Push] Sent to {Recipient}: {Title}", args.RecipientUsername, args.Title);

                return context.Completed(new PushNotificationLog
                {
                    Status = "Delivered",
                    DeliveredAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PushNotificationActivity failed.");
                return context.Faulted(ex);
            }
        }
    }
}
