using Fx.Core.Crypto.Abstract;
using FX.Core.Common.Settings.Models;
using System;

namespace FX.Core.Crypto.Settings.Models
{
    public interface IEncryptedSettingsObject<T>: ISettingsObject, ICloneable
    {
        /// <summary>
        /// The key that was used to encrypt this object, but kept also encrypted.
        /// Used to determine if a decription key is valid [decrypt(encryptedKey) == key]
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Returns a copy of this object, encrypted
        /// </summary>
        /// <returns></returns>
        T Encrypt(IEncryptor encryptor, string key);

        /// <summary>
        /// Returns a copy of this object, decrypted
        /// </summary>
        /// <returns></returns>
        T Decrypt(IEncryptor encryptor, string key);

        /// <summary>
        /// Tells if the given key is the key that was used to encrypt this object.
        /// Is based on inner KEY property and on the encryptor.
        /// </summary>
        /// <param name="encryptor"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IsCorrectKey(IEncryptor encryptor, string key);
    }
}
