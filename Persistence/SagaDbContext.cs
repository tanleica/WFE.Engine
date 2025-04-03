using WFE.Engine.Domain.Workflow;
using Microsoft.EntityFrameworkCore;

namespace WFE.Engine.Persistence
{
    public class SagaDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Workflow> Workflows { get; set; } = default!;
        public DbSet<WorkflowInstance> WorkflowInstances { get; set; } = default!;
        public DbSet<WorkflowStep> WorkflowSteps { get; set; } = default!;
        public DbSet<WorkflowActor> WorkflowActors { get; set; } = default!;
        public DbSet<StepProgress> StepProgresses { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔹 Workflow
            modelBuilder.Entity<Workflow>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).IsRequired();
                entity.Property(x => x.Description);
                entity.Property(x => x.IsActive).HasDefaultValue(true);
            });

            // 🔹 WorkflowInstance
            modelBuilder.Entity<WorkflowInstance>()
                .HasKey(w => w.CorrelationId);

            // 🔹 WorkflowStep
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
                entity.Property(x => x.Condition);
            });

            // 🔹 WorkflowActor
            modelBuilder.Entity<WorkflowActor>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.Step)
                      .WithMany(x => x.Actors)
                      .HasForeignKey(x => x.WorkflowStepId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(x => x.ActorName).IsRequired();
                entity.Property(x => x.Email);
                entity.Property(x => x.IsMandatory).HasDefaultValue(true);
                entity.Property(x => x.Order);
            });

            // 🔹 StepProgress
            modelBuilder.Entity<StepProgress>(entity =>
                {
                    entity.HasKey(e => e.Id);

                    entity.Property(e => e.CorrelationId).IsRequired();
                    entity.Property(e => e.WorkflowStepId).IsRequired();

                    entity.HasOne(e => e.WorkflowStep)
                        .WithMany()
                        .HasForeignKey(e => e.WorkflowStepId);
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