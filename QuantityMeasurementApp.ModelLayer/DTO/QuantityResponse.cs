namespace QuantityMeasurementApp.ModelLayer.DTO
{
    /// <summary>Standard response for every quantity operation.</summary>
    public class QuantityResponse
    {
        public bool     Success      { get; set; }
        public string   Operation    { get; set; } = "";
        public string   ResultValue  { get; set; } = "";
        public string   ResultUnit   { get; set; } = "";
        public string?  ErrorMessage { get; set; }
        public DateTime Timestamp    { get; set; } = DateTime.UtcNow;
    }
}
