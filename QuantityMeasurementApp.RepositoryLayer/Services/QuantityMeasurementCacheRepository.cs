// // ============================================================
// // PROJECT : QuantityMeasurementApp.RepositoryLayer
// // FILE    : QuantityMeasurementCacheRepository.cs
// // UC-16   : Updated to implement new interface methods.
// //
// // Still useful for unit tests and offline scenarios.
// // All 6 new UC-16 methods implemented in-memory.
// // ============================================================

// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using QuantityMeasurementApp.ModelLayer;

// namespace QuantityMeasurementApp.RepositoryLayer
// {
//     public sealed class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
//     {
//         // ── Singleton ────────────────────────────────────────────────────────────────

//         private static readonly Lazy<QuantityMeasurementCacheRepository> _instance =
//             new(() => new QuantityMeasurementCacheRepository());

//         public static QuantityMeasurementCacheRepository Instance => _instance.Value;

//         // ── State ─────────────────────────────────────────────────────────────────────

//         private const string DataFile = "quantity_history.dat";
//         private readonly List<QuantityMeasurementEntity> _cache = new();
//         private readonly object _lock = new();

//         private QuantityMeasurementCacheRepository() => LoadFromDisk();

//         // ── Save ─────────────────────────────────────────────────────────────────────

//         public void Save(QuantityMeasurementEntity entity)
//         {
//             if (entity is null) throw new ArgumentNullException(nameof(entity));
//             lock (_lock)
//             {
//                 _cache.Add(entity);
//                 AppendToDisk(entity);
//             }
//         }

//         // ── FindById ─────────────────────────────────────────────────────────────────

//         public QuantityMeasurementEntity? FindById(Guid id)
//         {
//             lock (_lock) { return _cache.Find(e => e.Id == id); }
//         }

//         // ── FindAll  (menu option 7) ──────────────────────────────────────────────────

//         public IReadOnlyList<QuantityMeasurementEntity> FindAll()
//         {
//             lock (_lock) { return _cache.AsReadOnly(); }
//         }

//         // ── FindByOperation  (menu option 8) ─────────────────────────────────────────

//         public IReadOnlyList<QuantityMeasurementEntity> FindByOperation(
//             QuantityMeasurementEntity.OperationType operation)
//         {
//             lock (_lock)
//             {
//                 return _cache
//                     .Where(e => e.Operation == operation)
//                     .ToList()
//                     .AsReadOnly();
//             }
//         }

//         // ── FindByMeasurementType  (menu option 9) ────────────────────────────────────

//         public IReadOnlyList<QuantityMeasurementEntity> FindByMeasurementType(string measurementType)
//         {
//             lock (_lock)
//             {
//                 return _cache
//                     .Where(e => e.FirstOperand.Type.ToString()
//                         .Equals(measurementType, StringComparison.OrdinalIgnoreCase))
//                     .ToList()
//                     .AsReadOnly();
//             }
//         }

//         // ── GetTotalCount  (menu option 10) ───────────────────────────────────────────

//         public int GetTotalCount()
//         {
//             lock (_lock) { return _cache.Count; }
//         }

//         // ── GetCountByOperation  (menu option 10) ─────────────────────────────────────

//         public int GetCountByOperation(QuantityMeasurementEntity.OperationType operation)
//         {
//             lock (_lock) { return _cache.Count(e => e.Operation == operation); }
//         }

//         // ── GetErrorCount  (menu option 10) ───────────────────────────────────────────

//         public int GetErrorCount()
//         {
//             lock (_lock) { return _cache.Count(e => e.HasError); }
//         }

//         // ── Clear  (menu option 11) ───────────────────────────────────────────────────

//         public void Clear()
//         {
//             lock (_lock)
//             {
//                 _cache.Clear();
//                 if (File.Exists(DataFile)) File.Delete(DataFile);
//             }
//         }

//         // ── ReleaseResources ──────────────────────────────────────────────────────────

//         public void ReleaseResources()
//         {
//             // Nothing to release for an in-memory cache.
//         }

//         // ── Disk helpers ──────────────────────────────────────────────────────────────

