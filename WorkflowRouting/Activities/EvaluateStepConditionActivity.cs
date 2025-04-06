using MassTransit;
using Dapper;
using WFE.Engine.WorkflowRouting.Helpers;
using WFE.Engine.Persistence;
using WFE.Engine.Contracts;
using WFE.Engine.Domain.Workflow;
using WFE.Engine.DTOs;
using WFE.Engine.Utilities;

namespace WFE.Engine.WorkflowRouting.Activities;

public class EvaluateStepConditionActivity : IActivity<EvaluateStepConditionArguments, EvaluateStepConditionLog>
{
    private readonly SagaDbContext _db;
    private readonly IRuleTreeEvaluator _ruleTreeEvaluator;
    private readonly ILogger<EvaluateStepConditionActivity> _logger;

    public EvaluateStepConditionActivity(
        SagaDbContext db,
        IRuleTreeEvaluator ruleTreeEvaluator,
        ILogger<EvaluateStepConditionActivity> logger)
    {
        _db = db;
        _ruleTreeEvaluator = ruleTreeEvaluator;
        _logger = logger;
    }

    /*
        public async Task<ExecutionResult> Execute(ExecuteContext<EvaluateStepConditionArguments> context)
        {
            var args = context.Arguments;

            _logger.LogInformation("🧩 Executing: {StepName} → EvaluateStepConditionActivity", args.StepName);

            Console.WriteLine("🧻 [EvaluateStepConditionActivity Execute]");
            Console.WriteLine($"    → CorrelationId: {args.CorrelationId}");
            Console.WriteLine($"    → StepName: {args.StepName}");
            Console.WriteLine($"    → ActorUsername: {args.Actor.Username}");

            _logger.LogInformation("🧻 WorkflowStepId passed: {WorkflowStepId}", args.WorkflowStepId);

            // Simulate human-like delay
            var delayMs = Random.Shared.Next(300, 2000);
            await Task.Delay(delayMs);

            bool canVote = true;
            string reason;
            string? filterMode = "SoftWarn"; // Default fallback

            // Step 1: Smart skip if no rule to evaluate
            if (args.RuleTree == null || !TreeHasExecutableLeaf(args.RuleTree))
            {
                reason = "No RuleTree to evaluate";
                _logger.LogInformation("ℹ️ Step '{StepName}' has no executable condition. Skipping DB evaluation.", args.StepName);
            }
            else
            {
                // Step 2: Evaluate via external DB
                var encrypted = context.GetVariable<string>("EncryptedConnectionString");
                var dbType = context.GetVariable<string>("DbType");

                if (string.IsNullOrWhiteSpace(encrypted) || string.IsNullOrWhiteSpace(dbType))
                    return context.Faulted(new InvalidOperationException("Missing RoutingSlip vars: DbType or ConnectionString"));

                var connectionString = encrypted.CoreDecrypt();
                var parameters = new DynamicParameters(args.DynamicSqlParameters);

                bool isAllowed = false;
                string? failedRule = null;

                await DbConnectionHelper.UseOpenConnectionAsync(dbType, connectionString, async conn =>
                {
                    (isAllowed, failedRule, filterMode) = await _ruleTreeEvaluator.EvaluateAsync(args.RuleTree, conn, parameters);
                });

                canVote = isAllowed || filterMode == "SoftWarn";
                reason = isAllowed ? "Rule passed" : $"Failed: {failedRule} ({filterMode})";

                if (!isAllowed && filterMode == "HardBlock")
                    _logger.LogWarning("❌ Step '{StepName}' blocked by rule: {Rule}", args.StepName, failedRule);
                else if (!isAllowed)
                    _logger.LogWarning("⚠️ Step '{StepName}' soft-warned by rule: {Rule}", args.StepName, failedRule);
            }

            // Step 3: Record StepProgress
            await _db.StepProgresses.AddAsync(new StepProgress
            {
                CorrelationId = args.CorrelationId,
                WorkflowStepId = args.WorkflowStepId,
                CompletedAt = DateTime.UtcNow,
                IsCompleted = false, // Not yet voted
                ConditionPassed = canVote,
                CanActorVote = canVote,
                FilterMode = filterMode,
                Reason = reason,
                ActorUsername = args.Actor.Username,
                ActorFullName = args.Actor.FullName,
                ActorEmail = args.Actor.Email,
                ActorEmployeeCode = args.Actor.EmployeeCode
            });

            await _db.SaveChangesAsync();

            // Step 4: Publish vote eligibility
            _logger.LogInformation("📣 Publishing IStepConditionPassed → User may vote: {CanVote}", canVote);

            await context.Publish<IStepConditionPassed>(new
            {
                args.CorrelationId,
                args.StepName,
                args.Actor,
                Reason = reason,
                IsApproved = canVote,
                PerformedAt = DateTime.UtcNow
            });

            return context.Completed<EvaluateStepConditionLog>(new()
            {
                EvaluatedStep = args.StepName,
                IsAllowed = canVote,
                Reason = reason
            });
        }
    */

