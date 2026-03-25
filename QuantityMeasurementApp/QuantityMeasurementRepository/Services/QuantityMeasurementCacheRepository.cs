using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuantityMeasurementRepository
{
    /// <summary>
    /// UC15/UC16: Singleton in-memory cache repository.
    /// Persists measurement history to a JSON file after every Save.
    /// JSON file is saved in the application base directory.
    /// </summary>
    public class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
    {
        // ── Singleton ────────────────────────────────────────────────────
        public static readonly Lazy<QuantityMeasurementCacheRepository> _instance =
            new(() => new QuantityMeasurementCacheRepository());

        public static QuantityMeasurementCacheRepository Instance => _instance.Value;

        // ── Fields ───────────────────────────────────────────────────────
        private readonly List<QuantityMeasurementEntity> _cache = new();
        private readonly object                          _lock  = new();
        private readonly string                          _filePath;

        private QuantityMeasurementCacheRepository()
        {
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), "quantity_measurements.json");

            // Load existing data from JSON file if it exists
            if (File.Exists(_filePath))
            {
                try
                {
                    string json = File.ReadAllText(_filePath);
                    List<EntityJsonModel>? loaded =
                        JsonSerializer.Deserialize<List<EntityJsonModel>>(json);

                    if (loaded != null)
                    {
                        foreach (var m in loaded)
                            _cache.Add(m.ToEntity());
                    }

                    Console.WriteLine($"[CacheRepository] Loaded {_cache.Count} record(s) from JSON file.");
                }
                catch
                {
                    Console.WriteLine("[CacheRepository] Could not read existing JSON file. Starting fresh.");
                }
            }
        }

        // ── Save ─────────────────────────────────────────────────────────

        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            lock (_lock)
            {
                _cache.Add(entity);
                PersistToJson();
            }

            Console.WriteLine($"[CacheRepository] Saved and written to JSON: {entity.OperationType}");
        }

        // ── GetAllMeasurements ────────────────────────────────────────────

        public IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements()
        {
            lock (_lock)
            {
                return _cache.AsReadOnly();
            }
        }

        // ── GetByOperation ────────────────────────────────────────────────

        public IReadOnlyList<QuantityMeasurementEntity> GetByOperation(string operation)
        {
            lock (_lock)
            {
                return _cache
                    .Where(e => e.OperationType.Equals(operation, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        // ── GetByCategory ─────────────────────────────────────────────────

        public IReadOnlyList<QuantityMeasurementEntity> GetByCategory(string category)
        {
            lock (_lock)
            {
                return _cache
                    .Where(e =>
                        (e.Operand1?.Category.Equals(category, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (e.Operand2?.Category.Equals(category, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
            }
        }

        // ── GetTotalCount ─────────────────────────────────────────────────

        public int GetTotalCount()
        {
            lock (_lock)
            {
                return _cache.Count;
            }
        }

        // ── Clear ─────────────────────────────────────────────────────────

        public void Clear()
        {
            lock (_lock)
            {
                _cache.Clear();
                PersistToJson();
            }

            Console.WriteLine("[CacheRepository] Cache cleared and JSON file updated.");
        }

        // ── Persist to JSON ───────────────────────────────────────────────

        private void PersistToJson()
        {
            try
            {
                List<EntityJsonModel> models = _cache.Select(e => new EntityJsonModel(e)).ToList();

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(models, options);
                File.WriteAllText(_filePath, json);

                Console.WriteLine($"[CacheRepository] JSON saved → {_filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CacheRepository] WARNING: Could not write JSON file: {ex.Message}");
            }
        }

        // ── JSON Model (for serialization) ────────────────────────────────

        private class EntityJsonModel
        {
            public string    OperationType { get; set; } = string.Empty;
            public DateTime  Timestamp     { get; set; }
            public bool      HasError      { get; set; }
            public string    ErrorMessage  { get; set; } = string.Empty;

            public QuantityDtoJson? Operand1 { get; set; }
            public QuantityDtoJson? Operand2 { get; set; }
            public QuantityDtoJson? Result   { get; set; }

            // Needed for deserialization
            public EntityJsonModel() { }

            public EntityJsonModel(QuantityMeasurementEntity e)
            {
                OperationType = e.OperationType;
                Timestamp     = e.Timestamp;
                HasError      = e.HasError;
                ErrorMessage  = e.ErrorMessage;
                Operand1      = e.Operand1 is not null ? new QuantityDtoJson(e.Operand1) : null;
                Operand2      = e.Operand2 is not null ? new QuantityDtoJson(e.Operand2) : null;
                Result        = e.Result   is not null ? new QuantityDtoJson(e.Result)   : null;
            }

            public QuantityMeasurementEntity ToEntity()
            {
                var op1 = Operand1?.ToDTO();
                var op2 = Operand2?.ToDTO();
                var res = Result?.ToDTO();

                if (HasError)
                    return new QuantityMeasurementEntity(OperationType, op1, op2, ErrorMessage);

                if (op2 is not null && res is not null)
                    return new QuantityMeasurementEntity(OperationType, op1!, op2, res);

                return new QuantityMeasurementEntity(OperationType, op1!, res);
            }
        }

        private class QuantityDtoJson
        {
            public double Value    { get; set; }
            public string UnitName { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;

            // Needed for deserialization
            public QuantityDtoJson() { }

            public QuantityDtoJson(QuantityMeasurementModel.QuantityDTO dto)
            {
                Value    = dto.Value;
                UnitName = dto.UnitName;
                Category = dto.Category;
            }

            public QuantityMeasurementModel.QuantityDTO ToDTO()
                => new QuantityMeasurementModel.QuantityDTO(Value, UnitName, Category);
        }
    }
}