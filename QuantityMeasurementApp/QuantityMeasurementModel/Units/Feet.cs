namespace QuantityMeasurementModel.Units
{
    /// <summary>
    /// Represents a measurement value in Feet.
    /// Value object used for equality comparison.
    /// </summary>
    public class Feet
    {
        private readonly double _value;

        public Feet(double value) => _value = value;

        public double GetValue() => _value;

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null || GetType() != obj.GetType()) return false;

            var other = (Feet)obj;
            return Math.Abs(_value - other._value) < 0.0001;
        }

        public override int GetHashCode() => _value.GetHashCode();

        public override string ToString() => $"{_value} ft";
    }
}
