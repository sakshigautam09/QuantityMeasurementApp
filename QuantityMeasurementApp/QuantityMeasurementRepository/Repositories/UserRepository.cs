using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementRepository.Context;
using QuantityMeasurementRepository.Interface;

namespace QuantityMeasurementRepository.Repositories
{
    /// <summary>
    /// User persistence repository.
    /// Password security: BCrypt hashing with auto-generated salt (see HashPassword/VerifyPassword).
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<UserRepository> _logger;

        private const int WorkFactor = 12;

        public UserRepository(ApplicationDbContext db, ILogger<UserRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<UserEntity> CreateUserAsync(UserEntity user)
        {
            if (!user.PasswordHash.StartsWith("$2"))
                user.PasswordHash = HashPassword(user.PasswordHash);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            _logger.LogInformation("[UserRepository] User registered: Id={Id} Username={U}", user.Id, user.Username);

            return user;
        }

        public async Task<UserEntity?> GetByUsernameAsync(string username)
            => await _db.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        public async Task<UserEntity?> GetByIdAsync(int id)
            => await _db.Users.FindAsync(id);

        public async Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiry)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user is null) return;
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = expiry;
            await _db.SaveChangesAsync();
        }

        public async Task<bool> UsernameExistsAsync(string username)
            => await _db.Users.AnyAsync(u => u.Username == username);

        public async Task<bool> EmailExistsAsync(string email)
            => await _db.Users.AnyAsync(u => u.Email == email);

        public async Task<UserEntity?> GetByEmailAsync(string email)
            => await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        public static string HashPassword(string plainText)
            => BCrypt.Net.BCrypt.HashPassword(plainText, WorkFactor);

        public static bool VerifyPassword(string plainText, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(plainText) || string.IsNullOrWhiteSpace(storedHash))
                return false;
            try { return BCrypt.Net.BCrypt.Verify(plainText, storedHash); }
            catch { return false; }
        }
    }
}
