using MassTransit;
using WFE.Engine.Contracts;
using WFE.Engine.DTOs;
using WFE.Engine.Events;

namespace WFE.Engine.WorkflowRouting.Consumers;

public class VoteRequestedConsumer(ILogger<VoteRequestedConsumer> logger) : IConsumer<IVoteRequested>
{
    private readonly ILogger<VoteRequestedConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<IVoteRequested> context)
    {
        var msg = context.Message;

        Console.WriteLine($"📢 [VoteRequestedConsumer Consume]");
        Console.WriteLine($"   → StepName: {msg.StepName}");
        Console.WriteLine($"   → ActorUsername: {msg.Actor.Username}");
        Console.WriteLine($"   → ActorFullName: {msg.Actor.FullName}");
        Console.WriteLine($"   → ActorEmail: {msg.Actor.Email}");
        Console.WriteLine($"   → ActorEmployeeCode: {msg.Actor.EmployeeCode}");

        var delay = Random.Shared.Next(1000, 3000);
        _logger.LogInformation("📢 Auto-vote simulation started (delay: {Delay} ms) for {User} on {Step}",
            delay, msg.Actor.Username, msg.StepName);

        await Task.Delay(delay);

        var isApproved = Random.Shared.NextDouble() >= 0.3; // ~30% chance to reject


        Console.WriteLine("💙💙💙 AHA?");
        Console.WriteLine($"   → ActorUsername: {msg.Actor.Username}");
        Console.WriteLine($"   → ActorFullName: {msg.Actor.FullName}");
        Console.WriteLine($"   → ActorEmail: {msg.Actor.Email}");
        Console.WriteLine($"   → ActorEmployeeCode: {msg.Actor.EmployeeCode}");

        var voteSubmitted = msg.CloneFrom<StepVoteSubmitted>(
            msg.StepName,
            msg.Actor,
            DateTime.UtcNow
        );

        await context.Publish<IStepVoteSubmitted>(new
        {
            msg.CorrelationId,
            msg.StepName,
            msg.Actor,
            IsApproved = isApproved,
            Reason = isApproved ? "Auto-approved for test" : "Simulated rejection",
            PerformedAt = DateTime.UtcNow
        });
        _logger.LogInformation("📢 Auto-vote submitted: {Decision} for {User} on step '{Step}'",
            isApproved ? "Approved" : "Rejected", msg.Actor.Username, msg.StepName);
    }
}
