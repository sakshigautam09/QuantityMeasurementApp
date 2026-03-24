// ============================================================
// PROJECT : QuantityMeasurementApp.BusinessLayer
// FILE    : Interface/IPasswordHasher.cs
// UC-17   : Password hashing contract with explicit salting.
// ============================================================

namespace QuantityMeasurementApp.BusinessLayer.Interface
{
    public interface IPasswordHasher
    {
        /// <summary>Step 1 — Generate a random cryptographic salt.</summary>
        string GenerateSalt(int workFactor = 12);

        /// <summary>Step 2 — Hash password using the provided salt.</summary>
        string HashPassword(string plainPassword, string salt);

        /// <summary>Step 3 — Verify plain password against stored hash.</summary>
        bool VerifyPassword(string plainPassword, string storedHash);
    }
}