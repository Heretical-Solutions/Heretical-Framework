namespace HereticalSolutions.Synchronization.Time.Timers.FloatDelta
{
    public class FloatInactiveState
        : IFloatTimerState
    {
        #region Progress

        public float GetProgressNormal(
            IFloatTimerContext context)
        {
            return 0f;
        }

        #endregion

        #region Countdown and Time elapsed

        public float GetTimeElapsed(
            IFloatTimerContext context)
        {
            return 0f;
        }

        public float GetCountdown(
            IFloatTimerContext context)
        {
            return 0f;
        }

        #endregion

        #region Controls

        public void Reset(
            IFloatTimerContext context)
        {
            context.CurrentTimeElapsed = 0f;
            context.CurrentDuration = context.DefaultDuration;
        }

        public void Start(
            IFloatTimerContext context)
        {
            context.CurrentTimeElapsed = 0f;

            context.SetState(ETimerState.STARTED);

            context.OnStart.Publish(context.Timer);
        }

        public void Pause(
            IFloatTimerContext context)
        {
            //Why bother?
        }

        public void Resume(
            IFloatTimerContext context)
        {
            //Why bother?
        }

        public void Abort(
            IFloatTimerContext context)
        {
            context.CurrentTimeElapsed = 0f;
        }

        public void Finish(
            IFloatTimerContext context)
        {
            //ENSURE WHETHER CALLING FINISH() ON INACTIVE TIMER SHOULD NOT BE CALLING A CALLBACK
        }

        public void Tick(
            IFloatTimerContext context,
            float delta)
        {
            //Why bother?
        }

        #endregion
    }
}