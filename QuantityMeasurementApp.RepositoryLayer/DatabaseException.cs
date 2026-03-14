// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : DatabaseException.cs
//
// UC-16 : Database Integration
//
// Purpose : Wraps all SqlException / connection errors thrown
//           inside the repository layer into a single typed
//           exception so that the Service layer only needs to
//           catch one exception type for persistence failures.
//           C# equivalent of Java's DatabaseException.
//
// NOTE : PURELY ADDITIVE – no existing code is modified.
// ============================================================

using System;

namespace QuantityMeasurementApp.RepositoryLayer
{
    public class DatabaseException : Exception
    {
        public DatabaseException() { }

        public DatabaseException(string message)
            : base(message) { }

        public DatabaseException(string message, Exception inner)
            : base(message, inner) { }
    }
}