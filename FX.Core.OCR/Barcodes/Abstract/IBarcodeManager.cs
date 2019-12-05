using FX.Core.OCR.Barcodes.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FX.Core.OCR.Barcodes.Abstract
{
    public interface IBarcodeManager
    {
        /// <summary>
        /// Starts the process of decoding all the given files following theese steps
        /// 1. Gets all the required files from directory
        /// 2. Selects a portion of those files (equal to the number of threads assigned) 
        /// 3. Starts the decoding process for the selected files
        /// 4. Moves to a new selection of files -> step 1
        /// </summary>
        /// <returns></returns>
        IEnumerable<DecodeStatistics> ProcessAllFiles();

    }
}
