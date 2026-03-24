// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : Interface/IUserRepository.cs
// UC-17   : User repository contract — separate interface file.
//
// WHY THIS FILE EXISTS:
//   Defines the contract for user data access.
//   AuthService depends on this interface, not on a concrete
//   implementation — follows Dependency Inversion Principle.
//   Only called during Register and Login.
//   All subsequent requests validate JWT by signature only.
// ============================================================

namespace QuantityMeasurementApp.RepositoryLayer.Interface
{
    public interface IUserRepository
    {
        Task<UserEntity?>  FindByUsernameAsync(string username);
        Task<UserEntity?>  FindByEmailAsync(string email);
        Task<bool>         ExistsAsync(string username, string email);
        Task<UserEntity>   CreateAsync(UserEntity user);
    }
}