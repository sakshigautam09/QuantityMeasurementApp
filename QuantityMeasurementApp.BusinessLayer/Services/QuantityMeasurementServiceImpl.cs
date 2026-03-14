// ============================================================
// PROJECT : QuantityMeasurementApp.BusinessLayer
// FILE    : QuantityMeasurementServiceImpl.cs
//
// Purpose : Implements IQuantityMeasurementService.
//           Converts QuantityDTO → QuantityModel (with IMeasurable unit)
//           → delegates to IQuantityModelService → converts back to DTO.
//
// The only switch statements here are for DTO mapping (reading the
// DTO enum to build the right MeasurableUnit wrapper).
// ALL arithmetic/conversion dispatch is through IMeasurable — no
// switch needed for the actual calculations.
// ============================================================

using System;
using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Interfaces;
using QuantityMeasurementApp.ModelLayer;
using QuantityMeasurementApp.RepositoryLayer;

namespace QuantityMeasurementApp.BusinessLayer
{
    public class QuantityMeasurementServiceImpl : IQuantityMeasurementService
    {
        private readonly IQuantityModelService          _modelSvc;
        private readonly ITemperatureService            _tempSvc;
        private readonly IQuantityMeasurementRepository _repo;

        public QuantityMeasurementServiceImpl(
            IQuantityModelService          modelService,
            ITemperatureService            temperatureService,
            IQuantityMeasurementRepository repository)
        {
            _modelSvc = modelService       ?? throw new ArgumentNullException(nameof(modelService));
            _tempSvc  = temperatureService ?? throw new ArgumentNullException(nameof(temperatureService));
            _repo     = repository         ?? throw new ArgumentNullException(nameof(repository));
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
                bool equal;

                if (first.Type == QuantityDTO.MeasurementType.Temperature)
                {
                    equal = _tempSvc.AreEqual(
                        _tempSvc.Create(first.Value,  ToTemperatureUnit(first.UnitLabel)),
                        _tempSvc.Create(second.Value, ToTemperatureUnit(second.UnitLabel)));
                }
                else
                {
                    // Pure IMeasurable dispatch — no switch needed for the comparison
                    equal = _modelSvc.AreEqual(ToModel(first), ToModel(second));
                }

                Persist(QuantityMeasurementEntity.OperationType.Compare, first, second, equal.ToString());
                return ToScalarDTO(equal ? 1.0 : 0.0, first);
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
                    // IMeasurable dispatch — no switch for the math
                    IMeasurable targetMeasurable = ToMeasurableUnit(targetUnit);
                    QuantityModel converted = _modelSvc.ConvertTo(ToModel(source), targetMeasurable);
                    result = FromModel(converted, targetUnit.Type);
                }

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

        public QuantityDTO AddWithTargetUnit(QuantityDTO first, QuantityDTO second, QuantityDTO targetUnit)
            => AddCore(first, second, targetUnit);

