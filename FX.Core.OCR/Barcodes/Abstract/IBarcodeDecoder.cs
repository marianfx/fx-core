using FX.Core.OCR.Barcodes.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FX.Core.OCR.Barcodes.Abstract
{
    public interface IBarcodeDecoder
    {
        /// <summary>
        /// Starts the decoding process. The decoder must be initialized with the Barcode Configuration.
        /// The process includes:
        /// 1. Create the list of Images (Bit maps); it will be a single image or a set of images if the input is multi-page tiff or pdf
        /// 2. for each image from the set, try to decode it and obtain resunt data, using the specific implementation
        /// 3. Create the result set with statistic and result data.
        /// </summary>
        void StartDecoding();

        /// <summary>
        /// Returns the object that contains all the data about an image/image collection decode process.
        /// Available only after a decoding has been executed
        /// </summary>
        /// <returns></returns>
        DecodeStatistics GetDecodeResult();
    }
}
