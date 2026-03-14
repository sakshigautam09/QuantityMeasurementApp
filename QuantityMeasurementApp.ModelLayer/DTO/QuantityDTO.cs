// ============================================================
// PROJECT : QuantityMeasurementApp.ModelLayer
// FILE    : QuantityDTO.cs
//
// UC-15 : N-Tier Architecture
//
// Purpose : Data Transfer Object used as the public API contract
//           between the Controller layer and the Service layer.
//           Self-contained – carries its own unit enums so that
//           consumers (controller / REST / tests) never need to
//           reference QuantityMeasurementApp.Core directly.
//
// NOTE    : This file is PURELY ADDITIVE.
//           No existing Core / Console / Tests code is modified.
// ============================================================

using System;

namespace QuantityMeasurementApp.ModelLayer
{
    /// <summary>
    /// Data Transfer Object for one quantity value + unit.
    /// Used as input/output contract between Controller ↔ Service.
    /// </summary>
    public class QuantityDTO
    {
        // ── Measurement category ─────────────────────────────────────────────────────

        public enum MeasurementType { Length, Weight, Volume, Temperature }

        // ── Unit enums (mirror Core enums; self-contained in DTO layer) ──────────────

        public enum LengthUnit      { Feet, Inch, Yard, Centimeter }
        public enum WeightUnit      { Gram, Kilogram, Tonne }
        public enum VolumeUnit      { Litre, Millilitre, Gallon }
        public enum TemperatureUnit { Celsius, Fahrenheit, Kelvin }

        // ── Properties ───────────────────────────────────────────────────────────────

        public double          Value                { get; }
        public MeasurementType Type                 { get; }

        public LengthUnit?      LengthUnitValue      { get; }
        public WeightUnit?      WeightUnitValue      { get; }
        public VolumeUnit?      VolumeUnitValue      { get; }
        public TemperatureUnit? TemperatureUnitValue { get; }

        // ── Constructors (one per measurement category) ───────────────────────────────

        public QuantityDTO(double value, LengthUnit unit)
        {
            Value           = value;
            Type            = MeasurementType.Length;
            LengthUnitValue = unit;
        }

        public QuantityDTO(double value, WeightUnit unit)
        {
            Value           = value;
            Type            = MeasurementType.Weight;
            WeightUnitValue = unit;
        }

        public QuantityDTO(double value, VolumeUnit unit)
        {
            Value           = value;
            Type            = MeasurementType.Volume;
            VolumeUnitValue = unit;
        }

        public QuantityDTO(double value, TemperatureUnit unit)
        {
            Value                = value;
            Type                 = MeasurementType.Temperature;
            TemperatureUnitValue = unit;
        }

        // ── Helpers ──────────────────────────────────────────────────────────────────

        public string UnitLabel => Type switch
        {
            MeasurementType.Length      => LengthUnitValue?.ToString()      ?? "",
            MeasurementType.Weight      => WeightUnitValue?.ToString()      ?? "",
            MeasurementType.Volume      => VolumeUnitValue?.ToString()      ?? "",
            MeasurementType.Temperature => TemperatureUnitValue?.ToString() ?? "",
            _                           => ""
        };

        public override string ToString() => $"{Value} {UnitLabel}";
    }
}
