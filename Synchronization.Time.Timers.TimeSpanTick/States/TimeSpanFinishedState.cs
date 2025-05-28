using System;

namespace HereticalSolutions.Synchronization.Time.Timers.TimeSpanTick
{
    public class TimeSpanFinishedState
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
            context.StartTime = DateTime.Now;

            context.EstimatedFinishTime = context.StartTime + context.CurrentDurationSpan;

            context.SavedProgress = default(TimeSpan);


            context.SetState(ETimerState.STARTED);

            context.OnStart.Publish(context.Timer);
        }

        public void Pause(
            ITimeSpanTimerContext context)
        {
            // Why bother?
        }

        public void Resume(
            ITimeSpanTimerContext context)
        {
            // Why bother?
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
            // Why bother?
        }

        public void Tick(
            ITimeSpanTimerContext context)
        {
            // Why bother?
        }

        #endregion
    }
}