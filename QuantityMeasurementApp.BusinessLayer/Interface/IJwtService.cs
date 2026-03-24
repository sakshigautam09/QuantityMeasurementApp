// ============================================================
// PROJECT : QuantityMeasurementApp.API
// FILE    : Interface/IJwtService.cs
// UC-17   : JWT service contract — separated from implementation.
//
// WHY THIS FILE EXISTS:
//   Defines the contract for JWT token generation.
//   AuthService depends on this interface, not on JwtService
//   directly — follows DIP and makes unit testing easier.
// ============================================================

using QuantityMeasurementApp.RepositoryLayer;

namespace QuantityMeasurementApp.BusinessLayer.Interface
{
    public interface IJwtService
    {
        string   GenerateToken(UserEntity user);
        DateTime GetExpiry();
    }
}