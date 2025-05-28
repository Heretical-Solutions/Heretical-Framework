using System;

namespace HereticalSolutions.Synchronization.Time.Timers.TimeSpanTick
{
    public class TimeSpanStartedState
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
            return context.Accumulate
                ? ((DateTime.Now - context.EstimatedFinishTime) + context.SavedProgress)
                : ((DateTime.Now - context.StartTime) + context.SavedProgress);
        }

        public TimeSpan GetCountdown(
            ITimeSpanTimerContext context)
        {
            return context.EstimatedFinishTime - DateTime.Now;
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
            //ENSURE THAT CALLING START() ON A RUNNING TIMER SHOULD IGNORE A CALL INSTEAD OF RESETTING THE TIMER
        }

        public void Pause(
            ITimeSpanTimerContext context)
        {
            context.SavedProgress = GetTimeElapsed(context);

            context.SetState(ETimerState.PAUSED);
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
                    if (context.FlushTimeElapsedOnRepeat)
                    {
                        context.OnFinishRepeated.Publish(context.Timer);


                        context.StartTime = DateTime.Now;

                        context.EstimatedFinishTime = context.StartTime + context.CurrentDurationSpan;

                        context.SavedProgress = default(TimeSpan);


                        context.OnStartRepeated.Publish(context.Timer);
                    }
                    else
                    {
                        while (DateTime.Now > context.EstimatedFinishTime)
                        {
                            context.OnFinishRepeated.Publish(context.Timer);


                            context.StartTime = context.EstimatedFinishTime;

                            context.EstimatedFinishTime = context.StartTime + context.CurrentDurationSpan;

                            context.SavedProgress = default(TimeSpan);


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