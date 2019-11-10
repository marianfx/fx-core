using FX.Core.Chronos.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FX.Core.Chronos.Abstract
{
    /// <summary>
    /// Chronos Data Wrapper should be implemented by classes that intend to have 2 parts:
    /// 1. Have an algorithm that is processed and produces some data, identifiable by an ID
    /// 2. Have methods that save and retrieve that data, based on an IDataSerializer (maybe)
    /// 3. Have 3 stages: not started, calculation in progress, processed / not exported, export in progress, finished
    /// </summary>
    public interface IChronosDataWrapper: IDisposable
    {
        #region Handle Calculation Start / Stop
        /// <summary>
        /// Returns true if the data is in the status 'calculation in progress'. It means the ID exists in the system and it is in the first steps of being processed (does not exist yet as data or as exported data).
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        bool IsCalculationRunning(string guid);

        /// <summary>
        /// Sets a process's status (identified by the ID) as 'calculation in progress' (the stage before 'processed'); this should be called after a process is being started, to be marked as started.
        /// </summary>
        /// <param name="id">The ID of the process</param>
        /// <param name="user">The user identifier (the user that triggered the process)</param>
        /// <param name="data">The data attached to the process (as Activity Token)</param>
        void SetActiveCalculation(string id, string user = null, dynamic data = null);

        /// <summary>
        /// If there's a process identified by the 'ID' that is in progress, it is marked as stopped, with the possibility of stopping it for success or for failure (with a log)
        /// </summary>
        /// <param name="id">The ID of the process. if there's no process with this ID, the application does nothing</param>
        /// <param name="remove">Specifies if the calculation should be stopped and removed from the system of if it should be kept in the system, with a status and a log. If removing, it will not be retrievable with `Data Exists` and similar methods.</param>
        /// <param name="log"></param>
        /// <param name="isSuccess"></param>
        void StopCalculation(string id, bool remove = true, IList<string> log = null, bool isSuccess = false);

        /// <summary>
        /// Searches for the active log of a process identified by the specified ID (the 'activity log'). This exists only if the calculation is running or was Stopped (but kept in the system)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActivityToken GetCalculationToken(string id);
        #endregion

        #region Handling Data

        /// <summary>
        /// Tells if there is some data stored that is identifiable by the given ID).
        /// Searches through data of which state is 'calculated'.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool ContainsData(string id);

        /// <summary>
        /// Searches through data with state 'calculated' and returns (if if finds any) the one with the specified ID. The method does a serializing (data is stored in a storage mechanism, from where it's read and serialized in C# T object).
        /// </summary>
        /// <typeparam name="T">The type of the expected data</typeparam>
        /// <param name="id"></param>
        /// <returns>The serialized object</returns>
        Task<T> GetData<T>(string id) where T : class;

        /// <summary>
        /// This action is executed at the end of the 'processing' phase. It does the saving part (saves data, attaching it to an ID). The ID can be offered beforehand (as it's default is null) or it will be generated and returned.
        /// </summary>
        /// <param name="response">The data to save</param>
        /// <param name="identifier">The ID of the data (if offered beforehand)</param>
        /// <returns>The ID that identified the data in the storage mechanism and based on which it can be retrieved</returns>
        Task<string> SaveResponseAndSetDeleteTimeout(object response, string identifier = null);

        /// <summary>
        /// The method that removes a process (identified by the ID) from the system (stops active calculations, stops events, stops and clears exports and associated data).\
        /// It also triggers a 'Clear' method (that deletes data from storage, according to settings).
        /// </summary>
        /// <param name="id">The process / data identifier</param>
        /// <returns></returns>
        Task<bool> DeleteAssociatedData(string id);
        #endregion

        #region Handling Data Export (Save)

        /// <summary>
        ///  Returns true if the data is in the status 'export in progress'. It means the ID exists in the system and it has finished being processed, and now the 2nd phase of export is started
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool IsExportRunning(string id);

        /// <summary>
        /// Marks the process as being in the stage 'exporting'.
        /// </summary>
        /// <param name="id">The id of the data / process</param>
        /// <param name="user">The user that triggered the process. Not necessarly needed.</param>
        void SetActiveExport(string id, string user = null);

        /// <summary>
        /// If there's a process, identified by the id, in the stage 'export running', the run is stopped and the export cancelled.
        /// </summary>
        /// <param name="id"></param>
        void StopExport(string id);

        /// <summary>
        /// Markes an export process as finished, for the given ID (the final stage possible for a process). The process must exist as active in order to be stopped.
        /// </summary>
        /// <param name="id">The id of the process</param>
        /// <param name="data">The data to set for the export object</param>
        /// <param name="user">The user this export belongs to</param>
        /// <param name="errors">The errors / log that happened during the process</param>
        void MarkExportFinished(string id, dynamic data, string user = null, dynamic errors = null);

        /// <summary>
        /// Get the data that is set in the export's envelope; it will return something only if there's a process in the 'finished / exported' state.
        /// The returned data is dynamic, for example if exporting to file system, it can be a file name / path.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        dynamic GetExportData(string id);

        /// <summary>
        /// Gets the errors / log set in the export's envelope; it will return something only if there's a process in the 'finished / exported' state.
        /// The returned data is dynamic, can be a list of errors or an error object etc.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        dynamic GetExportErrors(string id);
        #endregion

        #region Utilities
        /// <summary>
        /// This method should cleared old used data that is not yet deleted, but is expired / will not be used anymore. The mechanism depends on tha implementation of the storage and should be based on loaded settings.
        /// </summary>
        void ClearOldData();

        /// <summary>
        /// The method stops the eventual triggers that are set up to delete data (the data is saved, and a timer to delete it after a period of time is automatically set in the SaveDataAndSetDeleteTimeout).
        /// This method should cancel the timer. Implementation may vary.
        /// If there's no process with the given ID, it does nothing.
        /// </summary>
        /// <param name="id"></param>
        void ClearTimeoutTrigger(string id);
        #endregion
    }
}
