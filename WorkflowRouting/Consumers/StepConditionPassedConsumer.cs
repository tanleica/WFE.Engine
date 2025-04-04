using MassTransit;
using WFE.Engine.Persistence;
using WFE.Engine.Domain.Workflow;
using WFE.Engine.Contracts;
using Microsoft.EntityFrameworkCore;

namespace WFE.Engine.WorkflowRouting.Consumers
{
    public class StepConditionPassedConsumer(SagaDbContext db) : IConsumer<IStepConditionPassed>
    {
        private readonly SagaDbContext _db = db;

        public async Task Consume(ConsumeContext<IStepConditionPassed> context)
        {
            var message = context.Message;

            Console.WriteLine($"✅ Step Completed: {message.StepName} by {message.Actor.FullName}");

            // Save step progress
            await _db.StepProgresses.AddAsync(new StepProgress
            {
                CorrelationId = message.CorrelationId,
                WorkflowStepId = await _db.WorkflowSteps
                    .Where(s => s.StepName == message.StepName)
                    .Select(s => s.Id)
                    .FirstAsync(),

                ActorUsername = message.Actor.Username,
                ActorFullName = message.Actor.FullName,
                ActorEmail = message.Actor.Email,
                ActorEmployeeCode = message.Actor.EmployeeCode,
                CompletedAt = message.OccurredAt,
                IsCompleted = true,
                Reason = message.Reason
            });

            await _db.SaveChangesAsync();

            // 🧠 Optional: Evaluate if this was the final step and trigger IRequestApproved
            // (we'll decide this logic in the next steps)
        }
    }
}