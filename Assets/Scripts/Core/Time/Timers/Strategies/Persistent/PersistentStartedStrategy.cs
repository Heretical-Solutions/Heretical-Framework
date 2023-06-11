using System;

namespace HereticalSolutions.Time.Strategies
{
    public class PersistentStartedStrategy : ITimerStrategy<IPersistentTimerContext, TimeSpan>
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
            return context.Accumulate
                ? ((DateTime.Now - context.EstimatedFinishTime) + context.SavedProgress)
                : ((DateTime.Now - context.StartTime) + context.SavedProgress);
        }

        public TimeSpan GetCountdown(IPersistentTimerContext context)
        {
            return context.EstimatedFinishTime - DateTime.Now;
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
            //ENSURE THAT CALLING START() ON A RUNNING TIMER SHOULD IGNORE A CALL INSTEAD OF RESETTING THE TIMER
        }

        public void Pause(IPersistentTimerContext context)
        {
            context.SavedProgress = GetTimeElapsed(context);
            
            context.SetState(ETimerState.PAUSED);
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
            context.EstimatedFinishTime = DateTime.Now;
            
            
            context.SetState(ETimerState.FINISHED);
            
            context.OnFinishAsPublisher.Publish((IPersistentTimer)context);
        }

        public void Tick(IPersistentTimerContext context, float delta)
        {
            if (context.Accumulate)
            {
                var now = DateTime.Now;
                
                TimeSpan dateTimeDelta = now - context.EstimatedFinishTime;
                
                context.SavedProgress += dateTimeDelta;

                context.EstimatedFinishTime = now;
                
                return;
            }
            
            if (DateTime.Now > context.EstimatedFinishTime)
            {
                if (context.Repeat)
                {
                    context.OnFinishAsPublisher.Publish((IPersistentTimer)context);
                    
                    
                    context.StartTime = DateTime.Now;
                    
                    context.EstimatedFinishTime = context.StartTime + context.CurrentDurationSpan;
                                
                    context.SavedProgress = default(TimeSpan);
            
                    
                    context.OnStartAsPublisher.Publish((IPersistentTimer)context);
                }
                else
                    Finish(context);
            }
        }
        #endregion
    }
}