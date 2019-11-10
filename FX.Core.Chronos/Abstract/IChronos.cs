using System;

namespace FX.Core.Chronos.Abstract
{
    /// <summary>
    /// Represents a generic Timing Interface. Can Set a Timeout (repeatable, if needed) to execute a custom task, with a specific data passed as params, and can stop the timer from triggering.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IChronos<T, TResult>
    {
        /// <summary>
        /// Sets a new timeout (with the given interval).
        /// After the interval ends, it will execute the FUNC that is passed in the ToExecute method.
        /// By default, does this only one time (TIMEOUT behavior). If 'oneTimeOnly' is false, it can do it multiple times (INTERVAL behavior).
        /// </summary>
        /// <param name="toExecute"></param>
        /// <param name="interval"></param>
        /// <param name="data"></param>
        /// <param name="oneTimeOnly"></param>
        void SetTimeout(Func<T, TResult> toExecute, TimeSpan interval, T data, bool oneTimeOnly = true);

        /// <summary>
        /// Stops any future actions that the timer can do (if the action hasn't been triggered, it will not trigger).
        /// </summary>
        void StopTimer();
    }
}
