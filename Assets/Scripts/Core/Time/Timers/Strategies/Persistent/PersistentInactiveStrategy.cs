using System;

namespace HereticalSolutions.Time.Strategies
{
    public class PersistentInactiveStrategy : ITimerStrategy<IPersistentTimerContext, TimeSpan>
    {
        #region Progress

        public float GetProgress(IPersistentTimerContext context)
        {
            return 0f;
        }

        #endregion

        #region Countdown and Time elapsed

        public TimeSpan GetTimeElapsed(IPersistentTimerContext context)
        {
            return default(TimeSpan);
        }

        public TimeSpan GetCountdown(IPersistentTimerContext context)
        {
            return default(TimeSpan);
        }
        
        #endregion
        
        #region Controls

        public void Reset(IPersistentTimerContext context)
        {
            context.StartTime = default(DateTime);
            
            context.EstimatedFinishTime = default(DateTime);

            context.SavedProgress = default(TimeSpan);
            
            
            context.CurrentDurationSpan = context.DefaultDurationSpan;
        }

        public void Start(IPersistentTimerContext context)
        {
            context.StartTime = DateTime.Now;

            context.EstimatedFinishTime = context.StartTime + context.CurrentDurationSpan;
            
            context.SavedProgress = default(TimeSpan);
            
            
            context.SetState(ETimerState.STARTED);
            
            context.OnStartAsPublisher.Publish((IPersistentTimer)context);
        }

        public void Pause(IPersistentTimerContext context)
        {
            //Why bother?
        }

        public void Resume(IPersistentTimerContext context)
        {
            //Why bother?
        }
        
        public void Abort(IPersistentTimerContext context)
        {
            context.StartTime = default(DateTime);
            
            context.EstimatedFinishTime = default(DateTime);
            
            context.SavedProgress = default(TimeSpan);
        }
        
        public void Finish(IPersistentTimerContext context)
        {
            //ENSURE WHETHER CALLING FINISH() ON INACTIVE TIMER SHOULD NOT BE CALLING A CALLBACK
        }

        public void Tick(IPersistentTimerContext context, float delta)
        {
            //Why bother?
        }
        
        #endregion
    }
}