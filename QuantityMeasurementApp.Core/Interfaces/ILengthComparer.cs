using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.Core.Interfaces
{
    public interface ILengthComparer
    {
        Length Create(double value, LengthUnit unit);
        bool AreEqual(Length first, Length second);
    }
}