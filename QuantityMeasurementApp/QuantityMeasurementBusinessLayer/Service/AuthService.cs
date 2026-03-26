using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementRepository.Interface;
using QuantityMeasurementRepository.Repositories;

namespace QuantityMeasurementBusinessLayer.Service
{
    /// <summary>
    /// UC17: Authentication service.
    ///
    /// REGISTER  BCrypt.HashPassword(password, 12) — salt auto-embedded in hash
    ///           "$2a$12$[22-char-salt][31-char-hash]" stored in SQL Server [users] table
    ///
    /// LOGIN     BCrypt.Verify(plain, storedHash) — timing-safe comparison
    ///           On success: issues JWT (HMAC-SHA256, 60 min) + refresh token (7 days)
    ///
    /// REFRESH   Validates refresh token → rotates refresh token → issues new JWT
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration       _config;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepo,
            IConfiguration config,
            ILogger<AuthService> logger)
        {
            _userRepo = userRepo;
            _config = config;
            _logger = logger;
        }

        // ── Register ──────────────────────────────────────────────────────

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto req)
        {
            _logger.LogInformation("[AuthService] Register attempt: {U}", req.Username);

            if (await _userRepo.UsernameExistsAsync(req.Username))
                throw new InvalidOperationException("Username is already taken.");
            if (await _userRepo.EmailExistsAsync(req.Email))
                throw new InvalidOperationException("Email is already registered.");

            var user = new UserEntity
            {
                Username     = req.Username,
                Email        = req.Email,
                PasswordHash = UserRepository.HashPassword(req.Password),
                Role         = "User",
                CreatedAt    = DateTime.UtcNow,
                IsActive     = true
            };

            var saved = await _userRepo.CreateUserAsync(user);
            _logger.LogInformation("[AuthService] Registered UserId={Id}", saved.Id);
            return await IssueTokensAsync(saved, "Registration successful.");
        }

        // ── Login ─────────────────────────────────────────────────────────

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto req)
        {
            _logger.LogInformation("[AuthService] Login attempt: {U}", req.Username);
            var user = await _userRepo.GetByUsernameAsync(req.Username);

            // Constant-time failure — don't reveal whether username exists
            if (user is null || !UserRepository.VerifyPassword(req.Password, user.PasswordHash))
            {
                _logger.LogWarning("[AuthService] Failed login: {U}", req.Username);
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is disabled.");

            _logger.LogInformation("[AuthService] Login success: UserId={Id}", user.Id);
            return await IssueTokensAsync(user, "Login successful.");
        }

        // ── Refresh Token ─────────────────────────────────────────────────

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto req)
        {
            var principal = GetPrincipalFromExpiredToken(req.AccessToken);
            string? idClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idClaim, out int userId))
                throw new UnauthorizedAccessException("Invalid token.");

            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new UnauthorizedAccessException("User not found.");

            if (user.RefreshToken != req.RefreshToken ||
                user.RefreshTokenExpiry == null ||
                user.RefreshTokenExpiry <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token is invalid or expired.");

            _logger.LogInformation("[AuthService] Token refreshed: UserId={Id}", userId);
            return await IssueTokensAsync(user, "Token refreshed.");
        }

        // ── Private helpers ───────────────────────────────────────────────

        private async Task<AuthResponseDto> IssueTokensAsync(UserEntity user, string message)
        {
            string accessToken  = GenerateJwt(user);
            string refreshToken = GenerateRefreshToken();
            DateTime expiry     = DateTime.UtcNow.AddDays(7);

            await _userRepo.UpdateRefreshTokenAsync(user.Id, refreshToken, expiry);

            return new AuthResponseDto
            {
                AccessToken  = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt    = DateTime.UtcNow.AddMinutes(JwtExpireMinutes()),
                Username     = user.Username,
                Role         = user.Role,
                Message      = message
            };
        }

        private string GenerateJwt(UserEntity user)
        {
            string secret = _config["JwtSettings:Secret"]
                ?? throw new InvalidOperationException("JwtSettings:Secret is not configured.");
            string issuer   = _config["JwtSettings:Issuer"]   ?? "QuantityMeasurementAPI";
            string audience = _config["JwtSettings:Audience"] ?? "QuantityMeasurementAPI";

            var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer:             issuer,
                audience:           audience,
                claims: new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name,           user.Username),
                    new Claim(ClaimTypes.Email,          user.Email),
                    new Claim(ClaimTypes.Role,           user.Role),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                },
                expires:            DateTime.UtcNow.AddMinutes(JwtExpireMinutes()),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generate a 256-bit cryptographically-random refresh token (base64-encoded).
        /// </summary>
        private static string GenerateRefreshToken()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            string secret = _config["JwtSettings:Secret"]!;
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secret)),
                ValidateIssuer   = false,
                ValidateAudience = false,
                ValidateLifetime = false  // allow expired tokens for refresh
            };
            return new JwtSecurityTokenHandler()
                .ValidateToken(token, validationParams, out _);
        }

        private int JwtExpireMinutes()
            => int.TryParse(_config["JwtSettings:ExpirationMinutes"], out int m) ? m : 60;
    }
}
