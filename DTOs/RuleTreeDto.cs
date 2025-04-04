using WFE.Engine.Domain.Constants;

namespace WFE.Engine.DTOs
{
    public class RuleTreeDto
    {
        public string? LogicalOperator { get; set; } = LogicalOperators.Leaf;

        // Optional metadata for this rule node
        public string? RuleName { get; set; }
        public string? PredicateScript { get; set; } // SQL, expression, or other DSL
        public string? FilterMode { get; set; } = FilterModes.Forward;

        // New: Step details associated with this rule (if applicable)
        public StepNodeDto? Step { get; set; }

        // Children for branching logic
        public List<RuleTreeDto>? Children { get; set; }
    }
}