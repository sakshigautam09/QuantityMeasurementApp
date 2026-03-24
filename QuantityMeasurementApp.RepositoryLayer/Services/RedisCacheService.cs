// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : Services/RedisCacheService.cs
// UC-17   : Redis as primary read layer.
//           All history reads come from Redis first.
//           SQL Server is permanent backup only.
// ============================================================

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using QuantityMeasurementApp.ModelLayer;
using QuantityMeasurementApp.RepositoryLayer.Interface;
using StackExchange.Redis;

namespace QuantityMeasurementApp.RepositoryLayer.Services
{
    public class RedisCacheService : IRedisCache
    {
        private readonly IConnectionMultiplexer     _redis;
        private readonly IDatabase                  _db;
        private readonly ILogger<RedisCacheService> _logger;

        // All history stored under this list key in Redis
        public const string AllHistoryKey = "qm:history:all";

        private static readonly JsonSerializerOptions _json = new()
        {
            WriteIndented = false,
            Converters    = { new JsonStringEnumConverter() }
        };

        public RedisCacheService(
            IConnectionMultiplexer     redis,
            ILogger<RedisCacheService> logger)
        {
            _redis  = redis  ?? throw new ArgumentNullException(nameof(redis));
            _db     = _redis.GetDatabase();
            _logger = logger;
            _logger.LogInformation("RedisCacheService initialized.");
        }

        // ── Push one entity to Redis list ─────────────────────────────────────────

        public async Task PushToListAsync(string listKey, QuantityMeasurementEntity entity)
        {
            _logger.LogInformation("Redis PUSH to list: {Key}", listKey);
            string json = JsonSerializer.Serialize(CacheRow.From(entity), _json);
            // LPUSH — newest first
            await _db.ListLeftPushAsync(listKey, json);
            _logger.LogInformation("Redis PUSH success.");
        }

        // ── Get full list from Redis ──────────────────────────────────────────────

        public async Task<IReadOnlyList<QuantityMeasurementEntity>> GetListAsync(string listKey)
        {
            _logger.LogInformation("Redis GET list: {Key}", listKey);
            var values = await _db.ListRangeAsync(listKey, 0, -1);
            var result = new List<QuantityMeasurementEntity>();

            foreach (var val in values)
            {
                if (val.IsNullOrEmpty) continue;
                try
                {
                    var row = JsonSerializer.Deserialize<CacheRow>(val.ToString(), _json);
                    if (row is not null) result.Add(row.ToEntity());
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize Redis entry.");
                }
            }

            _logger.LogInformation("Redis GET list returned {Count} items.", result.Count);
            return result.AsReadOnly();
        }

        // ── Single entity set/get ─────────────────────────────────────────────────

        public async Task SetAsync(
            string key, QuantityMeasurementEntity entity, TimeSpan? expiry = null)
        {
            _logger.LogInformation("Redis SET: {Key}", key);
            string json = JsonSerializer.Serialize(CacheRow.From(entity), _json);
            await _db.StringSetAsync(key, json, expiry);
        }

        public async Task<QuantityMeasurementEntity?> GetAsync(string key)
        {
            _logger.LogInformation("Redis GET: {Key}", key);
            var json = await _db.StringGetAsync(key);
            if (json.IsNullOrEmpty)
            {
                _logger.LogWarning("Redis MISS: {Key}", key);
                return null;
            }
            try
            {
                var row = JsonSerializer.Deserialize<CacheRow>(json.ToString(), _json);
                _logger.LogInformation("Redis HIT: {Key}", key);
                return row?.ToEntity();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis GET deserialize failed: {Key}", key);
                return null;
            }
        }

        public async Task DeleteAsync(string key)
        {
            _logger.LogInformation("Redis DELETE: {Key}", key);
            await _db.KeyDeleteAsync(key);
        }

        public async Task<bool> ExistsAsync(string key)
            => await _db.KeyExistsAsync(key);

        // ── String operations ─────────────────────────────────────────────────────

        public async Task SetStringAsync(string key, string value, TimeSpan? expiry = null)
        {
            _logger.LogInformation("Redis SET string: {Key}", key);
            await _db.StringSetAsync(key, value, expiry);
        }

