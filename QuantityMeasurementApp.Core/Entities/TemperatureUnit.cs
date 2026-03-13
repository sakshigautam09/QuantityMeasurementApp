using System;
using QuantityMeasurementApp.Core.Interfaces;

namespace QuantityMeasurementApp.Core.Entities
{
    public enum TemperatureUnit : int
    {
        Celsius,
        Fahrenheit,
        Kelvin
    }

    public static class TemperatureUnitExtensions
    {
        public static double ConvertToBaseUnit(this TemperatureUnit unit, double value) => unit switch
        {
            TemperatureUnit.Celsius => value,                       // Base unit
            TemperatureUnit.Fahrenheit => (value - 32) * 5 / 9,
            TemperatureUnit.Kelvin => value - 273.15,
            _ => throw new InvalidOperationException("Unsupported temperature unit")
        };

        public static double ConvertFromBaseUnit(this TemperatureUnit unit, double baseValue) => unit switch
        {
            TemperatureUnit.Celsius => baseValue,
            TemperatureUnit.Fahrenheit => (baseValue * 9 / 5) + 32,
            TemperatureUnit.Kelvin => baseValue + 273.15,
            _ => throw new InvalidOperationException("Unsupported temperature unit")
        };

        public static string GetUnitName(this TemperatureUnit unit) => unit.ToString();
    }
}