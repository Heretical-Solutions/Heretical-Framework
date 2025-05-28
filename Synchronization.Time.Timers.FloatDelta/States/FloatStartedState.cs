namespace HereticalSolutions.Synchronization.Time.Timers.FloatDelta
{
    public class FloatStartedState
        : IFloatTimerState
    {
        #region Progress
        
        public float GetProgressNormal(
            IFloatTimerContext context)
        {
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
            //ENSURE THAT CALLING START() ON A RUNNING TIMER SHOULD IGNORE A CALL INSTEAD OF RESETTING THE TIMER
        }

        public void Pause(
            IFloatTimerContext context)
        {
            context.SetState(ETimerState.PAUSED);
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
            context.SetState(ETimerState.FINISHED);
            
            if (context.Repeat && context.FireRepeatCallbackOnFinish)
                context.OnFinishRepeated.Publish(context.Timer);
            
            context.OnFinish.Publish(context.Timer);
        }

        public void Tick(
            IFloatTimerContext context,
            float delta)
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
                    if (context.FlushTimeElapsedOnRepeat)
                    {
                        context.OnFinishRepeated.Publish(context.Timer);
                        
                        context.CurrentTimeElapsed = 0f;
                
                        context.OnStartRepeated.Publish(context.Timer);
                    }
                    else
                    {
                        while (context.CurrentTimeElapsed > context.CurrentDuration)
                        {
                            context.OnFinishRepeated.Publish(context.Timer);

                            context.CurrentTimeElapsed -= context.CurrentDuration;

                            context.OnStartRepeated.Publish(context.Timer);
                        }
                    }
                }
                else
                    Finish(context);
            }
        }
        
        #endregion
    }
}