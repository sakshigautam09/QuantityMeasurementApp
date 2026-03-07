using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.Core.Interfaces
{
    public interface IWeightService
    {
        Weight Create(double value, WeightUnit unit);

        bool AreEqual(Weight first, Weight second);

        double Convert(double value, WeightUnit source, WeightUnit target);

        Weight Add(Weight first, Weight second);

        Weight Add(Weight first, Weight second, WeightUnit targetUnit);
    }
}