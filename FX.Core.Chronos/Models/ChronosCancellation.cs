using FX.Core.Chronos.Abstract;
using System;
using System.Threading;

namespace FX.Core.Chronos.Models
{
    public class ChronosCancellation
    {
        private CancellationToken _requestToken;
        private IChronosDataWrapper _chronosRef;
        private readonly string _actionGuid;

        public ChronosCancellation() { }

        public ChronosCancellation(string guid, IChronosDataWrapper chronosRef, CancellationToken requestToken)
        {
            _actionGuid = guid;
            _chronosRef = chronosRef;
            _requestToken = requestToken;
        }

        public void ThrowIfCancelled()
        {
            // first throw if page was closed
            if (_requestToken != null)
            {
                try
                {
                    _requestToken.ThrowIfCancellationRequested();
                }
                catch (Exception)
                {
                    // clean used data
                    GC.Collect();

                    // stop parent
                    throw new ChronosCancelledException("Execution cancelled.");
                }
            }

            // 2nd throw if user pressed stop
            if (_chronosRef == null || string.IsNullOrWhiteSpace(_actionGuid))
                return;

            if (!_chronosRef.IsCalculationRunning(_actionGuid))
            {
                // clean used data
                GC.Collect();

                // stop parent
                throw new ChronosCancelledException("Execution cancelled.");
            }
        }
    }
}
