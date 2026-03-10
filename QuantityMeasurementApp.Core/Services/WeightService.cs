using System;
using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Interfaces;

namespace QuantityMeasurementApp.Core.Services
{
    public class WeightService : IWeightService
    {
        public Weight Create(double value, WeightUnit unit)
        {
            return new Weight(value, unit);
        }

        public bool AreEqual(Weight first, Weight second)
        {
            if (first == null || second == null)
                throw new ArgumentNullException();

            var q1 = new Quantity<WeightUnit>(first.Value, first.Unit);
            var q2 = new Quantity<WeightUnit>(second.Value, second.Unit);

            return q1.Equals(q2);
        }

        public double Convert(double value, WeightUnit source, WeightUnit target)
        {
            var q = new Quantity<WeightUnit>(value, source);
            return q.ConvertTo(target).Value;
        }

        public Weight Add(Weight first, Weight second)
        {
            var q1 = new Quantity<WeightUnit>(first.Value, first.Unit);
            var q2 = new Quantity<WeightUnit>(second.Value, second.Unit);

            var sum = q1.Add(q2);
            return new Weight(sum.Value, sum.Unit);
        }

        public Weight Add(Weight first, Weight second, WeightUnit targetUnit)
        {
            var q1 = new Quantity<WeightUnit>(first.Value, first.Unit);
            var q2 = new Quantity<WeightUnit>(second.Value, second.Unit);

            var sum = q1.Add(q2, targetUnit);
            return new Weight(sum.Value, sum.Unit);
        }

        public Weight Subtract(Weight first, Weight second)
        {
            var q1 = new Quantity<WeightUnit>(first.Value, first.Unit);
            var q2 = new Quantity<WeightUnit>(second.Value, second.Unit);

            var diff = q1.Subtract(q2);
            return new Weight(diff.Value, diff.Unit);
        }

        public Weight Subtract(Weight first, Weight second, WeightUnit targetUnit)
        {
            var q1 = new Quantity<WeightUnit>(first.Value, first.Unit);
            var q2 = new Quantity<WeightUnit>(second.Value, second.Unit);

            var diff = q1.Subtract(q2, targetUnit);
            return new Weight(diff.Value, diff.Unit);
        }

        public double Divide(Weight first, Weight second)
        {
            var q1 = new Quantity<WeightUnit>(first.Value, first.Unit);
            var q2 = new Quantity<WeightUnit>(second.Value, second.Unit);

            return q1.Divide(q2);
        }
    }
}