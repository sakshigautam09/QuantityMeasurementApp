// ============================================================
// PROJECT : QuantityMeasurementApp.BusinessLayer
// FILE    : Services/AesEncryptionService.cs
// UC-17   : AES-256 Encryption/Decryption implementation.
//           Implements IEncryptionService from BusinessLayer.
// ============================================================

using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuantityMeasurementApp.BusinessLayer.Interface;

namespace QuantityMeasurementApp.BusinessLayer.Services
{
    public class AesEncryptionService : IEncryptionService
    {
        private readonly byte[]                        _key;
        private readonly ILogger<AesEncryptionService> _logger;

        public AesEncryptionService(
            IConfiguration                 config,
            ILogger<AesEncryptionService>  logger)
        {
            _logger = logger;
            string keyBase64 = config["Encryption:Key"]
                ?? throw new InvalidOperationException(
                    "Encryption:Key missing from appsettings.json");
            _key = Convert.FromBase64String(keyBase64);
            if (_key.Length != 32)
                throw new InvalidOperationException(
                    "Encryption key must be 32 bytes (256-bit).");
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("PlainText cannot be empty.");
            _logger.LogInformation("Encrypting with AES-256.");
            using var aes         = Aes.Create();
            aes.Key               = _key;
            aes.Mode              = CipherMode.CBC;
            aes.Padding           = PaddingMode.PKCS7;
            aes.GenerateIV();
            byte[] iv             = aes.IV;
            using var encryptor   = aes.CreateEncryptor(aes.Key, iv);
            byte[] plainBytes     = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            byte[] result         = new byte[iv.Length + encryptedBytes.Length];
            Buffer.BlockCopy(iv,             0, result, 0,         iv.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);
            return Convert.ToBase64String(result);
        }

        public string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                throw new ArgumentException("EncryptedText cannot be empty.");
            _logger.LogInformation("Decrypting with AES-256.");
            byte[] fullBytes      = Convert.FromBase64String(encryptedText);
            byte[] iv             = new byte[16];
            byte[] encryptedBytes = new byte[fullBytes.Length - 16];
            Buffer.BlockCopy(fullBytes, 0,  iv,             0, 16);
            Buffer.BlockCopy(fullBytes, 16, encryptedBytes, 0, encryptedBytes.Length);
            using var aes       = Aes.Create();
            aes.Key             = _key;
            aes.IV              = iv;
            aes.Mode            = CipherMode.CBC;
            aes.Padding         = PaddingMode.PKCS7;
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] plainBytes   = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }

        public string GenerateKey()
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }

        public string GenerateIV()
        {
            using var aes = Aes.Create();
            aes.GenerateIV();
            return Convert.ToBase64String(aes.IV);
        }
    }
}