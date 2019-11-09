using System;
using System.Collections.Generic;
using System.IO;

namespace FX.Core.IO.Files.Models
{
    public enum DateFilterType { None, Older, Newer, Equal };

    public class FileFilters
    {
        public string SearchPattern { get; set; } = "*.*";
        public SearchOption SearchOption { get; set; } = SearchOption.AllDirectories;
        public DateFilterType DateFilterType { get; set; } = DateFilterType.None;
        public DateTime DateFilter { get; set; } = DateTime.Now;
        public IEnumerable<string> NamesToIgnore { get; set; }
    }
}
