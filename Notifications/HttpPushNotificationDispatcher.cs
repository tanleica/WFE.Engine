// File: Notifications/HttpPushNotificationDispatcher.cs
using WFE.Engine.Contracts;

namespace WFE.Engine.Notifications;

public class HttpPushNotificationDispatcher(
    ILogger<HttpPushNotificationDispatcher> logger,
    HttpClient httpClient) : IPushNotificationDispatcher
{
    public async Task SendAsync(
        string title,
        string message,
        Guid userId,
        Guid correlationId,
        Actor actor,
        CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            Title = title,
            Message = message,
            UserId = userId,
            CorrelationId = correlationId,
            Actor = new
            {
                actor.Id,
                actor.Username,
                actor.FullName,
                actor.Email,
                actor.EmployeeCode
            }
        };

        try
        {
            var response = await httpClient.PostAsJsonAsync(
                "https://alpha.histaff.vn/api/WebPush/SendSimpleWebPushNotification",
                payload,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("❌ Push notification failed: {StatusCode}", response.StatusCode);
            }
            else
            {
                logger.LogInformation("✅ Push notification sent to {UserId}: {Title}", userId, title);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "🔥 Push notification failed to send.");
        }
    }
}
