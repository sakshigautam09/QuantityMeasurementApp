using System;

namespace QuantityMeasurementApp.Core.Entities
{
    public sealed class Length
    {
        public double Value { get; }
        public LengthUnit Unit { get; }

        public Length(double value, LengthUnit unit)
        {
            
            if (value <= 0)
                throw new ArgumentException("Invalid input: Value must be greater than zero.");

            Value = value;
            Unit = unit;
        }

        public double ToFeet()
        {
            switch (Unit)
            {
                case LengthUnit.Feet:
                    return Value;

                case LengthUnit.Inch:
                    return Value / 12.0;

                case LengthUnit.Yard:
                    return Value * 3.0;

                case LengthUnit.Centimeter:
                    return (Value * 0.393701) / 12.0;

                default:
                    throw new InvalidOperationException("Unsupported unit.");
            }
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is not Length other)
                return false;

            return Math.Abs(ToFeet() - other.ToFeet()) < 0.0001;
        }

        public override int GetHashCode()
        {
            return ToFeet().GetHashCode();
        }

        public override string ToString()
        {
            return $"{Value} {Unit}";
        }
    }
}