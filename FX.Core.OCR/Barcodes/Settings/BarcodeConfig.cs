using System;
using System.Collections.Generic;
using System.Text;
using ZXing;

namespace FX.Core.OCR.Barcodes.Settings
{
    class BarcodeConfig : ICloneable
    {
        /// <summary>
        /// Specifies the full path to the file to be processed
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// True if the algorithm should try  to detect multiple barcodes in the image (image contains multiple recognizable and usable barcodes). Defaults to false;
        /// </summary>
        public bool TryMultipleBarcodeTypes { get; set; }

        /// <summary>
        /// Specify a value only if you want to filter and accept only a set of Barcode Formats. If null, includes all. Defaults to null.
        /// </summary>
        public IList<BarcodeFormat> PossibleFormats { get; set; }


        public BarcodeConfig()
        {

        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
