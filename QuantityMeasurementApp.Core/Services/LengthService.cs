using System;
using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Interfaces;

namespace QuantityMeasurementApp.Core.Services
{
    public class LengthService : ILengthService
    {
        public Length Create(double value, LengthUnit unit)
        {
            return new Length(value, unit);
        }

        public bool AreEqual(Length first, Length second)
        {
            if (first is null || second is null)
                throw new ArgumentNullException();

            return first.Equals(second);
        }

        public double Convert(double value, LengthUnit source, LengthUnit target)
        {
            return Length.Convert(value, source, target);
        }

        public Length Add(Length first, Length second)
        {
            if (first is null || second is null)
                throw new ArgumentNullException();

            return first.Add(second);
        }
        public Length Add(Length first, Length second, LengthUnit targetUnit)
        {
            if (first is null || second is null)
                throw new ArgumentNullException();

            return first.Add(second, targetUnit);
        }
    }
}