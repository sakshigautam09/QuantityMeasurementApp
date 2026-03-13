// ============================================================
// PROJECT : QuantityMeasurementApp.BusinessLayer
// FILE    : QuantityMeasurementServiceImpl.cs
//
// UC-15 : N-Tier Architecture
//
// Purpose : Implements IQuantityMeasurementService by DELEGATING
//           to the EXISTING Core services (ILengthService,
//           IWeightService, IVolumeService, ITemperatureService).
//
//           ALL UC1-UC14 business logic stays exactly where it was
//           (in QuantityMeasurementApp.Core.Services).  This class
//           is a thin orchestration layer that:
//             1. Accepts QuantityDTO input.
//             2. Maps DTO → Core entities/units.
//             3. Calls the appropriate existing Core service.
//             4. Maps result → QuantityDTO output.
//             5. Persists a QuantityMeasurementEntity to the repo.
//             6. Returns the QuantityDTO result.
//
// Design Patterns : Dependency Injection (constructor)
//
// NOTE : PURELY ADDITIVE.  The 4 existing Core services are
//        INJECTED, not modified.  No Core file is touched.
// ============================================================

using System;
using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Interfaces;
using QuantityMeasurementApp.ModelLayer;
using QuantityMeasurementApp.RepositoryLayer;

// Short aliases so DTO enums don't clash with Core enums
using DtoL = QuantityMeasurementApp.ModelLayer.QuantityDTO.LengthUnit;
using DtoW = QuantityMeasurementApp.ModelLayer.QuantityDTO.WeightUnit;
using DtoV = QuantityMeasurementApp.ModelLayer.QuantityDTO.VolumeUnit;
using DtoT = QuantityMeasurementApp.ModelLayer.QuantityDTO.TemperatureUnit;

namespace QuantityMeasurementApp.BusinessLayer
{
    public class QuantityMeasurementServiceImpl : IQuantityMeasurementService
    {
        // ── Injected existing Core services (UC1-UC14 logic lives here) ──────────────

        private readonly ILengthService      _lengthSvc;
        private readonly IWeightService      _weightSvc;
        private readonly IVolumeService      _volumeSvc;
        private readonly ITemperatureService _tempSvc;
        private readonly IQuantityMeasurementRepository _repo;

        // ── Constructor (Dependency Injection) ────────────────────────────────────────

        public QuantityMeasurementServiceImpl(
            ILengthService      lengthService,
            IWeightService      weightService,
            IVolumeService      volumeService,
            ITemperatureService temperatureService,
            IQuantityMeasurementRepository repository)
        {
            _lengthSvc = lengthService      ?? throw new ArgumentNullException(nameof(lengthService));
            _weightSvc = weightService      ?? throw new ArgumentNullException(nameof(weightService));
            _volumeSvc = volumeService      ?? throw new ArgumentNullException(nameof(volumeService));
            _tempSvc   = temperatureService ?? throw new ArgumentNullException(nameof(temperatureService));
            _repo      = repository         ?? throw new ArgumentNullException(nameof(repository));
        }

        // ════════════════════════════════════════════════════════════════════════════
        // COMPARE
        // ════════════════════════════════════════════════════════════════════════════

        public QuantityDTO Compare(QuantityDTO first, QuantityDTO second)
        {
            Validate(first, second);
            ValidateSameCategory(first, second, "compare");

            try
            {
                bool equal = first.Type switch
                {
                    // ── delegates straight to existing Core services ──────────────────
                    QuantityDTO.MeasurementType.Length =>
                        _lengthSvc.AreEqual(
                            _lengthSvc.Create(first.Value,  MapCoreL(first.LengthUnitValue!.Value)),
                            _lengthSvc.Create(second.Value, MapCoreL(second.LengthUnitValue!.Value))),

                    QuantityDTO.MeasurementType.Weight =>
                        _weightSvc.AreEqual(
                            _weightSvc.Create(first.Value,  MapCoreW(first.WeightUnitValue!.Value)),
                            _weightSvc.Create(second.Value, MapCoreW(second.WeightUnitValue!.Value))),

                    QuantityDTO.MeasurementType.Volume =>
                        _volumeSvc.AreEqual(
                            _volumeSvc.Create(first.Value,  MapCoreV(first.VolumeUnitValue!.Value)),
                            _volumeSvc.Create(second.Value, MapCoreV(second.VolumeUnitValue!.Value))),

                    QuantityDTO.MeasurementType.Temperature =>
                        _tempSvc.AreEqual(
                            _tempSvc.Create(first.Value,  MapCoreT(first.TemperatureUnitValue!.Value)),
                            _tempSvc.Create(second.Value, MapCoreT(second.TemperatureUnitValue!.Value))),

                    _ => throw new QuantityMeasurementException("Unsupported measurement type.")
                };

                Persist(QuantityMeasurementEntity.OperationType.Compare,
                        first, second, equal.ToString());

                return ToResultDTO(equal ? 1.0 : 0.0, first);
            }
            catch (QuantityMeasurementException) { throw; }
            catch (Exception ex) { throw Wrap("Compare", first, second, ex); }
        }

