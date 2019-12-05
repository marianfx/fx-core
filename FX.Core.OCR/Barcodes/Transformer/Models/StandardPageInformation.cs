using System;
using System.Collections.Generic;
using System.Text;

namespace FX.Core.OCR.Barcodes.Transformer.Models
{
    public class StandardPageInformation
    {
        public string FileName { get; set; }
        public string DocumentName { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public string BarcodeFormat { get; set; }
        public string BarcodeText { get; set; }
        public bool Success { get; set; }
        public string Note { get; set; }
        public TimeSpan Duration { get; set; }
        public int PageInTiff { get; set; }

        public override string ToString()
        {
            return Success
                ? $"OK: Page {PageNumber} / {TotalPages}"
                : $"FAIL: {Note} (Page {PageNumber}";
        }
    }
}
