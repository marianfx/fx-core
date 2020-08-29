using FX.Core.Config.Settings.Abstract;
using FX.Core.Storage.Serialization.Abstract;

namespace FX.Core.Config.Settings.Settings
{
    public class BasicSettingsConfig: ISettingsConfig
    {
        /// <summary>
        /// The serializer that knows how to load / save / delete data, using the Path
        /// </summary>
        public IDataSerializer DataSerializer { get; set; }

        /// <summary>
        /// The Settings path. It can be the physical path to a file, to a network address, or a unique name in memory.
        /// How it's used depends solely on the IDataSerializer Implementation (as they do come in pairs)
        /// </summary>
        public string Path { get; set; } = "appsettings.json";

        public BasicSettingsConfig() : base() { }

        public BasicSettingsConfig(IDataSerializer dataSerializer)
        {
            this.DataSerializer = dataSerializer;
        }
    }
}
