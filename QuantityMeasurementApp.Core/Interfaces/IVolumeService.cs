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
        Volume Subtract(Volume v1, Volume v2);
        Volume Subtract(Volume v1, Volume v2, VolumeUnit target);
        double Divide(Volume v1, Volume v2);
    }
}