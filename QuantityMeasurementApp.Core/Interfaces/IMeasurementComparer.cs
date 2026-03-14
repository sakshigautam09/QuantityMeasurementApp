using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.Core.Interfaces
{
    public interface IMeasurementComparer
    {
        FeetMeasurement CreateFeet(double value);
        InchMeasurement CreateInch(double value);

        bool AreFeetEqual(FeetMeasurement first, FeetMeasurement second);
        bool AreInchesEqual(InchMeasurement first, InchMeasurement second);

        bool AreFeetAndInchEqual(FeetMeasurement feet, InchMeasurement inch);
    }
}