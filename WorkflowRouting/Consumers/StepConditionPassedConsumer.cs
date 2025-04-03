using MassTransit;
using WFE.Engine.Persistence;
using WFE.Engine.Domain.Workflow;
using WFE.Engine.Contracts;
using Microsoft.EntityFrameworkCore;

namespace WFE.Engine.WorkflowRouting.Consumers
{
    public class StepConditionPassedConsumer : IConsumer<IStepConditionPassed>
    {
        private readonly SagaDbContext _db;

        public StepConditionPassedConsumer(SagaDbContext db)
        {
            _db = db;
        }

        public async Task Consume(ConsumeContext<IStepConditionPassed> context)
        {
            var message = context.Message;

            Console.WriteLine($"✅ Step Completed: {message.StepName} by {message.PerformedByFullName}");

            // Save step progress
            await _db.StepProgresses.AddAsync(new StepProgress
            {
                CorrelationId = message.CorrelationId,
                WorkflowStepId = await _db.WorkflowSteps
                    .Where(s => s.StepName == message.StepName)
                    .Select(s => s.Id)
                    .FirstAsync(),

                PerformedByUsername = message.PerformedByUsername,
                PerformedByFullName = message.PerformedByFullName,
                PerformedByEmail = message.PerformedByEmail,
                PerformedByEmployeeCode = message.PerformedByEmployeeCode,
                CompletedAt = message.PerformedAt,
                IsCompleted = true,
                Reason = message.Reason
            });

            await _db.SaveChangesAsync();

            // 🧠 Optional: Evaluate if this was the final step and trigger IRequestApproved
            // (we'll decide this logic in the next steps)
        }
    }
}