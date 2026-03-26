using Microsoft.Extensions.Logging;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementBusinessLayer.Units;
using QuantityMeasurementModel;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementRepository;
using QuantityMeasurementRepository.Interface;

namespace QuantityMeasurementBusinessLayer.Service
{
    /// <summary>
    /// Unified quantity measurement service.
    /// Sync operations → IMeasurementHistoryRepository (Console/cache).
    /// Async operations → IQuantityMeasurementRepository (WebAPI/EF).
    /// </summary>
    public class QuantityMeasurementService : IQuantityMeasurementService, IQuantityMeasurementWebService
    {
        private readonly IMeasurementHistoryRepository _historyRepo;
        private readonly IQuantityMeasurementRepository? _persistenceRepo;
        private readonly ILogger<QuantityMeasurementService> _logger;

        public QuantityMeasurementService(
            IMeasurementHistoryRepository historyRepo,
            IQuantityMeasurementRepository? persistenceRepo,
            ILogger<QuantityMeasurementService> logger)
        {
            _historyRepo = historyRepo ?? throw new ArgumentNullException(nameof(historyRepo));
            _persistenceRepo = persistenceRepo;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        private static readonly HashSet<string> ValidCategories =
            new(StringComparer.OrdinalIgnoreCase)
            { "LENGTH", "WEIGHT", "VOLUME", "TEMPERATURE" };

        // ── IQuantityMeasurementService (sync, Console) ────────────────────

        public QuantityDTO Compare(QuantityDTO q1, QuantityDTO q2)
        {
            ValidateNotNull(q1, q2);
            ValidateSameCategory(q1, q2, "Compare");
            try
            {
                var result = CoreCompare(q1, q2);
                _historyRepo.Save(new QuantityMeasurementEntity("COMPARE", q1, q2, result));
                return result;
            }
            catch (Exception ex) when (ex is not QuantityMeasurementException)
            {
                _historyRepo.Save(new QuantityMeasurementEntity("COMPARE", q1, q2, ex.Message));
                throw new QuantityMeasurementException($"Compare failed: {ex.Message}", ex);
            }
        }

        public QuantityDTO Convert(QuantityDTO q1, QuantityDTO targetUnitDTO)
        {
            ValidateNotNull(q1, targetUnitDTO);
            try
            {
                var result = CoreConvert(q1, targetUnitDTO.UnitName);
                _historyRepo.Save(new QuantityMeasurementEntity("CONVERT", q1, result));
                return result;
            }
            catch (Exception ex) when (ex is not QuantityMeasurementException)
            {
                _historyRepo.Save(new QuantityMeasurementEntity("CONVERT", q1, null, ex.Message));
                throw new QuantityMeasurementException($"Convert failed: {ex.Message}", ex);
            }
        }

        public QuantityDTO Add(QuantityDTO q1, QuantityDTO q2)
        {
            ValidateNotNull(q1, q2);
            ValidateSameCategory(q1, q2, "Add");
            try
            {
                var result = CoreArithmetic(q1, q2, "ADD");
                _historyRepo.Save(new QuantityMeasurementEntity("ADD", q1, q2, result));
                return result;
            }
            catch (NotSupportedException)
            {
                _historyRepo.Save(new QuantityMeasurementEntity("ADD", q1, q2, "Temperature does not support Add."));
                throw new QuantityMeasurementException("Temperature does not support Add.");
            }
            catch (Exception ex) when (ex is not QuantityMeasurementException)
            {
                _historyRepo.Save(new QuantityMeasurementEntity("ADD", q1, q2, ex.Message));
                throw new QuantityMeasurementException($"Add failed: {ex.Message}", ex);
            }
        }

        public QuantityDTO Subtract(QuantityDTO q1, QuantityDTO q2)
        {
            ValidateNotNull(q1, q2);
            ValidateSameCategory(q1, q2, "Subtract");
            try
            {
                var result = CoreArithmetic(q1, q2, "SUBTRACT");
                _historyRepo.Save(new QuantityMeasurementEntity("SUBTRACT", q1, q2, result));
                return result;
            }
            catch (NotSupportedException)
            {
                _historyRepo.Save(new QuantityMeasurementEntity("SUBTRACT", q1, q2, "Temperature does not support Subtract."));
                throw new QuantityMeasurementException("Temperature does not support Subtract.");
            }
            catch (Exception ex) when (ex is not QuantityMeasurementException)
            {
                _historyRepo.Save(new QuantityMeasurementEntity("SUBTRACT", q1, q2, ex.Message));
                throw new QuantityMeasurementException($"Subtract failed: {ex.Message}", ex);
            }
        }

        public QuantityDTO Divide(QuantityDTO q1, QuantityDTO q2)
        {
            ValidateNotNull(q1, q2);
            ValidateSameCategory(q1, q2, "Divide");
            try
            {
                var result = CoreArithmetic(q1, q2, "DIVIDE");
                _historyRepo.Save(new QuantityMeasurementEntity("DIVIDE", q1, q2, result));
                return result;
            }
            catch (NotSupportedException)
            {
                _historyRepo.Save(new QuantityMeasurementEntity("DIVIDE", q1, q2, "Temperature does not support Divide."));
                throw new QuantityMeasurementException("Temperature does not support Divide.");
            }
            catch (Exception ex) when (ex is not QuantityMeasurementException)
            {
                _historyRepo.Save(new QuantityMeasurementEntity("DIVIDE", q1, q2, ex.Message));
                throw new QuantityMeasurementException($"Divide failed: {ex.Message}", ex);
            }
        }

        // ── IQuantityMeasurementWebService (async, WebAPI) ──────────────────

        public async Task<QuantityMeasurementDto> CompareAsync(QuantityRequestDto q1, QuantityRequestDto q2, int? userId = null)
        {
            _logger.LogInformation("[QuantityMeasurementService] Compare {V1}{U1} vs {V2}{U2}", q1.Value, q1.Unit, q2.Value, q2.Unit);
            return await RunBinaryAsync("COMPARE", q1, q2, userId, (d1, d2) => CoreCompare(d1, d2));
        }

        public async Task<QuantityMeasurementDto> ConvertAsync(QuantityRequestDto q1, QuantityRequestDto target, int? userId = null)
        {
           _logger.LogInformation("[QuantityMeasurementService] Convert {V}{U} → {TU}", q1.Value, q1.Unit, target.Unit);
            if (!ValidCategories.Contains(q1.MeasurementType))
                throw new QuantityMeasurementException($"Invalid MeasurementType '{q1.MeasurementType}'. Must be LENGTH, WEIGHT, VOLUME, or TEMPERATURE.");
            var entity = MakeEntity("CONVERT", q1, target, userId);
            try
            {
                var res = CoreConvert(ToDto(q1), target.Unit);
                entity.ResultValue = res.Value;
                entity.ResultUnit = res.UnitName;
                entity.ResultCategory = res.Category;
            }
            catch (Exception ex)
            {
                entity.HasError = true;
                entity.ErrorMessage = ex.Message;
                _logger.LogWarning(ex, "[QuantityMeasurementService] Convert error");
            }
            await RequirePersistence().SaveAsync(entity);
            return QuantityMeasurementDto.FromEntity(entity);
        }

        public async Task<QuantityMeasurementDto> AddAsync(QuantityRequestDto q1, QuantityRequestDto q2, int? userId = null)
            => await RunBinaryAsync("ADD", q1, q2, userId, (d1, d2) => CoreArithmetic(d1, d2, "ADD"));

        public async Task<QuantityMeasurementDto> SubtractAsync(QuantityRequestDto q1, QuantityRequestDto q2, int? userId = null)
            => await RunBinaryAsync("SUBTRACT", q1, q2, userId, (d1, d2) => CoreArithmetic(d1, d2, "SUBTRACT"));

        public async Task<QuantityMeasurementDto> DivideAsync(QuantityRequestDto q1, QuantityRequestDto q2, int? userId = null)
            => await RunBinaryAsync("DIVIDE", q1, q2, userId, (d1, d2) => CoreArithmetic(d1, d2, "DIVIDE"));

        public async Task<IReadOnlyList<QuantityMeasurementDto>> GetHistoryByOperationAsync(string op)
            => QuantityMeasurementDto.FromList(await RequirePersistence().GetByOperationAsync(op));

        public async Task<IReadOnlyList<QuantityMeasurementDto>> GetHistoryByCategoryAsync(string cat)
            => QuantityMeasurementDto.FromList(await RequirePersistence().GetByCategoryAsync(cat));

        public async Task<IReadOnlyList<QuantityMeasurementDto>> GetErrorHistoryAsync()
            => QuantityMeasurementDto.FromList(await RequirePersistence().GetErroredAsync());

        public async Task<int> GetCountByOperationAsync(string op)
            => await RequirePersistence().GetCountByOperationAsync(op);

        // ── Core logic (no persistence) ────────────────────────────────────

        private QuantityDTO CoreCompare(QuantityDTO q1, QuantityDTO q2)
        {
            bool equal = q1.Category.ToUpperInvariant() switch
            {
                "LENGTH" => ModelEquals(ToModel<LengthUnitM>(q1), ToModel<LengthUnitM>(q2)),
                "WEIGHT" => ModelEquals(ToModel<WeightUnitM>(q1), ToModel<WeightUnitM>(q2)),
                "VOLUME" => ModelEquals(ToModel<VolumeUnitM>(q1), ToModel<VolumeUnitM>(q2)),
                "TEMPERATURE" => ModelEquals(ToModel<TemperatureUnit>(q1), ToModel<TemperatureUnit>(q2)),
                _ => throw new QuantityMeasurementException($"Unknown category: {q1.Category}")
            };
            return new QuantityDTO(equal ? 1 : 0, equal ? "EQUAL" : "NOT_EQUAL", "RESULT");
        }

        private QuantityDTO CoreConvert(QuantityDTO q1, string targetUnit)
        {
            return q1.Category.ToUpperInvariant() switch
            {
                "LENGTH" => FromModel(ToModel<LengthUnitM>(q1).ConvertTo(ResolveLengthUnit(targetUnit)), "LENGTH"),
                "WEIGHT" => FromModel(ToModel<WeightUnitM>(q1).ConvertTo(ResolveWeightUnit(targetUnit)), "WEIGHT"),
                "VOLUME" => FromModel(ToModel<VolumeUnitM>(q1).ConvertTo(ResolveVolumeUnit(targetUnit)), "VOLUME"),
                "TEMPERATURE" => FromModel(ToModel<TemperatureUnit>(q1).ConvertTo(ResolveTempUnit(targetUnit)), "TEMPERATURE"),
                _ => throw new QuantityMeasurementException($"Unknown category: {q1.Category}")
            };
        }

        private QuantityDTO CoreArithmetic(QuantityDTO q1, QuantityDTO q2, string op)
        {
            return q1.Category.ToUpperInvariant() switch
            {
                "LENGTH" => ApplyLengthOp(q1, q2, op),
                "WEIGHT" => ApplyWeightOp(q1, q2, op),
                "VOLUME" => ApplyVolumeOp(q1, q2, op),
                "TEMPERATURE" => throw new NotSupportedException("Temperature arithmetic not supported."),
                _ => throw new QuantityMeasurementException($"Unknown category: {q1.Category}")
            };
        }

        private QuantityDTO ApplyLengthOp(QuantityDTO q1, QuantityDTO q2, string op)
        {
            var modelA = ToModel<LengthUnitM>(q1);
            var modelB = ToModel<LengthUnitM>(q2);
            var qa = ToQuantity(modelA);
            var qb = ToQuantity(modelB);
            return op switch
            {
                "ADD" => FromModel(ToModel(qa.Add(qb), modelA.Unit), "LENGTH"),
                "SUBTRACT" => FromModel(ToModel(qa.Subtract(qb), modelA.Unit), "LENGTH"),
                "DIVIDE" => new QuantityDTO(qa.Divide(qb), "RATIO", "SCALAR"),
                _ => throw new InvalidOperationException($"Unknown op: {op}")
            };
        }

        private QuantityDTO ApplyWeightOp(QuantityDTO q1, QuantityDTO q2, string op)
        {
            var modelA = ToModel<WeightUnitM>(q1);
            var modelB = ToModel<WeightUnitM>(q2);
            var qa = ToQuantity(modelA);
            var qb = ToQuantity(modelB);
            return op switch
            {
                "ADD" => FromModel(ToModel(qa.Add(qb), modelA.Unit), "WEIGHT"),
                "SUBTRACT" => FromModel(ToModel(qa.Subtract(qb), modelA.Unit), "WEIGHT"),
                "DIVIDE" => new QuantityDTO(qa.Divide(qb), "RATIO", "SCALAR"),
                _ => throw new InvalidOperationException($"Unknown op: {op}")
            };
        }

        private QuantityDTO ApplyVolumeOp(QuantityDTO q1, QuantityDTO q2, string op)
        {
            var modelA = ToModel<VolumeUnitM>(q1);
            var modelB = ToModel<VolumeUnitM>(q2);
            var qa = ToQuantity(modelA);
            var qb = ToQuantity(modelB);
            return op switch
            {
                "ADD" => FromModel(ToModel(qa.Add(qb), modelA.Unit), "VOLUME"),
                "SUBTRACT" => FromModel(ToModel(qa.Subtract(qb), modelA.Unit), "VOLUME"),
                "DIVIDE" => new QuantityDTO(qa.Divide(qb), "RATIO", "SCALAR"),
                _ => throw new InvalidOperationException($"Unknown op: {op}")
            };
        }

        private async Task<QuantityMeasurementDto> RunBinaryAsync(
            string opName, QuantityRequestDto q1, QuantityRequestDto q2, int? userId,
            Func<QuantityDTO, QuantityDTO, QuantityDTO> op)
        {
            if (!ValidCategories.Contains(q1.MeasurementType))
                throw new QuantityMeasurementException($"Invalid MeasurementType '{q1.MeasurementType}'. Must be LENGTH, WEIGHT, VOLUME, or TEMPERATURE.");
            if (!ValidCategories.Contains(q2.MeasurementType))
                throw new QuantityMeasurementException($"Invalid MeasurementType '{q2.MeasurementType}'. Must be LENGTH, WEIGHT, VOLUME, or TEMPERATURE.");

            var entity = MakeEntity(opName, q1, q2, userId);
            try
            {
                var res = op(ToDto(q1), ToDto(q2));
                entity.ResultValue = res.Value;
                entity.ResultUnit = res.UnitName;
                entity.ResultCategory = res.Category;
            }
            catch (Exception ex)
            {
                entity.HasError = true;
                entity.ErrorMessage = ex.Message;
                _logger.LogWarning(ex, "[QuantityMeasurementService] {Op} error", opName);
            }
            await RequirePersistence().SaveAsync(entity);
            return QuantityMeasurementDto.FromEntity(entity);
        }

        private static QuantityMeasurementEFEntity MakeEntity(string op, QuantityRequestDto q1, QuantityRequestDto? q2, int? userId) => new()
        {
            Operation = op,
            Operand1Value = q1.Value,
            Operand1Unit = q1.Unit.ToUpperInvariant(),
            Operand1Category = q1.MeasurementType.ToUpperInvariant(),
            Operand2Value = q2?.Value,
            Operand2Unit = q2?.Unit?.ToUpperInvariant(),
            Operand2Category = q2?.MeasurementType?.ToUpperInvariant(),
            Timestamp = DateTime.UtcNow,
            UserId = userId
        };

        private static QuantityDTO ToDto(QuantityRequestDto r) => new(r.Value, r.Unit, r.MeasurementType);

        private IQuantityMeasurementRepository RequirePersistence() =>
            _persistenceRepo ?? throw new InvalidOperationException("Persistence repository not configured (WebAPI only).");

        // ── QuantityModel bridge ───────────────────────────────────────────

        private QuantityMeasurementBusinessLayer.Units.QuantityModel<U> ToModel<U>(QuantityDTO dto) where U : class, IMeasurable
        {
            ValidateValue(dto.Value, "Measurement value");
            var unit = (U)ResolveUnit(dto.UnitName, dto.Category);
            return new QuantityMeasurementBusinessLayer.Units.QuantityModel<U>(dto.Value, unit);
        }

        private static Quantity<U> ToQuantity<U>(QuantityMeasurementBusinessLayer.Units.QuantityModel<U> model) where U : class, IMeasurable
            => new(model.Value, model.Unit);

        private static QuantityMeasurementBusinessLayer.Units.QuantityModel<U> ToModel<U>(Quantity<U> q, U unit) where U : class, IMeasurable
            => new(q.Value, unit);

        private static QuantityDTO FromModel<U>(QuantityMeasurementBusinessLayer.Units.QuantityModel<U> model, string category) where U : class, IMeasurable
            => new(model.Value, model.Unit.GetUnitName(), category);

        private static bool ModelEquals<U>(QuantityMeasurementBusinessLayer.Units.QuantityModel<U> a, QuantityMeasurementBusinessLayer.Units.QuantityModel<U> b) where U : class, IMeasurable
            => Math.Abs(a.ToBaseUnit() - b.ToBaseUnit()) < 0.01;

        private static IMeasurable ResolveUnit(string unitName, string category) => category.ToUpperInvariant() switch
        {
            "LENGTH" => ResolveLengthUnit(unitName),
            "WEIGHT" => ResolveWeightUnit(unitName),
            "VOLUME" => ResolveVolumeUnit(unitName),
            "TEMPERATURE" => ResolveTempUnit(unitName),
            _ => throw new QuantityMeasurementException($"Unknown category: {category}")
        };

        private static LengthUnitM ResolveLengthUnit(string name)
        {
            string n = name.Trim().ToUpperInvariant();
            return n switch
            {
                "FEET" or "FOOT" or "FT" or "FT." => LengthUnitM.FEET,
                "INCHES" or "INCH" or "IN" or "IN." => LengthUnitM.INCHES,
                "YARDS" or "YARD" or "YD" or "YD." or "YDS" => LengthUnitM.YARDS,
                "CENTIMETERS" or "CENTIMETER" or "CM" or "CM." => LengthUnitM.CENTIMETERS,
                _ => throw new QuantityMeasurementException($"Unknown length unit: '{name}'. Use: feet/ft, inches/in, yards/yd, centimeters/cm")
            };
        }

        private static WeightUnitM ResolveWeightUnit(string name)
        {
            string n = name.Trim().ToUpperInvariant();
            return n switch
            {
                "KILOGRAM" or "KILOGRAMS" or "KG" or "KG." => WeightUnitM.KILOGRAM,
                "GRAM" or "GRAMS" or "G" or "GR" => WeightUnitM.GRAM,
                "POUND" or "POUNDS" or "LB" or "LB." or "LBS" or "LBS." => WeightUnitM.POUND,
                _ => throw new QuantityMeasurementException($"Unknown weight unit: '{name}'. Use: kilogram/kg, gram/g, pound/lb")
            };
        }

        private static VolumeUnitM ResolveVolumeUnit(string name)
        {
            string n = name.Trim().ToUpperInvariant();
            return n switch
            {
                "LITRE" or "LITRES" or "LITER" or "LITERS" or "L" or "LT" or "LTR" => VolumeUnitM.LITRE,
                "MILLILITRE" or "MILLILITRES" or "MILLILITER" or "MILLILITERS" or "ML" or "ML." => VolumeUnitM.MILLILITRE,
                "GALLON" or "GALLONS" or "GAL" or "GAL." => VolumeUnitM.GALLON,
                _ => throw new QuantityMeasurementException($"Unknown volume unit: '{name}'. Use: litre/l, millilitre/ml, gallon/gal")
            };
        }

        private static TemperatureUnit ResolveTempUnit(string name)
        {
            string n = name.Trim().ToUpperInvariant();
            return n switch
            {
                "CELSIUS" or "C" or "CEL" => TemperatureUnit.CELSIUS,
                "FAHRENHEIT" or "F" or "FAH" or "FAHR" => TemperatureUnit.FAHRENHEIT,
                "KELVIN" or "K" or "KEL" => TemperatureUnit.KELVIN,
                _ => throw new QuantityMeasurementException($"Unknown temperature unit: '{name}'. Use: celsius/c, fahrenheit/f, kelvin/k")
            };
        }

        private static void ValidateValue(double value, string label = "Value")
        {
            if (value < 0) throw new QuantityMeasurementException($"{label} cannot be negative.");
            if (value > 1_000_000) throw new QuantityMeasurementException($"{label} is too large (max: 1,000,000).");
        }

        private static void ValidateNotNull(QuantityDTO? q1, QuantityDTO? q2)
        {
            if (q1 == null) throw new QuantityMeasurementException("First operand cannot be null.");
            if (q2 == null) throw new QuantityMeasurementException("Second operand cannot be null.");
        }

        private static void ValidateSameCategory(QuantityDTO q1, QuantityDTO q2, string operation)
        {
            if (!string.Equals(q1.Category, q2.Category, StringComparison.OrdinalIgnoreCase))
                throw new QuantityMeasurementException($"Cannot {operation} across different categories: {q1.Category} and {q2.Category}.");
        }
    }
}