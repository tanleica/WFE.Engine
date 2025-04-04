using WFE.Engine.Domain.Constants;

namespace WFE.Engine.DTOs
{
    public class RuleNodeDto
    {
        public string? LogicalOperator { get; set; } // "And", "Or", or "Leaf"

        // Leaf properties (only used if this is a leaf)
        public string? RuleName { get; set; }
        public string? PredicateScript  { get; set; }

        public string? FilterMode { get; set; } = FilterModes.Forward;

        // Recursion: for And/Or/Leaf grouping
        public List<RuleNodeDto>? Children { get; set; }
    }
}