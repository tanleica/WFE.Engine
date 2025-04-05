using Dapper;
using WFE.Engine.DTOs;
using System.Data;
using DynamicExpresso;
using WFE.Engine.Domain.Constants;

namespace WFE.Engine.WorkflowRouting.Helpers
{
    public class RuleTreeEvaluator : IRuleTreeEvaluator
    {
        public async Task<(bool isAllowed, string? failedRuleName, string? filterMode)> EvaluateAsync(
            RuleNodeDto? node,
            IDbConnection? dbConnection = null,
            DynamicParameters? sqlParams = null,
            Dictionary<string, object>? inMemoryVars = null)
        {
            if (node == null)
                return (true, null, null); // Null node → interpreted as passed

            if (string.Equals(node.LogicalOperator, "Leaf", StringComparison.OrdinalIgnoreCase) || node.LogicalOperator == null)
            {
                if (string.IsNullOrWhiteSpace(node.PredicateScript))
                    return (true, null, null); // No condition → allow by default

                try
                {
                    // Check if PredicateScript is SQL (contains SELECT)
                    bool isSql = node.PredicateScript.Contains("SELECT", StringComparison.OrdinalIgnoreCase);

                    if (isSql)
                    {
                        if (dbConnection == null || sqlParams == null)
                            throw new InvalidOperationException($"🛑 SQL-based rule requires dbConnection and sqlParams: Rule = {node.RuleName}");

                        string wrappedSql = $"SELECT CASE WHEN ({node.PredicateScript}) THEN 1 ELSE 0 END";
                        Console.WriteLine($"🧠 Executing SQL Rule: {node.RuleName ?? "UnnamedRule"}");
                        Console.WriteLine($"→ SQL: {wrappedSql}");

                        var passed = await dbConnection.QueryFirstOrDefaultAsync<bool>(wrappedSql, sqlParams);
                        if (!passed)
                            return (false, node.RuleName ?? "UnnamedRule", node.FilterMode ?? FilterModes.SoftWarn);

                        return (true, null, null);
                    }
                    else
                    {
                        Console.WriteLine($"🧠 Evaluating In-Memory Rule: {node.RuleName ?? "UnnamedRule"}");
                        Console.WriteLine($"→ Expr: {node.PredicateScript}");

                        var interpreter = new Interpreter();
                        if (inMemoryVars != null)
                        {
                            foreach (var kvp in inMemoryVars)
                                interpreter.SetVariable(kvp.Key, kvp.Value);
                        }

                        bool result = interpreter.Eval<bool>(node.PredicateScript);
                        if (!result)
                            return (false, node.RuleName ?? "UnnamedRule", node.FilterMode ?? FilterModes.SoftWarn);

                        return (true, null, null);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"❌ Failed to evaluate rule '{node.RuleName ?? "UnnamedRule"}' → {ex.Message}", ex);
                }
            }

            // Logical Node (And/Or) — recurse children
            bool isAnd = string.Equals(node.LogicalOperator, "And", StringComparison.OrdinalIgnoreCase);
            var children = node.Children ?? new();

            foreach (var child in children)
            {
                var (childPassed, failedName, mode) = await EvaluateAsync(child, dbConnection, sqlParams, inMemoryVars);

                if (isAnd && !childPassed)
                    return (false, failedName, mode); // ⛔ fail-fast

                if (!isAnd && childPassed)
                    return (true, null, null); // ✅ shortcut
            }

            return isAnd
                ? (true, null, null) // ✅ all passed
                : (false, "No child matched", node.FilterMode ?? FilterModes.SoftWarn); // ⛔ none passed
        }
    }
}
