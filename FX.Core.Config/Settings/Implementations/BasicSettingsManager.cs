using FX.Core.Config.Settings.Abstract;
using FX.Core.Config.Settings.Models;
using FX.Core.Config.Settings.Settings;
using FX.Core.Storage.Serialization.Abstract;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FX.Core.Common.Settings.Implementations
{
    /// <summary>
    /// Implementation for the settings manager interface, that uses a Data Serializer abstraction to store / load data and a basic configuration object.
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public class BasicSettingsManager<TSettings>: IBasicSettingsManager<TSettings>
        where TSettings: class, ISettingsObject
    {
        public TSettings Settings { get; set; }

        protected readonly IDataSerializer _dataSaver;
        protected readonly BasicSettingsConfig _config;

        public BasicSettingsManager(IOptions<BasicSettingsConfig> options)
        {
            _dataSaver = options.Value.DataSerializer;
            _config = options.Value;
        }

        public virtual ISettingsConfig GetConfig()
        {
            return _config;
        }

        public virtual async Task<bool> CanLoadSettings()
        {
            return await _dataSaver.DataExists(_config.Path);
        }

        public virtual async Task LoadSettings()
        {
            Settings = await _dataSaver.GetData<TSettings>(_config.Path);
            if (Settings == null)
                throw new System.Exception("Cannot read settings");

            // validate
            Settings.Validate();
        }

        public virtual async Task SaveSettings()
        {
            if (Settings == null)
                throw new System.Exception("Settings not initialized, cannot save");

            // validate
            Settings.Validate();

            // save
            await _dataSaver.SaveDataAsync(Settings, _config.Path);
        }

        public virtual async Task DeleteSettings()
        {
            if (await _dataSaver.DataExists(_config.Path))
                await _dataSaver.DeleteDataSilentAsync(_config.Path);
        }
    }
}
