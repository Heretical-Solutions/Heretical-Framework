namespace HereticalSolutions.Time.Strategies
{
    public class RuntimeStartedStrategy : ITimerStrategy<IRuntimeTimerContext, float>
    {
        #region Progress
        
        public float GetProgress(IRuntimeTimerContext context)
        {
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
            //ENSURE THAT CALLING START() ON A RUNNING TIMER SHOULD IGNORE A CALL INSTEAD OF RESETTING THE TIMER
        }

        public void Pause(IRuntimeTimerContext context)
        {
            context.SetState(ETimerState.PAUSED);
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
            context.SetState(ETimerState.FINISHED);
            
            context.OnFinishAsPublisher.Publish((IRuntimeTimer)context);
        }

        public void Tick(IRuntimeTimerContext context, float delta)
        {
            context.CurrentTimeElapsed += delta;

            if (context.Accumulate)
            {
                return;
            }

            if (context.CurrentTimeElapsed > context.CurrentDuration)
            {
                if (context.Repeat)
                {
                    context.OnFinishAsPublisher.Publish((IRuntimeTimer)context);
                    
                    context.CurrentTimeElapsed = 0f;
            
                    context.OnStartAsPublisher.Publish((IRuntimeTimer)context);
                }
                else
                    Finish(context);
            }
        }
        
        #endregion
    }
}