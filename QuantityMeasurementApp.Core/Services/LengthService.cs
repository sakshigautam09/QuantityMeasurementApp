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
            if (first == null || second == null)
                throw new ArgumentNullException();

            var q1 = new Quantity<LengthUnit>(first.Value, first.Unit);
            var q2 = new Quantity<LengthUnit>(second.Value, second.Unit);

            return q1.Equals(q2);
        }

        public double Convert(double value, LengthUnit source, LengthUnit target)
        {
            var q = new Quantity<LengthUnit>(value, source);
            return q.ConvertTo(target).Value;
        }

        public Length Add(Length first, Length second)
        {
            var q1 = new Quantity<LengthUnit>(first.Value, first.Unit);
            var q2 = new Quantity<LengthUnit>(second.Value, second.Unit);

            var sum = q1.Add(q2);
            return new Length(sum.Value, sum.Unit);
        }

        public Length Add(Length first, Length second, LengthUnit targetUnit)
        {
            var q1 = new Quantity<LengthUnit>(first.Value, first.Unit);
            var q2 = new Quantity<LengthUnit>(second.Value, second.Unit);

            var sum = q1.Add(q2, targetUnit);
            return new Length(sum.Value, sum.Unit);
        }

        public Length Subtract(Length first, Length second)
        {
            var q1 = new Quantity<LengthUnit>(first.Value, first.Unit);
            var q2 = new Quantity<LengthUnit>(second.Value, second.Unit);

            var diff = q1.Subtract(q2);
            return new Length(diff.Value, diff.Unit);
        }

        public Length Subtract(Length first, Length second, LengthUnit targetUnit)
        {
            var q1 = new Quantity<LengthUnit>(first.Value, first.Unit);
            var q2 = new Quantity<LengthUnit>(second.Value, second.Unit);

            var diff = q1.Subtract(q2, targetUnit);
            return new Length(diff.Value, diff.Unit);
        }

        public double Divide(Length first, Length second)
        {
            var q1 = new Quantity<LengthUnit>(first.Value, first.Unit);
            var q2 = new Quantity<LengthUnit>(second.Value, second.Unit);

            return q1.Divide(q2);
        }
    }
}