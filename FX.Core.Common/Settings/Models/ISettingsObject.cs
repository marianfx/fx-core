namespace FX.Core.Common.Settings.Models
{
    public interface ISettingsObject
    {
        /// <summary>
        /// Validate the current settings object. Returns true if there are errors.
        /// </summary>
        /// <returns></returns>
        bool Validate();
    }
}
