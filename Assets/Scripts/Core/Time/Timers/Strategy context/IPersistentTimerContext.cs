using System;

using HereticalSolutions.Delegates;

namespace HereticalSolutions.Time
{
    public interface IPersistentTimerContext : ITimerWithState
    {
        #region Variables

        DateTime StartTime { get; set; }
        
        DateTime EstimatedFinishTime { get; set; }

        TimeSpan SavedProgress { get; set; }
        
        #endregion
        
        #region Duration
        
        TimeSpan CurrentDurationSpan { get; set; }
        
        TimeSpan DefaultDurationSpan { get; set; }
        
        #endregion

        #region Controls
        
        bool Accumulate { get; }

        bool Repeat { get; }
        
        #endregion

        #region Publishers
        
        IPublisherSingleArgGeneric<IPersistentTimer> OnStartAsPublisher { get; }
        
        IPublisherSingleArgGeneric<IPersistentTimer> OnFinishAsPublisher { get; }
        
        #endregion
    }
}