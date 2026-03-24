// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : QuantityMeasurementJsonCacheRepository.cs
// UC-17   : JSON file cache
//
// • Saves every operation as a JSON array in quantity_cache.json
//   next to the executable (offline fallback).
//
// • SyncToDatabase(dbRepo)
//     → copies each cached record to dbRepo
//     → on full success : clears in-memory list AND deletes
//       the JSON file (records now live in the DB)
//     → on partial success : keeps only the failed records in
//       memory and rewrites the JSON file with those records
//
// • Thread-safe via ReaderWriterLockSlim.
// ============================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using QuantityMeasurementApp.ModelLayer;
using QuantityMeasurementApp.RepositoryLayer.Interface;

namespace QuantityMeasurementApp.RepositoryLayer
{
    public sealed class QuantityMeasurementJsonCacheRepository
        : IQuantityMeasurementRepository
    {
        // ── Singleton ─────────────────────────────────────────────────────────────

        private static readonly Lazy<QuantityMeasurementJsonCacheRepository> _instance =
            new(() => new QuantityMeasurementJsonCacheRepository());

        public static QuantityMeasurementJsonCacheRepository Instance => _instance.Value;

        // ── Constants ─────────────────────────────────────────────────────────────

        private const string CacheFile = "quantity_cache.json";

        private readonly List<QuantityMeasurementEntity> _cache = new();
        private readonly ReaderWriterLockSlim _lock = new();

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Converters    = { new JsonStringEnumConverter() }
        };

        // ── Constructor ───────────────────────────────────────────────────────────

        private QuantityMeasurementJsonCacheRepository() => LoadFromFile();

