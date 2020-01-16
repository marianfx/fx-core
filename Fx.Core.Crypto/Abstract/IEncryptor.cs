namespace Fx.Core.Crypto.Abstract
{
    public interface IEncryptor
    {
        /// <summary>
        /// Encrypts the given string value, with the given key (a key-based encryptor)
        /// </summary>
        /// <param name="value">The text to encrypt</param>
        /// <param name="key">The key to use for encryption</param>
        /// <returns></returns>
        string Encrypt(string value, string key);

        /// <summary>
        /// Decrypts the given string value, with the given key (a key-based decryptor)
        /// </summary>
        /// <param name="value">The text to decrypt</param>
        /// <param name="key">The key to use for decryption</param>
        /// <returns></returns>
        string Decrypt(string value, string key);
    }
}
