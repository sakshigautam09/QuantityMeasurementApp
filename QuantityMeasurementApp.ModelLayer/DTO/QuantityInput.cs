using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.ModelLayer.DTO
{
    /// <summary>
    /// A measurement with a value.
    /// MeasurementType: "Length" | "Weight" | "Volume" | "Temperature"
    /// Units: Feet/Inch/Yard/Centimeter | Gram/Kilogram/Tonne | Litre/Millilitre/Gallon | Celsius/Fahrenheit/Kelvin
    /// </summary>
    public class QuantityInput
    {
        [Required]
        public double Value { get; set; }

        [Required(ErrorMessage = "Unit is required. E.g. Feet, Kilogram, Litre, Celsius")]
        public string Unit { get; set; } = "";

        [Required(ErrorMessage = "MeasurementType: Length | Weight | Volume | Temperature")]
        public string MeasurementType { get; set; } = "";
    }
}
