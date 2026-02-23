using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.Core.Interfaces
{
    public interface ILengthService
    {
        Length Create(double value, LengthUnit unit);
        bool AreEqual(Length first, Length second);
        double Convert(double value, LengthUnit source, LengthUnit target);
    }
}