using System;

namespace QuantityMeasurementApp.Core.Entities
{
    public class Weight
    {
        public double Value { get; }
        public WeightUnit Unit { get; }

        public Weight(double value, WeightUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        public static double Convert(double value, WeightUnit source, WeightUnit target)
        {
            double grams = source switch
            {
                WeightUnit.Kilogram => value * 1000,
                WeightUnit.Pound => value * 453.592,
                _ => value
            };

            return target switch
            {
                WeightUnit.Kilogram => grams / 1000,
                WeightUnit.Pound => grams / 453.592,
                _ => grams
            };
        }

        public Weight ConvertTo(WeightUnit targetUnit)
        {
            double converted = Convert(Value, Unit, targetUnit);
            return new Weight(converted, targetUnit);
        }

        public Weight Add(Weight other)
        {
            double first = Convert(Value, Unit, WeightUnit.Gram);
            double second = Convert(other.Value, other.Unit, WeightUnit.Gram);

            double totalGrams = first + second;

            // convert back to original unit
            double result = Convert(totalGrams, WeightUnit.Gram, Unit);

            return new Weight(result, Unit);
        }

        public Weight Add(Weight other, WeightUnit targetUnit)
        {
            double first = Convert(Value, Unit, WeightUnit.Gram);
            double second = Convert(other.Value, other.Unit, WeightUnit.Gram);

            double totalGrams = first + second;

            double result = Convert(totalGrams, WeightUnit.Gram, targetUnit);

            return new Weight(result, targetUnit);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Weight other)
                return false;

            double first = Convert(Value, Unit, WeightUnit.Gram);
            double second = Convert(other.Value, other.Unit, WeightUnit.Gram);

            return Math.Abs(first - second) < 0.01;
        }

        public override int GetHashCode()
        {
            return Convert(Value, Unit, WeightUnit.Gram).GetHashCode();
        }

        public override string ToString()
        {
            return $"{Value} {Unit}";
        }
    }
}