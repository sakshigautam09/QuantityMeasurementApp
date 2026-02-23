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
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be finite.");

            Value = value;
            Unit = unit;
        }

        public double ConvertTo(LengthUnit targetUnit)
        {
            double valueInFeet = Value * Unit.ToFeetFactor();
            return valueInFeet / targetUnit.ToFeetFactor();
        }

        public static double Convert(double value,
                                     LengthUnit source,
                                     LengthUnit target)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be finite.");

            double valueInFeet = value * source.ToFeetFactor();
            return valueInFeet / target.ToFeetFactor();
        }

        public Length Add(Length other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            if (!double.IsFinite(other.Value))
                throw new ArgumentException("Invalid value.");

            double thisFeet = Value * Unit.ToFeetFactor();
            double otherFeet = other.Value * other.Unit.ToFeetFactor();

            double sumFeet = thisFeet + otherFeet;

            double resultValue = sumFeet / Unit.ToFeetFactor();

            return new Length(resultValue, Unit);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is not Length other)
                return false;

            double thisFeet = Value * Unit.ToFeetFactor();
            double otherFeet = other.Value * other.Unit.ToFeetFactor();

            return Math.Abs(thisFeet - otherFeet) < EPSILON;
        }

        public override int GetHashCode()
        {
            return (Value * Unit.ToFeetFactor()).GetHashCode();
        }

        public override string ToString()
        {
            return $"{Value} {Unit}";
        }
    }
}