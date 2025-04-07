using WFE.Engine.Contracts;

namespace WFE.Engine.Notifications
{
    public interface IPushNotificationDispatcher
    {
        Task SendAsync(
            string title, 
            string message, 
            Guid userId, 
            Guid correlationId, 
            Actor actor,
            CancellationToken cancellationToken
            );
    }

}