        // ── IQuantityMeasurementRepository ────────────────────────────────────────

        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            _lock.EnterWriteLock();
            try   { _cache.Add(entity); WriteFile(); }
            finally { _lock.ExitWriteLock(); }
        }

        public QuantityMeasurementEntity? FindById(Guid id)
        {
            _lock.EnterReadLock();
            try   { return _cache.Find(e => e.Id == id); }
            finally { _lock.ExitReadLock(); }
        }

        public IReadOnlyList<QuantityMeasurementEntity> FindAll()
        {
            _lock.EnterReadLock();
            try   { return _cache.AsReadOnly(); }
            finally { _lock.ExitReadLock(); }
        }

        public IReadOnlyList<QuantityMeasurementEntity> FindByOperation(
            QuantityMeasurementEntity.OperationType op)
        {
            _lock.EnterReadLock();
            try   { return _cache.FindAll(e => e.Operation == op).AsReadOnly(); }
            finally { _lock.ExitReadLock(); }
        }

        public IReadOnlyList<QuantityMeasurementEntity> FindByMeasurementType(
            string measurementType)
        {
            _lock.EnterReadLock();
            try
            {
                return _cache
                    .FindAll(e => e.FirstOperand.Type.ToString()
                        .Equals(measurementType, StringComparison.OrdinalIgnoreCase))
                    .AsReadOnly();
            }
            finally { _lock.ExitReadLock(); }
        }

        public int GetTotalCount()
        {
            _lock.EnterReadLock();
            try   { return _cache.Count; }
            finally { _lock.ExitReadLock(); }
        }

        public int GetCountByOperation(QuantityMeasurementEntity.OperationType op)
        {
            _lock.EnterReadLock();
            try   { return _cache.FindAll(e => e.Operation == op).Count; }
            finally { _lock.ExitReadLock(); }
        }

        public int GetErrorCount()
        {
            _lock.EnterReadLock();
            try   { return _cache.FindAll(e => e.HasError).Count; }
            finally { _lock.ExitReadLock(); }
        }

        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _cache.Clear();
                DeleteFile();
            }
            finally { _lock.ExitWriteLock(); }
        }

        public void ReleaseResources() => _lock.Dispose();

        // ── Public cache-specific helpers ─────────────────────────────────────────

        public int  PendingCount      => GetTotalCount();
        public bool HasPendingRecords => PendingCount > 0;

        /// <summary>
        /// Copies every cached record to <paramref name="dbRepo"/>.
        /// On full success  → clears in-memory list and DELETES the JSON file.
        /// On partial error → keeps only failed records and rewrites the file.
        /// Returns the number successfully synced.
        /// </summary>
        public int SyncToDatabase(IQuantityMeasurementRepository dbRepo)
        {
            if (dbRepo is null) throw new ArgumentNullException(nameof(dbRepo));

            _lock.EnterWriteLock();
            try
            {
                if (_cache.Count == 0) return 0;

                int synced = 0;
                var failed = new List<QuantityMeasurementEntity>();

                foreach (var entity in _cache)
                {
                    try
                    {
                        dbRepo.Save(entity);
                        synced++;
                    }
                    catch (Exception ex)
                    {
                        System.Console.Error.WriteLine(
                            $"[JsonCache] Could not sync record {entity.Id}: {ex.Message}");
                        failed.Add(entity);
                    }
                }

                _cache.Clear();

                if (failed.Count == 0)
                {
                    // Full success — remove the file entirely
                    DeleteFile();
                }
                else
                {
                    // Partial — keep only the records that didn't make it
                    _cache.AddRange(failed);
                    WriteFile();
                }

                return synced;
            }
            finally { _lock.ExitWriteLock(); }
        }

        // ── File I/O ──────────────────────────────────────────────────────────────

        /// <summary>Serialises the full in-memory cache to disk. Call inside a write-lock.</summary>
        private void WriteFile()
        {
            try
            {
                var rows = new List<CacheRow>(_cache.Count);
                foreach (var e in _cache) rows.Add(CacheRow.FromEntity(e));

                File.WriteAllText(CacheFile,
                    JsonSerializer.Serialize(rows, _jsonOptions));
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine(
                    $"[JsonCache] Write warning: {ex.Message}");
            }
        }

        /// <summary>Deletes the cache file if it exists. Call inside a write-lock.</summary>
        private static void DeleteFile()
        {
            try
            {
                if (File.Exists(CacheFile)) File.Delete(CacheFile);
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine(
                    $"[JsonCache] Could not delete cache file: {ex.Message}");
            }
        }

        /// <summary>Loads the cache from disk at startup. Called from constructor (no lock needed).</summary>
        private void LoadFromFile()
        {
            if (!File.Exists(CacheFile)) return;

            try
            {
                string json = File.ReadAllText(CacheFile);
                if (string.IsNullOrWhiteSpace(json)) return;

                var rows = JsonSerializer.Deserialize<List<CacheRow>>(json, _jsonOptions);
                if (rows is null) return;

                int loaded = 0;
                foreach (var row in rows)
                {
                    try   { _cache.Add(row.ToEntity()); loaded++; }
                    catch (Exception ex)
                    {
                        System.Console.Error.WriteLine(
                            $"[JsonCache] Skipping bad row: {ex.Message}");
                    }
                }

                if (loaded > 0)
                    System.Console.WriteLine(
                        $"[JsonCache] {loaded} offline record(s) loaded from cache file.");
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine(
                    $"[JsonCache] Read warning: {ex.Message}");
            }
        }

        // ── Serialisable row ──────────────────────────────────────────────────────

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

            // Entity → Row
            public static CacheRow FromEntity(QuantityMeasurementEntity e) => new()
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

            // Row → Entity
            public QuantityMeasurementEntity ToEntity()
            {
                var op    = Enum.Parse<QuantityMeasurementEntity.OperationType>(Operation, true);
                var first = BuildDTO(MeasurementType, FirstValue, FirstUnit);

                QuantityDTO? second = SecondValue.HasValue && SecondUnit is not null
                    ? BuildDTO(MeasurementType, SecondValue.Value, SecondUnit)
                    : null;

                QuantityDTO? target = TargetUnit is not null
                    ? BuildDTO(MeasurementType, 0.0, TargetUnit)
                    : null;

                if (HasError)
                    return new QuantityMeasurementEntity(
                        op, first, second, ErrorMessage ?? "Unknown error", true);

                if (second is null)
                    return new QuantityMeasurementEntity(op, first, target!, Result);

                return new QuantityMeasurementEntity(op, first, second, Result, target);
            }

            private static QuantityDTO BuildDTO(string type, double value, string unit) =>
                type switch
                {
                    "Length"      => new QuantityDTO(value,
                                         Enum.Parse<QuantityDTO.LengthUnit>(unit, true)),
                    "Weight"      => new QuantityDTO(value,
                                         Enum.Parse<QuantityDTO.WeightUnit>(unit, true)),
                    "Volume"      => new QuantityDTO(value,
                                         Enum.Parse<QuantityDTO.VolumeUnit>(unit, true)),
                    "Temperature" => new QuantityDTO(value,
                                         Enum.Parse<QuantityDTO.TemperatureUnit>(unit, true)),
                    _ => throw new InvalidOperationException(
                             $"Unknown measurement type in cache: {type}")
                };
        }
    }
}