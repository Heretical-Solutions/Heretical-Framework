namespace HereticalSolutions.Time.Strategies
{
    public class RuntimeInactiveStrategy : ITimerStrategy<IRuntimeTimerContext, float>
    {
        #region Progress
        
        public float GetProgress(IRuntimeTimerContext context)
        {
            return 0f;
        }
        
        #endregion

        #region Countdown and Time elapsed

        public float GetTimeElapsed(IRuntimeTimerContext context)
        {
            return 0f;
        }
        
        public float GetCountdown(IRuntimeTimerContext context)
        {
            return 0f;
        }
        
        #endregion
        
        #region Controls

        public void Reset(IRuntimeTimerContext context)
        {
            context.CurrentTimeElapsed = 0f;

            context.CurrentDuration = context.DefaultDuration;
        }

        public void Start(IRuntimeTimerContext context)
        {
            context.CurrentTimeElapsed = 0f;
            
            context.SetState(ETimerState.STARTED);
            
            context.OnStartAsPublisher.Publish((IRuntimeTimer)context);
        }

        public void Pause(IRuntimeTimerContext context)
        {
            //Why bother?
        }

        public void Resume(IRuntimeTimerContext context)
        {
            //Why bother?
        }
        
        public void Abort(IRuntimeTimerContext context)
        {
            context.CurrentTimeElapsed = 0f;
        }
        
        public void Finish(IRuntimeTimerContext context)
        {
            //ENSURE WHETHER CALLING FINISH() ON INACTIVE TIMER SHOULD NOT BE CALLING A CALLBACK
        }

        public void Tick(IRuntimeTimerContext context, float delta)
        {
            //Why bother?
        }
        
        #endregion
    }
}