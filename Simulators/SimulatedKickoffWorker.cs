using MassTransit;
using WFE.Engine.Contracts;
using WFE.Engine.Domain.Constants;
using WFE.Engine.DTOs;
using WFE.Engine.RoutingSlips;
using WFE.Engine.WorkflowRouting.Builders;

namespace WFE.Engine.Simulators
{
    public class SimulatedKickoffWorker(IServiceProvider serviceProvider, ILogger<SimulatedKickoffWorker> logger) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ILogger<SimulatedKickoffWorker> _logger = logger;

        private static readonly List<string> FirstNames = ["An", "Bình", "Châu", "Dũng", "Giang", "Hà", "Khánh", "Lan", "Minh", "Ngọc", "Phúc", "Quân", "Trang", "Vy"];
        private static readonly List<string> LastNames = ["Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Phan", "Vũ", "Đặng", "Bùi", "Đỗ", "Hồ", "Ngô", "Dương", "Đinh"];
        private static readonly List<string> Titles = ["HR Specialist", "Finance Manager", "IT Supervisor", "QA Lead", "DevOps Engineer", "Business Analyst"];

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🕔 SimulatedKickoffWorker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var builder = scope.ServiceProvider.GetRequiredService<IRoutingSlipBuilderService>();
                    var executor = scope.ServiceProvider.GetRequiredService<RoutingSlipExecutor>();
                    var bus = scope.ServiceProvider.GetRequiredService<IBus>();

                    var (username, fullName, email) = GenerateRandomUser();
                    /*
                    var ruleTree = new RuleNodeDto
                    {
                        LogicalOperator = "Or",
                        Children = new List<RuleNodeDto>
                        {
                            new()
                            {
                                StepName = "Manager Approval",
                                PredicateScript = "LeaveDays <= 3",
                                FilterMode = FilterModes.Forward,
                                RuleName = "Short Leave"
                            },
                            new()
                            {
                                LogicalOperator = "And",
                                StepName = "HR Review",
                                RuleName = "Medium Leave",
                                FilterMode = FilterModes.HardBlock,
                                Children = new List<RuleNodeDto>
                                {
                                    new() { PredicateScript = "LeaveDays > 3", StepName = "Manager Approval" },
                                    new() { PredicateScript = "LeaveDays <= 10", StepName = "Manager Approval" }
                                }
                            },
                            new()
                            {
                                LogicalOperator = "And",
                                RuleName = "High Leave - Finance Path",
                                StepName = "Finance Director",
                                FilterMode = FilterModes.HardBlock,
                                Children = new List<RuleNodeDto>
                                {
                                    new() { PredicateScript = "LeaveDays > 10", StepName = "Manager Approval" },
                                    new() { PredicateScript = "Department == \"Finance\"", StepName = "Finance Director" }
                                }
                            },
                            new()
                            {
                                LogicalOperator = "And",
                                RuleName = "High Leave - Non-Finance",
                                StepName = "CEO Approval",
                                FilterMode = FilterModes.HardBlock,
                                Children = new List<RuleNodeDto>
                                {
                                    new() { PredicateScript = "LeaveDays > 10", StepName = "Manager Approval" },
                                    new() { PredicateScript = "Department != \"Finance\"", StepName = "CEO Approval" }
                                }
                            }
                        }
                    };
                    */

var ruleTree = new RuleNodeDto
    {
        StepName = "Root Decision Node",
        LogicalOperator = "Or",
        Children = new List<RuleNodeDto>
        {
            new RuleNodeDto
            {
                StepName = "Manager Approval (Short Leave)",
                PredicateScript = "LeaveDays <= 3",
                FilterMode = FilterModes.Forward
            },
            new RuleNodeDto
            {
                StepName = "Manager Approval (Long Leave)",
                PredicateScript = "LeaveDays > 3 && LeaveDays <= 10",
                FilterMode = FilterModes.HardBlock
            },
            new RuleNodeDto
            {
                StepName = "Finance Approval Path",
                LogicalOperator = "And",
                Children = new List<RuleNodeDto>
                {
                    new RuleNodeDto
                    {
                        StepName = "Manager Approval (Finance Path)",
                        PredicateScript = "LeaveDays > 10 && Department == \"Finance\""
                    },
                    new RuleNodeDto
                    {
                        StepName = "Finance Director Approval",
                        PredicateScript = "TotalCost > 1000"
                    }
                }
            }
        }
    };


