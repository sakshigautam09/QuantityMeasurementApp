using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementRepository.Interface
{
    public interface IUserRepository
    {
        Task<UserEntity?> GetByUsernameAsync(string username);
        Task<UserEntity?> GetByIdAsync(int id);
        Task<UserEntity>  CreateUserAsync(UserEntity user);
        Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiry);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }
}
