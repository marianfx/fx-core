using FX.Core.Common.Localization;
using FX.Core.Common.Validations.Abstract;
using FX.Core.IO.Files.Abstract;
using FX.Core.IO.Files.Enums;
using FX.Core.IO.Files.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FX.Core.IO.Files.Implementations
{
    public class FileHandler : FileHandlerSimple, IFileHandler
    {
        private readonly IValidator _validator;
        private readonly FileHandlerSettings _settings;

        public string RootPath { get; set; }
        public string CurrentFileName { get; set; } = "";

        FileHandler(IValidator validator, IOptions<FileHandlerSettings> settings, string rootPath)
        {
            _validator = validator;
            _settings = settings.Value;
            RootPath = rootPath;
        }

        public FileHandler(IValidator validator, IOptions<FileHandlerSettings> settings, IHostingEnvironment environment): this(validator, settings, environment.ContentRootPath)
        {
        }

        public FileInfo ValidateFile(string relativePath, string parentDirectory)
        {
            var errors = _validator.ValidateString(relativePath, Resources.File).GetErrors();
            if (errors.Count > 0)
                throw new Exception(errors[0]);

            var serverPath = Path.Combine(RootPath, parentDirectory);
            var fullStreamFile = Path.Combine(serverPath, relativePath);
            var fileInfo = new FileInfo(fullStreamFile);
            if (!fileInfo.Exists)
                throw new Exception(Resources.FileDoesNotExist);

            return fileInfo;
        }

        public void CopyFile(string fromPath, string toPath)
        {
            var fromServerPath = Path.Combine(RootPath, fromPath);
            var toServerPath = Path.Combine(RootPath, toPath);

            var fileInfoFrom = new FileInfo(fromServerPath);
            if (!fileInfoFrom.Exists)
                throw new Exception("Source " + Resources.FileDoesNotExist);

            if (File.Exists(toServerPath))
                File.Delete(toServerPath);

            var fi = new FileInfo(toServerPath);
            if (!fi.Directory.Exists)
                fi.Directory.Create();

            try
            {
                File.Copy(fromServerPath, toServerPath);
            }
            catch (Exception ex)
            {
                throw new Exception(Resources.FileCannotBeCopied + " " + ex.Message);
            }
        }

        public async Task CopyFileAsync(string fromPath, string toPath)
        {
            var fromServerPath = Path.Combine(RootPath, fromPath);
            var toServerPath = Path.Combine(RootPath, toPath);

            var fileInfoFrom = new FileInfo(fromServerPath);
            if (!fileInfoFrom.Exists)
                throw new Exception("Source " + Resources.FileDoesNotExist);

            if (File.Exists(toServerPath))
                File.Delete(toServerPath);

            var fi = new FileInfo(toServerPath);
            if (!fi.Directory.Exists)
                fi.Directory.Create();

            try
            {
                using (var sourceStream = File.Open(fromServerPath, FileMode.Open))
                {
                    using (var destinationStream = new FileStream(toServerPath, FileMode.Create))
                    {
                        await sourceStream.CopyToAsync(destinationStream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Resources.FileCannotBeCopied + " " + ex.Message);
            }
        }


        #region Streams (sending to CLIENT)
        public FileStream GetFileStream(string relativePath, FileTypes fileType, string parentDirectory)
        {
            if (!_settings.FileMimeTypes.Keys.Contains(fileType))
                throw new Exception(Resources.UnknownFileType);

            var fileInfo = ValidateFile(relativePath, parentDirectory);
            CurrentFileName = fileInfo.Name;
            return new FileStream(fileInfo.FullName, FileMode.Open);
        }
        #endregion

        #region Saving from FORM
        public async Task SaveFileFromForm(IFormFileCollection formFiles, string relativePath, string parentDirectory)
        {
            if (formFiles == null || formFiles.Count == 0 || formFiles[0].Length == 0)
                throw new Exception(Resources.NoFileSpecified);

            var file = formFiles[0];
            var serverPath = Path.Combine(RootPath, parentDirectory);
            try
            {
                if (!Directory.Exists(serverPath))
                    Directory.CreateDirectory(serverPath);

                var filePath = Path.Combine(serverPath, relativePath);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Resources.CannotAccessTheRequestedPath, ex);
            }
        }
        #endregion
    }
}
