using FX.Core.Config.Settings.Models;

namespace FX.Core.Config.Settings.Abstract
{
    public interface IBasicSettings<TSettings>
        where TSettings: ISettingsObject
    {
        /// <summary>
        /// Represents the current settings
        /// </summary>
        TSettings Settings { get; set; }

        /// <summary>
        /// Returns access to the configuration for this settings object
        /// </summary>
        /// <returns></returns>
        ISettingsConfig GetConfig();

        /// <summary>
        /// Specifies if settings can be loaded (from the inner saving mechanism)
        /// </summary>
        /// <returns></returns>
        bool CanLoadSettings();

        /// <summary>
        /// Loads the settings, using the inner loading mechanism
        /// </summary>
        void LoadSettings();

        /// <summary>
        /// Save the settings (if they've been edited) using the inner saving mechanisms
        /// </summary>
        void SaveSettings();

        /// <summary>
        /// Removes settings from the storage
        /// </summary>
        void DeleteSettings();
    }
}
