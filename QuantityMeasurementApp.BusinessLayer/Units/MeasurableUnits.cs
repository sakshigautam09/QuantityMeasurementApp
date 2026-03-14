// ============================================================
// PROJECT : QuantityMeasurementApp.BusinessLayer
// FILE    : MeasurableUnits.cs
//
// Purpose : Concrete classes that implement IMeasurable.
//           Each class wraps one Core unit enum value and
//           delegates ConvertToBaseUnit / ConvertFromBaseUnit
//           to the existing Core extension methods.
//
//           This is why IMeasurable exists — so that ALL unit
//           dispatch goes through the interface, with zero
//           switch statements anywhere in the service layer.
//
// Usage   : var unit = new LengthMeasurableUnit(LengthUnit.Feet);
//           double base = unit.ConvertToBaseUnit(12.0);
// ============================================================

using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Interfaces;

namespace QuantityMeasurementApp.BusinessLayer
{
    // ── Length ────────────────────────────────────────────────────────────────────

    public class LengthMeasurableUnit : IMeasurable
    {
        private readonly LengthUnit _unit;

        public LengthMeasurableUnit(LengthUnit unit) => _unit = unit;

        public double GetConversionFactor()              => _unit.GetConversionFactor();
        public double ConvertToBaseUnit(double value)    => _unit.ConvertToBaseUnit(value);
        public double ConvertFromBaseUnit(double base_)  => _unit.ConvertFromBaseUnit(base_);
        public string GetUnitName()                      => _unit.GetUnitName();

        public LengthUnit Unit => _unit;

        public override string ToString() => _unit.ToString();
    }

    // ── Weight ────────────────────────────────────────────────────────────────────

    public class WeightMeasurableUnit : IMeasurable
    {
        private readonly WeightUnit _unit;

        public WeightMeasurableUnit(WeightUnit unit) => _unit = unit;

        public double GetConversionFactor()              => _unit.GetConversionFactor();
        public double ConvertToBaseUnit(double value)    => _unit.ConvertToBaseUnit(value);
        public double ConvertFromBaseUnit(double base_)  => _unit.ConvertFromBaseUnit(base_);
        public string GetUnitName()                      => _unit.GetUnitName();

        public WeightUnit Unit => _unit;

        public override string ToString() => _unit.ToString();
    }

    // ── Volume ────────────────────────────────────────────────────────────────────

    public class VolumeMeasurableUnit : IMeasurable
    {
        private readonly VolumeUnit _unit;

        public VolumeMeasurableUnit(VolumeUnit unit) => _unit = unit;

        public double GetConversionFactor()              => _unit.GetConversionFactor();
        public double ConvertToBaseUnit(double value)    => _unit.ConvertToBaseUnit(value);
        public double ConvertFromBaseUnit(double base_)  => _unit.ConvertFromBaseUnit(base_);
        public string GetUnitName()                      => _unit.GetUnitName();

        public VolumeUnit Unit => _unit;

        public override string ToString() => _unit.ToString();
    }

    // ── Temperature ───────────────────────────────────────────────────────────────

    public class TemperatureMeasurableUnit : IMeasurable
    {
        private readonly TemperatureUnit _unit;

        public TemperatureMeasurableUnit(TemperatureUnit unit) => _unit = unit;

        public double GetConversionFactor()              => 1.0; // not used for temperature
        public double ConvertToBaseUnit(double value)    => _unit.ConvertToBaseUnit(value);
        public double ConvertFromBaseUnit(double base_)  => _unit.ConvertFromBaseUnit(base_);
        public string GetUnitName()                      => _unit.GetUnitName();

        public TemperatureUnit Unit => _unit;

        public override string ToString() => _unit.ToString();
    }
}