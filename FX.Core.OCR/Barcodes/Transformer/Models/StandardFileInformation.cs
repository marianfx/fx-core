using System;
using System.Collections.Generic;
using System.Text;

namespace FX.Core.OCR.Barcodes.Transformer.Models
{
    public class StandardFileInformation
    {
        public string FileName { get; set; }
        public int NumberOfPages { get; set; }
        public int NumberOfDocuments { get; set; }
        public int NumberOfPagesDetected { get; set; }
        public TimeSpan Duration { get; set; }
        public string Note { get; set; }

        public override string ToString()
        {
            return $"{NumberOfPages} images, {NumberOfDocuments} docs";
        }
    }
}
