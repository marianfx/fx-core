using System.IO;
using System.Threading.Tasks;
using FX.Core.Storage.Serialization.Abstract;
using FX.Core.Storage.Serialization.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FX.Core.Storage.Serialization.Implementations
{
    /// <summary>
    /// Implements the data serializer on disk, in JSON Files.
    /// The GUID represents file names (relative to a base path, that is set at initialization).
    /// </summary>
    public class JsonDataSerializer : IDataSerializer
    {
        private readonly JsonSerializationSettings _settings;
        private readonly JsonSerializer _serializer;


        public JsonDataSerializer(IOptions<JsonSerializationSettings> options)
        {
            _settings = options.Value;
            _serializer = new JsonSerializer
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None,
                PreserveReferencesHandling = _settings.PreserveReferences ? PreserveReferencesHandling.All : PreserveReferencesHandling.None
            };
        }

        public bool DataExists(string guid)
        {
            var fileName = _settings.UseIdDirectlyAsName ? guid : guid + ".json";
            var serverPath = Path.Combine(_settings.RootPath, fileName);

            return DataExistsOnPath(serverPath);
        }

        private static bool DataExistsOnPath(string serverPath)
        {
            return File.Exists(serverPath);
        }

        public async Task<T> GetData<T>(string guid) where T : class
        {
            var fileName = _settings.UseIdDirectlyAsName ? guid : guid + ".json";
            var serverPath = Path.Combine(_settings.RootPath, fileName);

            if (!DataExistsOnPath(serverPath))
                return null;

            using (var asyncFileStream = new FileStream(serverPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                using (var reader = new JsonTextReader(new StreamReader(asyncFileStream)))
                {
                    var output = _serializer.Deserialize<T>(reader);
                    return output;
                }
            }
        }

        public async Task SaveDataAsync(object response, string identifier)
        {
            var fileName = _settings.UseIdDirectlyAsName ? identifier : identifier + ".json";
            var serverPath = Path.Combine(_settings.RootPath, fileName);

            if (File.Exists(serverPath))
                File.Delete(serverPath);

            // make sure data directory exists
            var dataFolderPath = _settings.RootPath;
            if (!Directory.Exists(dataFolderPath))
                Directory.CreateDirectory(dataFolderPath);

            // write asynchronously to a file
            using (var asyncFileStream = new FileStream(serverPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, 4096, true))
            {
                using (var writer = new JsonTextWriter(new StreamWriter(asyncFileStream)))
                {
                    _serializer.Serialize(writer, response);
                    await writer.FlushAsync();
                }
            }
        }

        public async Task DeleteDataAsync(string guid)
        {
            var fileName = _settings.UseIdDirectlyAsName ? guid : guid + ".json";
            var serverPath = Path.Combine(_settings.RootPath, fileName);

            try
            {
                File.Delete(serverPath);
            }
            catch { }
        }
    }
}