                    /*
                    var kickoffDto = new KickoffRequestDto
                    {
                        WorkflowId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // Replace with real WorkflowId from DB
                        CorrelationId = Guid.NewGuid(),
                        Actor = new()
                        {
                            Id = Guid.NewGuid(),
                            Username = username,
                            FullName = fullName,
                            Email = email,
                            EmployeeCode = "EMP001"
                        },
                        Reason = "Auto test kickoff",
                        RequestedAt = DateTime.UtcNow,
                        EncryptedConnectionString = "<encrypted-connection-string>",
                        DbType = "SqlServer",

                        Attributes =
                        [
                            new() { Key = "LeaveDays", Value = "3", ValueClrType = "System.Int32" },
                            new() { Key = "TotalCost", Value = "1500", ValueClrType = "System.Decimal" },
                            new() { Key = "Department", Value = "Finance", ValueClrType = "System.String" }
                        ],
                        RuleTree = ruleTree
                    };
                    */

                    /*
                    var kickoffDto = new KickoffRequestDto
                    {
                        WorkflowId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        CorrelationId = Guid.NewGuid(),
                        Actor = new Actor
                        {
                            Id = Guid.NewGuid(),
                            Username = "john.doe",
                            FullName = "John Doe",
                            Email = "john.doe@company.com",
                            EmployeeCode = "EMP007"
                        },
                        Reason = "🏝️ Annual Leave Request",
                        RequestedAt = DateTime.UtcNow,
                        EncryptedConnectionString = "<encrypted>",
                        DbType = "SqlServer",

                        Attributes = new List<RequestAttributeDto>
                        {
                            new() { Key = "LeaveDays", Value = "5", ValueClrType = "System.Int32" },
                            new() { Key = "Department", Value = "Finance", ValueClrType = "System.String" }
                        },

                        RuleTree = new RuleNodeDto
                        {
                            LogicalOperator = "Or",
                            Children = new List<RuleNodeDto>
                            {
                                new()
                                {
                                    StepName = "Manager Approval",
                                    PredicateScript = "LeaveDays <= 3",
                                    FilterMode = FilterModes.Forward,
                                    RuleName = "Short Leave"
                                },
                                new()
                                {
                                    LogicalOperator = "And",
                                    StepName = "HR Review",
                                    RuleName = "Medium Leave",
                                    FilterMode = FilterModes.HardBlock,
                                    Children = new List<RuleNodeDto>
                                    {
                                        new() { PredicateScript = "LeaveDays > 3", StepName = "Manager Approval" },
                                        new() { PredicateScript = "LeaveDays <= 10", StepName = "Manager Approval" }
                                    }
                                },
                                new()
                                {
                                    LogicalOperator = "And",
                                    StepName = "Finance Director",
                                    RuleName = "High Leave - Finance Path",
                                    FilterMode = FilterModes.HardBlock,
                                    Children = new List<RuleNodeDto>
                                    {
                                        new() { PredicateScript = "LeaveDays > 10", StepName = "Manager Approval" },
                                        new() { PredicateScript = "Department == \"Finance\"", StepName = "Finance Director" }
                                    }
                                },
                                new()
                                {
                                    LogicalOperator = "And",
                                    StepName = "CEO Approval",
                                    RuleName = "High Leave - Non-Finance",
                                    FilterMode = FilterModes.HardBlock,
                                    Children = new List<RuleNodeDto>
                                    {
                                        new() { PredicateScript = "LeaveDays > 10", StepName = "Manager Approval" },
                                        new() { PredicateScript = "Department != \"Finance\"", StepName = "CEO Approval" }
                                    }
                                }
                            }
                        }
                    };
                    */
                    
