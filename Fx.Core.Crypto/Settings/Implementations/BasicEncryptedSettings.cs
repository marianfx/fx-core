using Fx.Core.Crypto.Abstract;
using Fx.Core.Crypto.Settings.Settings;
using FX.Core.Crypto.Settings.Abstract;
using FX.Core.Crypto.Settings.Models;
using FX.Core.Storage.Serialization.Abstract;
using Microsoft.Extensions.Options;

namespace FX.Core.Crypto.Settings.Implementations
{
    public class BasicEncryptedSettings<TSettings> : IEncryptedSettings<TSettings>
        where TSettings : class, IEncryptedSettingsObject<TSettings>
    {
        public TSettings Settings { get; set; }

        private readonly IDataSerializer _dataSaver;
        private readonly BasicEncryptedSettingsConfig _config;
        private readonly IEncryptor _encryptor;
        private string _key;

        public BasicEncryptedSettings(IDataSerializer dataSaver, 
            IOptions<BasicEncryptedSettingsConfig> options,
            IEncryptor encryptor)
        {
            _dataSaver = dataSaver;
            _config = options.Value;
            _encryptor = encryptor;
        }

        public void LoadSettings()
        {
            Settings = _dataSaver.GetData<TSettings>(_config.SettingsPath).GetAwaiter().GetResult();
            if (Settings == null)
                throw new System.Exception("Cannot read settings");

            // validate
            Settings.Validate();

            // decrypt
            if (!Settings.IsCorrectKey(_encryptor, _key))
                throw new System.Exception("Cannot decrypt settings, invalid key");

            Settings = Settings.Decrypt(_encryptor, _key);
        }

        public void SaveSettings()
        {
            if (Settings == null)
                throw new System.Exception("Settings not initialized, cannot save");

            // validate
            Settings.Validate();

            // encrypt
            var encrypted = Settings.Encrypt(_encryptor, _key);
            _dataSaver.SaveDataAsync(encrypted, _config.SettingsPath).GetAwaiter().GetResult();
        }

        public void SetKey(string key)
        {
            _key = key;
        }
    }
}
