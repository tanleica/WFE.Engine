using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WFE.Engine.Persistence
{
    public class SagaDbContextFactory : IDesignTimeDbContextFactory<SagaDbContext>
    {
        public SagaDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Make sure you run from root
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SagaDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("Postgres"));

            return new SagaDbContext(optionsBuilder.Options);
        }
    }
}