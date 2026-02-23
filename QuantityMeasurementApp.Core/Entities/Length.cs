using System;

namespace QuantityMeasurementApp.Core.Entities
{
    public sealed class Length
    {
        public double Value { get; }
        public LengthUnit Unit { get; }

        public Length(double value, LengthUnit unit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number.");

            if (value <= 0)
                throw new ArgumentException("Value must be greater than zero.");

            Value = value;
            Unit = unit;
        }

        // Instance Conversion
        public double ConvertTo(LengthUnit targetUnit)
        {
            double valueInFeet = Value * Unit.ToFeetFactor();
            return valueInFeet / targetUnit.ToFeetFactor();
        }

        // Static Conversion (UC5 explicit API)
        public static double Convert(double value,
                                     LengthUnit sourceUnit,
                                     LengthUnit targetUnit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be finite.");

            double valueInFeet = value * sourceUnit.ToFeetFactor();
            return valueInFeet / targetUnit.ToFeetFactor();
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is not Length other)
                return false;

            double thisFeet = Value * Unit.ToFeetFactor();
            double otherFeet = other.Value * other.Unit.ToFeetFactor();

            return Math.Abs(thisFeet - otherFeet) < 0.0001;
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