using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace QuantityMeasurementRepository.Context
{
    /// <summary>
    /// EF Core design-time factory for migrations (dotnet ef migrations add).
    /// UPDATED: Uses Npgsql (PostgreSQL) instead of SQL Server.
    /// Set DATABASE_URL env var locally or configure appsettings.Development.json.
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var baseDir = Directory.GetCurrentDirectory();
            string? configPath = null;
            foreach (var rel in new[]
                     {
                         Path.Combine("..", "QuantityMeasurementWebAPI", "Config"),
                         Path.Combine("..", "..", "QuantityMeasurementWebAPI", "Config"),
                         Path.Combine("..", "..", "..", "QuantityMeasurementWebAPI", "Config"),
                         Path.Combine("..", "..", "..", "..", "QuantityMeasurementWebAPI", "Config"),
                     })
            {
                var candidate = Path.GetFullPath(Path.Combine(baseDir, rel));
                if (Directory.Exists(candidate))
                {
                    configPath = candidate;
                    break;
                }
            }

            if (configPath is null)
                throw new DirectoryNotFoundException(
                    "Could not find QuantityMeasurementWebAPI/Config. Run dotnet ef from the solution folder.");

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(configPath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            // Support both DATABASE_URL env var and appsettings connection string
            var conn = Environment.GetEnvironmentVariable("DATABASE_URL")
                       ?? config.GetConnectionString("QuantityMeasurementDb");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(conn)
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}