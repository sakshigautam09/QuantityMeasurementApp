using System;
using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.ModelLayer
{
    /// <summary>
    /// Generic internal model class used within the Service layer.
    /// Wraps value + typed unit for arithmetic and conversion operations.
    /// U must be one of: LengthUnit, WeightUnit, VolumeUnit, TemperatureUnit.
    /// </summary>
    public class QuantityModel<U> where U : struct, Enum
    {
        // ─── Properties ─────────────────────────────────────────────────────────────

        public double Value { get; }
        public U      Unit  { get; }

        // ─── Constructor ─────────────────────────────────────────────────────────────

        public QuantityModel(double value, U unit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("QuantityModel value must be finite.", nameof(value));

            if (!Enum.IsDefined(typeof(U), unit))
                throw new ArgumentException($"Unknown unit '{unit}' for type {typeof(U).Name}.", nameof(unit));

            Value = value;
            Unit  = unit;
        }

        // ─── Conversion ──────────────────────────────────────────────────────────────

        public QuantityModel<U> ConvertTo(U targetUnit)
        {
            double baseValue  = ToBase(Unit,  Value);
            double converted  = FromBase(targetUnit, baseValue);
            return new QuantityModel<U>(Math.Round(converted, 6), targetUnit);
        }

        // ─── Arithmetic ──────────────────────────────────────────────────────────────

        public QuantityModel<U> Add(QuantityModel<U> other, U? targetUnit = null)
        {
            ValidateOther(other);
            U tu       = targetUnit ?? Unit;
            double sum = ToBase(Unit, Value) + ToBase(other.Unit, other.Value);
            return new QuantityModel<U>(Math.Round(FromBase(tu, sum), 6), tu);
        }

        public QuantityModel<U> Subtract(QuantityModel<U> other, U? targetUnit = null)
        {
            ValidateOther(other);
            U tu       = targetUnit ?? Unit;
            double diff = ToBase(Unit, Value) - ToBase(other.Unit, other.Value);
            return new QuantityModel<U>(Math.Round(FromBase(tu, diff), 6), tu);
        }

        public double Divide(QuantityModel<U> other)
        {
            ValidateOther(other);
            double denom = ToBase(other.Unit, other.Value);
            if (denom == 0.0)
                throw new DivideByZeroException("Cannot divide a quantity by zero.");
            return ToBase(Unit, Value) / denom;
        }

        // ─── Equality (base-unit comparison) ─────────────────────────────────────────

        public override bool Equals(object? obj)
        {
            if (obj is not QuantityModel<U> other) return false;
            return Math.Abs(ToBase(Unit, Value) - ToBase(other.Unit, other.Value)) < 1e-6;
        }

        public override int GetHashCode() => ToBase(Unit, Value).GetHashCode();

        public override string ToString() => $"{Value} {Unit}";

        // ─── Private Helpers ─────────────────────────────────────────────────────────

        private void ValidateOther(QuantityModel<U> other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));
        }

        internal static double ToBase(U unit, double value) => unit switch
        {
            LengthUnit      lu => lu.ConvertToBaseUnit(value),
            WeightUnit      wu => wu.ConvertToBaseUnit(value),
            VolumeUnit      vu => vu.ConvertToBaseUnit(value),
            TemperatureUnit tu => tu.ConvertToBaseUnit(value),
            _ => throw new InvalidOperationException($"Unsupported unit type: {typeof(U).Name}")
        };

        internal static double FromBase(U unit, double baseValue) => unit switch
        {
            LengthUnit      lu => lu.ConvertFromBaseUnit(baseValue),
            WeightUnit      wu => wu.ConvertFromBaseUnit(baseValue),
            VolumeUnit      vu => vu.ConvertFromBaseUnit(baseValue),
            TemperatureUnit tu => tu.ConvertFromBaseUnit(baseValue),
            _ => throw new InvalidOperationException($"Unsupported unit type: {typeof(U).Name}")
        };
    }
}
