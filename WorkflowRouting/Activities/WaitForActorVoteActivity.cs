using MassTransit;
using WFE.Engine.Contracts;
using WFE.Engine.Events;

namespace WFE.Engine.WorkflowRouting.Activities;

public class WaitForActorVoteActivity : IActivity<WaitForActorVoteArguments, WaitForActorVoteLog>
{
    public async Task<ExecutionResult> Execute(ExecuteContext<WaitForActorVoteArguments> context)
    {
        var args = context.Arguments;

        Console.WriteLine($"🎭 [WaitForActorVoteActivity Execute] Waiting for vote:");
        Console.WriteLine($"   → StepName: {args.StepName}");
        Console.WriteLine($"   → ActorUsername: {args.Actor.Username}");
        Console.WriteLine($"   → ActorFullName: {args.Actor.FullName}");
        Console.WriteLine($"   → ActorEmail: {args.Actor.Email}");
        Console.WriteLine($"   → ActorEmployeeCode: {args.Actor.EmployeeCode}");

        // ✅ Publish vote request event
         var evt = new StepVoteRequested
        {
            CorrelationId = args.CorrelationId,
            StepName = args.StepName,
            Actor = args.Actor,
            OccurredAt = DateTime.UtcNow
        };

        await context.Publish<IVoteRequested>(evt);

        return context.Completed<WaitForActorVoteLog>(new()
        {
            StepName = args.StepName,
            Actor = args.Actor,
        });
    }


    public Task<CompensationResult> Compensate(CompensateContext<WaitForActorVoteLog> context)
    {
        Console.WriteLine("🌀 [WaitForActorVote] Compensation is not required.");
        return Task.FromResult(context.Compensated());
    }
}
