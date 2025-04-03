
namespace WFE.Engine.DTOs
{
    public class PlannedRuleDto
    {
        public string RuleName { get; set; } = default!;
        public string? ConditionScript { get; set; }
        public string? DbType { get; set; }
        public string ConnectionString { get; set; } = default!;
        public string LogicalOperator { get; set; } = "Leaf";
        public List<PlannedRuleDto> Children { get; set; } = []; // 👈 RECURSIVE!
    }
}