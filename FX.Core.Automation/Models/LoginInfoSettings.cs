using Fx.Core.Crypto.Abstract;
using Fx.Core.Crypto.Settings.Models;
using FX.Core.Crypto.Settings.Models;
using System;

namespace FX.Core.Automation.Models
{
    public class LoginInfoSettings : BaseEncryptedSettingsObject<LoginInfoSettings>, IEncryptedSettingsObject<LoginInfoSettings>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CookieFile { get; set; }

        public override bool Validate()
        {
            base.Validate();
            if (string.IsNullOrWhiteSpace(Password))
                throw new Exception("Invalid Password");
            if (string.IsNullOrWhiteSpace(UserName))
                throw new Exception("Invalid Username");

            return true;
        }

        public override LoginInfoSettings Encrypt(IEncryptor encryptor, string key)
        {
            var encrypted = new LoginInfoSettings()
            {
                UserName = encryptor.Encrypt(UserName, key),
                Password = encryptor.Encrypt(Password, key),
                CookieFile = CookieFile,
                Key = encryptor.Encrypt(Key, key)
            };
            return encrypted;
        }

        public override LoginInfoSettings Decrypt(IEncryptor encryptor, string key)
        {
            var decrypted = new LoginInfoSettings()
            {
                UserName = encryptor.Decrypt(UserName, key),
                Password = encryptor.Decrypt(Password, key),
                CookieFile = CookieFile,
                Key = encryptor.Decrypt(Key, key)
            };
            return decrypted;
        }
    }
}
