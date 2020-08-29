using Fx.Core.Crypto.Abstract;
using Fx.Core.Crypto.Settings.Settings;
using FX.Core.Common.Settings.Implementations;
using FX.Core.Crypto.Settings.Abstract;
using FX.Core.Crypto.Settings.Models;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FX.Core.Crypto.Settings.Implementations
{
    public class BasicEncryptedSettingsManager<TSettings> : BasicSettingsManager<TSettings>, IEncryptedSettingsManager<TSettings>
        where TSettings : class, IEncryptedSettingsObject<TSettings>
    {
        protected readonly IEncryptor _encryptor;
        protected string _key;

        public BasicEncryptedSettingsManager(IOptions<BasicEncryptedSettingsConfig> options): base(options)
        {
            _encryptor = options.Value.Encryptor;
        }

        public override async Task LoadSettings()
        {
            Settings = await _dataSaver.GetData<TSettings>(_config.Path);
            if (Settings == null)
                throw new System.Exception("Cannot read settings");

            // validate
            Settings.Validate();

            // decrypt
            if (!Settings.IsCorrectKey(_encryptor, _key))
                throw new System.Exception("Cannot decrypt settings, invalid key");

            Settings = Settings.Decrypt(_encryptor, _key);
        }

        public override async Task SaveSettings()
        {
            if (Settings == null)
                throw new System.Exception("Settings not initialized, cannot save");

            // validate
            Settings.Validate();

            // encrypt
            var encrypted = Settings.Encrypt(_encryptor, _key);
            await _dataSaver.SaveDataAsync(encrypted, _config.Path);
        }

        public void SetKey(string key)
        {
            _key = key;
        }
    }
}
