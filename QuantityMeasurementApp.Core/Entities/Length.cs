using System;
using QuantityMeasurementApp.Core.Interfaces;

namespace QuantityMeasurementApp.Core.Entities
{
    public sealed class Length // Same as your current class
    {
        private const double EPSILON = 1e-6;

        public double Value { get; }
        public LengthUnit Unit { get; }

        public Length(double value, LengthUnit unit)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Invalid length value");

            Value = value;
            Unit = unit;
        }

        public double ConvertTo(LengthUnit targetUnit)
        {
            double baseValue = Unit.ConvertToBaseUnit(Value);
            return targetUnit.ConvertFromBaseUnit(baseValue);
        }

        public Length Add(Length other, LengthUnit? targetUnit = null)
        {
            targetUnit ??= Unit;

            double sumBase = Unit.ConvertToBaseUnit(Value) + other.Unit.ConvertToBaseUnit(other.Value);
            double resultValue = targetUnit.Value.ConvertFromBaseUnit(sumBase);

            return new Length(resultValue, targetUnit.Value);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Length other)
                return false;

            double thisBase = Unit.ConvertToBaseUnit(Value);
            double otherBase = other.Unit.ConvertToBaseUnit(other.Value);

            return Math.Abs(thisBase - otherBase) < EPSILON;
        }

        public override int GetHashCode() => Unit.ConvertToBaseUnit(Value).GetHashCode();

        public override string ToString() => $"{Value} {Unit}";
    }
}