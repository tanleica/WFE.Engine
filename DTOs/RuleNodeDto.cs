namespace WFE.Engine.DTOs
{
    public class RuleNodeDto
    {
         public string? LogicalOperator { get; set; } // "And", "Or", or "Leaf"

        // Leaf properties (only used if this is a leaf)
        public string? RuleName { get; set; }
        public string? ConditionScript { get; set; }

        public string? FilterMode { get; set; } = "SoftWarn"; // "SoftWarn" (default) or "HardBlock"

        // Recursion: for And/Or/Leaf grouping
        public List<RuleNodeDto>? Children { get; set; }
    }
}