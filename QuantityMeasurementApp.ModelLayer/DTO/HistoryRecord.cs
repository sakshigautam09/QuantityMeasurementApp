namespace QuantityMeasurementApp.ModelLayer.DTO
{
    /// <summary>One row from the operation history.</summary>
    public class HistoryRecord
    {
        public Guid     Id              { get; set; }
        public DateTime Timestamp       { get; set; }
        public string   Operation       { get; set; } = "";
        public string   MeasurementType { get; set; } = "";
        public string   FirstOperand    { get; set; } = "";
        public string?  SecondOperand   { get; set; }
        public string?  TargetUnit      { get; set; }
        public string   Result          { get; set; } = "";
        public bool     HasError        { get; set; }
        public string?  ErrorMessage    { get; set; }
    }
}