    public async Task<ExecutionResult> Execute(ExecuteContext<EvaluateStepConditionArguments> context)
    {
        var args = context.Arguments;

        _logger.LogInformation("🧩 Executing: {StepName} → EvaluateStepConditionActivity", args.StepName);

        Console.WriteLine("🧻 [EvaluateStepConditionActivity Execute]");
        Console.WriteLine($"    → CorrelationId: {args.CorrelationId}");
        Console.WriteLine($"    → StepName: {args.StepName}");
        Console.WriteLine($"    → ActorUsername: {args.Actor.Username}");

        _logger.LogInformation("🧻 WorkflowStepId passed: {WorkflowStepId}", args.WorkflowStepId);

        // Simulate human-like delay
        var delayMs = Random.Shared.Next(300, 2000);
        await Task.Delay(delayMs);

        bool canVote = true;
        string reason;
        string? filterMode = "SoftWarn"; // Default fallback

        // Step 1: Smart skip if no rule to evaluate
        if (args.RuleTree == null || !TreeHasExecutableLeaf(args.RuleTree))
        {
            reason = "No RuleTree to evaluate";
            _logger.LogInformation("ℹ️ Step '{StepName}' has no executable condition. Skipping DB evaluation.", args.StepName);
        }
        else
        {
            // Step 2: Evaluate via external DB
            var encrypted = context.GetVariable<string>("EncryptedConnectionString");
            var dbType = context.GetVariable<string>("DbType");

            if (string.IsNullOrWhiteSpace(encrypted) || string.IsNullOrWhiteSpace(dbType))
            {
                reason = "🔃 Missing connection info — assuming allowed (test mode)";
                canVote = true;
                _logger.LogWarning("⚠️ Skipping rule evaluation for step '{StepName}' due to missing connection info. Assuming canVote = true", args.StepName);
            }
            else
            {
                var connectionString = encrypted.CoreDecrypt();
                var parameters = new DynamicParameters(args.DynamicSqlParameters);

                bool isAllowed = false;
                string? failedRule = null;

                await DbConnectionHelper.UseOpenConnectionAsync(dbType, connectionString, async conn =>
                {
                    (isAllowed, failedRule, filterMode) = await _ruleTreeEvaluator.EvaluateAsync(args.RuleTree, conn, parameters);
                });

                canVote = isAllowed || filterMode == "SoftWarn";
                reason = isAllowed ? "Rule passed" : $"Failed: {failedRule} ({filterMode})";

                if (!isAllowed && filterMode == "HardBlock")
                    _logger.LogWarning("❌ Step '{StepName}' blocked by rule: {Rule}", args.StepName, failedRule);
                else if (!isAllowed)
                    _logger.LogWarning("⚠️ Step '{StepName}' soft-warned by rule: {Rule}", args.StepName, failedRule);
            }
        }

        // Step 3: Record StepProgress
        await _db.StepProgresses.AddAsync(new StepProgress
        {
            CorrelationId = args.CorrelationId,
            WorkflowStepId = args.WorkflowStepId,
            CompletedAt = DateTime.UtcNow,
            IsCompleted = false, // Not yet voted
            ConditionPassed = canVote,
            CanActorVote = canVote,
            FilterMode = filterMode,
            Reason = reason,
            ActorUsername = args.Actor.Username,
            ActorFullName = args.Actor.FullName,
            ActorEmail = args.Actor.Email,
            ActorEmployeeCode = args.Actor.EmployeeCode
        });

        await _db.SaveChangesAsync();

        // Step 4: Publish vote eligibility
        _logger.LogInformation("📣 Publishing IStepConditionPassed → User may vote: {CanVote}", canVote);

        await context.Publish<IStepConditionPassed>(new
        {
            args.CorrelationId,
            args.StepName,
            args.Actor,
            Reason = reason,
            IsApproved = canVote,
            PerformedAt = DateTime.UtcNow
        });

        return context.Completed<EvaluateStepConditionLog>(new()
        {
            EvaluatedStep = args.StepName,
            IsAllowed = canVote,
            Reason = reason
        });
    }

    public Task<CompensationResult> Compensate(CompensateContext<EvaluateStepConditionLog> context)
    {
        _logger.LogInformation("🌀 No compensation logic needed.");
        return Task.FromResult(context.Compensated());
    }

    private static bool TreeHasExecutableLeaf(RuleNodeDto node)
    {
        if ((node.LogicalOperator == null || node.LogicalOperator == "Leaf")
            && !string.IsNullOrWhiteSpace(node.PredicateScript))
            return true;

        return node.Children?.Any(TreeHasExecutableLeaf) == true;
    }
}
