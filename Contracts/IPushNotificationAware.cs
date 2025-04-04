namespace WFE.Engine.Contracts
{
    public interface IPushNotificationAware
    {
        string Title { get; }
        string Message { get; }
        Guid UserId { get; }

        // 🌐 Optional PWA/WebPush metadata
        string? IconUrl { get; }
        string? ImageUrl { get; }
        string? ClickActionUrl { get; }
        string? BadgeUrl { get; }
        string? Tag { get; }
    }
}