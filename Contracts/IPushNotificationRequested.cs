namespace WFE.Engine.Contracts
{
    public interface IPushNotificationRequested
    {
        Guid CorrelationId { get; }
        string Title { get; }
        string Message { get; }
        string RecipientUsername { get; }
        DateTime SentAt { get; }
    }
}
