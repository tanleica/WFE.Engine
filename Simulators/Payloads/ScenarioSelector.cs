using WFE.Engine.DTOs;

namespace WFE.Engine.Simulators.Payloads
{
    public static class ScenarioSelector
    {
        public enum ScenarioType
        {
            ShortLeave,
            EscalatedLeave,
            FinanceApproval
        }

        public static KickoffRequestDto Select(ScenarioType type)
        {
            return type switch
            {
                ScenarioType.ShortLeave      => ShortLeaveKickoffDto.Create(),
                ScenarioType.EscalatedLeave  => EscalatedLeaveKickoffDto.Create(),
                ScenarioType.FinanceApproval => FinanceApprovalKickoffDto.Create(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), "Invalid scenario type")
            };
        }
    }
}
