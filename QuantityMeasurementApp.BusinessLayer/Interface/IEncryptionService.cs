// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : Interface/IEncryptionService.cs
// UC-17   : Encryption/Decryption contract.
//           Lives in RepositoryLayer because it is used by
//           QuantityMeasurementEfRepository to encrypt/decrypt
//           ResultDisplay before saving/reading from DB.
// ============================================================

namespace QuantityMeasurementApp.BusinessLayer.Interface
{
    public interface IEncryptionService
    {
        /// <summary>Encrypts plain text using AES-256.</summary>
        string Encrypt(string plainText);

        /// <summary>Decrypts AES-256 encrypted text back to plain text.</summary>
        string Decrypt(string encryptedText);

        /// <summary>Generates a new random AES-256 key.</summary>
        string GenerateKey();

        /// <summary>Generates a new random IV.</summary>
        string GenerateIV();
    }
}