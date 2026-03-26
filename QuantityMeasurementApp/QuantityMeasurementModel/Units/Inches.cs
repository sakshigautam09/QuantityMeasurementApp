namespace QuantityMeasurementModel.Units
{
    /// <summary>
    /// Represents a measurement value in Inches.
    /// Value object used for equality comparison.
    /// </summary>
    public class Inches
    {
        private readonly double _value;

        public Inches(double value) => _value = value;

        public double GetValue() => _value;

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null || GetType() != obj.GetType()) return false;

            var other = (Inches)obj;
            return Math.Abs(_value - other._value) < 0.0001;
        }

        public override int GetHashCode() => _value.GetHashCode();

        public override string ToString() => $"{_value} in";
    }
}
