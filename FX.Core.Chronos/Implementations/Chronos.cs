using FX.Core.Chronos.Abstract;
using System;
using System.Timers;

namespace FX.Core.Chronos.Real
{
    public class Chronos<T, TResult>: IChronos<T, TResult>, IDisposable
    {
        private Timer _timer;
        private bool _oneTimeOnly = true;
        private Func<T, TResult> _action;
        private T _functionData;


        public Chronos()
        {
            _timer = new Timer();
        }

        public void SetTimeout(Func<T, TResult> toExecute, TimeSpan interval, T data, bool oneTimeOnly = true)
        {
            _action = toExecute;
            _oneTimeOnly = oneTimeOnly;
            _functionData = data;

            if (_timer == null)
                _timer = new Timer();

            StopTimer();
            _timer.Interval = interval.TotalMilliseconds;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        public void StopTimer()
        {
            try
            {
                _timer.Stop();
            }
            catch { }
        }

        public void Dispose()
        {
            StopTimer();
            if (_timer != null)
                _timer.Dispose();

            _timer = null;
        }

        /// <summary>
        /// The event that is executed each time the timer triggers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_oneTimeOnly)
                StopTimer();

            _action?.Invoke(_functionData);
        }
    }
}
