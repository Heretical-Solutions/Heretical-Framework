using System;

namespace HereticalSolutions.Time.Strategies
{
    public class PersistentFinishedStrategy : ITimerStrategy<IPersistentTimerContext, TimeSpan>
    {
        #region Progress
        
        public float GetProgress(IPersistentTimerContext context)
        {
            //THIS ONE IS AS EXPECTED. IF THE TIMER WAS FINISHED PREMATURELY BY A FINISH() CALL RATHER THAN TIMER ACTUALLY RUNNING OUT WE MIGHT BE CURIOUS HOW MUCH OF A PROGRESS WAS MADE SO FAR
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
            
            
            context.SetState(ETimerState.INACTIVE);
        }
        
        public void Finish(IPersistentTimerContext context)
        {
            //Why bother?
        }

        public void Tick(IPersistentTimerContext context, float delta)
        {
            //Why bother?
        }
        
        #endregion
    }
}