using FX.Core.OCR.Barcodes.Implementations;
using FX.Core.OCR.Barcodes.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FX.Core.OCR.Barcodes.Abstract
{
    public interface IBitmapReader
    {
        /// <summary>
        /// Returns a list with one image if the file at the path is not .tif/ .tiff or a list with multiple images if it is.
        /// </summary>
        /// <param name="filePath">Path to the file to be verified</param>
        /// <returns></returns>
        List<Bitmap> ReadBitmaps(string filePath);
    }
}
