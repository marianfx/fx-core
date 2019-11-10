using FX.Core.Config.Settings.Abstract;
using FX.Core.Config.Settings.Models;
using FX.Core.Config.Settings.Settings;
using FX.Core.Storage.Serialization.Abstract;
using Microsoft.Extensions.Options;

namespace FX.Core.Common.Settings.Implementations
{
    public class BasicSettings<TSettings>: IBasicSettings<TSettings>
        where TSettings: class, ISettingsObject
    {
        public TSettings Settings { get; set; }

        protected readonly IDataSerializer _dataSaver;
        protected readonly BasicSettingsConfig _config;

        public BasicSettings(IDataSerializer dataSaver,
            IOptions<BasicSettingsConfig> options)
        {
            _dataSaver = dataSaver;
            _config = options.Value;
        }

        public void LoadSettings()
        {
            Settings = _dataSaver.GetData<TSettings>(_config.SettingsPath).GetAwaiter().GetResult();
            if (Settings == null)
                throw new System.Exception("Cannot read settings");

            // validate
            Settings.Validate();
        }

        public void SaveSettings()
        {
            if (Settings == null)
                throw new System.Exception("Settings not initialized, cannot save");

            // validate
            Settings.Validate();

            // encrypt
            _dataSaver.SaveDataAsync(Settings, _config.SettingsPath).GetAwaiter().GetResult();
        }
    }
}