        public async Task<string?> GetStringAsync(string key)
        {
            var val = await _db.StringGetAsync(key);
            return val.IsNullOrEmpty ? null : val.ToString();
        }

        // ── Key management ────────────────────────────────────────────────────────

        public async Task<IEnumerable<string>> GetKeysAsync(string pattern)
        {
            _logger.LogInformation("Redis KEYS: {Pattern}", pattern);
            var server  = _redis.GetServer(_redis.GetEndPoints().First());
            var results = new List<string>();
            await foreach (var key in server.KeysAsync(pattern: pattern))
                results.Add(key.ToString());
            return results;
        }

        public async Task ClearHistoryCacheAsync()
        {
            _logger.LogWarning("Clearing all Redis history cache.");
            var keys = await GetKeysAsync("qm:history:*");
            foreach (var key in keys)
                await _db.KeyDeleteAsync(key);
            _logger.LogWarning("Redis history cache cleared.");
        }

        // ── Health ────────────────────────────────────────────────────────────────

        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                await _db.PingAsync();
                _logger.LogInformation("Redis ping OK.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis ping failed.");
                return false;
            }
        }

        // ════════════════════════════════════════════════════════════════════════
        // INNER CLASS — serializable row for Redis storage
        // ════════════════════════════════════════════════════════════════════════

        private sealed class CacheRow
        {
            public Guid     Id              { get; set; }
            public DateTime Timestamp       { get; set; }
            public string   Operation       { get; set; } = "";
            public string   MeasurementType { get; set; } = "";
            public double   FirstValue      { get; set; }
            public string   FirstUnit       { get; set; } = "";
            public double?  SecondValue     { get; set; }
            public string?  SecondUnit      { get; set; }
            public string?  TargetUnit      { get; set; }
            public string   Result          { get; set; } = "";
            public bool     HasError        { get; set; }
            public string?  ErrorMessage    { get; set; }

            public static CacheRow From(QuantityMeasurementEntity e) => new()
            {
                Id              = e.Id,
                Timestamp       = e.Timestamp,
                Operation       = e.Operation.ToString(),
                MeasurementType = e.FirstOperand.Type.ToString(),
                FirstValue      = e.FirstOperand.Value,
                FirstUnit       = e.FirstOperand.UnitLabel,
                SecondValue     = e.SecondOperand?.Value,
                SecondUnit      = e.SecondOperand?.UnitLabel,
                TargetUnit      = e.TargetUnit?.UnitLabel,
                Result          = e.ResultDisplay,
                HasError        = e.HasError,
                ErrorMessage    = e.ErrorMessage
            };

            public QuantityMeasurementEntity ToEntity()
            {
                var op    = Enum.Parse<QuantityMeasurementEntity.OperationType>(Operation, true);
                var first = Build(MeasurementType, FirstValue, FirstUnit);
                QuantityDTO? second = SecondValue.HasValue && SecondUnit is not null
                    ? Build(MeasurementType, SecondValue.Value, SecondUnit) : null;
                QuantityDTO? target = TargetUnit is not null
                    ? Build(MeasurementType, 0.0, TargetUnit) : null;

                if (HasError)
                    return new QuantityMeasurementEntity(op, first, second, ErrorMessage ?? "Unknown error", true);
                if (second is null)
                    return new QuantityMeasurementEntity(op, first, target!, Result);
                return new QuantityMeasurementEntity(op, first, second, Result, target);
            }

            private static QuantityDTO Build(string type, double value, string unit) =>
                type switch
                {
                    "Length"      => new QuantityDTO(value, Enum.Parse<QuantityDTO.LengthUnit>(unit, true)),
                    "Weight"      => new QuantityDTO(value, Enum.Parse<QuantityDTO.WeightUnit>(unit, true)),
                    "Volume"      => new QuantityDTO(value, Enum.Parse<QuantityDTO.VolumeUnit>(unit, true)),
                    "Temperature" => new QuantityDTO(value, Enum.Parse<QuantityDTO.TemperatureUnit>(unit, true)),
                    _ => throw new InvalidOperationException($"Unknown type: {type}")
                };
        }
    }
}