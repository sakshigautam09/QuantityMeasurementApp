using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.ModelLayer.DTO
{
    /// <summary>Body for POST /api/v1/quantities/add — two values + optional target unit.</summary>
    public class AddRequest
    {
        [Required] public QuantityInput  First      { get; set; } = new();
        [Required] public QuantityInput  Second     { get; set; } = new();
        public            UnitInput?     TargetUnit { get; set; }
    }
}
