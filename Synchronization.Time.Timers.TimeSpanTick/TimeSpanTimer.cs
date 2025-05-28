using System;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Delegates.NonAlloc;
using HereticalSolutions.Delegates.NonAlloc.Factories;

using HereticalSolutions.Persistence;

namespace HereticalSolutions.Synchronization.Time.Timers.TimeSpanTick
{
    public class TimeSpanTimer
        : ITimeSpanTimer,
          ISynchronizable,
          ISynchronizationSubscriber,
          IVisitable,
          ICleanuppable,
          IDisposable
    {
        private readonly ITimeSpanTimerContext context;

        private readonly INonAllocSubscription synchronizationSubscription;

        public TimeSpanTimer(
            ITimeSpanTimerContext context,
            NonAllocSubscriptionFactory subscriptionFactory)
        {
            this.context = context;

            synchronizationSubscription = subscriptionFactory
                .BuildSubscriptionNoArgs(
                    Synchronize);
        }

        #region IPersistentTimer

        #region ITimer

        public string ID => context.DTO.ID;

        #region Timer state

        public ETimerState State => context.CurrentState;

        #endregion

        #region Progress

        public float ProgressNormal => context
            .CurrentStateInstance
            .GetProgressNormal(
                context);

        #endregion

        #region Controls

        public bool Accumulate
        {
            get => context.Accumulate;
            set => context.Accumulate = value;
        }

        public bool Repeat
        {
            get => context.Repeat;
            set => context.Repeat = value;
        }

        public bool FlushTimeElapsedOnRepeat
        {
            get => context.FlushTimeElapsedOnRepeat;
            set => context.FlushTimeElapsedOnRepeat = value;
        }

        public bool FireRepeatCallbackOnFinish
        {
            get => context.FireRepeatCallbackOnFinish;
            set => context.FireRepeatCallbackOnFinish = value;
        }

        public void Reset()
        {
            context
                .CurrentStateInstance
                .Reset(
                    context);
        }

        public void Start()
        {
            context
                .CurrentStateInstance
                .Start(
                    context);
        }

        public void Pause()
        {
            context
                .CurrentStateInstance
                .Pause(
                    context);
        }

        public void Resume()
        {
            context
                .CurrentStateInstance
                .Resume(
                    context);
        }

        public void Abort()
        {
            context
                .CurrentStateInstance
                .Abort(
                    context);
        }

        public void Finish()
        {
            context
                .CurrentStateInstance
                .Finish(
                    context);
        }

        #endregion

        #region Callbacks

        public INonAllocSubscribable OnStart => context.OnStart
            as INonAllocSubscribable;

        public INonAllocSubscribable OnStartRepeated => context.OnStartRepeated
            as INonAllocSubscribable;

        public INonAllocSubscribable OnFinish => context.OnFinish
            as INonAllocSubscribable;

        public INonAllocSubscribable OnFinishRepeated => context.OnFinishRepeated
            as INonAllocSubscribable;

        #endregion

        #endregion

        public ITimeSpanTimerContext Context => context;

        #region Timer state

        public DateTime StartTime => context.StartTime;

        public DateTime EstimatedFinishTime => context.EstimatedFinishTime;

        #endregion

        #region Countdown and Time elapsed

        public TimeSpan TimeElapsedSpan => 
            context
                .CurrentStateInstance
                .GetTimeElapsed(
                    context);

        public TimeSpan CountdownSpan => 
            context
                .CurrentStateInstance
                .GetCountdown(
                    context);

        #endregion

        #region Duration

        public TimeSpan CurrentDurationSpan => context.CurrentDurationSpan;

        public TimeSpan DefaultDurationSpan => context.DefaultDurationSpan;

        #endregion

        #region Controls

        public void Reset(
            TimeSpan durationSpan)
        {
            Reset();

            context.CurrentDurationSpan = durationSpan;
        }

        /// <summary>
        /// Starts the timer with a specified duration span
        /// </summary>
        /// <param name="durationSpan">The duration span to set for the timer.</param>
        public void Start(
            TimeSpan durationSpan)
        {
            context.CurrentDurationSpan = durationSpan;

            Start();
        }

        #endregion

        #endregion

        #region ISynchronizableWithDelta

        public void Synchronize()
        {
            var PersistentTimerState = context.CurrentStateInstance
                as ITimeSpanTimerState;

            PersistentTimerState
                .Tick(
                    context);
        }

        #endregion

        #region ISynchronizationSubscriber

        public INonAllocSubscription SynchronizationSubscription =>
            synchronizationSubscription;

        #endregion

        #region IVisitable

        public bool AcceptSave(
            ISaveVisitor visitor,
            ref object dto)
        {
            return visitor.VisitSave<TimeSpanTimer>(
                ref dto,
                this,
                visitor);
        }

        public bool AcceptPopulate(
            IPopulateVisitor visitor,
            object dto)
        {
            return visitor.VisitPopulate<TimeSpanTimer>(
                dto,
                this,
                visitor);
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (context is ICleanuppable)
                (context as ICleanuppable).Cleanup();

            if (synchronizationSubscription is ICleanuppable)
                (synchronizationSubscription as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (context is IDisposable)
                (context as IDisposable).Dispose();

            if (synchronizationSubscription is IDisposable)
                (synchronizationSubscription as IDisposable).Dispose();
        }

        #endregion
    }
}