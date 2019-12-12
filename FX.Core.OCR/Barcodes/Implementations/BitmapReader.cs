using FX.Core.OCR.Barcodes.Abstract;
using FX.Core.OCR.Barcodes.Settings;
using FX.Core.OCR.Barcodes.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FX.Core.OCR.Barcodes.Implementations
{
    public class BitmapReader : IBitmapReader
    {
        public List<Bitmap> ReadBitmaps(string filePath)
        {
            bool isTff = FileUtils.IsTiff(filePath);
                return isTff ? TiffUtils.GetBitmapsFromTiff(filePath) : new List<Bitmap>() { (Bitmap)Bitmap.FromFile(filePath) };
        }
    }
}
