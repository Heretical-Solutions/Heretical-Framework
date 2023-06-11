using System;

using HereticalSolutions.Delegates;

namespace HereticalSolutions.Time
{
    public interface IPersistentTimer : ITimer
    {
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

        void Reset(TimeSpan duration);

        void Start(TimeSpan duration);

        #endregion
        
        #region Callbacks

        INonAllocSubscribableSingleArgGeneric<IPersistentTimer> OnStart { get; }
        
        INonAllocSubscribableSingleArgGeneric<IPersistentTimer> OnFinish { get; }

        #endregion
    }
}