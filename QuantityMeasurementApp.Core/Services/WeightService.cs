using System;
using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Interfaces;

namespace QuantityMeasurementApp.Core.Services
{
    public class WeightService : IWeightService
    {
        public Weight Create(double value, WeightUnit unit)
        {
            return new Weight(value, unit);
        }

        public bool AreEqual(Weight first, Weight second)
        {
            if (first is null || second is null)
                throw new ArgumentNullException("Weight values cannot be null.");

            return first.Equals(second);
        }

        public double Convert(double value, WeightUnit source, WeightUnit target)
        {
            return Weight.Convert(value, source, target);
        }

        public Weight Add(Weight first, Weight second)
        {
            if (first is null || second is null)
                throw new ArgumentNullException();

            return first.Add(second);
        }

        public Weight Add(Weight first, Weight second, WeightUnit targetUnit)
        {
            if (first is null || second is null)
                throw new ArgumentNullException();

            return first.Add(second, targetUnit);
        }
    }
}