using FX.Core.Config.Settings.Abstract;
using FX.Core.Crypto.Settings.Models;

namespace FX.Core.Crypto.Settings.Abstract
{
    public interface IEncryptedSettingsManager<TSettings>: IBasicSettingsManager<TSettings>
        where TSettings : IEncryptedSettingsObject<TSettings>
    {
        /// <summary>
        /// Sets the encryption/decryption key.
        /// The system does not work without it.
        /// </summary>
        /// <param name="key"></param>
        void SetKey(string key);
    }
}
