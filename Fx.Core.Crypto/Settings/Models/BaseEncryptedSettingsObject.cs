using Fx.Core.Crypto.Abstract;
using FX.Core.Crypto.Settings.Models;
using System;

namespace Fx.Core.Crypto.Settings.Models
{
    public abstract class BaseEncryptedSettingsObject<T>: IEncryptedSettingsObject<T>
    {
        public string Key { get; set; }

        public virtual bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Key) || Key.Length < 4)
                throw new Exception("Invalid PIN (at least 4 characters)");

            return true;
        }

        public abstract T Encrypt(IEncryptor encryptor, string key);

        public abstract T Decrypt(IEncryptor encryptor, string key);

        public virtual bool IsCorrectKey(IEncryptor encryptor, string key)
        {
            return key == encryptor.Decrypt(Key, key);
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}
