using WFE.Engine.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WFE.Engine.RoutingSlips;

public class VoteTallyingService(SagaDbContext db)
{
    private readonly SagaDbContext _db = db;

    public async Task<int> CountApprovalsAsync(Guid correlationId, Guid workflowStepId)
    {
        return await _db.StepProgresses
            .CountAsync(p =>
                p.CorrelationId == correlationId &&
                p.WorkflowStepId == workflowStepId &&
                p.IsCompleted == true &&
                p.ConditionPassed == true &&
                p.CanActorVote == true // Optional
            );
    }

    public async Task<int> CountRejectionsAsync(Guid correlationId, Guid workflowStepId)
    {
        return await _db.StepProgresses
            .CountAsync(p =>
                p.CorrelationId == correlationId &&
                p.WorkflowStepId == workflowStepId &&
                p.IsCompleted == true &&
                p.ConditionPassed == true &&
                p.CanActorVote == true &&
                p.Reason != null && p.Reason.ToLower().Contains("reject") // or use flag if available
            );
    }

    public async Task<int> CountTotalVotesAsync(Guid correlationId, Guid workflowStepId)
    {
        return await _db.StepProgresses
            .CountAsync(p =>
                p.CorrelationId == correlationId &&
                p.WorkflowStepId == workflowStepId &&
                p.IsCompleted == true &&
                p.ConditionPassed == true &&
                p.CanActorVote == true
            );
    }
}
