using System;
using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Interfaces;

namespace QuantityMeasurementApp.Core.Services
{
    public class FeetComparerService : IFeetComparer
    {
        private const double MAX_ALLOWED = 100000;

        public FeetMeasurement Create(double value)
        {
            
            if (value < 0)
                throw new ArgumentException("Invalid input: Negative values are not permitted.");

            if (value == 0)
                throw new ArgumentException("Invalid input: Zero measurement is not allowed.");

            if (value > MAX_ALLOWED)
                throw new OverflowException(
                    $"Invalid input: Value exceeds allowed maximum ({MAX_ALLOWED} ft).");

            return new FeetMeasurement(value);
        }

        public bool AreEqual(FeetMeasurement first, FeetMeasurement second)
        {
            if (first is null || second is null)
                throw new ArgumentException("Comparison values cannot be null.");

            return first.Equals(second);
        }
    }
}