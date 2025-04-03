using MassTransit;
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
                    var executor = scope.ServiceProvider.GetRequiredService<RoutingSlipExecutor>();

                    var (username, fullName, email) = GenerateRandomUser();

                    var kickoffDto = new KickoffRequestDto
                    {
                        WorkflowId = Guid.NewGuid(),
                        RequestedByUsername = username,
                        RequestedByFullName = fullName,
                        RequestedByEmail = email,
                        RequestedByEmployeeCode = "EMP001",
                        Reason = "Auto test kickoff",
                        RequestedAt = DateTime.UtcNow,
                        EncryptedConnectionString = "<encrypted-connection-string>",
                        DbType = "SqlServer",

                        Steps =
                        [
                            new()
                            {
                                WorkflowStepId = Guid.NewGuid(), // ✅ Sanbox fake ID
                                StepName = "Manager Approval",
                                ApprovalType = "Sequential",
                                StepOrder = 1,
                                Actors = [GenerateRandomActor(), GenerateRandomActor()],
                                RuleTree = new RuleNodeDto
                                {
                                    ConditionScript = "LeaveDays <= 5",
                                    LogicalOperator = "AND",
                                    FilterMode = "SoftWarn"
                                }
                            },
                            new()
                            {
                                WorkflowStepId = Guid.NewGuid(), // ✅ Sanbox fake ID
                                StepName = "HR Approval",
                                ApprovalType = "Sequential",
                                StepOrder = 2,
                                Actors = [GenerateRandomActor(), GenerateRandomActor()],
                                RuleTree = new RuleNodeDto
                                {
                                    ConditionScript = "TotalCost > 1000",
                                    LogicalOperator = "AND",
                                    FilterMode = "HardBlock"
                                }
                            }
                        ],

                        Attributes =
                        [
                            new()
                            {
                                Key = "LeaveDays",
                                Value = "3",
                                ValueClrType = "System.Int32"
                            },
                            new()
                            {
                                Key = "TotalCost",
                                Value = "1500",
                                ValueClrType = "System.Decimal"
                            }
                        ]


                    };

                    var builder = scope.ServiceProvider.GetRequiredService<IRoutingSlipBuilderService>();
                    var routingSlip = await builder.BuildAsync(kickoffDto);

                    var bus = scope.ServiceProvider.GetRequiredService<IBus>();
                    await bus.Execute(routingSlip);

                    _logger.LogInformation("🕔 🚀 SimulatedKickoffWorker ExecuteAsync: RoutingSlip built for request by {User}. Activities: {Count}", kickoffDto.RequestedByUsername, routingSlip.Itinerary.Count);


                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during simulated kickoff.");
                }

                //await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }

        private static PlannedActorDto GenerateRandomActor()
        {
            var rand = new Random();

            var first = FirstNames[rand.Next(FirstNames.Count)];
            var last = LastNames[rand.Next(LastNames.Count)];
            var fullName = $"{last} {first}";
            var username = $"{first.ToLower()}.{last.ToLower()}";
            var email = $"{username}@company.com";
            var empCode = $"EMP{rand.Next(1000, 9999)}";
            var title = Titles[rand.Next(Titles.Count)];

            return new PlannedActorDto
            {
                Username = username,
                FullName = $"{fullName} ({title})",
                Email = email,
                EmployeeCode = empCode
            };
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
