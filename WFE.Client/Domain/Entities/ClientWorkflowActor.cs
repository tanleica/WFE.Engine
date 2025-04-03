namespace WFE.Client.Domain.Entities;

public class ClientWorkflowActor
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ActorUsername { get; set; } = default!;
    public string ActorFullName { get; set; } = default!;
    public string ActorEmail { get; set; } = default!;
    public string ActorEmployeeCode { get; set; } = default!;

    public bool IsMandatory { get; set; } = true;
    public int Order { get; set; }

    public Guid WorkflowStepId { get; set; }
    public ClientWorkflowStep? Step { get; set; }
}
