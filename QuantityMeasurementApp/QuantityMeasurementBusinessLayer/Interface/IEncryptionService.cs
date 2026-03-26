namespace QuantityMeasurementBusinessLayer.Interface
{
    /// <summary>
    /// AES encryption service for sensitive fields. Keys stored in configuration.
    /// </summary>
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
