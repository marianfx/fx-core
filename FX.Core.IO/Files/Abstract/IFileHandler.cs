using FX.Core.IO.Files.Enums;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace FX.Core.IO.Files.Abstract
{
    public interface IFileHandler : IFileHandlerSimple
    {
        /// <summary>
        /// Returns always the current processed file path;
        /// </summary>
        string CurrentFileName { get; set; }

        /// <summary>
        /// Given a file path, validates if the path is valid and the file exists.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="parentDirectory"></param>
        /// <returns></returns>
        FileInfo ValidateFile(string relativePath, string parentDirectory);

        /// <summary>
        /// Copies a file
        /// </summary>
        /// <param name="fromPath"></param>
        /// <param name="toPath"></param>
        void CopyFile(string fromPath, string toPath);

        /// <summary>
        /// Returns a file stream based on parameters.
        /// </summary>
        /// <param name="relativePath">The file relative path.</param>
        /// <param name="fileType">The file type</param>
        /// <param name="parentDirectory">The path of the server containing directory (if exists)</param>
        /// <returns></returns>
        FileStream GetFileStream(string relativePath, FileTypes fileType, string parentDirectory);

        /// <summary>
        /// Given a Form File collection (presumably from a Request), saves the first file to the path specified.
        /// Throws error if no files are specified.
        /// </summary>
        /// <param name="formFiles"></param>
        /// <param name="relativePath"></param>
        /// <param name="parentDirectory"></param>
        Task SaveFileFromForm(IFormFileCollection formFiles, string relativePath, string parentDirectory);
    }
}
