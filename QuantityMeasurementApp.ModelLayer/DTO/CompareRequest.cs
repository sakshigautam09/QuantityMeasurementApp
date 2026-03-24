using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.ModelLayer.DTO
{
    /// <summary>Body for POST /api/v1/quantities/compare — no target unit.</summary>
    public class CompareRequest
    {
        [Required] public QuantityInput First  { get; set; } = new();
        [Required] public QuantityInput Second { get; set; } = new();
    }
}
