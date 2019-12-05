using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FX.Core.OCR.Barcodes.Transformer.Models
{
    public class OutputStatistics
    {
        public IList<StandardFileInformation> Files { get; set; } = new List<StandardFileInformation>();
        public Dictionary<string, List<StandardPageInformation>> Documents { get; set; } = new Dictionary<string, List<StandardPageInformation>>();
        public IList<string> DocumentsMetadata { get; set; } = new List<string>();
        public IList<StandardPageInformation> AllPages { get; set; } = new List<StandardPageInformation>();

        public IList<StandardPageInformation> Duplicates { get; set; } = new List<StandardPageInformation>();

        public IList<StandardPageInformation> UnidentifiedPages => AllPages.Where(x => string.IsNullOrWhiteSpace(x.DocumentName)).ToList();
        public int FilesProcessed => Files?.Count ?? 0;
        public double SuccessRate
        {
            get
            {
                return 1.0 * (AllPages.Count - UnidentifiedPages.Count) / AllPages.Count;
            }
        }
        public TimeSpan TotalProcessTime { get; set; }
        public int ThreadsUsed { get; set; }
        public TimeSpan TotalProcessTimeNoThreads
        {
            get
            {
                return new TimeSpan((long)Files.Sum(x => x.Duration.Ticks));
            }
        }

        public TimeSpan AvgPerFile
        {
            get
            {
                return new TimeSpan((long)Files.Average(x => x.Duration.Ticks));
            }
        }

        public TimeSpan AvgPerPage
        {
            get
            {
                return new TimeSpan((long)AllPages.Average(x => x.Duration.Ticks));
            }
        }
    }
}
