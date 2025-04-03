using Microsoft.EntityFrameworkCore;
using WFE.Client.Domain.Entities;

namespace WFE.Client.Persistence;

public class ClientDbContext(DbContextOptions<ClientDbContext> options) : DbContext(options)
{
    public DbSet<LeaveRequest> LeaveRequests { get; set; } = default!;
    public DbSet<ClientWorkflow> ClientWorkflows { get; set; } = default!;
    public DbSet<ClientWorkflowStep> WorkflowSteps { get; set; } = default!;
    public DbSet<ClientWorkflowActor> WorkflowActors { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Workflow
        modelBuilder.Entity<ClientWorkflow>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.HasMany(e => e.Steps)
                  .WithOne(s => s.Workflow)
                  .HasForeignKey(s => s.WorkflowId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Workflow Step
        modelBuilder.Entity<ClientWorkflowStep>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.StepName).IsRequired();
            entity.Property(e => e.StepOrder).IsRequired();
            entity.Property(e => e.ApprovalType).IsRequired();
            entity.Property(e => e.Condition);
            entity.Property(e => e.FilterMode).HasDefaultValue("SoftWarn");

            // ✅ Optional relationship to Workflow
            entity.HasOne(e => e.Workflow)
                  .WithMany(w => w.Steps)
                  .HasForeignKey(e => e.WorkflowId)
                  .IsRequired(false); // 👈 Optional, skip validation during POST
        });

        // Workflow Actor
        modelBuilder.Entity<ClientWorkflowActor>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ActorUsername).IsRequired();
            entity.Property(e => e.ActorFullName).IsRequired();
            entity.Property(e => e.ActorEmail).IsRequired();
            entity.Property(e => e.ActorEmployeeCode);
            entity.Property(e => e.Order).HasDefaultValue(1);

            // ✅ Optional relationship to WorkflowStep
            entity.HasOne(e => e.Step)
                  .WithMany(s => s.Actors)
                  .HasForeignKey(e => e.WorkflowStepId)
                  .IsRequired(false); // 👈 Make optional to avoid model binding errors
        });

    }
}