using FX.Core.Common.Settings.Abstract;
using FX.Core.Crypto.Settings.Models;

namespace FX.Core.Crypto.Settings.Abstract
{
    public interface IEncryptedSettings<TSettings>: IBasicSettings<TSettings>
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
