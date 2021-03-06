﻿using FX.Core.Config.Settings.Models;
using System.Threading.Tasks;

namespace FX.Core.Config.Settings.Abstract
{
    /// <summary>
    /// Provides a base interface for a manager that stores 'settings' (application, class etc.) that can be: accessed, loaded, saved and deleted.
    /// Settings come with a configuration (to help facilitate the operations)
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public interface IBasicSettingsManager<TSettings>
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
        Task<bool> CanLoadSettings();

        /// <summary>
        /// Loads the settings, using the inner loading mechanism
        /// </summary>
        Task LoadSettings();

        /// <summary>
        /// Save the settings (if they've been edited) using the inner saving mechanisms
        /// </summary>
        Task SaveSettings();

        /// <summary>
        /// Removes settings from the storage
        /// </summary>
        Task DeleteSettings();
    }
}
