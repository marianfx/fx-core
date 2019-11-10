using System;

namespace FX.Core.Chronos.Models
{
    public class ChronosCancelledException : Exception
    {
        public ChronosCancelledException() { }
        public ChronosCancelledException(string message) : base(message) { }
    }
}
