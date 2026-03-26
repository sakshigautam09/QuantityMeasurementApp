using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementModel.Dto
{
    /// <summary>JWT refresh request payload.</summary>
    public class RefreshTokenRequestDto
    {
        [Required] public string AccessToken  { get; set; } = string.Empty;
        [Required] public string RefreshToken { get; set; } = string.Empty;
    }
}
