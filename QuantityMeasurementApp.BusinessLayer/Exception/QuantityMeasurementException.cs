// ============================================================
// PROJECT : QuantityMeasurementApp.BusinessLayer
// FILE    : QuantityMeasurementException.cs
//
// UC-15 : N-Tier Architecture
//
// Purpose : Custom runtime exception for all domain-level errors
//           in the service layer (cross-category ops, unsupported
//           arithmetic, null inputs, division by zero, etc.).
//
// NOTE : PURELY ADDITIVE – no existing code is modified.
// ============================================================

using System;

namespace QuantityMeasurementApp.BusinessLayer
{
    public class QuantityMeasurementException : Exception
    {
        public QuantityMeasurementException() { }

        public QuantityMeasurementException(string message)
            : base(message) { }

        public QuantityMeasurementException(string message, Exception inner)
            : base(message, inner) { }
    }
}
