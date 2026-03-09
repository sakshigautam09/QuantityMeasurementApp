using System;

namespace QuantityMeasurementApp.Core.Entities
{
    public enum LengthUnit
    {
        Feet,
        Inch,
        Yard,
        Centimeter
    }

    public static class LengthUnitExtensions
    {
        public static double GetConversionFactor(this LengthUnit unit)
        {
            return unit switch
            {
                LengthUnit.Feet => 1.0,
                LengthUnit.Inch => 1.0 / 12.0,
                LengthUnit.Yard => 3.0,
                LengthUnit.Centimeter => 1.0 / 30.48,
                _ => throw new InvalidOperationException("Unsupported unit.")
            };
        }

        // Convert value of this unit → base unit (Feet)
        public static double ConvertToBaseUnit(this LengthUnit unit, double value)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be finite.");

            return value * unit.GetConversionFactor();
        }

        // Convert base unit (Feet) → this unit
        public static double ConvertFromBaseUnit(this LengthUnit unit, double baseValue)
        {
            if (!double.IsFinite(baseValue))
                throw new ArgumentException("Value must be finite.");

            return baseValue / unit.GetConversionFactor();
        }
    }
}