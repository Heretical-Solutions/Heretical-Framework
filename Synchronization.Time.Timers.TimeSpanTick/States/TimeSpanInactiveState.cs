using System;

namespace HereticalSolutions.Synchronization.Time.Timers.TimeSpanTick
{
    public class TimeSpanInactiveState
        : ITimeSpanTimerState
    {
        #region Progress
        
        public float GetProgressNormal(
            ITimeSpanTimerContext context)
        {
            return 0f;
        }
        
        #endregion
        
        #region Countdown and Time elapsed
        
        public TimeSpan GetTimeElapsed(
            ITimeSpanTimerContext context)
        {
            return default(TimeSpan);
        }
        
        public TimeSpan GetCountdown(
            ITimeSpanTimerContext context)
        {
            return default(TimeSpan);
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
        }
        
        public void Start(
            ITimeSpanTimerContext context)
        {
            context.StartTime = DateTime.Now;

            context.EstimatedFinishTime = context.StartTime + context.CurrentDurationSpan;
            
            context.SavedProgress = default(TimeSpan);
            
            
            context.SetState(ETimerState.STARTED);
            
            context.OnStart.Publish(context.Timer);
        }
        
        public void Pause(
            ITimeSpanTimerContext context)
        {
            //Why bother?
        }
        
        public void Resume(
            ITimeSpanTimerContext context)
        {
            //Why bother?
        }
        
        public void Abort(
            ITimeSpanTimerContext context)
        {
            context.StartTime = default(DateTime);
            
            context.EstimatedFinishTime = default(DateTime);
            
            context.SavedProgress = default(TimeSpan);
        }
        
        public void Finish(
            ITimeSpanTimerContext context)
        {
            //ENSURE WHETHER CALLING FINISH() ON INACTIVE TIMER SHOULD NOT BE CALLING A CALLBACK
        }
        
        public void Tick(
            ITimeSpanTimerContext context)
        {
            //Why bother?
        }
        
        #endregion
    }
}