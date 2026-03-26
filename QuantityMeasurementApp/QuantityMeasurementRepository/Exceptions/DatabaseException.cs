namespace QuantityMeasurementRepository.Exceptions
{
    /// <summary>
    /// UC16: Wraps ADO.NET / SQL Server errors so upper layers
    /// remain decoupled from SqlException.
    /// </summary>
    public class DatabaseException : Exception
    {
        public DatabaseException(string message) : base(message) { }

        public DatabaseException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