        // ════════════════════════════════════════════════════════════════════════════
        // CONVERT
        // ════════════════════════════════════════════════════════════════════════════

        public QuantityDTO Convert(QuantityDTO source, QuantityDTO targetUnit)
        {
            Validate(source, targetUnit);
            ValidateSameCategory(source, targetUnit, "convert");

            try
            {
                QuantityDTO result = source.Type switch
                {
                    QuantityDTO.MeasurementType.Length =>
                        LengthDTO(
                            _lengthSvc.Convert(
                                source.Value,
                                MapCoreL(source.LengthUnitValue!.Value),
                                MapCoreL(targetUnit.LengthUnitValue!.Value)),
                            targetUnit.LengthUnitValue!.Value),

                    QuantityDTO.MeasurementType.Weight =>
                        WeightDTO(
                            _weightSvc.Convert(
                                source.Value,
                                MapCoreW(source.WeightUnitValue!.Value),
                                MapCoreW(targetUnit.WeightUnitValue!.Value)),
                            targetUnit.WeightUnitValue!.Value),

                    QuantityDTO.MeasurementType.Volume =>
                        VolumeDTO(
                            _volumeSvc.Convert(
                                source.Value,
                                MapCoreV(source.VolumeUnitValue!.Value),
                                MapCoreV(targetUnit.VolumeUnitValue!.Value)),
                            targetUnit.VolumeUnitValue!.Value),

                    QuantityDTO.MeasurementType.Temperature =>
                        TempDTO(
                            _tempSvc.Convert(
                                source.Value,
                                MapCoreT(source.TemperatureUnitValue!.Value),
                                MapCoreT(targetUnit.TemperatureUnitValue!.Value)),
                            targetUnit.TemperatureUnitValue!.Value),

                    _ => throw new QuantityMeasurementException("Unsupported measurement type.")
                };

                // Single-operand entity (Convert)
                _repo.Save(new QuantityMeasurementEntity(
                    QuantityMeasurementEntity.OperationType.Convert,
                    source, targetUnit, result.ToString()));

                return result;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (Exception ex) { throw Wrap("Convert", source, targetUnit, ex); }
        }

        // ════════════════════════════════════════════════════════════════════════════
        // ADD
        // ════════════════════════════════════════════════════════════════════════════

        public QuantityDTO Add(QuantityDTO first, QuantityDTO second)
            => AddCore(first, second, null);

        public QuantityDTO Add(QuantityDTO first, QuantityDTO second, QuantityDTO targetUnit)
            => AddCore(first, second, targetUnit);

        private QuantityDTO AddCore(QuantityDTO first, QuantityDTO second, QuantityDTO? tu)
        {
            Validate(first, second);
            ValidateSameCategory(first, second, "add");
            ValidateArithmetic(first, "addition");

            try
            {
                QuantityDTO result = first.Type switch
                {
                    QuantityDTO.MeasurementType.Length => tu is null
                        ? LengthResult(_lengthSvc.Add(
                            _lengthSvc.Create(first.Value,  MapCoreL(first.LengthUnitValue!.Value)),
                            _lengthSvc.Create(second.Value, MapCoreL(second.LengthUnitValue!.Value))))
                        : LengthResult(_lengthSvc.Add(
                            _lengthSvc.Create(first.Value,  MapCoreL(first.LengthUnitValue!.Value)),
                            _lengthSvc.Create(second.Value, MapCoreL(second.LengthUnitValue!.Value)),
                            MapCoreL(tu.LengthUnitValue!.Value))),

                    QuantityDTO.MeasurementType.Weight => tu is null
                        ? WeightResult(_weightSvc.Add(
                            _weightSvc.Create(first.Value,  MapCoreW(first.WeightUnitValue!.Value)),
                            _weightSvc.Create(second.Value, MapCoreW(second.WeightUnitValue!.Value))))
                        : WeightResult(_weightSvc.Add(
                            _weightSvc.Create(first.Value,  MapCoreW(first.WeightUnitValue!.Value)),
                            _weightSvc.Create(second.Value, MapCoreW(second.WeightUnitValue!.Value)),
                            MapCoreW(tu.WeightUnitValue!.Value))),

                    QuantityDTO.MeasurementType.Volume => tu is null
                        ? VolumeResult(_volumeSvc.Add(
                            _volumeSvc.Create(first.Value,  MapCoreV(first.VolumeUnitValue!.Value)),
                            _volumeSvc.Create(second.Value, MapCoreV(second.VolumeUnitValue!.Value))))
                        : VolumeResult(_volumeSvc.Add(
                            _volumeSvc.Create(first.Value,  MapCoreV(first.VolumeUnitValue!.Value)),
                            _volumeSvc.Create(second.Value, MapCoreV(second.VolumeUnitValue!.Value)),
                            MapCoreV(tu.VolumeUnitValue!.Value))),

                    _ => throw new QuantityMeasurementException("Unsupported type for addition.")
                };

                Persist(QuantityMeasurementEntity.OperationType.Add, first, second,
                        result.ToString(), tu);
                return result;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (Exception ex) { throw Wrap("Add", first, second, ex); }
        }

        // ════════════════════════════════════════════════════════════════════════════
        // SUBTRACT
        // ════════════════════════════════════════════════════════════════════════════

        public QuantityDTO Subtract(QuantityDTO first, QuantityDTO second)
            => SubtractCore(first, second, null);

        public QuantityDTO Subtract(QuantityDTO first, QuantityDTO second, QuantityDTO targetUnit)
            => SubtractCore(first, second, targetUnit);

        private QuantityDTO SubtractCore(QuantityDTO first, QuantityDTO second, QuantityDTO? tu)
        {
            Validate(first, second);
            ValidateSameCategory(first, second, "subtract");
            ValidateArithmetic(first, "subtraction");

            try
            {
                QuantityDTO result = first.Type switch
                {
                    QuantityDTO.MeasurementType.Length => tu is null
                        ? LengthResult(_lengthSvc.Subtract(
                            _lengthSvc.Create(first.Value,  MapCoreL(first.LengthUnitValue!.Value)),
                            _lengthSvc.Create(second.Value, MapCoreL(second.LengthUnitValue!.Value))))
                        : LengthResult(_lengthSvc.Subtract(
                            _lengthSvc.Create(first.Value,  MapCoreL(first.LengthUnitValue!.Value)),
                            _lengthSvc.Create(second.Value, MapCoreL(second.LengthUnitValue!.Value)),
                            MapCoreL(tu.LengthUnitValue!.Value))),

                    QuantityDTO.MeasurementType.Weight => tu is null
                        ? WeightResult(_weightSvc.Subtract(
                            _weightSvc.Create(first.Value,  MapCoreW(first.WeightUnitValue!.Value)),
                            _weightSvc.Create(second.Value, MapCoreW(second.WeightUnitValue!.Value))))
                        : WeightResult(_weightSvc.Subtract(
                            _weightSvc.Create(first.Value,  MapCoreW(first.WeightUnitValue!.Value)),
                            _weightSvc.Create(second.Value, MapCoreW(second.WeightUnitValue!.Value)),
                            MapCoreW(tu.WeightUnitValue!.Value))),

                    QuantityDTO.MeasurementType.Volume => tu is null
                        ? VolumeResult(_volumeSvc.Subtract(
                            _volumeSvc.Create(first.Value,  MapCoreV(first.VolumeUnitValue!.Value)),
                            _volumeSvc.Create(second.Value, MapCoreV(second.VolumeUnitValue!.Value))))
                        : VolumeResult(_volumeSvc.Subtract(
                            _volumeSvc.Create(first.Value,  MapCoreV(first.VolumeUnitValue!.Value)),
                            _volumeSvc.Create(second.Value, MapCoreV(second.VolumeUnitValue!.Value)),
                            MapCoreV(tu.VolumeUnitValue!.Value))),

                    _ => throw new QuantityMeasurementException("Unsupported type for subtraction.")
                };

                Persist(QuantityMeasurementEntity.OperationType.Subtract, first, second,
                        result.ToString(), tu);
                return result;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (Exception ex) { throw Wrap("Subtract", first, second, ex); }
        }

        // ════════════════════════════════════════════════════════════════════════════
        // DIVIDE
        // ════════════════════════════════════════════════════════════════════════════

        public QuantityDTO Divide(QuantityDTO first, QuantityDTO second)
        {
            Validate(first, second);
            ValidateSameCategory(first, second, "divide");
            ValidateArithmetic(first, "division");

            try
            {
                double ratio = first.Type switch
                {
                    QuantityDTO.MeasurementType.Length =>
                        _lengthSvc.Divide(
                            _lengthSvc.Create(first.Value,  MapCoreL(first.LengthUnitValue!.Value)),
                            _lengthSvc.Create(second.Value, MapCoreL(second.LengthUnitValue!.Value))),

                    QuantityDTO.MeasurementType.Weight =>
                        _weightSvc.Divide(
                            _weightSvc.Create(first.Value,  MapCoreW(first.WeightUnitValue!.Value)),
                            _weightSvc.Create(second.Value, MapCoreW(second.WeightUnitValue!.Value))),

                    QuantityDTO.MeasurementType.Volume =>
                        _volumeSvc.Divide(
                            _volumeSvc.Create(first.Value,  MapCoreV(first.VolumeUnitValue!.Value)),
                            _volumeSvc.Create(second.Value, MapCoreV(second.VolumeUnitValue!.Value))),

                    _ => throw new QuantityMeasurementException("Unsupported type for division.")
                };

                Persist(QuantityMeasurementEntity.OperationType.Divide, first, second,
                        ratio.ToString("G6"));
                return ToResultDTO(ratio, first);
            }
            catch (QuantityMeasurementException) { throw; }
            catch (DivideByZeroException ex)
            {
                PersistError(QuantityMeasurementEntity.OperationType.Divide, first, second, ex.Message);
                throw new QuantityMeasurementException("Division by zero.", ex);
            }
            catch (Exception ex) { throw Wrap("Divide", first, second, ex); }
        }

        // ════════════════════════════════════════════════════════════════════════════
        // PRIVATE – DTO unit  →  Core unit mappings
        // ════════════════════════════════════════════════════════════════════════════

        private static LengthUnit MapCoreL(DtoL u) => u switch
        {
            DtoL.Feet       => LengthUnit.Feet,
            DtoL.Inch       => LengthUnit.Inch,
            DtoL.Yard       => LengthUnit.Yard,
            DtoL.Centimeter => LengthUnit.Centimeter,
            _ => throw new QuantityMeasurementException($"Unknown LengthUnit: {u}")
        };

        private static WeightUnit MapCoreW(DtoW u) => u switch
        {
            DtoW.Gram     => WeightUnit.Gram,
            DtoW.Kilogram => WeightUnit.Kilogram,
            DtoW.Tonne    => WeightUnit.Tonne,
            _ => throw new QuantityMeasurementException($"Unknown WeightUnit: {u}")
        };

        private static VolumeUnit MapCoreV(DtoV u) => u switch
        {
            DtoV.Litre      => VolumeUnit.Litre,
            DtoV.Millilitre => VolumeUnit.Millilitre,
            DtoV.Gallon     => VolumeUnit.Gallon,
            _ => throw new QuantityMeasurementException($"Unknown VolumeUnit: {u}")
        };

        private static TemperatureUnit MapCoreT(DtoT u) => u switch
        {
            DtoT.Celsius    => TemperatureUnit.Celsius,
            DtoT.Fahrenheit => TemperatureUnit.Fahrenheit,
            DtoT.Kelvin     => TemperatureUnit.Kelvin,
            _ => throw new QuantityMeasurementException($"Unknown TemperatureUnit: {u}")
        };

        // ════════════════════════════════════════════════════════════════════════════
        // PRIVATE – Core entity  →  QuantityDTO
        // ════════════════════════════════════════════════════════════════════════════

        // Length
        private static QuantityDTO LengthResult(Length l)   => LengthDTO(l.Value, MapDtoL(l.Unit));
        private static QuantityDTO LengthDTO(double v, DtoL u) => new(v, u);

        private static DtoL MapDtoL(LengthUnit u) => u switch
        {
            LengthUnit.Feet       => DtoL.Feet,
            LengthUnit.Inch       => DtoL.Inch,
            LengthUnit.Yard       => DtoL.Yard,
            LengthUnit.Centimeter => DtoL.Centimeter,
            _ => throw new QuantityMeasurementException($"Unknown LengthUnit: {u}")
        };

        // Weight
        private static QuantityDTO WeightResult(Weight w)   => WeightDTO(w.Value, MapDtoW(w.Unit));
        private static QuantityDTO WeightDTO(double v, DtoW u) => new(v, u);

        private static DtoW MapDtoW(WeightUnit u) => u switch
        {
            WeightUnit.Gram     => DtoW.Gram,
            WeightUnit.Kilogram => DtoW.Kilogram,
            WeightUnit.Tonne    => DtoW.Tonne,
            _ => throw new QuantityMeasurementException($"Unknown WeightUnit: {u}")
        };

        // Volume
        private static QuantityDTO VolumeResult(Volume v)     => VolumeDTO(v.Value, MapDtoV(v.Unit));
        private static QuantityDTO VolumeDTO(double v, DtoV u) => new(v, u);

        private static DtoV MapDtoV(VolumeUnit u) => u switch
        {
            VolumeUnit.Litre      => DtoV.Litre,
            VolumeUnit.Millilitre => DtoV.Millilitre,
            VolumeUnit.Gallon     => DtoV.Gallon,
            _ => throw new QuantityMeasurementException($"Unknown VolumeUnit: {u}")
        };

        // Temperature
        private static QuantityDTO TempDTO(double v, DtoT u) => new(v, u);

        // Generic scalar result (Compare / Divide output)
        private static QuantityDTO ToResultDTO(double value, QuantityDTO source) => source.Type switch
        {
            QuantityDTO.MeasurementType.Length      => new(value, source.LengthUnitValue!.Value),
            QuantityDTO.MeasurementType.Weight      => new(value, source.WeightUnitValue!.Value),
            QuantityDTO.MeasurementType.Volume      => new(value, source.VolumeUnitValue!.Value),
            QuantityDTO.MeasurementType.Temperature => new(value, source.TemperatureUnitValue!.Value),
            _ => throw new QuantityMeasurementException("Unsupported type.")
        };

        // ════════════════════════════════════════════════════════════════════════════
        // PRIVATE – validation helpers
        // ════════════════════════════════════════════════════════════════════════════

        private static void Validate(QuantityDTO a, QuantityDTO b)
        {
            if (a is null) throw new QuantityMeasurementException("First operand is null.");
            if (b is null) throw new QuantityMeasurementException("Second operand is null.");
        }

        private static void ValidateSameCategory(QuantityDTO a, QuantityDTO b, string op)
        {
            if (a.Type != b.Type)
                throw new QuantityMeasurementException(
                    $"Cannot {op} quantities of different categories: {a.Type} vs {b.Type}.");
        }

        private static void ValidateArithmetic(QuantityDTO dto, string op)
        {
            if (dto.Type == QuantityDTO.MeasurementType.Temperature)
                throw new QuantityMeasurementException(
                    $"Temperature does not support {op} on absolute values.");
        }

        // ════════════════════════════════════════════════════════════════════════════
        // PRIVATE – repository helpers
        // ════════════════════════════════════════════════════════════════════════════

        private void Persist(
            QuantityMeasurementEntity.OperationType op,
            QuantityDTO first, QuantityDTO second, string result,
            QuantityDTO? tu = null)
        {
            try { _repo.Save(new QuantityMeasurementEntity(op, first, second, result, tu)); }
            catch { /* non-fatal */ }
        }

        private void PersistError(
            QuantityMeasurementEntity.OperationType op,
            QuantityDTO first, QuantityDTO? second, string msg)
        {
            try { _repo.Save(new QuantityMeasurementEntity(op, first, second, msg, true)); }
            catch { /* non-fatal */ }
        }

        private QuantityMeasurementException Wrap(
            string op, QuantityDTO first, QuantityDTO? second, Exception ex)
        {
            PersistError(
                Enum.Parse<QuantityMeasurementEntity.OperationType>(op, true),
                first, second, ex.Message);
            return new QuantityMeasurementException($"{op} failed: {ex.Message}", ex);
        }
    }
}
