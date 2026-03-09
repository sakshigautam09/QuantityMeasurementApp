using System;

namespace QuantityMeasurementApp.Core.Entities
{
    public class Volume
    {
        private const double EPSILON = 0.0001;

        public double Value { get; }
        public VolumeUnit Unit { get; }

        public Volume(double value, VolumeUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        public static double Convert(double value, VolumeUnit source, VolumeUnit target)
        {
            double litres = source switch
            {
                VolumeUnit.Millilitre => value / 1000,
                VolumeUnit.Gallon => value * 3.78541,
                _ => value
            };

            return target switch
            {
                VolumeUnit.Millilitre => litres * 1000,
                VolumeUnit.Gallon => litres / 3.78541,
                _ => litres
            };
        }

        public Volume ConvertTo(VolumeUnit target)
        {
            double result = Convert(Value, Unit, target);
            return new Volume(result, target);
        }

        public Volume Add(Volume other)
        {
            double first = Convert(Value, Unit, VolumeUnit.Litre);
            double second = Convert(other.Value, other.Unit, VolumeUnit.Litre);

            double total = first + second;

            double result = Convert(total, VolumeUnit.Litre, Unit);

            return new Volume(result, Unit);
        }

        public Volume Add(Volume other, VolumeUnit targetUnit)
        {
            double first = Convert(Value, Unit, VolumeUnit.Litre);
            double second = Convert(other.Value, other.Unit, VolumeUnit.Litre);

            double total = first + second;

            double result = Convert(total, VolumeUnit.Litre, targetUnit);

            return new Volume(result, targetUnit);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Volume other)
                return false;

            double first = Convert(Value, Unit, VolumeUnit.Litre);
            double second = Convert(other.Value, other.Unit, VolumeUnit.Litre);

            return Math.Abs(first - second) < EPSILON;
        }

        public override int GetHashCode()
        {
            return Convert(Value, Unit, VolumeUnit.Litre).GetHashCode();
        }

        public override string ToString()
        {
            return $"{Value} {Unit}";
        }
    }
}