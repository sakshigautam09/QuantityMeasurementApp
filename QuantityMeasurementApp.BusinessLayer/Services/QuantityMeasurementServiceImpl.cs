// ============================================================
// PROJECT : QuantityMeasurementApp.BusinessLayer
// FILE    : Services/QuantityMeasurementServiceImpl.cs
// UC-17   : Saves to Redis (primary read) + SQL Server (permanent).
//           Encrypts ResultDisplay before saving to both stores.
//
// WRITE FLOW:
//   result → AES Encrypt → Save to Redis list + Save to SQL Server
//
// READ FLOW (in Controller):
//   Redis first → if HIT return → if MISS load SQL → repopulate Redis
// ============================================================

using System;
using Microsoft.Extensions.Logging;
using QuantityMeasurementApp.BusinessLayer.Interface;
using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Interfaces;
using QuantityMeasurementApp.ModelLayer;
using QuantityMeasurementApp.RepositoryLayer;
using QuantityMeasurementApp.RepositoryLayer.Interface;
using QuantityMeasurementApp.RepositoryLayer.Services;

namespace QuantityMeasurementApp.BusinessLayer
{
    public class QuantityMeasurementServiceImpl : IQuantityMeasurementService
    {
        private readonly IQuantityModelService          _modelSvc;
        private readonly ITemperatureService            _tempSvc;
        private readonly IQuantityMeasurementRepository _repo;
        private readonly IEncryptionService             _encryptor;
        private readonly IRedisCache                    _cache;
        private readonly ILogger<QuantityMeasurementServiceImpl> _logger;

        // ── Full constructor — used by API (DI injects all dependencies) ─────────

        public QuantityMeasurementServiceImpl(
            IQuantityModelService          modelService,
            ITemperatureService            temperatureService,
            IQuantityMeasurementRepository repository,
            IEncryptionService             encryptor,
            IRedisCache                    cache,
            ILogger<QuantityMeasurementServiceImpl> logger)
        {
            _modelSvc  = modelService       ?? throw new ArgumentNullException(nameof(modelService));
            _tempSvc   = temperatureService ?? throw new ArgumentNullException(nameof(temperatureService));
            _repo      = repository         ?? throw new ArgumentNullException(nameof(repository));
            _encryptor = encryptor          ?? throw new ArgumentNullException(nameof(encryptor));
            _cache     = cache              ?? throw new ArgumentNullException(nameof(cache));
            _logger    = logger             ?? throw new ArgumentNullException(nameof(logger));
        }

        // ── Console constructor — no encryption, no Redis, no logger needed ──────

        public QuantityMeasurementServiceImpl(
            IQuantityModelService          modelService,
            ITemperatureService            temperatureService,
            IQuantityMeasurementRepository repository)
            : this(
                modelService,
                temperatureService,
                repository,
                new NoOpEncryptionService(),          // no encryption in Console
                new NoOpRedisCache(),                  // no Redis in Console
                Microsoft.Extensions.Logging.Abstractions.NullLogger<QuantityMeasurementServiceImpl>.Instance)
        { }

        // ── Compare ───────────────────────────────────────────────────────────────

        public QuantityDTO Compare(QuantityDTO first, QuantityDTO second)
        {
            Validate(first, second);
            ValidateSameCategory(first, second, "compare");
            _logger.LogInformation("Compare: {First} vs {Second}", first, second);

            try
            {
                bool equal = first.Type == QuantityDTO.MeasurementType.Temperature
                    ? _tempSvc.AreEqual(
                        _tempSvc.Create(first.Value,  ToTemperatureUnit(first.UnitLabel)),
                        _tempSvc.Create(second.Value, ToTemperatureUnit(second.UnitLabel)))
                    : _modelSvc.AreEqual(ToModel(first), ToModel(second));

                _logger.LogInformation("Compare result: {Result}", equal);
                Persist(QuantityMeasurementEntity.OperationType.Compare,
                    first, second, equal.ToString());
                return ToScalarDTO(equal ? 1.0 : 0.0, first);
            }
            catch (QuantityMeasurementException) { throw; }
            catch (Exception ex) { throw WrapAndLog("Compare", first, second, ex); }
        }

        // ── Convert ───────────────────────────────────────────────────────────────

