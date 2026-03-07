using System;
using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Interfaces;

namespace QuantityMeasurementApp.Core.Services
{
    public class LengthComparerService : ILengthComparer
    {
        private const double MAX_ALLOWED = 100000;

        public Length Create(double value, LengthUnit unit)
        {
            if (value > MAX_ALLOWED)
                throw new OverflowException(
                    $"Invalid input: Value exceeds allowed maximum ({MAX_ALLOWED}).");

            return new Length(value, unit);
        }

        public bool AreEqual(Length first, Length second)
        {
            if (first is null || second is null)
                throw new ArgumentNullException("Length values cannot be null.");

            return first.Equals(second);
        }
    }
}