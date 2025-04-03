using System.Data;
using WFE.Engine.DTOs;
using Dapper;

namespace WFE.Engine.WorkflowRouting.Helpers
{
    public static class RuleTreeEvaluator
    {
        public static async Task<(bool isAllowed, string? failedRuleName, string? filterMode)> EvaluateAsync(
            RuleNodeDto node,
            IDbConnection connection,
            DynamicParameters parameters)
        {
            // 🔹 Leaf Node Evaluation
            if (node.LogicalOperator == null || node.LogicalOperator == "Leaf")
            {
                if (string.IsNullOrWhiteSpace(node.ConditionScript))
                    throw new InvalidOperationException("Missing ConditionScript in leaf node.");

                var wrappedSql = $"SELECT CASE WHEN ({node.ConditionScript}) THEN 1 ELSE 0 END";

                try
                {
                    Console.WriteLine($"🧠 Executing ConditionScript for rule: {node.RuleName ?? "UnnamedRule"}");
                    Console.WriteLine($"→ SQL: {wrappedSql}");
                    foreach (var param in parameters.ParameterNames)
                        Console.WriteLine($"→ Param: {param} = {parameters.Get<object>(param)}");

                    var passed = await connection.QueryFirstOrDefaultAsync<bool>(wrappedSql, parameters);

                    if (!passed)
                        return (false, node.RuleName ?? "UnnamedRule", node.FilterMode ?? "SoftWarn");

                    return (true, null, null); // ✅ Passed
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"❌ Error while evaluating rule '{node.RuleName ?? "UnnamedRule"}': {ex.Message}", ex);
                }
            }

            // 🔹 Logical Operator Node (AND / OR)
            var isAnd = node.LogicalOperator == "And";
            foreach (var child in node.Children ?? Enumerable.Empty<RuleNodeDto>())
            {
                var (result, failedName, mode) = await EvaluateAsync(child, connection, parameters);

                if (isAnd && !result)
                    return (false, failedName, mode); // ⛔ AND failed

                if (!isAnd && result)
                    return (true, null, null); // ✅ OR shortcut
            }

            // Final result for grouped node
            return isAnd
                ? (true, null, null) // ✅ All passed in AND
                : (false, "No child matched", node.FilterMode ?? "SoftWarn"); // ⛔ All failed in OR
        }
    }
}
