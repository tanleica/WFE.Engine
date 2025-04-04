namespace WFE.Engine.Contracts
{
    public interface IPushNotificationRequested : IWorkflowEvent, IStepAware, IActorCarrier, IPushNotificationAware
    {
    }
}
