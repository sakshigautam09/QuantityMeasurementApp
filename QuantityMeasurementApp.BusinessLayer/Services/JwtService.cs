// ============================================================
// PROJECT : QuantityMeasurementApp.BusinessLayer
// FILE    : Services/JwtService.cs
// UC-17   : JWT generation with ILogger added.
// ============================================================

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementApp.BusinessLayer.Interface;
using QuantityMeasurementApp.RepositoryLayer;

namespace QuantityMeasurementApp.BusinessLayer.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration      _config;
        private readonly ILogger<JwtService> _logger;

        public JwtService(IConfiguration config, ILogger<JwtService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public string GenerateToken(UserEntity user)
        {
            _logger.LogInformation("Generating JWT for: {Username}", user.Username);

            var key   = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub,        user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.Username),
                new(JwtRegisteredClaimNames.Email,      user.Email),
                new(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer:             _config["Jwt:Issuer"],
                audience:           _config["Jwt:Audience"],
                claims:             claims,
                notBefore:          DateTime.UtcNow,
                expires:            GetExpiry(),
                signingCredentials: creds);

            string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInformation("JWT generated. Expires: {Expiry}", GetExpiry());
            return tokenStr;
        }

        public DateTime GetExpiry()
            => DateTime.UtcNow.AddMinutes(
                double.Parse(_config["Jwt:ExpiryMinutes"] ?? "60"));
    }
}