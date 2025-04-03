namespace WFE.Engine.WorkflowRouting.Activities
{
    public class PushNotificationLog
    {
        public string Status { get; set; } = "Delivered";
        public DateTime DeliveredAt { get; set; } = DateTime.UtcNow;
    }
}
