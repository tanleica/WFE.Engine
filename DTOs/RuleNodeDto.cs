using System.ComponentModel.DataAnnotations;
using WFE.Engine.Domain.Constants;

namespace WFE.Engine.DTOs
{
    public class RuleNodeDto
    {
        // 🔹 Required: Every node must have a StepName (especially for leaf execution)
        public string StepName { get; set; } = string.Empty;

        // 🔹 Node type: "And", "Or", or "Leaf" (null also means Leaf by default)
        public string? LogicalOperator { get; set; }

        // 🔹 Optional rule metadata for human-friendly naming
        public string? RuleName { get; set; }

        // 🔹 SQL or in-memory predicate
        public string? PredicateScript { get; set; }

        // 🔹 "Forward", "SoftWarn", "HardBlock" — default to Forward
        public string FilterMode { get; set; } = FilterModes.Forward;

        // 🔹 Recursion: child nodes if this is a group
        public List<RuleNodeDto>? Children { get; set; }
    }
}
