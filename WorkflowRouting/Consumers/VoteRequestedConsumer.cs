using MassTransit;
using WFE.Engine.Contracts;

namespace WFE.Engine.WorkflowRouting.Consumers;

public class VoteRequestedConsumer(ILogger<VoteRequestedConsumer> logger) : IConsumer<IVoteRequested>
{
    private readonly ILogger<VoteRequestedConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<IVoteRequested> context)
    {
        var msg = context.Message;

        Console.WriteLine($"📢 [VoteRequestedConsumer Consume]");
        Console.WriteLine($"   → StepName: {msg.StepName}");
        Console.WriteLine($"   → ActorUsername: {msg.ActorUsername}");
        Console.WriteLine($"   → ActorFullName: {msg.ActorFullName}");
        Console.WriteLine($"   → ActorEmail: {msg.ActorEmail}");
        Console.WriteLine($"   → ActorEmployeeCode: {msg.ActorEmployeeCode}");

        var delay = Random.Shared.Next(1000, 3000);
        _logger.LogInformation("📢 Auto-vote simulation started (delay: {Delay} ms) for {User} on {Step}",
            delay, msg.ActorUsername, msg.StepName);

        await Task.Delay(delay);

        var isApproved = Random.Shared.NextDouble() >= 0.3; // ~30% chance to reject


        Console.WriteLine("💙💙💙 AHA?");
        Console.WriteLine($"   → ActorUsername: {msg.ActorUsername}");
        Console.WriteLine($"   → ActorFullName: {msg.ActorFullName}");
        Console.WriteLine($"   → ActorEmail: {msg.ActorEmail}");
        Console.WriteLine($"   → ActorEmployeeCode: {msg.ActorEmployeeCode}");

        await context.Publish<IStepVoteSubmitted>(new
        {
            msg.CorrelationId,
            msg.StepName,
            msg.ActorUsername,
            msg.ActorFullName,
            msg.ActorEmail,
            msg.ActorEmployeeCode,
            IsApproved = isApproved,
            Reason = isApproved ? "Auto-approved for test" : "Simulated rejection",
            PerformedAt = DateTime.UtcNow
        });

        _logger.LogInformation("📢 Auto-vote submitted: {Decision} for {User} on step '{Step}'",
            isApproved ? "Approved" : "Rejected", msg.ActorUsername, msg.StepName);
    }
}
