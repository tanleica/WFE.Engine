using MassTransit.Courier.Contracts;
using WFE.Engine.DTOs;

namespace WFE.Engine.WorkflowRouting.Builders
{
    public interface IRoutingSlipBuilderService
    {
        Task<RoutingSlip> BuildAsync(KickoffRequestDto request);
    }
}