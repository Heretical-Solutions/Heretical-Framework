namespace HereticalSolutions.Time.Strategies
{
    public class RuntimeFinishedStrategy : ITimerStrategy<IRuntimeTimerContext, float>
    {
        #region Progress
        
        public float GetProgress(IRuntimeTimerContext context)
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

        public float GetTimeElapsed(IRuntimeTimerContext context)
        {
            return context.CurrentTimeElapsed;
        }
        
        public float GetCountdown(IRuntimeTimerContext context)
        {
            return context.CurrentDuration - context.CurrentTimeElapsed;
        }
        
        #endregion
        
        #region Controls
        
        public void Reset(IRuntimeTimerContext context)
        {
            context.CurrentTimeElapsed = 0f;

            context.CurrentDuration = context.DefaultDuration;
            
            context.SetState(ETimerState.INACTIVE);
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
            
            context.SetState(ETimerState.INACTIVE);
        }
        
        public void Finish(IRuntimeTimerContext context)
        {
            //Why bother?
        }

        public void Tick(IRuntimeTimerContext context, float delta)
        {
            //Why bother?
        }
        
        #endregion
    }
}