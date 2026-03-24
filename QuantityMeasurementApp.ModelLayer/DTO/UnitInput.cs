using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.ModelLayer.DTO
{
    /// <summary>
    /// Unit-only input — no value needed.
    /// Used for convert target and add/subtract target unit.
    /// </summary>
    public class UnitInput
    {
        [Required(ErrorMessage = "Unit is required. E.g. Feet, Kilogram, Litre, Celsius")]
        public string Unit { get; set; } = "";

        [Required(ErrorMessage = "MeasurementType: Length | Weight | Volume | Temperature")]
        public string MeasurementType { get; set; } = "";
    }
}
