using System.ComponentModel.DataAnnotations;
using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementModel.Dto
{
    /// <summary>UC17: Two-operand input DTO for all binary API operations.</summary>
    public class QuantityInputDto
    {
        [Required(ErrorMessage = "thisQuantityDTO is required.")]
        public QuantityRequestDto ThisQuantityDTO { get; set; } = null!;

        public QuantityRequestDto? ThatQuantityDTO { get; set; }
    }

    /// <summary>UC17: Single quantity — value + unit + measurement type.</summary>
    public class QuantityRequestDto
    {
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Value must be 0 or greater.")]
        public double Value { get; set; }

        [Required(ErrorMessage = "Unit is required (e.g. FEET, KG, CELSIUS).")]
        [MaxLength(50)]
        public string Unit { get; set; } = string.Empty;

        [Required(ErrorMessage = "MeasurementType is required (LENGTH/WEIGHT/VOLUME/TEMPERATURE).")]
        [MaxLength(20)]
        // Removed the RegularExpression attribute — user types freely in Swagger
        // Validation is handled in the service layer instead
        public string MeasurementType { get; set; } = string.Empty;
    }

    /// <summary>UC17: Full result DTO returned by every operation endpoint.</summary>
    public class QuantityMeasurementDto
    {
        public double?  ThisValue             { get; set; }
        public string?  ThisUnit              { get; set; }
        public string?  ThisMeasurementType   { get; set; }
        public double?  ThatValue             { get; set; }
        public string?  ThatUnit              { get; set; }
        public string?  ThatMeasurementType   { get; set; }
        public string   Operation             { get; set; } = string.Empty;
        public string?  ResultString          { get; set; }  // "true"/"false" for COMPARE
        public double?  ResultValue           { get; set; }
        public string?  ResultUnit            { get; set; }
        public string?  ResultMeasurementType { get; set; }
        public string?  ErrorMessage          { get; set; }
        public bool     IsError               { get; set; }
        public DateTime Timestamp             { get; set; }

        public static QuantityMeasurementDto FromEntity(QuantityMeasurementEFEntity e)
            => new QuantityMeasurementDto
            {
                ThisValue             = e.Operand1Value,
                ThisUnit              = e.Operand1Unit,
                ThisMeasurementType   = e.Operand1Category,
                ThatValue             = e.Operand2Value,
                ThatUnit              = e.Operand2Unit,
                ThatMeasurementType   = e.Operand2Category,
                Operation             = e.Operation,
                ResultString          = e.Operation == "COMPARE"
                                            ? (e.ResultValue == 1 ? "true" : "false")
                                            : null,
                ResultValue           = e.Operation != "COMPARE" ? e.ResultValue : null,
                ResultUnit            = e.ResultUnit,
                ResultMeasurementType = e.ResultCategory,
                ErrorMessage          = e.ErrorMessage,
                IsError               = e.HasError,
                Timestamp             = e.Timestamp
            };

        public static List<QuantityMeasurementDto> FromList(
            IEnumerable<QuantityMeasurementEFEntity> entities)
            => new List<QuantityMeasurementDto>(entities.Select(FromEntity));
    }

    /// <summary>UC17: Standard error response body returned by GlobalExceptionHandler.</summary>
    public class ErrorResponseDto
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int      Status    { get; set; }
        public string   Error     { get; set; } = string.Empty;
        public string   Message   { get; set; } = string.Empty;
        public string   Path      { get; set; } = string.Empty;
    }
}