        private QuantityDTO AddCore(QuantityDTO first, QuantityDTO second, QuantityDTO? tu)
        {
            Validate(first, second);
            ValidateSameCategory(first, second, "add");
            ValidateArithmetic(first, "addition");

            try
            {
                QuantityModel result = tu is null
                    ? _modelSvc.Add(ToModel(first), ToModel(second))
                    : _modelSvc.AddWithTargetUnit(ToModel(first), ToModel(second), ToMeasurableUnit(tu));

                QuantityDTO dto = FromModel(result, first.Type);
                Persist(QuantityMeasurementEntity.OperationType.Add, first, second, dto.ToString(), tu);
                return dto;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (Exception ex) { throw Wrap("Add", first, second, ex); }
        }

        // ════════════════════════════════════════════════════════════════════════════
        // SUBTRACT
        // ════════════════════════════════════════════════════════════════════════════

        public QuantityDTO Subtract(QuantityDTO first, QuantityDTO second)
            => SubtractCore(first, second, null);

        public QuantityDTO SubtractWithTargetUnit(QuantityDTO first, QuantityDTO second, QuantityDTO targetUnit)
            => SubtractCore(first, second, targetUnit);

        private QuantityDTO SubtractCore(QuantityDTO first, QuantityDTO second, QuantityDTO? tu)
        {
            Validate(first, second);
            ValidateSameCategory(first, second, "subtract");
            ValidateArithmetic(first, "subtraction");

            try
            {
                QuantityModel result = tu is null
                    ? _modelSvc.Subtract(ToModel(first), ToModel(second))
                    : _modelSvc.SubtractWithTargetUnit(ToModel(first), ToModel(second), ToMeasurableUnit(tu));

                QuantityDTO dto = FromModel(result, first.Type);
                Persist(QuantityMeasurementEntity.OperationType.Subtract, first, second, dto.ToString(), tu);
                return dto;
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
                // Pure IMeasurable dispatch — no switch
                double ratio = _modelSvc.Divide(ToModel(first), ToModel(second));

                Persist(QuantityMeasurementEntity.OperationType.Divide, first, second, ratio.ToString("G6"));
                return ToScalarDTO(ratio, first);
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
        // PRIVATE – DTO → QuantityModel
        //
        // Switch here is for DTO MAPPING only — deciding which MeasurableUnit
        // wrapper to create. The math itself never switches on type.
        // ════════════════════════════════════════════════════════════════════════════

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

            _ => throw new QuantityMeasurementException($"Unsupported measurement type: {dto.Type}")
        };

        // ════════════════════════════════════════════════════════════════════════════
        // PRIVATE – QuantityModel → QuantityDTO
        // ════════════════════════════════════════════════════════════════════════════

        private static QuantityDTO FromModel(QuantityModel model, QuantityDTO.MeasurementType type)
        {
            string unitName = model.Unit.GetUnitName();
            return type switch
            {
                QuantityDTO.MeasurementType.Length =>
                    new QuantityDTO(model.Value, Enum.Parse<QuantityDTO.LengthUnit>(unitName, true)),

                QuantityDTO.MeasurementType.Weight =>
                    new QuantityDTO(model.Value, Enum.Parse<QuantityDTO.WeightUnit>(unitName, true)),

                QuantityDTO.MeasurementType.Volume =>
                    new QuantityDTO(model.Value, Enum.Parse<QuantityDTO.VolumeUnit>(unitName, true)),

                QuantityDTO.MeasurementType.Temperature =>
                    new QuantityDTO(model.Value, Enum.Parse<QuantityDTO.TemperatureUnit>(unitName, true)),

                _ => throw new QuantityMeasurementException($"Cannot convert model to DTO for type: {type}")
            };
        }

        private static QuantityDTO ToScalarDTO(double value, QuantityDTO source) => source.Type switch
        {
            QuantityDTO.MeasurementType.Length      => new(value, source.LengthUnitValue!.Value),
            QuantityDTO.MeasurementType.Weight      => new(value, source.WeightUnitValue!.Value),
            QuantityDTO.MeasurementType.Volume      => new(value, source.VolumeUnitValue!.Value),
            QuantityDTO.MeasurementType.Temperature => new(value, source.TemperatureUnitValue!.Value),
            _ => throw new QuantityMeasurementException("Unsupported type.")
        };

        private static TemperatureUnit ToTemperatureUnit(string label)
        {
            if (Enum.TryParse<TemperatureUnit>(label, true, out var result)) return result;
            throw new QuantityMeasurementException($"Unknown temperature unit: {label}");
        }

        // ════════════════════════════════════════════════════════════════════════════
        // PRIVATE – validation
        // ════════════════════════════════════════════════════════════════════════════

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

        private QuantityMeasurementException Wrap(string op, QuantityDTO first, QuantityDTO? second, Exception ex)
        {
            PersistError(Enum.Parse<QuantityMeasurementEntity.OperationType>(op, true), first, second, ex.Message);
            return new QuantityMeasurementException($"{op} failed: {ex.Message}", ex);
        }
    }
}