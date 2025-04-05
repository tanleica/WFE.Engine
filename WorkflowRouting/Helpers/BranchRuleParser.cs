using System.Text.Json;
using WFE.Engine.DTOs;

namespace WFE.Engine.WorkflowRouting.Helpers
{
    public class BranchRuleParser : IBranchRuleParser
    {
        public RuleNodeDto? Parse(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonSerializer.Deserialize<RuleNodeDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}