using System.Collections.Generic;

namespace FX.Core.Chronos.Models
{
    public class ActivityToken
    {
        public string Id { get; set; }
        public string User { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public dynamic Data { get; set; }
        public dynamic ErrorObject { get; set; }
        public bool IsFinished { get; set; } = false;
        public bool IsSuccess { get; set; } = false;
        public IList<string> Log { get; set; } = new List<string>();

        public ActivityToken()
        {
            Id = string.Empty;
            Name = "Unknown";
            Type = "Unknown";
            User = string.Empty;
            Data = null;
            IsFinished = false;
            Log = new List<string>();
        }
    }
}
