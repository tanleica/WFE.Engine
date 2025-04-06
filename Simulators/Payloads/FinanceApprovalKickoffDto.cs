using WFE.Engine.DTOs;
using WFE.Engine.Domain.Constants;

namespace WFE.Engine.Simulators.Payloads
{
    public static class FinanceApprovalKickoffDto
    {
        public static KickoffRequestDto Create() => new()
        {
            WorkflowId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            RequestedAt = DateTime.UtcNow,
            Reason = "Finance approval for large expense",
            EncryptedConnectionString = "<your-encrypted-connection>",
            DbType = "SqlServer",
            Actor = new()
            {
                Id = Guid.NewGuid(),
                Username = "alice.king",
                FullName = "Alice King",
                Email = "alice.king@example.com",
                EmployeeCode = "E789"
            },
            Attributes = new List<RequestAttributeDto>
            {
                new RequestAttributeDto
                {
                    Key = "TotalCost",
                    Value = "15000",
                    ValueClrType = "System.Decimal"
                }
            },
            RuleTree = new RuleNodeDto
            {
                LogicalOperator = "And",
                StepName = "Finance Director Approval",
                Children = new List<RuleNodeDto>
                {
                    new RuleNodeDto
                    {
                        StepName = "Manager Approval",
                        PredicateScript = "TotalCost <= 5000",
                        FilterMode = FilterModes.SoftWarn
                    },
                    new RuleNodeDto
                    {
                        StepName = "Finance Director Approval",
                        PredicateScript = "TotalCost > 10000",
                        FilterMode = FilterModes.HardBlock
                    }
                }
            }
        };
    }
}
