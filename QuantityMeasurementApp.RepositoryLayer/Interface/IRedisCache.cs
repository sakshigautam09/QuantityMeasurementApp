// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : Interface/IRedisCache.cs
// UC-17   : Redis cache contract — primary read layer.
// ============================================================

using QuantityMeasurementApp.ModelLayer;

namespace QuantityMeasurementApp.RepositoryLayer.Interface
{
    public interface IRedisCache
    {
        // ── Single entity ─────────────────────────────────────────────────────────
        Task SetAsync(string key, QuantityMeasurementEntity entity, TimeSpan? expiry = null);
        Task<QuantityMeasurementEntity?> GetAsync(string key);
        Task DeleteAsync(string key);
        Task<bool> ExistsAsync(string key);

        // ── String (used for serialized lists) ────────────────────────────────────
        Task SetStringAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetStringAsync(string key);

        // ── List operations for history ───────────────────────────────────────────
        Task PushToListAsync(string listKey, QuantityMeasurementEntity entity);
        Task<IReadOnlyList<QuantityMeasurementEntity>> GetListAsync(string listKey);

        // ── Key management ────────────────────────────────────────────────────────
        Task<IEnumerable<string>> GetKeysAsync(string pattern);
        Task ClearHistoryCacheAsync();

        // ── Health ────────────────────────────────────────────────────────────────
        Task<bool> IsAvailableAsync();
    }
}