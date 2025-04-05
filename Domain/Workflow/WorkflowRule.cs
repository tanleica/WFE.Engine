using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using WFE.Engine.DTOs;

namespace WFE.Engine.Domain.Workflow
{
    public class WorkflowRule
    {
        public Guid Id { get; set; }

        public Guid WorkflowStepId { get; set; }
        public WorkflowStep Step { get; set; } = default!;

        public string RuleName { get; set; } = "Default";

        [NotMapped]
        public RuleNodeDto Node { get; set; } = new(); // will be serialized to EntryConditionJson
        public string EntryConditionJson
        {
            get => JsonSerializer.Serialize(Node);
            set => Node = JsonSerializer.Deserialize<RuleNodeDto>(value) ?? new();
        }
    }

}
