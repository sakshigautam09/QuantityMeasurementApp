using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuantityMeasurementBusinessLayer.Interface;

namespace QuantityMeasurementBusinessLayer.Service
{
    /// <summary>
    /// AES-256-GCM encryption for sensitive fields. Key from configuration.
    /// </summary>
    public class AesEncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly ILogger<AesEncryptionService> _logger;

        private const int NonceSize = 12;
        private const int TagSize = 16;
        private const int KeySize = 32;

        public AesEncryptionService(IConfiguration configuration, ILogger<AesEncryptionService> logger)
        {
            _logger = logger;
            string? keyBase64 = configuration["Encryption:Key"];
            if (string.IsNullOrWhiteSpace(keyBase64) || keyBase64.Length < 32)
            {
                _logger.LogWarning("Encryption:Key not configured or too short. Using fallback (DEV ONLY).");
                _key = DeriveKeyFromSecret("DevFallbackKey_ChangeInProduction_32Chars!");
            }
            else
            {
                try
                {
                    _key = Convert.FromBase64String(keyBase64);
                    if (_key.Length != KeySize)
                        _key = DeriveKeyFromSecret(keyBase64);
                }
                catch
                {
                    _key = DeriveKeyFromSecret(keyBase64);
                }
            }
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
            byte[] cipher = new byte[plainBytes.Length];
            byte[] tag = new byte[TagSize];

            using var aes = new AesGcm(_key, TagSize);
            aes.Encrypt(nonce, plainBytes, cipher, tag);

            byte[] result = new byte[NonceSize + tag.Length + cipher.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
            Buffer.BlockCopy(tag, 0, result, NonceSize, tag.Length);
            Buffer.BlockCopy(cipher, 0, result, NonceSize + tag.Length, cipher.Length);
            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;

            byte[] full = Convert.FromBase64String(cipherText);
            if (full.Length < NonceSize + TagSize) return string.Empty;

            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];
            byte[] cipher = new byte[full.Length - NonceSize - TagSize];
            Buffer.BlockCopy(full, 0, nonce, 0, NonceSize);
            Buffer.BlockCopy(full, NonceSize, tag, 0, TagSize);
            Buffer.BlockCopy(full, NonceSize + TagSize, cipher, 0, cipher.Length);

            byte[] plain = new byte[cipher.Length];
            using var aes = new AesGcm(_key, TagSize);
            aes.Decrypt(nonce, cipher, tag, plain);
            return System.Text.Encoding.UTF8.GetString(plain);
        }

        private static byte[] DeriveKeyFromSecret(string secret)
        {
            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(secret));
            return hash;
        }
    }
}
