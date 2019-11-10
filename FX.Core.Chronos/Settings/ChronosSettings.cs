using System.Collections.Generic;

namespace FX.Core.Chronos.Settings
{
    public class ChronosSettings
    {
        public int HoursToKeepData { get; set; } = 24;
        public IEnumerable<string> SavedDataDirectories { get; set; } = new List<string>();
    }
}
