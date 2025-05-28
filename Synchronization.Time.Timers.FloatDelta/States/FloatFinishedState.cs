namespace HereticalSolutions.Synchronization.Time.Timers.FloatDelta
{
    public class FloatFinishedState
        : IFloatTimerState
    {
        #region Progress
        
        public float GetProgressNormal(
            IFloatTimerContext context)
        {
            //THIS ONE IS AS EXPECTED. IF THE TIMER WAS FINISHED PREMATURELY BY A FINISH() CALL RATHER THAN TIMER ACTUALLY RUNNING OUT WE MIGHT BE CURIOUS HOW MUCH OF A PROGRESS WAS MADE SO FAR
            if (context.Accumulate)
                return 0f;
            
            if ((context.CurrentDuration - MathHelpers.EPSILON) < 0f)
                return 0f;
                        
            return (context.CurrentTimeElapsed / context.CurrentDuration).Clamp(0f, 1f);
        }
        
        #endregion

        #region Countdown and Time elapsed

        public float GetTimeElapsed(
            IFloatTimerContext context)
        {
            return context.CurrentTimeElapsed;
        }
        
        public float GetCountdown(
            IFloatTimerContext context)
        {
            return context.CurrentDuration - context.CurrentTimeElapsed;
        }
        
        #endregion
        
        #region Controls
        
        public void Reset(
            IFloatTimerContext context)
        {
            context.CurrentTimeElapsed = 0f;

            context.CurrentDuration = context.DefaultDuration;
            
            context.SetState(ETimerState.INACTIVE);
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
            
            context.SetState(ETimerState.INACTIVE);
        }
        
        public void Finish(
            IFloatTimerContext context)
        {
            //Why bother?
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