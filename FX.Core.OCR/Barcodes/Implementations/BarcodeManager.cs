using FX.Core.OCR.Barcodes.Abstract;
using FX.Core.OCR.Barcodes.Models;
using FX.Core.OCR.Barcodes.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FX.Core.OCR.Barcodes.Implementations
{
    class BarcodeManager : IBarcodeManager
    {

        private readonly ProcessingConfig _config;
        private readonly IBitmapReader _bitmapReader;

        public BarcodeManager(IOptions<ProcessingConfig> options, IBitmapReader bitmapReader)
        {
            _config = options.Value;
            _bitmapReader = bitmapReader;
        }

        public IEnumerable<DecodeStatistics> ProcessAllFiles()
        {
            if (string.IsNullOrWhiteSpace(_config.InputDirectory) ||
                !Directory.Exists(_config.InputDirectory))
                throw new Exception("Invalid input directory");

            var startTime = DateTime.Now.Ticks;
            var fileList = Directory.GetFiles(_config.InputDirectory)
                .Where(file => _config.AcceptedFileExtensions.Any(ext => file.ToLower().EndsWith(ext)))
                .ToList();

            // start processing
            var currentState = 0;
            var errors = new List<string> { "One or more exceptions occured. " };
            var errorsTemp = new List<string>();
            var generalConfig = new BarcodeConfig()
            {
                PossibleFormats = _config.PossibleFormats,
                TryMultipleBarcodeTypes = _config.TryMultipleBarcodeTypes,
            };

            var allResults = new List<DecodeStatistics>();
            var _accessLock = new ReaderWriterLockSlim();

            while (true)
            {
                var files = fileList.Skip(currentState).Take(_config.NumberOfThreads);
                if (!files.Any())
                    break;

                var tasks = new List<Task>();
                foreach (var file in files)
                {
                    var taskHere = Task.Run(() =>
                    {
                        try
                        {
                            var barcodeConfig = generalConfig.Clone() as BarcodeConfig;
                            barcodeConfig.FilePath = file;
                            var options = Options.Create(barcodeConfig);

                            var decoder = new BarcodeDecoder(options, _bitmapReader);
                            decoder.StartDecoding();
                            var result = decoder.GetDecodeResult();

                            _accessLock.EnterWriteLock();
                            allResults.Add(result);
                            _accessLock.ExitWriteLock();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error on file '" + file + "': " + ex.Message);
                        }
                    });
                    tasks.Add(taskHere);
                }

                try
                {
                    Task.WaitAll(tasks.ToArray());
                }
                catch (AggregateException ae)
                {
                    if (ae.InnerExceptions != null)
                        errorsTemp.AddRange(ae.InnerExceptions.Select(x => x.Message));
                }
                finally
                {
                    currentState += files.Count();
                }
            }

            if (errorsTemp.Count > 0)
            {
                errors.AddRange(errorsTemp);
            }
            _accessLock.Dispose();
            return allResults;
        }
    }
}
