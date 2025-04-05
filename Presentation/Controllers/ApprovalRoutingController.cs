using MassTransit;
using Microsoft.AspNetCore.Mvc;
using WFE.Engine.DTOs;
using WFE.Engine.WorkflowRouting.Builders;
using WFE.Engine.Presentation.Responses;
using WFE.Engine.Contracts;
using WFE.Engine.Events;

namespace WFE.Engine.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApprovalRoutingController : ControllerBase
{
    private readonly IBus _bus;
    private readonly IRoutingSlipBuilderService _builder;

    public ApprovalRoutingController(
        IBus bus,
        IRoutingSlipBuilderService builder)
    {
        _bus = bus;
        _builder = builder;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartApprovalAsync([FromBody] KickoffRequestDto request)
    {
        var routingSlip = await _builder.BuildAsync(request);

        // ✅ Use IBus.Execute extension method for RoutingSlip
        Console.WriteLine($"✅ Bus is executing a routing slip with tracking number {routingSlip.TrackingNumber} ");
        await _bus.Execute(routingSlip);

        // 🧠 Extract CorrelationId and TrackingNumber for response
        routingSlip.Variables.TryGetValue("CorrelationId", out var correlationIdObj);
        var correlationId = correlationIdObj?.ToString();

        return Ok(ApiResponse<object>.Ok(new
        {
            trackingNumber = routingSlip.TrackingNumber,
            correlationId,
            request.Actor.Username,
            request.Actor.FullName,
            request.Actor.Email,
            request.Actor.EmployeeCode,
            request.RequestedAt
        }, $"✅ Routing slip started for '{request.Actor.FullName}' ({request.Actor.EmployeeCode})"));
    }

    [HttpPost("vote")]
    public async Task<IActionResult> SubmitStepVote([FromBody] StepVoteDto dto, [FromServices] IPublishEndpoint publishEndpoint)
    {
        // Publish a domain event
        await publishEndpoint.Publish<IStepVoteSubmitted>(new StepVoteSubmitted()
        {
            CorrelationId = dto.CorrelationId,
            StepName = dto.StepName,
            Actor = dto.Actor,
            //dto.Reason,
            OccurredAt = DateTime.UtcNow
        });

        return Ok(ApiResponse<object>.Ok(new
        {
            Success = true
        }, $"✅ Vote submitted for step '{dto.StepName}'"));
    }
}
