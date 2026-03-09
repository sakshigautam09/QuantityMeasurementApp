using System;

namespace QuantityMeasurementApp.Core.Entities
{
    public sealed class Quantity<U>
    {
        private readonly double value;
        private readonly U unit;

        public Quantity(double value, U unit)
        {
            if (unit == null)
                throw new ArgumentException("Unit cannot be null");

            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be finite");

            this.value = value;
            this.unit = unit;
        }

        public double Value => value;
        public U Unit => unit;

        public Quantity<U> ConvertTo(U targetUnit)
        {
            double baseValue = ConvertToBaseUnit(unit, value);
            double converted = ConvertFromBaseUnit(targetUnit, baseValue);
            return new Quantity<U>(Math.Round(converted, 2), targetUnit);
        }

        public Quantity<U> Add(Quantity<U> other) => Add(other, unit);

        public Quantity<U> Add(Quantity<U> other, U targetUnit)
        {
            ValidateOperand(other);
            double sumBase = ConvertToBaseUnit(unit, value) + ConvertToBaseUnit(other.unit, other.value);
            double result = ConvertFromBaseUnit(targetUnit, sumBase);
            return new Quantity<U>(Math.Round(result, 2), targetUnit);
        }

        public Quantity<U> Subtract(Quantity<U> other) => Subtract(other, unit);

        public Quantity<U> Subtract(Quantity<U> other, U targetUnit)
        {
            ValidateOperand(other);
            double diffBase = ConvertToBaseUnit(unit, value) - ConvertToBaseUnit(other.unit, other.value);
            double result = ConvertFromBaseUnit(targetUnit, diffBase);
            return new Quantity<U>(Math.Round(result, 2), targetUnit);
        }

        public double Divide(Quantity<U> other)
        {
            ValidateOperand(other);
            double baseOther = ConvertToBaseUnit(other.unit, other.value);
            if (baseOther == 0.0) throw new DivideByZeroException();
            return ConvertToBaseUnit(unit, value) / baseOther;
        }

        private void ValidateOperand(Quantity<U> other)
        {
            if (other == null) throw new ArgumentException("Other quantity cannot be null");
            if (!double.IsFinite(other.value)) throw new ArgumentException("Other value must be finite");
        }

        private double ConvertToBaseUnit(U u, double v) => u switch
        {
            LengthUnit lu => lu.ConvertToBaseUnit(v),
            WeightUnit wu => wu.ConvertToBaseUnit(v),
            VolumeUnit vu => vu.ConvertToBaseUnit(v),
            _ => throw new InvalidOperationException("Unsupported unit type")
        };

        private double ConvertFromBaseUnit(U u, double v) => u switch
        {
            LengthUnit lu => lu.ConvertFromBaseUnit(v),
            WeightUnit wu => wu.ConvertFromBaseUnit(v),
            VolumeUnit vu => vu.ConvertFromBaseUnit(v),
            _ => throw new InvalidOperationException("Unsupported unit type")
        };
    }
}