using WFE.Engine.Domain.Workflow;
using WFE.Engine.Domain.Outbox;
using Microsoft.EntityFrameworkCore;

namespace WFE.Engine.Persistence
{
    public class SagaDbContext(DbContextOptions options) : DbContext(options)
    {
        // 🟩 Core Workflow Tables
        public DbSet<Workflow> Workflows => Set<Workflow>();
        public DbSet<WorkflowInstance> WorkflowInstances => Set<WorkflowInstance>();
        public DbSet<WorkflowStep> WorkflowSteps => Set<WorkflowStep>();
        public DbSet<WorkflowActor> WorkflowActors => Set<WorkflowActor>();
        public DbSet<StepProgress> StepProgresses => Set<StepProgress>();
        public DbSet<StepAssignment> StepAssignments => Set<StepAssignment>();

        // 🟦 Outbox (MassTransit built-in saga durability)
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
        public DbSet<OutboxState> OutboxStates => Set<OutboxState>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🟩 Workflow
            modelBuilder.Entity<Workflow>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).IsRequired();
                entity.Property(x => x.Description);
                entity.Property(x => x.IsActive).HasDefaultValue(true);
            });

            // 🟩 WorkflowInstance
            modelBuilder.Entity<WorkflowInstance>(entity =>
            {
                entity.HasKey(x => x.CorrelationId);
                entity.Property(x => x.WorkflowId).IsRequired();
                entity.Property(x => x.CurrentStep);
                entity.Property(x => x.IsApproved).HasDefaultValue(false);
                entity.Property(x => x.IsRejected);
                entity.Property(x => x.LastActionAt);
            });

            // 🟩 WorkflowStep
            modelBuilder.Entity<WorkflowStep>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.Workflow)
                      .WithMany(x => x.Steps)
                      .HasForeignKey(x => x.WorkflowId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(x => x.StepOrder);
                entity.Property(x => x.StepName).IsRequired();
                entity.Property(x => x.ApprovalType).IsRequired();
                entity.Property(x => x.ConditionScript);
            });

            // 🟦 WorkflowActor
            modelBuilder.Entity<WorkflowActor>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.Step)
                      .WithMany(x => x.Actors)
                      .HasForeignKey(x => x.WorkflowStepId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(x => x.Username).IsRequired();
                entity.Property(x => x.FullName).IsRequired();
                entity.Property(x => x.Email).IsRequired();
                entity.Property(x => x.EmployeeCode);
                entity.Property(x => x.IsMandatory).HasDefaultValue(true);
                entity.Property(x => x.Order);
            });

            // 🟨 StepProgress
            modelBuilder.Entity<StepProgress>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CorrelationId).IsRequired();
                entity.Property(e => e.WorkflowStepId).IsRequired();
                entity.HasOne(e => e.WorkflowStep)
                      .WithMany()
                      .HasForeignKey(e => e.WorkflowStepId);
            });

            // 🟨 StepAssignment
            modelBuilder.Entity<StepAssignment>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.CorrelationId).IsRequired();
                entity.Property(x => x.WorkflowStepId).IsRequired();

                entity.Property(x => x.UserId).IsRequired();
                entity.Property(x => x.Username).IsRequired();
                entity.Property(x => x.FullName).IsRequired();
                entity.Property(x => x.Email).IsRequired();
                entity.Property(x => x.EmployeeCode);

                entity.Property(x => x.IsCurrent).HasDefaultValue(true);
                entity.Property(x => x.AssignedAt).IsRequired();

                entity.HasOne(x => x.Step)
                    .WithMany()
                    .HasForeignKey(x => x.WorkflowStepId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 🟥 MassTransit Outbox Tables
            modelBuilder.Entity<OutboxMessage>(entity =>
            {
                entity.ToTable("OutboxMessages");

                entity.HasKey(e => e.MessageId);

                entity.Property(e => e.Destination).IsRequired();
                entity.Property(e => e.Payload).IsRequired();
                entity.Property(e => e.MessageType).IsRequired();
                entity.Property(e => e.Created).IsRequired();
                entity.Property(e => e.Processed).HasDefaultValue(false);
            });

            modelBuilder.Entity<OutboxState>(entity =>
            {
                entity.ToTable("OutboxStates");

                entity.HasKey(e => e.StateId);

                entity.Property(e => e.MessageId).IsRequired();
                entity.Property(e => e.Created).IsRequired();
                entity.Property(e => e.Delivered);
                entity.Property(e => e.Status).HasDefaultValue("Pending");
                entity.Property(e => e.RetryCount).HasDefaultValue(0);

                // Optional: FK and index
                entity.HasOne(e => e.Message)
                      .WithOne(m => m.State)
                      .HasForeignKey<OutboxState>(e => e.MessageId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.MessageId).IsUnique();
            });

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<WorkflowInstance>())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    if (entry.Entity.WorkflowId == Guid.Empty)
                        throw new InvalidOperationException("🚨 WorkflowInstance.WorkflowId cannot be Guid.Empty");
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}