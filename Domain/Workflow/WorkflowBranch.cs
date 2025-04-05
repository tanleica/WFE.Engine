using System.ComponentModel.DataAnnotations.Schema;

namespace WFE.Engine.Domain.Workflow;

public class WorkflowBranch
{
    public Guid Id { get; set; }
    public Guid WorkflowId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Priority { get; set; } = 1; // Lower = evaluated first
    public string? EntryConditionJson { get; set; }
    public List<WorkflowStep> Steps { get; set; } = [];
    public List<WorkflowRule> Rules { get; set; } = [];
}
