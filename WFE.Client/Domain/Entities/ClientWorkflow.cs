using System.ComponentModel.DataAnnotations;

namespace WFE.Client.Domain.Entities;

public class ClientWorkflow
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public List<ClientWorkflowStep> Steps { get; set; } = new();
}
