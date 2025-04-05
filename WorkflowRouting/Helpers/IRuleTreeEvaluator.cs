using WFE.Engine.DTOs;
using System.Data;
using Dapper;

namespace WFE.Engine.WorkflowRouting.Helpers
{
    public interface IRuleTreeEvaluator
    {
        Task<(bool isAllowed, string? failedRuleName, string? filterMode)> EvaluateAsync(
            RuleNodeDto? node,
            IDbConnection? connection,
            DynamicParameters? parameters,
            Dictionary<string, object>? inMemoryVars = null // for in-memory evaluation
        );
    }
}
