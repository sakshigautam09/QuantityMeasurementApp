using System;
using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Interfaces;

namespace QuantityMeasurementApp.Core.Services
{
    public class TemperatureService : ITemperatureService
    {
        public Temperature Create(double value, TemperatureUnit unit)
        {
            return new Temperature(value, unit);
        }

        public bool AreEqual(Temperature first, Temperature second)
        {
            if (first == null || second == null)
                throw new ArgumentNullException();

            var q1 = new Quantity<TemperatureUnit>(first.Value, first.Unit);
            var q2 = new Quantity<TemperatureUnit>(second.Value, second.Unit);

            return Math.Round(q1.Divide(q2), 5) == 1;
        }

        public double Convert(double value, TemperatureUnit source, TemperatureUnit target)
        {
            var q = new Quantity<TemperatureUnit>(value, source);
            return q.ConvertTo(target).Value;
        }
    }
}