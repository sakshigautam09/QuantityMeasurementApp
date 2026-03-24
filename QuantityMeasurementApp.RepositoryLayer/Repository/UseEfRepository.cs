// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : Repository/UserEfRepository.cs
// UC-17   : EF Core user repository with ILogger.
//           Email stored as plain text in DB.
// ============================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuantityMeasurementApp.RepositoryLayer.Context;
using QuantityMeasurementApp.RepositoryLayer.Interface;

namespace QuantityMeasurementApp.RepositoryLayer.Repository
{
    public class UserEfRepository : IUserRepository
    {
        private readonly QuantityMeasurementDbContext _context;
        private readonly ILogger<UserEfRepository>    _logger;

        public UserEfRepository(
            QuantityMeasurementDbContext context,
            ILogger<UserEfRepository>    logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger  = logger  ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserEntity?> FindByUsernameAsync(string username)
        {
            _logger.LogInformation("Finding user: {Username}", username);

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username.Trim() && u.IsActive);

            if (user is null)
                _logger.LogWarning("User not found: {Username}", username);

            return user;
        }

        public async Task<UserEntity?> FindByEmailAsync(string email)
        {
            _logger.LogInformation("Finding user by email.");

            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    u.Email == email.Trim().ToLower() && u.IsActive);
        }

        public async Task<bool> ExistsAsync(string username, string email)
        {
            _logger.LogInformation("Checking duplicate: {Username}", username);

            return await _context.Users
                .AnyAsync(u =>
                    u.Username == username.Trim() ||
                    u.Email    == email.Trim().ToLower());
        }

        public async Task<UserEntity> CreateAsync(UserEntity user)
        {
            _logger.LogInformation("Creating user: {Username}", user.Username);

            user.Id        = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive  = true;
            user.Username  = user.Username.Trim();

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User created: {Id}", user.Id);
            return user;
        }
    }
}