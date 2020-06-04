using FX.Core.Storage.Serialization.Abstract;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FX.Core.Storage.Serialization.Implementations
{
    /// <summary>
    /// Serializes data in memory, using thread-safe locks
    /// </summary>
    public class MemoryDataSerializer: IDataSerializer, IDisposable
    {
        private ReaderWriterLockSlim _activeDataLock;
        private Dictionary<string, object> _temporaryData;

        public MemoryDataSerializer()
        {
            _activeDataLock = new ReaderWriterLockSlim();
            _temporaryData = new Dictionary<string, object>();
        }

        public Task<bool> DataExists(string guid)
        {
            _activeDataLock.EnterReadLock();
            try
            {
                var exists = !(_temporaryData == null || !_temporaryData.ContainsKey(guid));
                return Task.FromResult(exists);
            }
            finally
            {
                _activeDataLock.ExitReadLock();
            }
        }

        public Task<T> GetData<T>(string guid) where T : class
        {
            if (!DataExists(guid).GetAwaiter().GetResult())
                return Task.FromResult<T>(null);

            _activeDataLock.EnterReadLock();
            try
            {
                return Task.FromResult(_temporaryData[guid] as T);
            }
            finally
            {
                _activeDataLock.ExitReadLock();
            }
        }

        public Task SaveDataAsync(object response, string identifier)
        {
            _activeDataLock.EnterWriteLock();
            try
            {
                if (_temporaryData == null)
                    _temporaryData = new Dictionary<string, object>();

                _temporaryData.Add(identifier, response);
            }
            finally
            {
                _activeDataLock.ExitWriteLock();
            }

            return Task.CompletedTask;
        }

        public Task DeleteDataSilentAsync(string guid)
        {
            _activeDataLock.EnterWriteLock();
            try
            {
                var dict = _temporaryData;
                if (dict != null && dict.ContainsKey(guid))
                {
                    dict[guid] = null;
                    dict.Remove(guid);
                }
            }
            finally
            {
                _activeDataLock.ExitWriteLock();
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _activeDataLock.EnterWriteLock();
            try
            {
                if (_temporaryData != null)
                    _temporaryData.Clear();

                _temporaryData = null;
            }
            finally
            {
                _activeDataLock.ExitWriteLock();
            }

            _activeDataLock.Dispose();
            _activeDataLock = null;
        }
    }
}
