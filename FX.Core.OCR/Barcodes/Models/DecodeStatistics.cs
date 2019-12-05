using System;
using System.Collections.Generic;
using System.Linq;

namespace FX.Core.OCR.Barcodes.Models
{
    public class DecodeStatistics
    {
        public string FileName { get; set; }
        public IList<DecodeResult> Results { get; set; }
        public TimeSpan PreprocessDuration { get; set; }
        public TimeSpan ExecuteDuration { get; set; }
        public bool Success
        {
            get
            {
                return Results != null && Results.Any() && Results.All(x => x.Success);
            }
        }
        public string Note { get; set; }
    }
}
