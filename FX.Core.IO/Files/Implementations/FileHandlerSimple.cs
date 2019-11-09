using FX.Core.IO.Files.Abstract;
using FX.Core.IO.Files.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FX.Core.IO.Files.Implementations
{
    public class FileHandlerSimple : IFileHandlerSimple
    {
        public IEnumerable<FileInfo> GetFilesFromDirectory(string directoryPath, FileFilters filters = null)
        {
            if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
                throw new Exception("Invalid directory path");

            var filesQuery = Directory.GetFiles(directoryPath, filters.SearchPattern, filters.SearchOption)
                    .Select(f => new FileInfo(f));
            
            if (filters.DateFilterType != DateFilterType.None)
            {
                filesQuery = filters.DateFilterType == DateFilterType.Older
                    ? filesQuery.Where(f => f.CreationTime < filters.DateFilter)
                    : filters.DateFilterType == DateFilterType.Newer
                        ? filesQuery.Where(f => f.CreationTime > filters.DateFilter)
                        : filesQuery.Where(f => f.CreationTime == filters.DateFilter);
            }


            if (filters.NamesToIgnore != null)
                filesQuery = filesQuery.Where(f => !filters.NamesToIgnore.Contains(f.Name));

            return filesQuery.ToList();
        }

        public void DeleteFileSilently(string fullPath)
        {
            try
            {
                File.Delete(fullPath);
            }
            catch (Exception)
            {
                // Console.WriteLine(ex);
            }
        }

        public void DeleteFilesSilently(IEnumerable<string> allPaths)
        {
            foreach (var path in allPaths)
            {
                DeleteFileSilently(path);
            }
        }

        public void DeleteFilesOlderThanDateSilently(string directoryPath, DateTime date, IEnumerable<string> fileNamesToIgnore = null)
        {
            var filesToDelete = GetFilesFromDirectory(directoryPath, new FileFilters
            {
                DateFilterType = DateFilterType.Older,
                DateFilter = date,
                NamesToIgnore = fileNamesToIgnore
            });

            var fileNamesToDelete = filesToDelete.Select(x => x.FullName);
            DeleteFilesSilently(fileNamesToDelete);
        }


        public async Task DeleteFileSilentlyAsync(string fullPath)
        {
            await Task.Run(() =>
            {
                DeleteFileSilently(fullPath);
            });
        }

        public async Task DeleteFilesSilentlyAsync(IEnumerable<string> allPaths)
        {
            await Task.Run(() =>
            {
                DeleteFilesSilently(allPaths);
            });
        }


        public async Task DeleteFilesOlderThanDateSilentlyAsync(string directoryPath, DateTime date, IEnumerable<string> fileNamesToIgnore = null)
        {
            var filesToDelete = GetFilesFromDirectory(directoryPath, new FileFilters
            {
                DateFilterType = DateFilterType.Older,
                DateFilter = date,
                NamesToIgnore = fileNamesToIgnore
            });

            var fileNamesToDelete = filesToDelete.Select(x => x.FullName);
            await DeleteFilesSilentlyAsync(fileNamesToDelete);
        }

    }
}
