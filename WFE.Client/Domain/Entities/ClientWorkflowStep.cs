using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WFE.Client.Domain.Entities;

public class ClientWorkflowStep
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string StepName { get; set; } = string.Empty;

    public int StepOrder { get; set; }

    public string ApprovalType { get; set; } = "Parallel"; // Can be changed at runtime

    public string? Condition { get; set; }

    public string FilterMode { get; set; } = "SoftWarn"; // "SoftWarn" or "HardBlock"

    // Foreign Key
    public Guid? WorkflowId { get; set; }

    [ForeignKey(nameof(WorkflowId))]

    [JsonIgnore]
    public ClientWorkflow? Workflow { get; set; }

    // Navigation
    public List<ClientWorkflowActor> Actors { get; set; } = new();
}
