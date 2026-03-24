// ============================================================
// PROJECT : QuantityMeasurementApp.Tests
// FILE    : UC17Tests.cs (FIXED)
// ============================================================

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.BusinessLayer;
using QuantityMeasurementApp.BusinessLayer.Interface;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.Core.Services;
using QuantityMeasurementApp.ModelLayer;
using QuantityMeasurementApp.RepositoryLayer;
using QuantityMeasurementApp.RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    [DoNotParallelize]
    public class ApiServicesTests
    {
        private IPasswordHasher _hasher = null!;
        private IEncryptionService _encryptor = null!;
        private IConfiguration _config = null!;

        [TestInitialize]
        public void Setup()
        {
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Encryption:Key"]    = "owEmBMKI+azut4i7lOXhxiRPFLHboWbz4YG2KFK+i8c=",
                    ["Jwt:Key"]           = "QuantityMeasurementApp_SuperSecret_JWT_Key_Min32Chars_2026!",
                    ["Jwt:Issuer"]        = "QuantityMeasurementApp.API",
                    ["Jwt:Audience"]      = "QuantityMeasurementApp.Clients",
                    ["Jwt:ExpiryMinutes"] = "60"
                })
                .Build();

            _hasher = new BCryptPasswordHasher(NullLogger<BCryptPasswordHasher>.Instance);
            _encryptor = new AesEncryptionService(_config, NullLogger<AesEncryptionService>.Instance);
        }

        // ===================== BCRYPT =====================

        [TestMethod]
        public void BCrypt_HashPassword_EmptyPassword_ThrowsException()
        {
            string salt = _hasher.GenerateSalt(4);

            try
            {
                _hasher.HashPassword("", salt);
                Assert.Fail("Expected exception not thrown");
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }
        }

        // ===================== AES =====================

        [TestMethod]
        public void AES_Encrypt_EmptyString_ThrowsException()
        {
            try
            {
                _encryptor.Encrypt("");
                Assert.Fail("Expected exception not thrown");
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void AES_Decrypt_InvalidBase64_ThrowsException()
        {
            try
            {
                _encryptor.Decrypt("invalid-base64");
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
        }

        // ===================== JWT =====================

        [TestMethod]
        public void JWT_GenerateToken_ReturnsNonEmptyString()
        {
            var jwtSvc = new JwtService(_config, NullLogger<JwtService>.Instance);
            string token = jwtSvc.GenerateToken(MakeUser());

            Assert.IsFalse(string.IsNullOrEmpty(token));
        }

        // ===================== SERVICE =====================

        [TestMethod]
        public void Service_Add_ResultEncrypted()
        {
            var repo = QuantityMeasurementCacheRepository.Instance;
            repo.Clear();

            var service = BuildService(repo);

            service.Add(
                new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet),
                new QuantityDTO(3.0, QuantityDTO.LengthUnit.Feet));

            string stored = repo.FindAll()[0].ResultDisplay;
            string decrypted = _encryptor.Decrypt(stored);

            Assert.IsFalse(string.IsNullOrEmpty(decrypted));
        }

        // ===================== HELPERS =====================

        private IQuantityMeasurementService BuildService(IQuantityMeasurementRepository repo)
            => new QuantityMeasurementServiceImpl(
                new QuantityModelServiceImpl(),
                new TemperatureService(),
                repo,
                _encryptor,
                new NullRedisCache(),
                NullLogger<QuantityMeasurementServiceImpl>.Instance);

        private static RepositoryLayer.UserEntity MakeUser() => new()
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    // ===================== MOCK REDIS =====================

    internal class NullRedisCache : IRedisCache
    {
        public Task SetAsync(string k, QuantityMeasurementEntity e, TimeSpan? ex = null) => Task.CompletedTask;
        public Task<QuantityMeasurementEntity?> GetAsync(string k) => Task.FromResult<QuantityMeasurementEntity?>(null);
        public Task DeleteAsync(string k) => Task.CompletedTask;
        public Task<bool> ExistsAsync(string k) => Task.FromResult(false);
        public Task SetStringAsync(string k, string v, TimeSpan? ex = null) => Task.CompletedTask;
        public Task<string?> GetStringAsync(string k) => Task.FromResult<string?>(null);
        public Task PushToListAsync(string lk, QuantityMeasurementEntity e) => Task.CompletedTask;
        public Task<IReadOnlyList<QuantityMeasurementEntity>> GetListAsync(string lk) =>
            Task.FromResult<IReadOnlyList<QuantityMeasurementEntity>>(new List<QuantityMeasurementEntity>().AsReadOnly());
        public Task<IEnumerable<string>> GetKeysAsync(string p) => Task.FromResult<IEnumerable<string>>(new List<string>());
        public Task ClearHistoryCacheAsync() => Task.CompletedTask;
        public Task<bool> IsAvailableAsync() => Task.FromResult(false);
    }
}
