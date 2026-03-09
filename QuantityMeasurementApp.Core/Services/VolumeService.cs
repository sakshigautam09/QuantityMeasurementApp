using System;
using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Interfaces;

namespace QuantityMeasurementApp.Core.Services
{
    public class VolumeService : IVolumeService
    {
        public Volume Create(double value, VolumeUnit unit)
        {
            return new Volume(value, unit);
        }

        public bool AreEqual(Volume first, Volume second)
        {
            if (first is null || second is null)
                throw new ArgumentNullException();

            return first.Equals(second);
        }

        public double Convert(double value, VolumeUnit source, VolumeUnit target)
        {
            return Volume.Convert(value, source, target);
        }

        public Volume Add(Volume first, Volume second)
        {
            if (first is null || second is null)
                throw new ArgumentNullException();

            return first.Add(second);
        }

        public Volume Add(Volume first, Volume second, VolumeUnit targetUnit)
        {
            if (first is null || second is null)
                throw new ArgumentNullException();

            return first.Add(second, targetUnit);
        }
    }
}