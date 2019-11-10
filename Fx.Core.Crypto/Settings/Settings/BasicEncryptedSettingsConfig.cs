using System.IO;

namespace Fx.Core.Crypto.Settings.Settings
{
    public class BasicEncryptedSettingsConfig
    {
        /// <summary>
        /// The Settings path, relative to the application's data saver base path
        /// </summary>
        public string SettingsPath { get; set; } = "appsettings.json";
    }
}
