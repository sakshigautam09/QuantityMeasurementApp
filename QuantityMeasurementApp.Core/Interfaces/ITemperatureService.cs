using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.Core.Interfaces
{
    public interface ITemperatureService
    {
        Temperature Create(double value, TemperatureUnit unit);

        bool AreEqual(Temperature first, Temperature second);

        double Convert(double value, TemperatureUnit source, TemperatureUnit target);
    }
}