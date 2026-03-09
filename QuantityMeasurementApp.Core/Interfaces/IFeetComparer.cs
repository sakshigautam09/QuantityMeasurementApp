using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.Core.Interfaces
{
    public interface IFeetComparer
    {
        FeetMeasurement Create(double value);
        bool AreEqual(FeetMeasurement first, FeetMeasurement second);
    }
}