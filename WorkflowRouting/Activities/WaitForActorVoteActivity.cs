using MassTransit;
using WFE.Engine.Contracts;

namespace WFE.Engine.WorkflowRouting.Activities;

public class WaitForActorVoteActivity : IActivity<WaitForActorVoteArguments, WaitForActorVoteLog>
{
    public async Task<ExecutionResult> Execute(ExecuteContext<WaitForActorVoteArguments> context)
    {
        var args = context.Arguments;

        Console.WriteLine($"🎭 [WaitForActorVoteActivity Execute] Waiting for vote:");
        Console.WriteLine($"   → StepName: {args.StepName}");
        Console.WriteLine($"   → ActorUsername: {args.ActorUsername}");
        Console.WriteLine($"   → ActorFullName: {args.ActorFullName}");
        Console.WriteLine($"   → ActorEmail: {args.ActorEmail}");
        Console.WriteLine($"   → ActorEmployeeCode: {args.ActorEmployeeCode}");

        // ✅ Publish vote request event
        await context.Publish<IVoteRequested>(new
        {
            args.CorrelationId,
            args.StepName,
            args.ActorUsername,
            args.ActorFullName,
            args.ActorEmail,
            args.ActorEmployeeCode,
            RequestedAt = DateTime.UtcNow
        });

        return context.Completed<WaitForActorVoteLog>(new()
        {
            StepName = args.StepName,
            ActorUsername = args.ActorUsername,
            ActorFullName = args.ActorFullName,
            ActorEmail = args.ActorEmail,
            ActorEmployeeCode = args.ActorEmployeeCode
        });
    }


    public Task<CompensationResult> Compensate(CompensateContext<WaitForActorVoteLog> context)
    {
        Console.WriteLine("🌀 [WaitForActorVote] Compensation is not required.");
        return Task.FromResult(context.Compensated());
    }
}
