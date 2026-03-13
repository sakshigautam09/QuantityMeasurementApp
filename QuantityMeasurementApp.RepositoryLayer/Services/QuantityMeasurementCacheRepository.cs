// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : QuantityMeasurementCacheRepository.cs
//
// UC-15 : N-Tier Architecture
//
// Purpose : Singleton in-memory cache repository.
//           • Stores QuantityMeasurementEntity in a List<T>.
//           • Appends each saved entity as a text line to disk
//             so history survives application restarts.
//           • Loads the count of prior records from disk on startup.
//
// Design Patterns : Singleton
//
// NOTE : PURELY ADDITIVE – no existing code is modified.
// ============================================================

using System;
using System.Collections.Generic;
using System.IO;
using QuantityMeasurementApp.ModelLayer;

namespace QuantityMeasurementApp.RepositoryLayer
{
    public sealed class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
    {
        // ── Singleton ────────────────────────────────────────────────────────────────

        private static readonly Lazy<QuantityMeasurementCacheRepository> _instance =
            new(() => new QuantityMeasurementCacheRepository());

        public static QuantityMeasurementCacheRepository Instance => _instance.Value;

        // ── State ─────────────────────────────────────────────────────────────────────

        private const string DataFile = "quantity_history.dat";
        private readonly List<QuantityMeasurementEntity> _cache = new();
        private readonly object _lock = new();

        // ── Private constructor ────────────────────────────────────────────────────────

        private QuantityMeasurementCacheRepository() => LoadFromDisk();

        // ── IQuantityMeasurementRepository ────────────────────────────────────────────

        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            lock (_lock)
            {
                _cache.Add(entity);
                AppendToDisk(entity);
            }
        }

        public QuantityMeasurementEntity? FindById(Guid id)
        {
            lock (_lock) { return _cache.Find(e => e.Id == id); }
        }

        public IReadOnlyList<QuantityMeasurementEntity> FindAll()
        {
            lock (_lock) { return _cache.AsReadOnly(); }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _cache.Clear();
                if (File.Exists(DataFile)) File.Delete(DataFile);
            }
        }

        // ── Disk helpers ──────────────────────────────────────────────────────────────

        private void AppendToDisk(QuantityMeasurementEntity entity)
        {
            try { File.AppendAllText(DataFile, entity + Environment.NewLine); }
            catch (Exception ex)
            { System.Console.Error.WriteLine($"[Repository] Disk write warning: {ex.Message}"); }
        }

        private void LoadFromDisk()
        {
            if (!File.Exists(DataFile)) return;
            try
            {
                int n = 0;
                foreach (var _ in File.ReadLines(DataFile)) n++;
                if (n > 0) System.Console.WriteLine($"[Repository] {n} historical record(s) found on disk.");
            }
            catch (Exception ex)
            { System.Console.Error.WriteLine($"[Repository] Disk read warning: {ex.Message}"); }
        }
    }
}
