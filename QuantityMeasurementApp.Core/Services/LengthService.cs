using System;
using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Interfaces;

namespace QuantityMeasurementApp.Core.Services
{
    public class LengthService : ILengthService
    {
        private const double MAX_ALLOWED = 100000;

        public Length Create(double value, LengthUnit unit)
        {
            if (value > MAX_ALLOWED)
                throw new OverflowException("Value exceeds allowed maximum.");

            return new Length(value, unit);
        }

        public bool AreEqual(Length first, Length second)
        {
            if (first is null || second is null)
                throw new ArgumentNullException("Length cannot be null.");

            return first.Equals(second);
        }

        public double Convert(double value,
                              LengthUnit source,
                              LengthUnit target)
        {
            return Length.Convert(value, source, target);
        }
    }
}