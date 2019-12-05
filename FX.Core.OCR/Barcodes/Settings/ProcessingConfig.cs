using System;
using System.Collections.Generic;
using ZXing;

namespace FX.Core.OCR.Barcodes.Settings
{
    class ProcessingConfig
    {
        /// <summary>
        /// Must represent a path to a directory where the files will be read for. Will scann all directory for files with [AcceptedFileExtensions] extension
        /// </summary>
        public string InputDirectory { get; set; }

        /// <summary>
        /// Must represent a path to a directory where the files will be written in
        /// </summary>
        public string OutputDirectory { get; set; }


        /// <summary>
        /// A list with the accepted file extensions. Filters (case insensitive) the input directory by these file types.
        /// By default: *.tif/*.tiff
        /// </summary>
        public IList<string> AcceptedFileExtensions { get; set; } = new List<string>()
        {
            ".tiff", ".tif"
        };

        /// <summary>
        /// How many threads to use for image parallel processing.
        /// </summary>
        public int NumberOfThreads { get; set; } = 4;

        /// <summary>
        /// True if the algorithm should try  to detect multiple barcodes in the image (image contains multiple recognizable and usable barcodes). Defaults to false;
        /// </summary>
        public bool TryMultipleBarcodeTypes { get; set; }

        /// <summary>
        /// Specify a value only if you want to filter and accept only a set of Barcode Formats. If null, includes all. Defaults to null.
        /// </summary>
        public IList<BarcodeFormat> PossibleFormats { get; set; }
    }
}
