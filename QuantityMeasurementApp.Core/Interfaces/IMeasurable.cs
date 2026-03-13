using System;

namespace QuantityMeasurementApp.Core.Interfaces
{
    [Flags]
    public enum ArithmeticOperation
    {
        None = 0,
        Addition = 1,
        Subtraction = 2,
        Division = 4
    }

    public interface IMeasurable
    {
        double GetConversionFactor();
        double ConvertToBaseUnit(double value);
        double ConvertFromBaseUnit(double baseValue);
        string GetUnitName();

        // Optional arithmetic support
        ArithmeticOperation SupportedOperations => ArithmeticOperation.Addition 
                                                  | ArithmeticOperation.Subtraction 
                                                  | ArithmeticOperation.Division;

        bool SupportsOperation(ArithmeticOperation operation) => 
            (SupportedOperations & operation) != 0;

        void ValidateOperationSupport(ArithmeticOperation operation)
        {
            if (!SupportsOperation(operation))
                throw new NotSupportedException($"{GetType().Name} does not support {operation} operation.");
        }
    }
}