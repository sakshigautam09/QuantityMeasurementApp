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

        // Static Convert method (required by tests)
        public static double Convert(double value, LengthUnit source, LengthUnit target)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Invalid length value");

            double baseValue = source.ConvertToBaseUnit(value);
            return target.ConvertFromBaseUnit(baseValue);
        }

        //ConvertTo returns Length object
        public Length ConvertTo(LengthUnit targetUnit)
        {
            double baseValue = Unit.ConvertToBaseUnit(Value);
            double converted = targetUnit.ConvertFromBaseUnit(baseValue);
            return new Length(converted, targetUnit);
        }

        public Length Add(Length other, LengthUnit? targetUnit = null)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            targetUnit ??= Unit;

            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit.Value))
                throw new ArgumentException("Invalid target unit.");

            double sumBase = Unit.ConvertToBaseUnit(Value) + 
                            other.Unit.ConvertToBaseUnit(other.Value);

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