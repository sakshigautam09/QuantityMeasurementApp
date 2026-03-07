using System;

namespace QuantityMeasurementApp.Core.Entities
{
    public sealed class Length
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

        public static double Convert(double value, LengthUnit source, LengthUnit target)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be finite.");

            double baseValue = source.ConvertToBaseUnit(value);
            return target.ConvertFromBaseUnit(baseValue);
        }

        public Length Add(Length other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            if (!double.IsFinite(other.Value))
                throw new ArgumentException("Invalid value.");

            double thisBase = Unit.ConvertToBaseUnit(Value);
            double otherBase = other.Unit.ConvertToBaseUnit(other.Value);

            double sumBase = thisBase + otherBase;

            double resultValue = Unit.ConvertFromBaseUnit(sumBase);

            return new Length(resultValue, Unit);
        }

        // UC7 - Explicit Target Unit Add
        public Length Add(Length other, LengthUnit targetUnit)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            if (!double.IsFinite(other.Value))
                throw new ArgumentException("Invalid value.");

            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit))
                throw new ArgumentException("Invalid target unit.");

            double thisBase = Unit.ConvertToBaseUnit(Value);
            double otherBase = other.Unit.ConvertToBaseUnit(other.Value);

            double sumBase = thisBase + otherBase;

            double resultValue = targetUnit.ConvertFromBaseUnit(sumBase);

            return new Length(resultValue, targetUnit);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is not Length other)
                return false;

            double thisBase = Unit.ConvertToBaseUnit(Value);
            double otherBase = other.Unit.ConvertToBaseUnit(other.Value);

            return Math.Abs(thisBase - otherBase) < EPSILON;
        }

        public override int GetHashCode()
        {
            return Unit.ConvertToBaseUnit(Value).GetHashCode();
        }

        public override string ToString()
        {
            return $"{Value} {Unit}";
        }
    }
}