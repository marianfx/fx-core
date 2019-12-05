using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace FX.Core.OCR.Barcodes.Utils
{
    public class TiffUtils
    {
        /*
         * Combination of :
         * - this: https://stackoverflow.com/questions/33767100/c-how-do-i-convert-a-multi-page-tiff-via-memorystream-into-one-long-image
         * - this: https://www.ryadel.com/en/multipage-tiff-files-asp-net-c-sharp-gdi-alternative/
         */
        public static List<Bitmap> GetBitmapsFromTiff(string filename)
        {
            var output = new List<Bitmap>();
            var tiffImage = Image.FromFile(filename);
            if (tiffImage == null || tiffImage.FrameDimensionsList == null || tiffImage.FrameDimensionsList.Length <= 0)
                return new List<Bitmap>();

            var objectGuid = tiffImage.FrameDimensionsList[0];

            var dimensin = new FrameDimension(objectGuid);
            var nPages = tiffImage.GetFrameCount(dimensin);

            foreach (var guid in tiffImage.FrameDimensionsList)
            {
                var currentFrame = new FrameDimension(guid);
                for (var i = 0; i < nPages; i++)
                {
                    var outStream = new MemoryStream();
                    tiffImage.SelectActiveFrame(currentFrame, i);
                    tiffImage.Save(outStream, ImageFormat.Bmp);

                    var bitmap = new Bitmap(outStream);
                    output.Add(bitmap);
                }
            }

            return output;
        }
    }
}
