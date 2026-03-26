using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementModel.Dto
{
    /// <summary>Registration request payload.</summary>
    public class RegisterRequestDto
    {
        [Required][MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required][EmailAddress][MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required][MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
