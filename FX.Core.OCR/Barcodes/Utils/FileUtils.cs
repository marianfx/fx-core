using System.Drawing;
using System.IO;

namespace FX.Core.OCR.Barcodes.Utils
{
    public class FileUtils
    {
        /// <summary>
        /// Returns true if a file has .tif or .tiff extension (case insensitive)
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool IsTiff(string filename)
        {
            return filename != null
                && (filename.ToLower().EndsWith(".tif")
                || filename.ToLower().EndsWith(".tiff"));
        }

        /// <summary>
        /// Converts image to byte array
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static byte[] ImageToByte2(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                return stream.ToArray();
            }
        }
    }
}
