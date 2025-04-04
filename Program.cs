using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources; // We will need it later
using OpenTelemetry.Trace; // We will need it later
using WFE.Engine.Persistence;
using WFE.Engine.WorkflowRouting.Builders;
using WFE.Engine.WorkflowRouting.Activities;
using WFE.Engine.WorkflowTemplates;
using WFE.Engine.RoutingSlips;
using WFE.Engine.Simulators;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddScoped<IRoutingSlipBuilderService, RoutingSlipBuilderService>();
builder.Services.AddScoped<IWorkflowTemplateBuilderService, WorkflowTemplateBuilderService>();

// Register EF Core
builder.Services.AddDbContext<SagaDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("Postgres"), npg =>
        npg.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
    ).EnableSensitiveDataLogging());
    

builder.Services.AddScoped<VoteTallyingService>();
builder.Services.AddScoped<StepVoteEvaluator>();
builder.Services.AddScoped<RoutingSlipExecutor>();

var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
Console.WriteLine($"Detected Service Name: {assemblyName}");

// Register MassTransit (simplified here)
builder.Services.AddMassTransit(x =>
{

    // Register Consumers
    var entryAssembly = typeof(Program).Assembly;
    x.AddConsumers(entryAssembly);
    x.AddSagaStateMachines(entryAssembly);
    x.AddSagas(entryAssembly);
    x.AddActivities(entryAssembly);

    x.SetKebabCaseEndpointNameFormatter();
    x.SetInMemorySagaRepositoryProvider();
    //x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("159.223.59.17", "/", h =>
        {
            h.Username("admin"); // or your RabbitMQ user
            h.Password("A123231312a@");
        });

        cfg.ConfigureEndpoints(context); // 👈 auto-binds consumers/sagas
        
    });
});

builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        /*
         tracerProviderBuilder
             .SetResourceBuilder(ResourceBuilder.CreateDefault()
             .AddService(assemblyName ?? "WFE.Engine")) // Service name for trace grouping
             .AddAspNetCoreInstrumentation()
             .AddHttpClientInstrumentation()
             .AddConsoleExporter(options =>
             {
                 options.Targets = OpenTelemetry.Exporter.ConsoleExporterOutputTargets.Console;
             });
        */
    });

builder.Services.AddHostedService<SimulatedKickoffWorker>();
builder.Services.AddHttpClient();

// Register controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Required for minimal APIs too
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WFE.Engine API",
        Version = "v1"
    });
});

// Register CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://159.223.59.17:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors(); // 👈 Enable CORS

// Enable Swagger and Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FamilySagaWorkflow API v1");
    c.RoutePrefix = string.Empty; // 👈 Serve at root (/)
});

// Configure HTTP pipeline
app.MapControllers();

await app.RunAsync();
