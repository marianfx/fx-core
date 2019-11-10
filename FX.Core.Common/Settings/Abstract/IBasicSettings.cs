using FX.Core.Common.Settings.Models;

namespace FX.Core.Common.Settings.Abstract
{
    public interface IBasicSettings<TSettings>
        where TSettings: ISettingsObject
    {
        /// <summary>
        /// Represents the current settings
        /// </summary>
        TSettings Settings { get; set; }

        /// <summary>
        /// Loads the settings, using the inner loading mechanism
        /// </summary>
        void LoadSettings();

        /// <summary>
        /// Save the settings (if they've been edited) using the inner saving mechanisms
        /// </summary>
        void SaveSettings();
    }
}
