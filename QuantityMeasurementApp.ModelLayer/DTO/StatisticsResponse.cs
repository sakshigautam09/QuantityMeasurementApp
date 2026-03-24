namespace QuantityMeasurementApp.ModelLayer.DTO
{
    /// <summary>Response for GET /api/v1/quantities/statistics</summary>
    public class StatisticsResponse
    {
        public int TotalRecords                    { get; set; }
        public int ErrorCount                      { get; set; }
        public Dictionary<string, int> ByOperation { get; set; } = new();
    }
}
