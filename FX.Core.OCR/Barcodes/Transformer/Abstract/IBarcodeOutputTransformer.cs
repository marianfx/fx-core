using FX.Core.OCR.Barcodes.Transformer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FX.Core.OCR.Barcodes.Transformer.Abstract
{
    interface IBarcodeOutputTransformer<T>
    {
        /// <summary>
        /// Transform the decoded data and statistics to another type of output
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        T Transform(TransformationInput input);
    }
}
