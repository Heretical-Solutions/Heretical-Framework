using System;

namespace HereticalSolutions.Time.Strategies
{
    public class PersistentPausedStrategy : ITimerStrategy<IPersistentTimerContext, TimeSpan>
    {
        #region Progress
        
        public float GetProgress(IPersistentTimerContext context)
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

        public TimeSpan GetTimeElapsed(IPersistentTimerContext context)
        {
            return context.SavedProgress;
        }

        public TimeSpan GetCountdown(IPersistentTimerContext context)
        {
            return context.CurrentDurationSpan - context.SavedProgress;
        }
        
        #endregion

        #region Controls
        
        public void Reset(IPersistentTimerContext context)
        {
            context.StartTime = default(DateTime);
            
            context.EstimatedFinishTime = default(DateTime);

            context.SavedProgress = default(TimeSpan);
            
            
            context.CurrentDurationSpan = context.DefaultDurationSpan;
            
            context.SetState(ETimerState.INACTIVE);
        }

        public void Start(IPersistentTimerContext context)
        {
            //ENSURE THAT CALLING START() ON A PAUSED TIMER SHOULD IGNORE A CALL INSTEAD OF RESETTING THE TIMER
        }

        public void Pause(IPersistentTimerContext context)
        {
            //Why bother?
        }

        public void Resume(IPersistentTimerContext context)
        {
            context.StartTime = DateTime.Now;
            
            
            context.SetState(ETimerState.STARTED);
        }
        
        public void Abort(IPersistentTimerContext context)
        {
            context.StartTime = default(DateTime);
            
            context.EstimatedFinishTime = default(DateTime);

            context.SavedProgress = default(TimeSpan);
            
            
            context.SetState(ETimerState.INACTIVE);
        }
        
        public void Finish(IPersistentTimerContext context)
        {
            context.EstimatedFinishTime = DateTime.Now;
            
            
            context.SetState(ETimerState.FINISHED);
            
            context.OnFinishAsPublisher.Publish((IPersistentTimer)context);
        }

        public void Tick(IPersistentTimerContext context, float delta)
        {
            //Why bother?
        }
        
        #endregion
    }
}