                    var kickoffDto = new KickoffRequestDto
                    {
                        WorkflowId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        CorrelationId = Guid.NewGuid(),
                        Actor = new Actor
                        {
                            Id = Guid.NewGuid(),
                            Username = "john.doe",
                            FullName = "John Doe",
                            Email = "john.doe@company.com",
                            EmployeeCode = "EMP007"
                        },
                        Reason = "🏝️ Annual Leave Request",
                        RequestedAt = DateTime.UtcNow,
                        EncryptedConnectionString = "c3lwW36ZX/Z0W2aYasKAzBPjFEEOCD3Sso8jD7rvfDJMexit4X+cX1IVbhHtsAl7YZizZuHDnniKucbPiWsZ/njgtga+SkTKJZ+9Jlh2oZDQm/4xFe7jeR7zIPMAE6vtxrdJYbJjIwWTKbfAcp+aroaT1noNdKhxyek1E+CiQ2WL2FYhaxSPL1q3TTDBX2kkrdNh4pxgvr78pIeNC6ZrUQ==",
                        DbType = "SqlServer",

                        Attributes = new List<RequestAttributeDto>
                        {
                            new() { Key = "LeaveDays", Value = "5", ValueClrType = "System.Int32" },
                            new() { Key = "Department", Value = "Finance", ValueClrType = "System.String" }
                        },

                        RuleTree = new RuleNodeDto
                        {
                            LogicalOperator = "Or",
                            StepName = "Decision Point",
                            Children = new List<RuleNodeDto>
                            {
                                new()
                                {
                                    StepName = "Manager Approval",
                                    PredicateScript = "LeaveDays <= 3",
                                    FilterMode = FilterModes.Forward,
                                    RuleName = "Short Leave"
                                },
                                new()
                                {
                                    LogicalOperator = "And",
                                    StepName = "HR Review", // This node now has StepName
                                    RuleName = "Medium Leave",
                                    FilterMode = FilterModes.HardBlock,
                                    Children = new List<RuleNodeDto>
                                    {
                                        new() { PredicateScript = "LeaveDays > 3", StepName = "Manager Approval 1" },
                                        new() { PredicateScript = "LeaveDays <= 10", StepName = "Manager Approval 2" }
                                    }
                                },
                                new()
                                {
                                    LogicalOperator = "And",
                                    StepName = "Finance Director", // This node now has StepName
                                    RuleName = "High Leave - Finance Path",
                                    FilterMode = FilterModes.HardBlock,
                                    Children = new List<RuleNodeDto>
                                    {
                                        new() { PredicateScript = "LeaveDays > 10", StepName = "Manager Approval OPQ" },
                                        new() { PredicateScript = "Department == \"Finance\"", StepName = "Finance Director" }
                                    }
                                },
                                new()
                                {
                                    LogicalOperator = "And",
                                    StepName = "CEO Approval", // This node now has StepName
                                    RuleName = "High Leave - Non-Finance",
                                    FilterMode = FilterModes.HardBlock,
                                    Children = new List<RuleNodeDto>
                                    {
                                        new() { PredicateScript = "LeaveDays > 10", StepName = "Manager Approval X" },
                                        new() { PredicateScript = "Department != \"Finance\"", StepName = "CEO Approval" }
                                    }
                                }
                            }
                        }
                    };
                    



                    var routingSlip = await builder.BuildAsync(kickoffDto);
                    await bus.Execute(routingSlip);

                    _logger.LogInformation("🕔 🚀 RoutingSlip built for {User}, with {Count} activities.", kickoffDto.Actor.Username, routingSlip.Itinerary.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during simulated kickoff.");
                }

                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }

        private static (string, string, string) GenerateRandomUser()
        {
            var rand = new Random();
            var first = FirstNames[rand.Next(FirstNames.Count)];
            var last = LastNames[rand.Next(LastNames.Count)];
            var fullName = $"{last} {first}";
            var username = $"{first.ToLower()}.{last.ToLower()}";
            var email = $"{username}@company.com";
            return (username, fullName, email);
        }
    }
}
