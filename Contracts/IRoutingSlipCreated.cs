namespace WFE.Engine.Contracts
{
    public interface IRoutingSlipCreated
    {
        Guid TrackingNumber { get; }      // MassTransit RoutingSlip ID
        Guid CorrelationId { get; }       // Your custom workflow/request ID
        DateTime Timestamp { get; }
    }
}
