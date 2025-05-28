using System;

namespace HereticalSolutions.Synchronization.Time.Timers.TimeSpanTick
{
    public class TimeSpanPausedState
        : ITimeSpanTimerState
    {
        #region Progress
        
        public float GetProgressNormal(
            ITimeSpanTimerContext context)
        {
            if (context.Accumulate)
                return 0f;
            
            double timeElapsed = GetTimeElapsed(context).TotalMilliseconds;

            double currentDuration = context.CurrentDurationSpan.TotalMilliseconds;
            
            if (currentDuration <= 0)
                return 0f;
                        
            return ((float)(timeElapsed / currentDuration)).Clamp(0f, 1f);
        }
        
        #endregion

        #region Countdown and Time elapsed

        public TimeSpan GetTimeElapsed(
            ITimeSpanTimerContext context)
        {
            return context.SavedProgress;
        }

        public TimeSpan GetCountdown(
            ITimeSpanTimerContext context)
        {
            return context.CurrentDurationSpan - context.SavedProgress;
        }
        
        #endregion

        #region Controls
        
        public void Reset(
            ITimeSpanTimerContext context)
        {
            context.StartTime = default(DateTime);
            
            context.EstimatedFinishTime = default(DateTime);

            context.SavedProgress = default(TimeSpan);
            
            
            context.CurrentDurationSpan = context.DefaultDurationSpan;
            
            context.SetState(ETimerState.INACTIVE);
        }

        public void Start(
            ITimeSpanTimerContext context)
        {
            //ENSURE THAT CALLING START() ON A PAUSED TIMER SHOULD IGNORE A CALL INSTEAD OF RESETTING THE TIMER
        }

        public void Pause(
            ITimeSpanTimerContext context)
        {
            //Why bother?
        }

        public void Resume(
            ITimeSpanTimerContext context)
        {
            context.StartTime = DateTime.Now;
            
            
            context.SetState(ETimerState.STARTED);
        }
        
        public void Abort(
            ITimeSpanTimerContext context)
        {
            context.StartTime = default(DateTime);
            
            context.EstimatedFinishTime = default(DateTime);

            context.SavedProgress = default(TimeSpan);
            
            
            context.SetState(ETimerState.INACTIVE);
        }
        
        public void Finish(
            ITimeSpanTimerContext context)
        {
            context.EstimatedFinishTime = DateTime.Now;
            
            
            context.SetState(ETimerState.FINISHED);
            
            if (context.Repeat && context.FireRepeatCallbackOnFinish)
                context.OnFinishRepeated.Publish(context.Timer);
            
            context.OnFinish.Publish(context.Timer);
        }

        public void Tick(
            ITimeSpanTimerContext context)
        {
            //Why bother?
        }
        
        #endregion
    }
}