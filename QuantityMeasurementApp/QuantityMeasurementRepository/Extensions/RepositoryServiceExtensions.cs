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
    /// </summary>
    public static class RepositoryServiceExtensions
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var conn = configuration.GetConnectionString("QuantityMeasurementDb");
            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException(
                    "ConnectionStrings:QuantityMeasurementDb is missing. Set it in QuantityMeasurementWebAPI/Config/appsettings.json (or User Secrets).");

            services.AddDbContext<ApplicationDbContext>(opts =>
                opts.UseSqlServer(
                    conn,
                    sql =>
                    {
                        sql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(4), null);
                        sql.CommandTimeout(60);
                    }));

            string redisConn = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            if (TryConnectRedis(redisConn))
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
