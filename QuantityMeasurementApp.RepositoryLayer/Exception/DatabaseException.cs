// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : Exception/DatabaseException.cs
// UC-16   : Database Integration
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