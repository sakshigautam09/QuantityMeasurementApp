// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : Models/UserEntity.cs
// UC-17   : User entity mapped to dbo.users table by EF Core.
//
// WHY THIS FILE EXISTS:
//   Represents a registered user. Mapped to DB via EF Core.
//   Password is NEVER stored plain — only BCrypt hash.
//   JWT is stateless: after login no server-side session exists.
// ============================================================

namespace QuantityMeasurementApp.RepositoryLayer
{
    public class UserEntity
    {
        public Guid     Id           { get; set; }
        public string   Username     { get; set; } = "";
        public string   Email        { get; set; } = "";
        public string   PasswordHash { get; set; } = "";  // BCrypt hash
        public DateTime CreatedAt    { get; set; }
        public bool     IsActive     { get; set; }
    }
}