using System;

namespace QuantityMeasurementApp.Core.Entities
{
    public enum WeightUnit
    {
        Gram,
        Kilogram,
        Tonne
    }

    public static class WeightUnitExtensions
    {
        public static double GetConversionFactor(this WeightUnit unit)
        {
            return unit switch
            {
                WeightUnit.Gram => 1.0,
                WeightUnit.Kilogram => 1000.0,
                WeightUnit.Tonne => 1_000_000.0,
                _ => throw new InvalidOperationException("Unsupported unit.")
            };
        }

        public static double ConvertToBaseUnit(this WeightUnit unit, double value) => value * unit.GetConversionFactor();
        public static double ConvertFromBaseUnit(this WeightUnit unit, double baseValue) => baseValue / unit.GetConversionFactor();
        public static string GetUnitName(this WeightUnit unit) => unit.ToString();
    }
}