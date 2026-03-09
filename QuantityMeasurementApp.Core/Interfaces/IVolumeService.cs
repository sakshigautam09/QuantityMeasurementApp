using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.Core.Interfaces
{
    public interface IVolumeService
    {
        Volume Create(double value, VolumeUnit unit);

        bool AreEqual(Volume first, Volume second);

        double Convert(double value, VolumeUnit source, VolumeUnit target);

        Volume Add(Volume first, Volume second);

        Volume Add(Volume first, Volume second, VolumeUnit targetUnit);
    }
}