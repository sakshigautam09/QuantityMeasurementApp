// ============================================================
// PROJECT : QuantityMeasurementApp.API
// FILE    : Services/QuantityMapper.cs
// UC-17   : Converts API inputs → business layer QuantityDTO
// ============================================================

using QuantityMeasurementApp.ModelLayer.DTO;
using QuantityMeasurementApp.ModelLayer;

namespace QuantityMeasurementApp.BusinessLayer.Services
{
    public static class QuantityMapper
    {
        /// <summary>
        /// Converts a <see cref="QuantityInput"/> (value + unit + type)
        /// into a business layer <see cref="QuantityDTO"/>.
        /// </summary>
        public static QuantityDTO ToDTO(QuantityInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            return input.MeasurementType.Trim() switch
            {
                "Length" =>
                    new QuantityDTO(input.Value,
                        ParseEnum<QuantityDTO.LengthUnit>(input.Unit,
                            "Valid length units: Feet, Inch, Yard, Centimeter")),

                "Weight" =>
                    new QuantityDTO(input.Value,
                        ParseEnum<QuantityDTO.WeightUnit>(input.Unit,
                            "Valid weight units: Gram, Kilogram, Tonne")),

                "Volume" =>
                    new QuantityDTO(input.Value,
                        ParseEnum<QuantityDTO.VolumeUnit>(input.Unit,
                            "Valid volume units: Litre, Millilitre, Gallon")),

                "Temperature" =>
                    new QuantityDTO(input.Value,
                        ParseEnum<QuantityDTO.TemperatureUnit>(input.Unit,
                            "Valid temperature units: Celsius, Fahrenheit, Kelvin")),

                var t => throw new ArgumentException(
                    $"Unknown MeasurementType '{t}'. " +
                    "Valid values: Length, Weight, Volume, Temperature.")
            };
        }

        /// <summary>
        /// Converts a <see cref="UnitInput"/> (unit + type only, no value)
        /// into a business layer <see cref="QuantityDTO"/> with value = 0.
        /// Used for: convert targetUnit, add/subtract targetUnit.
        /// </summary>
        public static QuantityDTO ToUnitHint(UnitInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            return input.MeasurementType.Trim() switch
            {
                "Length" =>
                    new QuantityDTO(0.0,
                        ParseEnum<QuantityDTO.LengthUnit>(input.Unit,
                            "Valid length units: Feet, Inch, Yard, Centimeter")),

                "Weight" =>
                    new QuantityDTO(0.0,
                        ParseEnum<QuantityDTO.WeightUnit>(input.Unit,
                            "Valid weight units: Gram, Kilogram, Tonne")),

                "Volume" =>
                    new QuantityDTO(0.0,
                        ParseEnum<QuantityDTO.VolumeUnit>(input.Unit,
                            "Valid volume units: Litre, Millilitre, Gallon")),

                "Temperature" =>
                    new QuantityDTO(0.0,
                        ParseEnum<QuantityDTO.TemperatureUnit>(input.Unit,
                            "Valid temperature units: Celsius, Fahrenheit, Kelvin")),

                var t => throw new ArgumentException(
                    $"Unknown MeasurementType '{t}'. " +
                    "Valid values: Length, Weight, Volume, Temperature.")
            };
        }

        // ── Private ───────────────────────────────────────────────────────────────

        private static T ParseEnum<T>(string value, string hint) where T : struct, Enum
        {
            if (Enum.TryParse<T>(value.Trim(), ignoreCase: true, out var result))
                return result;

            throw new ArgumentException(
                $"Unknown unit '{value}'. {hint}");
        }
    }
}