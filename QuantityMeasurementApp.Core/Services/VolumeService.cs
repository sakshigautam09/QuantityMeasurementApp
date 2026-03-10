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
            if (first == null || second == null)
                throw new ArgumentNullException();

            var q1 = new Quantity<VolumeUnit>(first.Value, first.Unit);
            var q2 = new Quantity<VolumeUnit>(second.Value, second.Unit);

            return q1.Equals(q2);
        }

        public double Convert(double value, VolumeUnit source, VolumeUnit target)
        {
            var q = new Quantity<VolumeUnit>(value, source);
            return q.ConvertTo(target).Value;
        }

        public Volume Add(Volume first, Volume second)
        {
            var q1 = new Quantity<VolumeUnit>(first.Value, first.Unit);
            var q2 = new Quantity<VolumeUnit>(second.Value, second.Unit);

            var sum = q1.Add(q2);
            return new Volume(sum.Value, sum.Unit);
        }

        public Volume Add(Volume first, Volume second, VolumeUnit targetUnit)
        {
            var q1 = new Quantity<VolumeUnit>(first.Value, first.Unit);
            var q2 = new Quantity<VolumeUnit>(second.Value, second.Unit);

            var sum = q1.Add(q2, targetUnit);
            return new Volume(sum.Value, sum.Unit);
        }

        public Volume Subtract(Volume first, Volume second)
        {
            var q1 = new Quantity<VolumeUnit>(first.Value, first.Unit);
            var q2 = new Quantity<VolumeUnit>(second.Value, second.Unit);

            var diff = q1.Subtract(q2);
            return new Volume(diff.Value, diff.Unit);
        }

        public Volume Subtract(Volume first, Volume second, VolumeUnit targetUnit)
        {
            var q1 = new Quantity<VolumeUnit>(first.Value, first.Unit);
            var q2 = new Quantity<VolumeUnit>(second.Value, second.Unit);

            var diff = q1.Subtract(q2, targetUnit);
            return new Volume(diff.Value, diff.Unit);
        }

        public double Divide(Volume first, Volume second)
        {
            var q1 = new Quantity<VolumeUnit>(first.Value, first.Unit);
            var q2 = new Quantity<VolumeUnit>(second.Value, second.Unit);

            return q1.Divide(q2);
        }
    }
}