//         private void AppendToDisk(QuantityMeasurementEntity entity)
//         {
//             try { File.AppendAllText(DataFile, entity + Environment.NewLine); }
//             catch (Exception ex)
//             { System.Console.Error.WriteLine($"[Repository] Disk write warning: {ex.Message}"); }
//         }

//         private void LoadFromDisk()
//         {
//             if (!File.Exists(DataFile)) return;
//             try
//             {
//                 int n = File.ReadLines(DataFile).Count();
//                 if (n > 0)
//                     System.Console.WriteLine($"[Repository] {n} historical record(s) found on disk.");
//             }
//             catch (Exception ex)
//             { System.Console.Error.WriteLine($"[Repository] Disk read warning: {ex.Message}"); }
//         }
//     }
// }









// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : QuantityMeasurementCacheRepository.cs
// UC-16   : Refactored to remove LINQ usage.
//
// Still useful for unit tests and offline scenarios.
// All 6 new UC-16 methods implemented in-memory.
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

        private QuantityMeasurementCacheRepository() => LoadFromDisk();

        // ── Save ─────────────────────────────────────────────────────────────────────

        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            lock (_lock)
            {
                _cache.Add(entity);
                AppendToDisk(entity);
            }
        }

        // ── FindById ─────────────────────────────────────────────────────────────────

        public QuantityMeasurementEntity? FindById(Guid id)
        {
            lock (_lock)
            {
                foreach (var e in _cache)
                {
                    if (e.Id == id) return e;
                }
                return null;
            }
        }

        // ── FindAll (menu option 7) ──────────────────────────────────────────────────

        public IReadOnlyList<QuantityMeasurementEntity> FindAll()
        {
            lock (_lock) { return _cache.AsReadOnly(); }
        }

        // ── FindByOperation (menu option 8) ─────────────────────────────────────────

        public IReadOnlyList<QuantityMeasurementEntity> FindByOperation(
            QuantityMeasurementEntity.OperationType operation)
        {
            lock (_lock)
            {
                List<QuantityMeasurementEntity> result = new();
                foreach (var e in _cache)
                {
                    if (e.Operation == operation)
                        result.Add(e);
                }
                return result.AsReadOnly();
            }
        }

        // ── FindByMeasurementType (menu option 9) ────────────────────────────────────

        public IReadOnlyList<QuantityMeasurementEntity> FindByMeasurementType(string measurementType)
        {
            lock (_lock)
            {
                List<QuantityMeasurementEntity> result = new();
                foreach (var e in _cache)
                {
                    if (e.FirstOperand.Type.ToString().Equals(measurementType, StringComparison.OrdinalIgnoreCase))
                        result.Add(e);
                }
                return result.AsReadOnly();
            }
        }

        // ── GetTotalCount (menu option 10) ───────────────────────────────────────────

        public int GetTotalCount()
        {
            lock (_lock) { return _cache.Count; }
        }

        // ── GetCountByOperation (menu option 10) ─────────────────────────────────────

        public int GetCountByOperation(QuantityMeasurementEntity.OperationType operation)
        {
            lock (_lock)
            {
                int count = 0;
                foreach (var e in _cache)
                {
                    if (e.Operation == operation) count++;
                }
                return count;
            }
        }

        // ── GetErrorCount (menu option 10) ───────────────────────────────────────────

        public int GetErrorCount()
        {
            lock (_lock)
            {
                int count = 0;
                foreach (var e in _cache)
                {
                    if (e.HasError) count++;
                }
                return count;
            }
        }

        // ── Clear (menu option 11) ───────────────────────────────────────────────────

        public void Clear()
        {
            lock (_lock)
            {
                _cache.Clear();
                if (File.Exists(DataFile)) File.Delete(DataFile);
            }
        }

        // ── ReleaseResources ──────────────────────────────────────────────────────────

        public void ReleaseResources()
        {
            // Nothing to release for an in-memory cache.
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
                using (var reader = new StreamReader(DataFile))
                {
                    while (reader.ReadLine() != null) n++;
                }

                if (n > 0)
                    System.Console.WriteLine($"[Repository] {n} historical record(s) found on disk.");
            }
            catch (Exception ex)
            { System.Console.Error.WriteLine($"[Repository] Disk read warning: {ex.Message}"); }
        }
    }
}