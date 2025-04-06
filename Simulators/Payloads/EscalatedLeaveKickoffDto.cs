using WFE.Engine.DTOs;
using WFE.Engine.Domain.Constants;

namespace WFE.Engine.Simulators.Payloads
{
    public static class EscalatedLeaveKickoffDto
    {
        public static KickoffRequestDto Create() => new()
        {
            WorkflowId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            RequestedAt = DateTime.UtcNow,
            Reason = "Leave escalation test",
            EncryptedConnectionString = "",
            DbType = "SqlServer",
            Actor = new()
            {
                Id = Guid.NewGuid(),
                Username = "jane.smith",
                FullName = "Jane Smith",
                Email = "jane.smith@example.com",
                EmployeeCode = "E456"
            },
            Attributes = new List<RequestAttributeDto>
            {
                new RequestAttributeDto
                {
                    Key = "LeaveDays",
                    Value = "10",
                    ValueClrType = "System.Int32"
                }
            },
            RuleTree = new RuleNodeDto
            {
                StepName = "Manager Approval",
                PredicateScript = "LeaveDays > 3 && LeaveDays <= 15",
                FilterMode = FilterModes.SoftWarn,
                Children = new List<RuleNodeDto>
                {
                    new RuleNodeDto
                    {
                        StepName = "HR Director Approval",
                        PredicateScript = "LeaveDays > 7",
                        FilterMode = FilterModes.HardBlock
                    }
                }
            }
        };
    }
}
