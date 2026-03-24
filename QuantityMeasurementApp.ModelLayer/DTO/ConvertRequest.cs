using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.ModelLayer.DTO
{
    /// <summary>Body for POST /api/v1/quantities/convert — source + unit only target.</summary>
    public class ConvertRequest
    {
        [Required] public QuantityInput Source     { get; set; } = new();
        [Required] public UnitInput     TargetUnit { get; set; } = new();
    }
}
