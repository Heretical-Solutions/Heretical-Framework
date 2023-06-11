using HereticalSolutions.Delegates;

namespace HereticalSolutions.Time
{
    public interface IRuntimeTimer : ITimer
    {
        #region Countdown and Time elapsed

        float TimeElapsed { get; }
        
        float Countdown { get; }

        #endregion

        #region Duration

        float CurrentDuration { get; }

        float DefaultDuration { get; }

        #endregion

        #region Controls

        void Reset(float duration);

        void Start(float duration);

        #endregion
        
        #region Callbacks

        INonAllocSubscribableSingleArgGeneric<IRuntimeTimer> OnStart { get; }
        
        INonAllocSubscribableSingleArgGeneric<IRuntimeTimer> OnFinish { get; }

        #endregion
    }
}