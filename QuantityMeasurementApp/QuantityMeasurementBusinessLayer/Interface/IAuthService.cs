using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementBusinessLayer.Interface
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
