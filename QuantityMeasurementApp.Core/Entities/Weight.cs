using System;

namespace QuantityMeasurementApp.Core.Entities
{
    public sealed class Weight
    {
        private const double EPSILON = 1e-6;

        public double Value { get; }
        public WeightUnit Unit { get; }

        public Weight(double value, WeightUnit unit)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Invalid weight value");

            Value = value;
            Unit = unit;
        }

        public Weight ConvertTo(WeightUnit targetUnit)
        {
            double baseValue = Unit.ConvertToBaseUnit(Value);
            double converted = targetUnit.ConvertFromBaseUnit(baseValue);
            return new Weight(converted, targetUnit);
        }

        public Weight Add(Weight other, WeightUnit? targetUnit = null)
        {
            targetUnit ??= Unit;

            double sumBase =
                Unit.ConvertToBaseUnit(Value) +
                other.Unit.ConvertToBaseUnit(other.Value);

            double resultValue =
                targetUnit.Value.ConvertFromBaseUnit(sumBase);

            return new Weight(resultValue, targetUnit.Value);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Weight other)
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