using System;
using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Interfaces;

namespace QuantityMeasurementApp.Core.Services
{
    public class MeasurementComparerService : IMeasurementComparer
    {
        private const double MAX_ALLOWED = 100000;

        // ====== Validation Common Logic ======
        private void Validate(double value)
        {
            
            if (value < 0)
                throw new ArgumentException("Invalid input: Negative value not allowed.");

            if (value == 0)
                throw new ArgumentException("Invalid input: Zero not allowed.");

            if (value > MAX_ALLOWED)
                throw new OverflowException("Value exceeds allowed maximum.");
        }

        public FeetMeasurement CreateFeet(double value)
        {
            Validate(value);
            return new FeetMeasurement(value);
        }

        public InchMeasurement CreateInch(double value)
        {
            Validate(value);
            return new InchMeasurement(value);
        }

        public bool AreFeetEqual(FeetMeasurement first, FeetMeasurement second)
        {
            if (first is null || second is null)
                throw new ArgumentException("Feet values cannot be null.");

            return first.Equals(second);
        }

        public bool AreInchesEqual(InchMeasurement first, InchMeasurement second)
        {
            if (first is null || second is null)
                throw new ArgumentException("Inch values cannot be null.");

            return first.Equals(second);
        }

        // 1 foot = 12 inches
        public bool AreFeetAndInchEqual(FeetMeasurement feet, InchMeasurement inch)
        {
            if (feet is null || inch is null)
                throw new ArgumentException("Values cannot be null.");

            double inchConvertedToFeet = inch.Value / 12.0;

            return Math.Abs(feet.Value - inchConvertedToFeet) < 0.0001;
        }
    }
}