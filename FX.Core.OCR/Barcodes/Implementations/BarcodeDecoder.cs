using FX.Core.OCR.Barcodes.Models;
using FX.Core.OCR.Barcodes.Settings;
using FX.Core.OCR.Barcodes.Utils;
using FX.Core.OCR.Barcodes.Transformer.Implementations;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;
using FX.Core.OCR.Barcodes.Abstract;

namespace FX.Core.OCR.Barcodes.Implementations
{
    class BarcodeDecoder : IBarcodeDecoder
    {
        private readonly BarcodeConfig _config;
        private readonly ZXing.BarcodeReader barcodeReader;
        private long _timeStart;
        private readonly IBitmapReader _bitmapReader;

        private readonly IList<DecodeResult> results;
        private DecodeStatistics statistics;

        public BarcodeDecoder(IOptions<BarcodeConfig> optionsAccessor, IBitmapReader bitmapReader)
        {
            _config = optionsAccessor.Value;
            _bitmapReader = bitmapReader;
            barcodeReader = new ZXing.BarcodeReader
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new DecodingOptions
                {
                    TryHarder = true,
                    PossibleFormats = _config.PossibleFormats
                }
            };
            results = new List<DecodeResult>();
        }

        public void StartDecoding()
        {
            _timeStart = DateTime.Now.Ticks;
            if (!File.Exists(_config.FilePath))
                throw new Exception("File not found!");

            // initialize with list of images based on format
            var listOfBitmaps = _bitmapReader.ReadBitmaps(_config.FilePath);
            statistics = new DecodeStatistics() { PreprocessDuration = new TimeSpan(DateTime.Now.Ticks - _timeStart) };
            Decode(listOfBitmaps);
        }


        private void Decode(IEnumerable<Bitmap> bitmaps)
        {
            _timeStart = DateTime.Now.Ticks;
            results.Clear();
            var fileName = new FileInfo(_config.FilePath).Name;

            var i = 1;
            foreach (var bitmap in bitmaps)
            {
                var singleStart = DateTime.Now.Ticks;

                // https://github.com/micjahn/ZXing.Net/issues/132 requires bindings package
                // as here: https://stackoverflow.com/questions/28561772/c-sharp-with-zxing-net-decoding-the-qr-code
                var imageAsLuminance = new BitmapLuminanceSource(bitmap);

                try
                {
                    var mappedResult = CallDecode(imageAsLuminance, singleStart);

                    var singleTime = new TimeSpan(DateTime.Now.Ticks - singleStart);
                    mappedResult.FileName = fileName;
                    mappedResult.Duration = singleTime;
                    mappedResult.PageInTiff = i;
                    results.Add(mappedResult);
                }
                catch (Exception)
                {
                    string message = "Decoder failed with read exception.";
                    var singleTime = new TimeSpan(DateTime.Now.Ticks - singleStart);
                    var mappedResult = new DecodeResult();
                    mappedResult.Success = false;
                    mappedResult.Note = message;
                    mappedResult.FileName = fileName;
                    mappedResult.Duration = singleTime;
                    mappedResult.PageInTiff = i;
                    results.Add(mappedResult);
                }

                i++;
            }

            // save analysis
            var timerStop = DateTime.Now.Ticks;
            statistics.FileName = fileName;
            statistics.ExecuteDuration = new TimeSpan(timerStop - _timeStart);
            statistics.Results = results;

            if (results == null)
            {
                statistics.Note = "No barcode found in image";
            }
        }

        private DecodeResult CallDecode(BitmapLuminanceSource imageAsLuminance, long singleStart)
        {
            var tempResults = new List<Result>();
            if (_config.TryMultipleBarcodeTypes)
            {
                tempResults = barcodeReader.DecodeMultiple(imageAsLuminance)?.ToList() ?? new List<Result>();
            }
            else
            {
                var result = barcodeReader.Decode(imageAsLuminance);
                tempResults.Add(result);
            }

            var mappedResult = new DecodeResult();
            if (tempResults.Count > 0)
                mappedResult = new DecodeResult(tempResults[0]);

            // search for the correct one (that works) if multiple results
            foreach (var item in tempResults)
            {
                if (item == null) continue;

                var encodedData = StandardOutputTransformer.RetrieveNameAndPages(item.Text);
                if (!string.IsNullOrWhiteSpace(encodedData.Item1) && encodedData.Item2 != 0 && encodedData.Item3 != 0)
                {
                    mappedResult = new DecodeResult(item);
                    break;
                }
            }

            return mappedResult;
        }

        public DecodeStatistics GetDecodeResult()
        {
            if (statistics == null)
                throw new Exception("Decoding not yet started, no results available!");

            return statistics;
        }

    }
}
