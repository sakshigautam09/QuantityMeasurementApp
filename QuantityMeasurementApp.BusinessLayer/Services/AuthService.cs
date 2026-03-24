// ============================================================
// PROJECT : QuantityMeasurementApp.BusinessLayer
// FILE    : Services/AuthService.cs
// UC-17   : BCrypt hashing + JWT. Email stored as plain text.
//           Encryption/Decryption demonstrated separately via
//           EncryptionController (/api/v1/encryption/encrypt|decrypt)
// ============================================================

using QuantityMeasurementApp.BusinessLayer.Interface;
using QuantityMeasurementApp.ModelLayer.DTO;
using QuantityMeasurementApp.RepositoryLayer;
using QuantityMeasurementApp.RepositoryLayer.Interface;
using Microsoft.Extensions.Logging;

namespace QuantityMeasurementApp.BusinessLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository      _users;
        private readonly IJwtService          _jwt;
        private readonly IPasswordHasher      _hasher;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository      users,
            IJwtService          jwt,
            IPasswordHasher      hasher,
            ILogger<AuthService> logger)
        {
            _users  = users;
            _jwt    = jwt;
            _hasher = hasher;
            _logger = logger;
        }

        // ── Register ──────────────────────────────────────────────────────────────

        public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
        {
            if (await _users.ExistsAsync(req.Username, req.Email))
                return Fail("Username or email is already registered.");

            // Step 1 — Generate salt explicitly
            string salt = _hasher.GenerateSalt(workFactor: 12);

            // Step 2 — Hash password with salt (one-way, cannot decrypt)
            string hashedPassword = _hasher.HashPassword(req.Password, salt);

            // Save — email plain text, password hashed
            var user = new UserEntity
            {
                Username     = req.Username.Trim(),
                Email        = req.Email.Trim().ToLower(),  // plain text
                PasswordHash = hashedPassword               // BCrypt hash
            };

            var created = await _users.CreateAsync(user);
            _logger.LogInformation("Registered: {Username}", created.Username);

            return new AuthResponse
            {
                Success   = true,
                Message   = "Registration successful.",
                Token     = _jwt.GenerateToken(created),
                Username  = created.Username,
                Email     = created.Email,
                ExpiresAt = _jwt.GetExpiry()
            };
        }

        // ── Login ─────────────────────────────────────────────────────────────────

        public async Task<AuthResponse> LoginAsync(LoginRequest req)
        {
            var user = await _users.FindByUsernameAsync(req.Username.Trim());
            if (user is null)
            {
                _logger.LogWarning("Not found: {Username}", req.Username);
                return Fail("Invalid username or password.");
            }

            // Step 3 — Verify (re-hash + compare, NOT decryption)
            bool valid = _hasher.VerifyPassword(req.Password, user.PasswordHash);
            if (!valid)
            {
                _logger.LogWarning("Wrong password: {Username}", req.Username);
                return Fail("Invalid username or password.");
            }

            _logger.LogInformation("Logged in: {Username}", user.Username);

            return new AuthResponse
            {
                Success   = true,
                Message   = "Login successful.",
                Token     = _jwt.GenerateToken(user),
                Username  = user.Username,
                Email     = user.Email,
                ExpiresAt = _jwt.GetExpiry()
            };
        }

        private static AuthResponse Fail(string msg) => new()
        {
            Success = false,
            Message = msg,
            Token   = ""
        };
    }
}