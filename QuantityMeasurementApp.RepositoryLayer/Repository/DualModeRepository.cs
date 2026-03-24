// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : DualModeRepository.cs
// UC-17   : Two constructors:
//
//  CONSOLE mode (manual — user chose once at startup):
//    new DualModeRepository(cache, db, StorageMode.Cache)
//    new DualModeRepository(cache, db, StorageMode.Database)
//    Save() always goes to the chosen destination.
//    Fixed for the whole session — no switching.
//
//  API mode (automatic — no user prompt):
//    new DualModeRepository(cache, db)
//    DB-first on every Save().
//    Falls back to cache if DB unreachable.
//    Auto-flushes cache when DB comes back (throttled 30s).
//
//  ALL read operations merge both sources in both modes.
// ============================================================

using System;
using System.Collections.Generic;
using QuantityMeasurementApp.ModelLayer;
using QuantityMeasurementApp.RepositoryLayer.Interface;

namespace QuantityMeasurementApp.RepositoryLayer
{
    public sealed class DualModeRepository : IQuantityMeasurementRepository
    {
        private readonly QuantityMeasurementJsonCacheRepository _cache;
        private readonly IQuantityMeasurementRepository         _db;

        // null = API auto-routing mode
        private readonly StorageMode? _fixedMode;

        // Auto-routing state (API mode only)
        private bool     _dbReachable        = false;
        private DateTime _lastReachableCheck = DateTime.MinValue;
        private const int ThrottleSeconds    = 30;

        // ── Constructor A: Console mode — user chose once at startup ──────────────

        public DualModeRepository(
            QuantityMeasurementJsonCacheRepository cache,
            IQuantityMeasurementRepository         db,
            StorageMode                            fixedMode)
        {
            _cache     = cache ?? throw new ArgumentNullException(nameof(cache));
            _db        = db    ?? throw new ArgumentNullException(nameof(db));
            _fixedMode = fixedMode;
        }

        // ── Constructor B: API mode — auto DB-first / cache fallback ─────────────

        public DualModeRepository(
            QuantityMeasurementJsonCacheRepository cache,
            IQuantityMeasurementRepository         db)
        {
            _cache     = cache ?? throw new ArgumentNullException(nameof(cache));
            _db        = db    ?? throw new ArgumentNullException(nameof(db));
            _fixedMode = null;
        }

        // ── Expose current mode for the menu header ───────────────────────────────

        /// <summary>
        /// Returns the mode chosen by the user at startup.
        /// Returns Database if running in API auto-routing mode.
        /// </summary>
        public StorageMode CurrentMode => _fixedMode ?? StorageMode.Database;

        // ════════════════════════════════════════════════════════════════════════
        // WRITE
        // ════════════════════════════════════════════════════════════════════════

        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            switch (_fixedMode)
            {
                case StorageMode.Cache:
                    _cache.Save(entity);
                    break;

                case StorageMode.Database:
                    _db.Save(entity);
                    break;

                default:
                    // API auto-routing
                    AutoFlushAndSave(entity);
                    break;
            }
        }

        // ════════════════════════════════════════════════════════════════════════
        // READ — always merges cache + DB
        // ════════════════════════════════════════════════════════════════════════

        public QuantityMeasurementEntity? FindById(Guid id)
            => _cache.FindById(id) ?? DbSafe(() => _db.FindById(id), null);

        public IReadOnlyList<QuantityMeasurementEntity> FindAll()
        {
            var list = new List<QuantityMeasurementEntity>();
            DbSafeAdd(list, () => _db.FindAll());
            list.AddRange(_cache.FindAll());
            return list.AsReadOnly();
        }

        public IReadOnlyList<QuantityMeasurementEntity> FindByOperation(
            QuantityMeasurementEntity.OperationType op)
        {
            var list = new List<QuantityMeasurementEntity>();
            DbSafeAdd(list, () => _db.FindByOperation(op));
            list.AddRange(_cache.FindByOperation(op));
            return list.AsReadOnly();
        }

        public IReadOnlyList<QuantityMeasurementEntity> FindByMeasurementType(string t)
        {
            var list = new List<QuantityMeasurementEntity>();
            DbSafeAdd(list, () => _db.FindByMeasurementType(t));
            list.AddRange(_cache.FindByMeasurementType(t));
            return list.AsReadOnly();
        }

        public int GetTotalCount()
            => DbSafe(() => _db.GetTotalCount(), 0) + _cache.GetTotalCount();

        public int GetCountByOperation(QuantityMeasurementEntity.OperationType op)
            => DbSafe(() => _db.GetCountByOperation(op), 0)
             + _cache.GetCountByOperation(op);

        public int GetErrorCount()
            => DbSafe(() => _db.GetErrorCount(), 0) + _cache.GetErrorCount();

        public void Clear()
        {
            DbSafe(() => { _db.Clear(); return 0; }, 0);
            _cache.Clear();
        }

        public void ReleaseResources()
        {
            DbSafe(() => { _db.ReleaseResources(); return 0; }, 0);
            _cache.ReleaseResources();
        }

        // ════════════════════════════════════════════════════════════════════════
        // PUBLIC HELPERS — used by Console Controller menu option 12
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>Number of records pending in the JSON cache file.</summary>
        public int CachePendingCount => _cache.PendingCount;

        /// <summary>
        /// Manually push all JSON cache records to DB.
        /// Forces a fresh DB reachability check (bypasses 30s throttle).
        /// </summary>
        public int SyncCacheToDatabase()
        {
            if (_cache.PendingCount == 0) return 0;

            _lastReachableCheck = DateTime.MinValue; // force fresh check
            RefreshDbReachable();

            if (!_dbReachable)
            {
                System.Console.WriteLine(
                    "[Sync] Database is not reachable. Records remain in cache.");
                return 0;
            }

            return _cache.SyncToDatabase(_db);
        }

        // ════════════════════════════════════════════════════════════════════════
        // PRIVATE — API auto-routing
        // ════════════════════════════════════════════════════════════════════════

        private void AutoFlushAndSave(QuantityMeasurementEntity entity)
        {
            RefreshDbReachable();

            if (_dbReachable)
            {
                if (_cache.PendingCount > 0)
                {
                    int flushed = _cache.SyncToDatabase(_db);
                    if (flushed > 0)
                        System.Console.WriteLine(
                            $"[Auto-Sync] {flushed} offline record(s) pushed to database.");
                }
                _db.Save(entity);
            }
            else
            {
                _cache.Save(entity);
            }
        }

        private void RefreshDbReachable()
        {
            if ((DateTime.UtcNow - _lastReachableCheck).TotalSeconds < ThrottleSeconds)
                return;

            _lastReachableCheck = DateTime.UtcNow;
            try   { _ = _db.GetTotalCount(); _dbReachable = true;  }
            catch { _dbReachable = false; }
        }

        private static T DbSafe<T>(Func<T> fn, T fallback)
        {
            try   { return fn(); }
            catch { return fallback; }
        }

        private static void DbSafeAdd(
            List<QuantityMeasurementEntity> target,
            Func<IReadOnlyList<QuantityMeasurementEntity>> fn)
        {
            try   { target.AddRange(fn()); }
            catch { }
        }
    }
}
