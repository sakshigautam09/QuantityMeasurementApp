// ============================================================
// PROJECT : QuantityMeasurementApp.BusinessLayer
// FILE    : QuantityModelServiceImpl.cs
// ============================================================

using System;
using QuantityMeasurementApp.Core.Interfaces;
using QuantityMeasurementApp.ModelLayer;

namespace QuantityMeasurementApp.BusinessLayer
{
    public class QuantityModelServiceImpl : IQuantityModelService
    {
        public QuantityModel ConvertTo(QuantityModel source, IMeasurable targetUnit)
        {
            double baseValue = source.Unit.ConvertToBaseUnit(source.Value);
            double converted = targetUnit.ConvertFromBaseUnit(baseValue);
            return new QuantityModel(Math.Round(converted, 6), targetUnit);
        }

        public QuantityModel Add(QuantityModel first, QuantityModel second)
            => AddWithTargetUnit(first, second, first.Unit);

        public QuantityModel AddWithTargetUnit(QuantityModel first, QuantityModel second, IMeasurable targetUnit)
        {
            double sum = first.Unit.ConvertToBaseUnit(first.Value)
                       + second.Unit.ConvertToBaseUnit(second.Value);
            return new QuantityModel(Math.Round(targetUnit.ConvertFromBaseUnit(sum), 6), targetUnit);
        }

        public QuantityModel Subtract(QuantityModel first, QuantityModel second)
            => SubtractWithTargetUnit(first, second, first.Unit);

        public QuantityModel SubtractWithTargetUnit(QuantityModel first, QuantityModel second, IMeasurable targetUnit)
        {
            double diff = first.Unit.ConvertToBaseUnit(first.Value)
                        - second.Unit.ConvertToBaseUnit(second.Value);
            return new QuantityModel(Math.Round(targetUnit.ConvertFromBaseUnit(diff), 6), targetUnit);
        }

        public double Divide(QuantityModel first, QuantityModel second)
        {
            double denom = second.Unit.ConvertToBaseUnit(second.Value);
            if (denom == 0.0) throw new DivideByZeroException("Cannot divide by zero quantity.");
            return first.Unit.ConvertToBaseUnit(first.Value) / denom;
        }

        public bool AreEqual(QuantityModel first, QuantityModel second)
            => Math.Abs(
                first.Unit.ConvertToBaseUnit(first.Value) -
                second.Unit.ConvertToBaseUnit(second.Value)) < 1e-9;
    }
}