using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.Core.Interfaces
{
    public interface ILengthService
    {
        Length Create(double value, LengthUnit unit);
        bool AreEqual(Length first, Length second);
        double Convert(double value, LengthUnit source, LengthUnit target);
        Length Add(Length first, Length second);
        Length Add(Length first, Length second, LengthUnit targetUnit);

    // UC - 12
        Length Subtract(Length l1, Length l2);
        Length Subtract(Length l1, Length l2, LengthUnit target);
        double Divide(Length l1, Length l2);
    }
}