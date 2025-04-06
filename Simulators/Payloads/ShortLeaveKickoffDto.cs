using WFE.Engine.DTOs;
using WFE.Engine.Domain.Constants;

namespace WFE.Engine.Simulators.Payloads
{
    public static class ShortLeaveKickoffDto
    {
        public static KickoffRequestDto Create() => new()
        {
            WorkflowId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            RequestedAt = DateTime.UtcNow,
            Reason = "Quick leave request for under 3 days",
            EncryptedConnectionString = "<your-encrypted-connection>",
            DbType = "SqlServer",
            Actor = new()
            {
                Id = Guid.NewGuid(),
                Username = "john.doe",
                FullName = "John Doe",
                Email = "john.doe@example.com",
                EmployeeCode = "E123"
            },
            Attributes = new List<RequestAttributeDto>
            {
                new RequestAttributeDto
                {
                    Key = "LeaveDays",
                    Value = "2",
                    ValueClrType = "System.Int32"
                }
            },
            RuleTree = new RuleNodeDto
            {
                StepName = "Manager Approval",
                PredicateScript = "LeaveDays <= 3",
                FilterMode = FilterModes.Forward
            }
        };
    }
}
