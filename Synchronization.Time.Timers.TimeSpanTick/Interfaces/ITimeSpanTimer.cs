using System;

namespace HereticalSolutions.Synchronization.Time.Timers.TimeSpanTick
{
    public interface ITimeSpanTimer
        : ITimer
    {
        ITimeSpanTimerContext Context { get; }

        #region Timer state

        DateTime StartTime { get; }

        DateTime EstimatedFinishTime { get; }

        #endregion

        #region Countdown and Time elapsed

        TimeSpan TimeElapsedSpan { get; }
        
        TimeSpan CountdownSpan { get; }

        #endregion

        #region Duration

        TimeSpan CurrentDurationSpan { get; }

        TimeSpan DefaultDurationSpan { get; }

        #endregion

        #region Controls

        void Reset(
            TimeSpan duration);

        void Start(
            TimeSpan duration);

        #endregion
    }
}