using WFE.Engine.Contracts;
using WFE.Engine.Events;

namespace WFE.Engine.DTOs
{
    public static class EventCloner
    {
        public static TTarget CloneFrom<TTarget>(this IWorkflowEvent source, TTarget template)
            where TTarget : WorkflowEventBase
        {
            return template with
            {
                CorrelationId = source.CorrelationId,
                OccurredAt = source.OccurredAt,
                CanGoFurther = source.CanGoFurther,
                Reason = source.Reason
            };
        }

        // CloneFrom with Actor
        public static TTarget CloneFrom<TTarget>(
            this IWorkflowEvent source,
            TTarget template,
            Actor actor
        ) where TTarget : WorkflowEventBase
        {
            return template with
            {
                CorrelationId = source.CorrelationId,
                OccurredAt = source.OccurredAt,
                CanGoFurther = source.CanGoFurther,
                Reason = source.Reason,
                Actor = actor
            };
        }


        // Overload for Step-aware + Actor
        public static TTarget CloneFrom<TTarget>(
            this IWorkflowEvent source,
            string stepName,
            Actor actor,
            DateTime? occurredAt = null,
            bool canGoFurther = true,
            string? reason = null)
            where TTarget : WorkflowEventBase, new()
        {
            return new TTarget
            {
                CorrelationId = source.CorrelationId,
                OccurredAt = occurredAt ?? DateTime.UtcNow,
                StepName = stepName,
                Actor = actor,
                CanGoFurther = canGoFurther,
                Reason = reason
            };
        }

        public static TTarget CloneFrom<TTarget>(
            this IWorkflowEvent source,
            string stepName,
            Actor actor,
            DateTime occurredAt,
            bool canGoFurther = true,
            string? reason = null)
            where TTarget : WorkflowEventBase, IWorkflowEvent, IStepAware, IActorCarrier, new()
        {
            return new TTarget
            {
                CorrelationId = source.CorrelationId,
                OccurredAt = occurredAt,
                StepName = stepName,
                Actor = actor,
                CanGoFurther = canGoFurther,
                Reason = reason
            };
        }

    }
}