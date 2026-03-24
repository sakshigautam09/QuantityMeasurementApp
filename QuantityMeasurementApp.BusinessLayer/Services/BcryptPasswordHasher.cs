// ============================================================
// PROJECT : QuantityMeasurementApp.BusinessLayer
// FILE    : Services/BCryptPasswordHasher.cs
// UC-17   : BCrypt hashing with ILogger.
// ============================================================

using Microsoft.Extensions.Logging;
using QuantityMeasurementApp.BusinessLayer.Interface;

namespace QuantityMeasurementApp.BusinessLayer.Services
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        private readonly ILogger<BCryptPasswordHasher> _logger;

        public BCryptPasswordHasher(ILogger<BCryptPasswordHasher> logger)
        {
            _logger = logger;
        }

        public string GenerateSalt(int workFactor = 12)
        {
            _logger.LogInformation("Generating BCrypt salt. WorkFactor: {WorkFactor}", workFactor);
            return BCrypt.Net.BCrypt.GenerateSalt(workFactor);
        }

        public string HashPassword(string plainPassword, string salt)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("Password cannot be empty.", nameof(plainPassword));
            if (string.IsNullOrWhiteSpace(salt))
                throw new ArgumentException("Salt cannot be empty.", nameof(salt));

            _logger.LogInformation("Hashing password with BCrypt.");
            return BCrypt.Net.BCrypt.HashPassword(plainPassword, salt);
        }

        public bool VerifyPassword(string plainPassword, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(plainPassword)) return false;
            if (string.IsNullOrWhiteSpace(storedHash))    return false;

            _logger.LogInformation("Verifying BCrypt password.");
            bool result = BCrypt.Net.BCrypt.Verify(plainPassword, storedHash);
            _logger.LogInformation("Verification result: {Result}", result);
            return result;
        }
    }
}