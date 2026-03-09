using System;

namespace QuantityMeasurementApp.Core.Entities
{
    public sealed class InchMeasurement
    {
        public double Value { get; }

        public InchMeasurement(double value)
        {
            Value = value;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is not InchMeasurement other)
                return false;

            return Math.Abs(Value - other.Value) < 0.0001;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}