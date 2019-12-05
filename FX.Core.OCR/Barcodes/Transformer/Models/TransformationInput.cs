using FX.Core.OCR.Barcodes.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FX.Core.OCR.Barcodes.Transformer.Models
{
    public class TransformationInput
    {
        public IList<DecodeStatistics> Results { get; set; }
        public TimeSpan Duration { get; set; }
        public IList<string> Errors { get; set; }
        public int ThreadsUsed { get; set; }
    }
}
