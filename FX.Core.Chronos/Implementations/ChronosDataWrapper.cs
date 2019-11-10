using FX.Core.Chronos.Abstract;
using FX.Core.Chronos.Models;
using FX.Core.Chronos.Settings;
using FX.Core.IO.Files.Abstract;
using FX.Core.Storage.Serialization.Abstract;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FX.Core.Chronos.Real
{
    public class ChronosDataWrapper: IChronosDataWrapper
    {
        // https://docs.microsoft.com/en-us/dotnet/api/system.threading.readerwriterlockslim?view=netframework-4.7.2
        private ReaderWriterLockSlim _activeTimersLock;
        private Dictionary<string, Chronos<string, Task<bool>>> _activeTimers; // method; string is the key to the dictionary where to delete from

        private ReaderWriterLockSlim _activeCalculationsLock;
        private Dictionary<string, ActivityToken> _activeCalculations;

        private ReaderWriterLockSlim _activeExportsLock;
        private Dictionary<string, ActivityToken> _activeExports;

        private ReaderWriterLockSlim _lastTimeDeletedLock;
        private DateTime _lastTimeIDeletedFiles;
        private int _hoursToKeepData;

        private readonly IDataSerializer _dataSaver;
        private readonly IFileHandlerSimple _fileHandler;
        private readonly ChronosSettings _settings;
        private readonly string _contentPath;

        public ChronosDataWrapper(string contentPath, 
            IDataSerializer chronosSaver, 
            IFileHandlerSimple fileHandler,
            IOptions<ChronosSettings> options)
        {
            _dataSaver = chronosSaver;
            _fileHandler = fileHandler;
            _contentPath = contentPath;
            _settings = options.Value;

            _activeTimersLock = new ReaderWriterLockSlim();
            _activeTimers = new Dictionary<string, Chronos<string, Task<bool>>>();

            _activeCalculationsLock = new ReaderWriterLockSlim();
            _activeCalculations = new Dictionary<string, ActivityToken>();

            _activeExportsLock = new ReaderWriterLockSlim();
            _activeExports = new Dictionary<string, ActivityToken>();

            _lastTimeDeletedLock = new ReaderWriterLockSlim();
            _lastTimeIDeletedFiles = DateTime.Now;
            _hoursToKeepData = _settings.HoursToKeepData;
        }


        #region Handle Calculation Start / Stop
        public bool IsCalculationRunning(string guid)
        {
            _activeCalculationsLock.EnterReadLock();
            try
            {
                return _activeCalculations != null && _activeCalculations.TryGetValue(guid, out ActivityToken token) && !(token?.IsFinished ?? false);
            }
            finally
            {
                _activeCalculationsLock.ExitReadLock();
            }
        }

        public void SetActiveCalculation(string id, string user = null, dynamic data = null)
        {
            _activeCalculationsLock.EnterWriteLock();
            try
            {
                _activeCalculations = _activeCalculations ?? new Dictionary<string, ActivityToken>();
                if (_activeCalculations.ContainsKey(id))
                    _activeCalculations.Remove(id);

                _activeCalculations.Add(id, new ActivityToken { Id = id, User = user, Data = data });
            }
            finally
            {
                _activeCalculationsLock.ExitWriteLock();
            }
        }

        public void StopCalculation(string id, bool remove = true, IList<string> log = null, bool isSuccess = false)
        {
            // update calculations
            _activeCalculationsLock.EnterWriteLock();
            try
            {
                if (_activeCalculations != null)
                {
                    if (remove)
                    {
                        if (_activeCalculations.ContainsKey(id))
                            _activeCalculations.Remove(id);
                    }
                    else
                    {
                        if (_activeCalculations.ContainsKey(id))
                        {
                            if (_activeCalculations[id] == null)
                                _activeCalculations[id] = new ActivityToken();

                            _activeCalculations[id].Log = log ?? new List<string>();
                            _activeCalculations[id].IsSuccess = isSuccess;
                            _activeCalculations[id].IsFinished = true;
                        }
                    }
                }
            }
            finally
            {
                _activeCalculationsLock.ExitWriteLock();
            }


            // export
            StopExport(id);
        }

        public ActivityToken GetCalculationToken(string id)
        {
            _activeCalculationsLock.EnterReadLock();
            try
            {
                ActivityToken token = null;
                if (_activeCalculations != null)
                    _activeCalculations.TryGetValue(id, out token);

                return token;
            }
            finally
            {
                _activeCalculationsLock.ExitReadLock();
            }
        }
        #endregion


        #region Handling Data
        public bool ContainsData(string id)
        {
            return _dataSaver.DataExists(id);
        }

        public async Task<T> GetData<T>(string id) where T : class
        {
            return await _dataSaver.GetData<T>(id);
        }

        public async Task<string> SaveResponseAndSetDeleteTimeout(object response, string identifier = null)
        {
            // save data
            var guid = identifier ?? Guid.NewGuid().ToString();
            await _dataSaver.SaveDataAsync(response, guid);

            // mark calc. as not active
            var propsImportant = new List<string> { "Log", "IsSuccess" };
            var objProps = response.GetType().GetProperties().Where(x => propsImportant.Contains(x.Name));

            var logProp = objProps.FirstOrDefault(x => x.Name == "Log");
            var logValues = logProp != null ? logProp.GetValue(response) as IList<string> : null;

            var isSuccessProp = objProps.FirstOrDefault(x => x.Name == "IsSuccess");
            var isSuccessValue = isSuccessProp != null ? isSuccessProp.GetValue(response) as bool? : null;

            var log = logValues;
            var isSuccess = isSuccessValue ?? false;
            StopCalculation(guid, false, log, isSuccess);


            // set up timer
            _activeTimersLock.EnterWriteLock();
            try
            {
                if (_activeTimers == null)
                    _activeTimers = new Dictionary<string, Chronos<string, Task<bool>>>();

                var timer = new Chronos<string, Task<bool>>();
                timer.SetTimeout(DeleteAssociatedData, TimeSpan.FromHours(_hoursToKeepData), guid);
                _activeTimers.Add(guid, timer);
            }
            finally
            {
                _activeTimersLock.ExitWriteLock();
            }

            // clean used data
            GC.Collect();

            return guid;
        }

        public async Task<bool> DeleteAssociatedData(string id)
        {
            // clear timer
            ClearTimeoutTrigger(id);

            // remove active calculations
            StopCalculation(id);

            // remove active exports
            StopExport(id);

            // try deleting older files
            ClearOldData();

            // clear data
            await _dataSaver.DeleteDataAsync(id);

            // clean used data
            GC.Collect();

            return true;
        }
        #endregion


        #region Handling Data Export (Save)
        public bool IsExportRunning(string id)
        {
            _activeExportsLock.EnterReadLock();
            try
            {
                return _activeExports != null && _activeExports.TryGetValue(id, out ActivityToken token) && !(token?.IsFinished ?? false);
            }
            finally
            {
                _activeExportsLock.ExitReadLock();
            }
        }

        public void SetActiveExport(string id, string user = null)
        {
            _activeExportsLock.EnterWriteLock();
            try
            {
                _activeExports = _activeExports ?? new Dictionary<string, ActivityToken>();
                if (_activeExports.ContainsKey(id))
                    _activeExports.Remove(id);

                _activeExports.Add(id, new ActivityToken { Id = id, User = user });
            }
            finally
            {
                _activeExportsLock.ExitWriteLock();
            }
        }

        public void StopExport(string id)
        {
            _activeExportsLock.EnterWriteLock();
            try
            {
                if (_activeExports != null && _activeExports.ContainsKey(id))
                    _activeExports.Remove(id);
            }
            finally
            {
                _activeExportsLock.ExitWriteLock();
            }
        }

        public void MarkExportFinished(string id, dynamic data, string user = null, dynamic errors = null)
        {
            _activeExportsLock.EnterWriteLock();
            try
            {
                _activeExports = _activeExports ?? new Dictionary<string, ActivityToken>();
                if (!_activeExports.ContainsKey(id))
                    _activeExports.Add(id, new ActivityToken { Id = id, User = user });

                _activeExports[id].Data = data;
                _activeExports[id].IsFinished = true;
                _activeExports[id].ErrorObject = errors;
            }
            finally
            {
                _activeExportsLock.ExitWriteLock();
            }
        }

        public dynamic GetExportData(string id)
        {
            _activeExportsLock.EnterReadLock();
            try
            {
                ActivityToken token = null;
                if (_activeExports != null)
                    _activeExports.TryGetValue(id, out token);

                return (token?.Data as string) ?? "";
            }
            finally
            {
                _activeExportsLock.ExitReadLock();
            }
        }

        public dynamic GetExportErrors(string id)
        {
            _activeExportsLock.EnterReadLock();
            try
            {
                ActivityToken token = null;
                if (_activeExports != null)
                    _activeExports.TryGetValue(id, out token);

                return token?.ErrorObject as dynamic;
            }
            finally
            {
                _activeExportsLock.ExitReadLock();
            }
        }
        #endregion


        #region Utilities
        public void ClearOldData()
        {
            // check time passed
            _lastTimeDeletedLock.EnterReadLock();
            var difference = DateTime.Now - _lastTimeIDeletedFiles;
            _lastTimeDeletedLock.ExitReadLock();


            // delete if proper time passed
            if (difference.TotalHours > _hoursToKeepData) // delete only if number of hours has passed
            {
                _lastTimeDeletedLock.EnterWriteLock();
                try
                {
                    var newChangedMaybeDifference = DateTime.Now - _lastTimeIDeletedFiles;
                    if (newChangedMaybeDifference.TotalHours > _hoursToKeepData)
                    {
                        var dateToDelete = DateTime.Now.AddHours(-1 * _hoursToKeepData);
                        foreach (var filePath in _settings.SavedDataDirectories)
                        {
                            var fullPath = Path.Combine(_contentPath, filePath);
                            // trigger file delete, don't wait for them to finish
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                            _fileHandler.DeleteFilesOlderThanDateSilentlyAsync(fullPath, dateToDelete);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        }

                        _lastTimeIDeletedFiles = DateTime.Now;
                    }
                }
                finally
                {
                    _lastTimeDeletedLock.ExitWriteLock();
                }
            }
        }

        public void ClearTimeoutTrigger(string id)
        {
            _activeTimersLock.EnterWriteLock();
            try
            {
                if (_activeTimers != null && _activeTimers.ContainsKey(id))
                {
                    // stop timer too (otherwise might still execute)
                    if (_activeTimers[id] != null)
                        _activeTimers[id].Dispose();

                    _activeTimers[id] = null;
                    _activeTimers.Remove(id);
                }
            }
            finally
            {
                _activeTimersLock.ExitWriteLock();
            }
        }

        public void Dispose()
        {
            // dispose timers
            _activeTimersLock.EnterWriteLock();
            try
            {
                if (_activeTimers != null)
                {
                    foreach (var tmr in _activeTimers.Keys)
                        _activeTimers[tmr]?.Dispose();

                    _activeTimers.Clear();
                }
                _activeTimers = null;
            }
            finally
            {
                _activeTimersLock.ExitWriteLock();
            }

            // dispose active calculations
            _activeCalculationsLock.EnterWriteLock();
            try
            {
                if (_activeCalculations != null)
                {
                    _activeCalculations.Clear();
                }
                _activeCalculations = null;
            }
            finally
            {
                _activeCalculationsLock.ExitWriteLock();
            }

            // dispose active exports
            _activeExportsLock.EnterWriteLock();
            try
            {
                if (_activeExports != null)
                {
                    _activeExports.Clear();
                }
                _activeExports = null;
            }
            finally
            {
                _activeExportsLock.ExitWriteLock();
            }

            // dispose locks
            if (_activeCalculationsLock != null)
                _activeCalculationsLock.Dispose();
            if (_activeTimersLock != null)
                _activeTimersLock.Dispose();
            if (_activeExportsLock != null)
                _activeExportsLock.Dispose();

            _activeCalculationsLock = null;
            _activeTimersLock = null;
            _activeExportsLock = null;
        }
        #endregion
    }
}
