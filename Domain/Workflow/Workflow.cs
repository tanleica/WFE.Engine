namespace WFE.Engine.Domain.Workflow
{
    public class Workflow
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        public List<WorkflowBranch> Branches { get; set; } = []; 

        public ICollection<WorkflowStep> Steps { get; set; } = [];
    }
}