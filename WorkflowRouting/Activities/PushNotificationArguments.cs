namespace WFE.Engine.WorkflowRouting.Activities
{
    public class PushNotificationArguments
    {
        public Guid CorrelationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string RecipientUsername { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
}