        public QuantityDTO Convert(QuantityDTO source, QuantityDTO targetUnit)
        {
            Validate(source, targetUnit);
            ValidateSameCategory(source, targetUnit, "convert");
            _logger.LogInformation("Convert: {Source} → {Target}", source, targetUnit.UnitLabel);

            try
            {
                QuantityDTO result;
                if (source.Type == QuantityDTO.MeasurementType.Temperature)
                {
                    double converted = _tempSvc.Convert(
                        source.Value,
                        ToTemperatureUnit(source.UnitLabel),
                        ToTemperatureUnit(targetUnit.UnitLabel));
                    result = new QuantityDTO(converted, targetUnit.TemperatureUnitValue!.Value);
                }
                else
                {
                    result = FromModel(
                        _modelSvc.ConvertTo(ToModel(source), ToMeasurableUnit(targetUnit)),
                        targetUnit.Type);
                }

                _logger.LogInformation("Convert result: {Result}", result);
                string encResult = _encryptor.Encrypt(result.ToString());
                var entity = new QuantityMeasurementEntity(
                    QuantityMeasurementEntity.OperationType.Convert,
                    source, targetUnit, encResult);
                PersistBoth(entity);
                return result;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (Exception ex) { throw WrapAndLog("Convert", source, targetUnit, ex); }
        }

        // ── Add ───────────────────────────────────────────────────────────────────

        public QuantityDTO Add(QuantityDTO first, QuantityDTO second)
            => AddCore(first, second, null);

        public QuantityDTO AddWithTargetUnit(QuantityDTO first, QuantityDTO second, QuantityDTO tu)
            => AddCore(first, second, tu);

        private QuantityDTO AddCore(QuantityDTO first, QuantityDTO second, QuantityDTO? tu)
        {
            Validate(first, second);
            ValidateSameCategory(first, second, "add");
            ValidateArithmetic(first, "addition");
            _logger.LogInformation("Add: {First} + {Second}", first, second);

            try
            {
                var result = tu is null
                    ? _modelSvc.Add(ToModel(first), ToModel(second))
                    : _modelSvc.AddWithTargetUnit(ToModel(first), ToModel(second), ToMeasurableUnit(tu));
                var dto = FromModel(result, first.Type);
                _logger.LogInformation("Add result: {Result}", dto);
                Persist(QuantityMeasurementEntity.OperationType.Add, first, second, dto.ToString(), tu);
                return dto;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (Exception ex) { throw WrapAndLog("Add", first, second, ex); }
        }

        // ── Subtract ──────────────────────────────────────────────────────────────

        public QuantityDTO Subtract(QuantityDTO first, QuantityDTO second)
            => SubtractCore(first, second, null);

        public QuantityDTO SubtractWithTargetUnit(QuantityDTO first, QuantityDTO second, QuantityDTO tu)
            => SubtractCore(first, second, tu);

        private QuantityDTO SubtractCore(QuantityDTO first, QuantityDTO second, QuantityDTO? tu)
        {
            Validate(first, second);
            ValidateSameCategory(first, second, "subtract");
            ValidateArithmetic(first, "subtraction");
            _logger.LogInformation("Subtract: {First} - {Second}", first, second);

            try
            {
                var result = tu is null
                    ? _modelSvc.Subtract(ToModel(first), ToModel(second))
                    : _modelSvc.SubtractWithTargetUnit(ToModel(first), ToModel(second), ToMeasurableUnit(tu));
                var dto = FromModel(result, first.Type);
                _logger.LogInformation("Subtract result: {Result}", dto);
                Persist(QuantityMeasurementEntity.OperationType.Subtract, first, second, dto.ToString(), tu);
                return dto;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (Exception ex) { throw WrapAndLog("Subtract", first, second, ex); }
        }

        // ── Divide ────────────────────────────────────────────────────────────────

        public QuantityDTO Divide(QuantityDTO first, QuantityDTO second)
        {
            Validate(first, second);
            ValidateSameCategory(first, second, "divide");
            ValidateArithmetic(first, "division");
            _logger.LogInformation("Divide: {First} / {Second}", first, second);

            try
            {
                double ratio = _modelSvc.Divide(ToModel(first), ToModel(second));
                _logger.LogInformation("Divide result: {Result}", ratio);
                Persist(QuantityMeasurementEntity.OperationType.Divide,
                    first, second, ratio.ToString("G6"));
                return ToScalarDTO(ratio, first);
            }
            catch (QuantityMeasurementException) { throw; }
            catch (DivideByZeroException ex)
            {
                _logger.LogError(ex, "Divide by zero.");
                PersistError(QuantityMeasurementEntity.OperationType.Divide, first, second, ex.Message);
                throw new QuantityMeasurementException("Division by zero.", ex);
            }
            catch (Exception ex) { throw WrapAndLog("Divide", first, second, ex); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // PERSIST — Encrypt + Save to Redis + Save to SQL Server
        // ════════════════════════════════════════════════════════════════════════

        private void Persist(
            QuantityMeasurementEntity.OperationType op,
            QuantityDTO first, QuantityDTO second,
            string result, QuantityDTO? tu = null)
        {
            try
            {
                string encResult = _encryptor.Encrypt(result);
                var entity = new QuantityMeasurementEntity(op, first, second, encResult, tu);
                PersistBoth(entity);
            }
            catch (Exception ex)
            { _logger.LogWarning(ex, "Could not persist operation."); }
        }

        private void PersistError(
            QuantityMeasurementEntity.OperationType op,
            QuantityDTO first, QuantityDTO? second, string msg)
        {
            try
            {
                var entity = new QuantityMeasurementEntity(op, first, second, msg, true);
                PersistBoth(entity);
            }
            catch (Exception ex)
            { _logger.LogWarning(ex, "Could not persist error."); }
        }

        /// <summary>
        /// Saves entity to BOTH Redis (primary read) and SQL Server (permanent backup).
        /// </summary>
        private void PersistBoth(QuantityMeasurementEntity entity)
        {
            // 1. Save to Redis (primary — fast reads)
            try
            {
                _cache.PushToListAsync(RedisCacheService.AllHistoryKey, entity)
                      .GetAwaiter().GetResult();
                _logger.LogInformation("Saved to Redis: {Id}", entity.Id);
            }
            catch (Exception ex)
            { _logger.LogWarning(ex, "Redis save failed — continuing to SQL."); }

            // 2. Save to SQL Server (permanent backup)
            try
            {
                _repo.Save(entity);
                _logger.LogInformation("Saved to SQL Server: {Id}", entity.Id);
            }
            catch (Exception ex)
            { _logger.LogError(ex, "SQL Server save failed."); }
        }

        // ── Mapping helpers ───────────────────────────────────────────────────────

        private static QuantityModel ToModel(QuantityDTO dto)
            => new QuantityModel(dto.Value, ToMeasurableUnit(dto));

        private static IMeasurable ToMeasurableUnit(QuantityDTO dto) => dto.Type switch
        {
            QuantityDTO.MeasurementType.Length =>
                new LengthMeasurableUnit(Enum.Parse<LengthUnit>(dto.UnitLabel, true)),
            QuantityDTO.MeasurementType.Weight =>
                new WeightMeasurableUnit(Enum.Parse<WeightUnit>(dto.UnitLabel, true)),
            QuantityDTO.MeasurementType.Volume =>
                new VolumeMeasurableUnit(Enum.Parse<VolumeUnit>(dto.UnitLabel, true)),
            QuantityDTO.MeasurementType.Temperature =>
                new TemperatureMeasurableUnit(Enum.Parse<TemperatureUnit>(dto.UnitLabel, true)),
            _ => throw new QuantityMeasurementException($"Unsupported type: {dto.Type}")
        };

        private static QuantityDTO FromModel(QuantityModel model, QuantityDTO.MeasurementType type)
        {
            string u = model.Unit.GetUnitName();
            return type switch
            {
                QuantityDTO.MeasurementType.Length =>
                    new QuantityDTO(model.Value, Enum.Parse<QuantityDTO.LengthUnit>(u, true)),
                QuantityDTO.MeasurementType.Weight =>
                    new QuantityDTO(model.Value, Enum.Parse<QuantityDTO.WeightUnit>(u, true)),
                QuantityDTO.MeasurementType.Volume =>
                    new QuantityDTO(model.Value, Enum.Parse<QuantityDTO.VolumeUnit>(u, true)),
                QuantityDTO.MeasurementType.Temperature =>
                    new QuantityDTO(model.Value, Enum.Parse<QuantityDTO.TemperatureUnit>(u, true)),
                _ => throw new QuantityMeasurementException($"Cannot convert: {type}")
            };
        }

        private static QuantityDTO ToScalarDTO(double v, QuantityDTO s) => s.Type switch
        {
            QuantityDTO.MeasurementType.Length      => new(v, s.LengthUnitValue!.Value),
            QuantityDTO.MeasurementType.Weight      => new(v, s.WeightUnitValue!.Value),
            QuantityDTO.MeasurementType.Volume      => new(v, s.VolumeUnitValue!.Value),
            QuantityDTO.MeasurementType.Temperature => new(v, s.TemperatureUnitValue!.Value),
            _ => throw new QuantityMeasurementException("Unsupported type.")
        };

        private static TemperatureUnit ToTemperatureUnit(string label)
        {
            if (Enum.TryParse<TemperatureUnit>(label, true, out var r)) return r;
            throw new QuantityMeasurementException($"Unknown temperature unit: {label}");
        }

        private static void Validate(QuantityDTO a, QuantityDTO b)
        {
            if (a is null) throw new QuantityMeasurementException("First operand is null.");
            if (b is null) throw new QuantityMeasurementException("Second operand is null.");
        }

        private static void ValidateSameCategory(QuantityDTO a, QuantityDTO b, string op)
        {
            if (a.Type != b.Type)
                throw new QuantityMeasurementException($"Cannot {op} {a.Type} with {b.Type}.");
        }

        private static void ValidateArithmetic(QuantityDTO dto, string op)
        {
            if (dto.Type == QuantityDTO.MeasurementType.Temperature)
                throw new QuantityMeasurementException($"Temperature does not support {op}.");
        }

        private QuantityMeasurementException WrapAndLog(
            string op, QuantityDTO first, QuantityDTO? second, Exception ex)
        {
            _logger.LogError(ex, "{Op} failed.", op);
            PersistError(Enum.Parse<QuantityMeasurementEntity.OperationType>(op, true),
                first, second, ex.Message);
            return new QuantityMeasurementException($"{op} failed: {ex.Message}", ex);
        }
    }
    // ════════════════════════════════════════════════════════════════════════
    // NO-OP IMPLEMENTATIONS — used by Console constructor
    // No encryption and no Redis in Console app
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Passes data through unchanged — no encryption.
    /// Used by Console app which does not need AES encryption.
    /// </summary>
    internal sealed class NoOpEncryptionService : QuantityMeasurementApp.BusinessLayer.Interface.IEncryptionService
    {
        public string Encrypt(string plainText)     => plainText;  // returns as-is
        public string Decrypt(string encryptedText) => encryptedText; // returns as-is
        public string GenerateKey()                 => string.Empty;
        public string GenerateIV()                  => string.Empty;
    }

    /// <summary>
    /// Does nothing — no Redis in Console app.
    /// </summary>
    internal sealed class NoOpRedisCache : QuantityMeasurementApp.RepositoryLayer.Interface.IRedisCache
    {
        public System.Threading.Tasks.Task SetAsync(string k, QuantityMeasurementApp.ModelLayer.QuantityMeasurementEntity e, System.TimeSpan? ex = null)
            => System.Threading.Tasks.Task.CompletedTask;
        public System.Threading.Tasks.Task<QuantityMeasurementApp.ModelLayer.QuantityMeasurementEntity?> GetAsync(string k)
            => System.Threading.Tasks.Task.FromResult<QuantityMeasurementApp.ModelLayer.QuantityMeasurementEntity?>(null);
        public System.Threading.Tasks.Task DeleteAsync(string k)   => System.Threading.Tasks.Task.CompletedTask;
        public System.Threading.Tasks.Task<bool> ExistsAsync(string k) => System.Threading.Tasks.Task.FromResult(false);
        public System.Threading.Tasks.Task SetStringAsync(string k, string v, System.TimeSpan? ex = null) => System.Threading.Tasks.Task.CompletedTask;
        public System.Threading.Tasks.Task<string?> GetStringAsync(string k) => System.Threading.Tasks.Task.FromResult<string?>(null);
        public System.Threading.Tasks.Task PushToListAsync(string lk, QuantityMeasurementApp.ModelLayer.QuantityMeasurementEntity e) => System.Threading.Tasks.Task.CompletedTask;
        public System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyList<QuantityMeasurementApp.ModelLayer.QuantityMeasurementEntity>> GetListAsync(string lk)
            => System.Threading.Tasks.Task.FromResult<System.Collections.Generic.IReadOnlyList<QuantityMeasurementApp.ModelLayer.QuantityMeasurementEntity>>(new System.Collections.Generic.List<QuantityMeasurementApp.ModelLayer.QuantityMeasurementEntity>().AsReadOnly());
        public System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<string>> GetKeysAsync(string p) => System.Threading.Tasks.Task.FromResult<System.Collections.Generic.IEnumerable<string>>(new System.Collections.Generic.List<string>());
        public System.Threading.Tasks.Task ClearHistoryCacheAsync() => System.Threading.Tasks.Task.CompletedTask;
        public System.Threading.Tasks.Task<bool> IsAvailableAsync() => System.Threading.Tasks.Task.FromResult(false);
    }

}