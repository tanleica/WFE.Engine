using WFE.Engine.Contracts;

namespace WFE.Engine.Notifications;

public class PushNotificationDispatcher(
    ILogger<PushNotificationDispatcher> logger,
    HttpClient httpClient
) : IPushNotificationDispatcher
{
    private readonly ILogger<PushNotificationDispatcher> _logger = logger;
    private readonly HttpClient _httpClient = httpClient;

    public Task SendAsync(
        string title, 
        string message, 
        Guid userId, 
        Guid correlationId, 
        Actor actor,
        CancellationToken cancellationToken = default
        )
    {
        _logger.LogInformation("📢 Push Notification\n\tTo       : {Username}\n\tTitle    : {Title}\n\tMessage  : {Message}\n\tStep     : (dynamic)\n\tReason   : (dynamic)\n\tWhen     : {Now}",
            actor.Username,
            title,
            message,
            DateTime.UtcNow.ToLocalTime());

        // Placeholder for actual HTTP call to push gateway
        // return _httpClient.PostAsJsonAsync("https://your-api/push", new { title, message, userId });

        return Task.CompletedTask;
    }
}
