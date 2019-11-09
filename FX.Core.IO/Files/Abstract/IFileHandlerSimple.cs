using FX.Core.IO.Files.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FX.Core.IO.Files.Abstract
{
    public interface IFileHandlerSimple
    {
        /// <summary>
        /// Returns all the files from a directory, applying filters to the output if filters are specified.
        /// Can throw: directory invalid, IO exceptions
        /// </summary>
        /// <param name="directoryPath">The directory where to extract files from</param>
        /// <param name="filters">The filtering object. Defaults to null.</param>
        /// <returns></returns>
        IEnumerable<FileInfo> GetFilesFromDirectory(string directoryPath, FileFilters filters = null);

        /// <summary>
        /// Deletes a file, by catching all the possible exceptions.
        /// </summary>
        /// <param name="fullPath">The absolute path to the file</param>
        void DeleteFileSilently(string fullPath);

        /// <summary>
        /// Deletes a list of files, by catching all the possible exceptions.
        /// Exceptions are catched per file - so if a file fails to be deleted, it will try to delete the next one etc.
        /// </summary>
        /// <param name="allPaths">The list of absolute paths that represent files.</param>
        void DeleteFilesSilently(IEnumerable<string> allPaths);

        /// <summary>
        /// Selects all the files from the specified directory, that are older (strict) than the given date and their names are not included in the list of forbidden names.
        /// Silently means that if an exception occurs for a file, the process will continue with the next file.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="date"></param>
        /// <param name="fileNamesToIgnore"></param>
        void DeleteFilesOlderThanDateSilently(string directoryPath, DateTime date, IEnumerable<string> fileNamesToIgnore = null);


        /// <summary>
        /// Check 'DeleteFileSilently'
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        Task DeleteFileSilentlyAsync(string fullPath);

        /// <summary>
        /// Check 'DeleteFilesSilently'
        /// </summary>
        /// <param name="allPaths"></param>
        /// <returns></returns>
        Task DeleteFilesSilentlyAsync(IEnumerable<string> allPaths);

        /// <summary>
        /// Check 'DeleteFilesOlderThanDate'
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="date"></param>
        /// <param name="fileNamesToIgnore"></param>
        /// <returns></returns>
        Task DeleteFilesOlderThanDateSilentlyAsync(string directoryPath, DateTime date, IEnumerable<string> fileNamesToIgnore = null);
    }
}
