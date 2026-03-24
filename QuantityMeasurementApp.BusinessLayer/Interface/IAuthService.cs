// ============================================================
// PROJECT : QuantityMeasurementApp.API
// FILE    : Interface/IAuthService.cs
// UC-17   : Auth service contract — separated from implementation.
//
// WHY THIS FILE EXISTS:
//   Defines the contract for authentication operations.
//   AuthController depends on this interface — DIP.
//   Can be easily mocked in unit tests.
// ============================================================

using QuantityMeasurementApp.ModelLayer.DTO;

namespace QuantityMeasurementApp.BusinessLayer.Interface
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}