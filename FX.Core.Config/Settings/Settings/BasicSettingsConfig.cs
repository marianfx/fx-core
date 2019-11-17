using FX.Core.Config.Settings.Abstract;

namespace FX.Core.Config.Settings.Settings
{
    public class BasicSettingsConfig: ISettingsConfig
    {
        /// <summary>
        /// The Settings path, relative to the application's data saver base path
        /// </summary>
        public string SettingsPath { get; set; } = "appsettings.json";
    }
}
