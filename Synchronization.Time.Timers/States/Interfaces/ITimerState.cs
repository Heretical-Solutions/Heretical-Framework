namespace HereticalSolutions.Synchronization.Time.Timers
{
    public interface ITimerState<TStateContext, TTimeValue>
    {
        #region Progress
        
        float GetProgressNormal(
            TStateContext context);

        #endregion

        #region Countdown and Time elapsed
        
        TTimeValue GetTimeElapsed(
            TStateContext context);

        TTimeValue GetCountdown(
            TStateContext context);
        
        #endregion
        
        #region Controls
        
        void Reset(
            TStateContext context);

        void Start(
            TStateContext context);

        void Pause(
            TStateContext context);

        void Resume(
            TStateContext context);
        
        void Abort(
            TStateContext context);

        void Finish(
            TStateContext context);

        #endregion
    }
}