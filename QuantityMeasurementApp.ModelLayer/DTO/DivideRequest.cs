using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.ModelLayer.DTO
{
    /// <summary>Body for POST /api/v1/quantities/divide — no target unit.</summary>
    public class DivideRequest
    {
        [Required] public QuantityInput First  { get; set; } = new();
        [Required] public QuantityInput Second { get; set; } = new();
    }
}
