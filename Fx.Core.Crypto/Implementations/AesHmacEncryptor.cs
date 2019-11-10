using Fx.Core.Crypto.Abstract;
using Fx.Core.Crypto.Algo;

namespace Fx.Core.Crypto.Implementations
{
    public class AesHmacEncryptor : IEncryptor
    {
        public string Decrypt(string value, string key)
        {
            var keyToUse = AesHmacEncryption.MapTo12Characters(key);
            return AesHmacEncryption.SimpleDecryptWithPassword(value, keyToUse);
        }

        public string Encrypt(string value, string key)
        {
            var keyToUse = AesHmacEncryption.MapTo12Characters(key);
            return AesHmacEncryption.SimpleEncryptWithPassword(value, keyToUse);
        }
    }
}
