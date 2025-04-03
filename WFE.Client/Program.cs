using Microsoft.EntityFrameworkCore;
using WFE.Client.Persistence;

namespace WFE.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder.Services);

        builder.Services.AddDbContext<ClientDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("FakeWFEPlan"))
        );

        var app = builder.Build();

        ConfigureApp(app);
        await app.RunAsync();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddHttpClient();

    }

    private static void ConfigureApp(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();
    }
}