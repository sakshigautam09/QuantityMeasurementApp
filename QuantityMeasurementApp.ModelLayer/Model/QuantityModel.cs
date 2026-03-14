// ============================================================
// PROJECT : QuantityMeasurementApp.ModelLayer
// FILE    : QuantityModel.cs
//
// Purpose : Pure data-holder (POJO).
//           Holds value + IMeasurable unit.
//           NO arithmetic, NO conversion logic — all of that
//           lives in IQuantityModelService (BusinessLayer).
//
// Unit is IMeasurable so the service layer dispatches entirely
// through the interface — zero switch statements needed.
// ============================================================

using System;
using QuantityMeasurementApp.Core.Interfaces;

namespace QuantityMeasurementApp.ModelLayer
{
    public class QuantityModel
    {
        public double      Value { get; }
        public IMeasurable Unit  { get; }

        public QuantityModel(double value, IMeasurable unit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be finite.", nameof(value));

            Unit  = unit  ?? throw new ArgumentNullException(nameof(unit));
            Value = value;
        }

        public override string ToString() => $"{Value} {Unit.GetUnitName()}";
    }
}