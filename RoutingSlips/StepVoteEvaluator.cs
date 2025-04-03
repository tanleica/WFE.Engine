using WFE.Engine.Domain.Constants;

namespace WFE.Engine.RoutingSlips;

public class StepVoteEvaluator(VoteTallyingService tallyingService)
{
    private readonly VoteTallyingService _tallyingService = tallyingService;

    public async Task<(bool IsStepApproved, bool IsStepRejected)> EvaluateStepAsync(
        string approvalType,
        Guid correlationId,
        Guid workflowStepId,
        int totalActors)
    {
        var approvals = await _tallyingService.CountApprovalsAsync(correlationId, workflowStepId);
        var rejections = await _tallyingService.CountRejectionsAsync(correlationId, workflowStepId);
        var totalVotes = await _tallyingService.CountTotalVotesAsync(correlationId, workflowStepId);

        switch (approvalType)
        {
            case ApprovalTypes.Sequential:
                return (approvals > 0, rejections > 0);

            case ApprovalTypes.ShortCircuit:
                if (approvals > 0) return (true, false);
                if (rejections > 0) return (false, true);
                return (false, false);

            case ApprovalTypes.Parallel:
                if (rejections > 0) return (false, true);
                if (approvals == totalActors) return (true, false);
                return (false, false);

            default:
                throw new InvalidOperationException($"Unsupported approval type: {approvalType}");
        }
    }
}
