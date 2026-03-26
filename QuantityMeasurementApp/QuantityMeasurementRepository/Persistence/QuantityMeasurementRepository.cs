using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementRepository.Context;
using QuantityMeasurementRepository.Interface;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuantityMeasurementRepository.Persistence
{
    /// <summary>
    /// EF Core repository for quantity_measurements.
    /// Writes → SQL Server (SSMS) via EF Core.
    /// Reads → Redis cache (5-min TTL) first, falls back to SQL Server on cache miss.
    /// </summary>
    public class QuantityMeasurementRepository : IQuantityMeasurementRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IDistributedCache _cache;
        private readonly ILogger<QuantityMeasurementRepository> _logger;

        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public QuantityMeasurementRepository(
            ApplicationDbContext db,
            IDistributedCache cache,
            ILogger<QuantityMeasurementRepository> logger)
        {
            _db = db;
            _cache = cache;
            _logger = logger;
        }

        public async Task SaveAsync(QuantityMeasurementEFEntity entity)
        {
            _db.Measurements.Add(entity);
            await _db.SaveChangesAsync();
            _logger.LogInformation("[QuantityMeasurementRepository] Saved: Op={Op} Cat={Cat} Error={E}",
                entity.Operation, entity.Operand1Category, entity.HasError);
            await InvalidateCacheAsync(entity.Operation, entity.Operand1Category);
        }

        public async Task<IReadOnlyList<QuantityMeasurementEFEntity>> GetAllAsync()
        {
            const string cacheKey = "qm:all";
            var cached = await TryGetFromCacheAsync<List<QuantityMeasurementEFEntity>>(cacheKey);
            if (cached is not null) { _logger.LogDebug("[QuantityMeasurementRepository] Cache HIT: {Key}", cacheKey); return cached; }

            _logger.LogDebug("[QuantityMeasurementRepository] Cache MISS: {Key} → querying SQL Server", cacheKey);
            var list = await _db.Measurements.OrderByDescending(m => m.Timestamp).ToListAsync();
            await SetCacheAsync(cacheKey, list);
            return list;
        }

        public async Task<IReadOnlyList<QuantityMeasurementEFEntity>> GetByOperationAsync(string operation)
        {
            string cacheKey = $"qm:op:{operation.ToUpperInvariant()}";
            var cached = await TryGetFromCacheAsync<List<QuantityMeasurementEFEntity>>(cacheKey);
            if (cached is not null) return cached;

            var list = await _db.Measurements
                .Where(m => m.Operation == operation.ToUpperInvariant())
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();
            await SetCacheAsync(cacheKey, list);
            return list;
        }

        public async Task<IReadOnlyList<QuantityMeasurementEFEntity>> GetByCategoryAsync(string category)
        {
            string cacheKey = $"qm:cat:{category.ToUpperInvariant()}";
            var cached = await TryGetFromCacheAsync<List<QuantityMeasurementEFEntity>>(cacheKey);
            if (cached is not null) return cached;

            var list = await _db.Measurements
                .Where(m => m.Operand1Category == category.ToUpperInvariant() ||
                            m.Operand2Category == category.ToUpperInvariant())
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();
            await SetCacheAsync(cacheKey, list);
            return list;
        }

        public async Task<IReadOnlyList<QuantityMeasurementEFEntity>> GetErroredAsync()
            => await _db.Measurements.Where(m => m.HasError).OrderByDescending(m => m.Timestamp).ToListAsync();

        public async Task<int> GetCountByOperationAsync(string operation)
            => await _db.Measurements.CountAsync(m => m.Operation == operation.ToUpperInvariant() && !m.HasError);

        public async Task<int> GetTotalCountAsync()
            => await _db.Measurements.CountAsync();

        public async Task ClearAllAsync()
        {
            await _db.Measurements.ExecuteDeleteAsync();
            _logger.LogWarning("[QuantityMeasurementRepository] All measurements deleted from SQL Server.");
            try { await _cache.RemoveAsync("qm:all"); }
            catch (Exception ex) { _logger.LogWarning(ex, "[QuantityMeasurementRepository] Could not clear Redis cache key qm:all."); }
        }

        private async Task<T?> TryGetFromCacheAsync<T>(string key) where T : class
        {
            try
            {
                byte[]? bytes = await _cache.GetAsync(key);
                if (bytes is null) return null;
                return JsonSerializer.Deserialize<T>(bytes, SerializerOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[QuantityMeasurementRepository] Redis GET failed for key={Key}. Falling back to SQL Server.", key);
                return null;
            }
        }

        private async Task SetCacheAsync<T>(string key, T value)
        {
            try
            {
                var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheTtl };
                await _cache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(value, SerializerOptions), options);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[QuantityMeasurementRepository] Redis SET failed for key={Key}. Continuing without cache.", key);
            }
        }
        private async Task InvalidateCacheAsync(string operation, string? category)
        {
            try
            {
                await _cache.RemoveAsync("qm:all");
                await _cache.RemoveAsync($"qm:op:{operation.ToUpperInvariant()}");
                if (!string.IsNullOrWhiteSpace(category))
                    await _cache.RemoveAsync($"qm:cat:{category.ToUpperInvariant()}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[QuantityMeasurementRepository] Cache invalidation failed. Data may be briefly stale.");
            }
        }
    }
}
