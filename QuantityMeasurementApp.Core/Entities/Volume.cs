using System;

namespace QuantityMeasurementApp.Core.Entities
{
    public sealed class Volume
    {
        private const double EPSILON = 1e-6;

        public double Value { get; }
        public VolumeUnit Unit { get; }

        public Volume(double value, VolumeUnit unit)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Invalid volume value");

            Value = value;
            Unit = unit;
        }

        public Volume ConvertTo(VolumeUnit targetUnit)
        {
            double baseValue = Unit.ConvertToBaseUnit(Value);
            double converted = targetUnit.ConvertFromBaseUnit(baseValue);
            return new Volume(converted, targetUnit);
        }

        public Volume Add(Volume other, VolumeUnit? targetUnit = null)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            var unitToUse = targetUnit ?? Unit;

            double sumBase = Unit.ConvertToBaseUnit(Value) + other.Unit.ConvertToBaseUnit(other.Value);
            double resultValue = unitToUse.ConvertFromBaseUnit(sumBase);

            return new Volume(resultValue, unitToUse);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Volume other)
                return false;

            double thisBase = Unit.ConvertToBaseUnit(Value);
            double otherBase = other.Unit.ConvertToBaseUnit(other.Value);

            return Math.Abs(thisBase - otherBase) < EPSILON;
        }

        public override int GetHashCode() => Unit.ConvertToBaseUnit(Value).GetHashCode();

        public override string ToString() => $"{Value} {Unit}";
    }
}