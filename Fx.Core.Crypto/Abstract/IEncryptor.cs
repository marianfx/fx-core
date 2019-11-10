namespace Fx.Core.Crypto.Abstract
{
    public interface IEncryptor
    {
        string Encrypt(string value, string key);
        string Decrypt(string value, string key);
    }
}
