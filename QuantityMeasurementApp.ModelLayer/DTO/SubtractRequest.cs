using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.ModelLayer.DTO
{
    /// <summary>Body for POST /api/v1/quantities/subtract — two values + optional target unit.</summary>
    public class SubtractRequest
    {
        [Required] public QuantityInput  First      { get; set; } = new();
        [Required] public QuantityInput  Second     { get; set; } = new();
        public            UnitInput?     TargetUnit { get; set; }
    }
}
