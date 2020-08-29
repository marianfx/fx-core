using Fx.Core.Crypto.Abstract;
using FX.Core.Config.Settings.Abstract;
using FX.Core.Config.Settings.Settings;
using FX.Core.Storage.Serialization.Abstract;

namespace Fx.Core.Crypto.Settings.Settings
{
    public class BasicEncryptedSettingsConfig: BasicSettingsConfig, ISettingsConfig
    {
        /// <summary>
        /// The smart algorithm that knows how to encrypt / decrypt data
        /// </summary>
        public IEncryptor Encryptor { get; set; }

        public BasicEncryptedSettingsConfig(): base() { }

        public BasicEncryptedSettingsConfig(IDataSerializer dataSerializer, IEncryptor encryptor): base(dataSerializer)
        {
            this.Encryptor = encryptor;
        }
    }
}
