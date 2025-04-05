using WFE.Engine.DTOs;

namespace WFE.Engine.WorkflowRouting.Helpers
{
    public interface IBranchRuleParser
    {
        RuleNodeDto? Parse(string? json);
    }
}