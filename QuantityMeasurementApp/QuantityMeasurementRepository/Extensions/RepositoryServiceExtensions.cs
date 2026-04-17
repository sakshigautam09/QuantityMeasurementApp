using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuantityMeasurementRepository;
using QuantityMeasurementRepository.Repositories;
using QuantityMeasurementRepository.Context;
using QuantityMeasurementRepository.Interface;
using QuantityMeasurementRepository.Persistence;

namespace QuantityMeasurementRepository.Extensions
{
    /// <summary>
    /// Dependency injection extensions for Repository layer (DbContext, Redis, Repositories).
    /// UPDATED: SQL Server → PostgreSQL (Npgsql) for Render deployment.
    /// </summary>
    public static class RepositoryServiceExtensions
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Render provides DATABASE_URL env var automatically for PostgreSQL services.
            // We check that first, then fall back to appsettings ConnectionString.
            var conn = Environment.GetEnvironmentVariable("DATABASE_URL")
                       ?? configuration.GetConnectionString("QuantityMeasurementDb");

            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException(
                    "No database connection found. Set DATABASE_URL env var (Render) " +
                    "or ConnectionStrings:QuantityMeasurementDb in appsettings.json.");

            // Render's DATABASE_URL uses the postgres:// URI format.
            // Npgsql accepts both URI and traditional connection string formats.
            services.AddDbContext<ApplicationDbContext>(opts =>
                opts.UseNpgsql(
                    conn,
                    npgsql =>
                    {
                        npgsql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(4), null);
                        npgsql.CommandTimeout(60);
                    }));

            string redisConn = Environment.GetEnvironmentVariable("REDIS_URL")
                               ?? configuration.GetConnectionString("Redis")
                               ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(redisConn) && TryConnectRedis(redisConn))
            {
                services.AddStackExchangeRedisCache(opts =>
                {
                    opts.Configuration = redisConn;
                    opts.InstanceName = "QM:";
                });
                Console.WriteLine($"[Startup] Redis connected: {redisConn}");
            }
            else
            {
                services.AddMemoryCache();
                services.AddSingleton<IDistributedCache, MemoryDistributedCache>();
                Console.WriteLine("[Startup] Redis unavailable — using in-memory cache fallback.");
            }

            services.AddScoped<Interface.IQuantityMeasurementRepository, Persistence.QuantityMeasurementRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<IMeasurementHistoryRepository>(_ =>
                QuantityMeasurementCacheRepository.Instance);

            return services;
        }

        private static bool TryConnectRedis(string connectionString)
        {
            try
            {
                using var redis = StackExchange.Redis.ConnectionMultiplexer.Connect(
                    new StackExchange.Redis.ConfigurationOptions
                    {
                        EndPoints = { connectionString },
                        ConnectTimeout = 2000,
                        AbortOnConnectFail = false
                    });
                return redis.IsConnected;
            }
            catch { return false; }
        }
    }
}