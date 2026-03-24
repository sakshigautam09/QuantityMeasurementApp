using System;
using QuantityMeasurementApp.Core.Interfaces;

namespace QuantityMeasurementApp.Core.Entities
{
    public sealed class Temperature : IMeasurable
    {
        private const double EPSILON = 1e-6;

        public double Value { get; }
        public TemperatureUnit Unit { get; }

        public Temperature(double value, TemperatureUnit unit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Temperature value must be finite");

            Value = value;
            Unit = unit;
        }

        // Conversion helpers
        public double ConvertToBaseUnit(double value) => Unit.ConvertToBaseUnit(value);
        public double ConvertFromBaseUnit(double baseValue) => Unit.ConvertFromBaseUnit(baseValue);
        public string GetUnitName() => Unit.GetUnitName();
        public double GetConversionFactor() => 1.0; // Not used for temperature

        public Temperature ConvertTo(TemperatureUnit targetUnit)
        {
            double baseValue = ConvertToBaseUnit(Value);
            double converted = targetUnit.ConvertFromBaseUnit(baseValue);
            return new Temperature(Math.Round(converted, 2), targetUnit);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Temperature other)
                return false;

            double thisBase = ConvertToBaseUnit(Value);
            double otherBase = other.ConvertToBaseUnit(other.Value);

            return Math.Abs(thisBase - otherBase) < EPSILON;
        }

        public override int GetHashCode() => ConvertToBaseUnit(Value).GetHashCode();

        public override string ToString() => $"{Value} {Unit}";

        // Arithmetic operations: not supported
        public Temperature Add(Temperature other)
        {
            ValidateOperationSupport(ArithmeticOperation.Addition);
            return null!; // placeholder, will throw
        }

        public Temperature Subtract(Temperature other)
        {
            ValidateOperationSupport(ArithmeticOperation.Subtraction);
            return null!;
        }

        public double Divide(Temperature other)
        {
            ValidateOperationSupport(ArithmeticOperation.Division);
            return 0;
        }

        public void ValidateOperationSupport(ArithmeticOperation operation)
        {
            throw new NotSupportedException("Temperature does not support arithmetic operations on absolute values.");
        }

        public ArithmeticOperation SupportedOperations => ArithmeticOperation.None;
    }
}