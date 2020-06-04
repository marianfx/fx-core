using System.Threading.Tasks;

namespace FX.Core.Storage.Serialization.Abstract
{
    public interface IDataSerializer
    {
        /// <summary>
        /// Specifies if a data (represented by an ID) exista as Saved
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        Task<bool> DataExists(string guid);

        /// <summary>
        /// Returns data (of type T) from storage, if it exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="guid"></param>
        /// <returns></returns>
        Task<T> GetData<T>(string guid) where T : class;

        /// <summary>
        /// Saves data (identified by 'identifier') using the specific implementation
        /// </summary>
        /// <param name="response"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        Task SaveDataAsync(object response, string identifier);

        /// <summary>
        /// Deletes data that is saved, based on ID
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        Task DeleteDataSilentAsync(string guid);
    }
}
