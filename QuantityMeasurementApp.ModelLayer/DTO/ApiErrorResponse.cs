namespace QuantityMeasurementApp.ModelLayer.DTO
{
    /// <summary>Standard error response body for all API errors.</summary>
    public class ApiErrorResponse
    {
        public int      StatusCode { get; set; }
        public string   Error      { get; set; } = "";
        public string   Message    { get; set; } = "";
        public DateTime Timestamp  { get; set; } = DateTime.UtcNow;
    }
}
