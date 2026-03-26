using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementModel.Dto
{
    /// <summary>Login request payload.</summary>
    public class LoginRequestDto
    {
        [Required] public string Username { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }
}
