using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WFE.Engine.DTOs;
using WFE.Engine.RoutingSlips;
using WFE.Engine.WorkflowRouting.Builders;

namespace WFE.Engine.Simulators;

public class SimulatedKickoffWorker(IServiceProvider serviceProvider, ILogger<SimulatedKickoffWorker> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<SimulatedKickoffWorker> _logger = logger;

    private static readonly List<string> FirstNames = ["An", "Bình", "Châu", "Dũng", "Giang", "Hà", "Khánh", "Lan", "Minh", "Ngọc", "Phúc", "Quân", "Trang", "Vy"];
    private static readonly List<string> LastNames = ["Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Phan", "Vũ", "Đặng", "Bùi", "Đỗ", "Hồ", "Ngô", "Dương", "Đinh"];
    private static readonly List<string> Titles = ["HR Specialist", "Finance Manager", "IT Supervisor", "QA Lead", "DevOps Engineer", "Business Analyst"];
    private static readonly string JsonFolder = "SampleKickoffs"; // you can change this if needed

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🕔 SimulatedKickoffWorker started.");

        var rnd = new Random();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var builder = scope.ServiceProvider.GetRequiredService<IRoutingSlipBuilderService>();
                var executor = scope.ServiceProvider.GetRequiredService<RoutingSlipExecutor>();

                /*
                var jsonFiles = Directory.GetFiles(JsonFolder, "*.json");
                if (jsonFiles.Length == 0)
                {
                    _logger.LogWarning("⚠️ No kickoff JSON files found in folder: {JsonFolder}", JsonFolder);
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                    continue;
                }
                */

                var jsonFiles = new[] { Path.Combine(JsonFolder, "multi-branch-demo.json") };
                var selectedFile = jsonFiles[0]; // hardcoded for now


                //var selectedFile = jsonFiles[rnd.Next(jsonFiles.Length)];
                var json = await File.ReadAllTextAsync(selectedFile, stoppingToken);

                var kickoffDto = JsonSerializer.Deserialize<KickoffRequestDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (kickoffDto is null)
                {
                    _logger.LogWarning("❌ Failed to deserialize kickoff DTO from file: {File}", selectedFile);
                    continue;
                }

                // Randomize Actor
                var fullName = $"{LastNames[rnd.Next(LastNames.Count)]} {FirstNames[rnd.Next(FirstNames.Count)]}";
                kickoffDto.Actor.FullName = fullName;
                kickoffDto.Actor.Username = fullName.ToLower().Replace(" ", ".");
                kickoffDto.Actor.Email = $"{kickoffDto.Actor.Username}@histaff.vn";

                //var slip = await builder.BuildAsync(kickoffDto);
                await executor.StartAsync(kickoffDto, stoppingToken);

                _logger.LogInformation("🚀 RoutingSlip executed for {Username} — DTO loaded from {File}", kickoffDto.Actor.Username, Path.GetFileName(selectedFile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred during simulated kickoff.");
            }

            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken); // wait longer
        }
    }
}
