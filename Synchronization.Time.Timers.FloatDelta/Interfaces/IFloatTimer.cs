namespace HereticalSolutions.Synchronization.Time.Timers.FloatDelta
{
    public interface IFloatTimer
        : ITimer
    {
        IFloatTimerContext Context { get; }

        #region Countdown and Time elapsed

        float TimeElapsed { get; }
        
        float Countdown { get; }

        #endregion

        #region Duration

        float CurrentDuration { get; }

        float DefaultDuration { get; }

        #endregion

        #region Controls

        void Reset(
            float duration);

        void Start(
            float duration);

        void Resume(
            float duration);

        #endregion
    }
}