using WFE.Engine.Contracts;

namespace WFE.Engine.Events
{
    public record PushNotificationRequested : WorkflowEventBase, IPushNotificationRequested
    {
        public string Title { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public Guid UserId { get; init; } = Guid.Empty;

        // 🌐 Optional PWA/WebPush metadata
        public string? IconUrl { get; init; }
        public string? ImageUrl { get; init; }
        public string? ClickActionUrl { get; init; }
        public string? BadgeUrl { get; init; }
        public string? Tag { get; init; }
    }
}
