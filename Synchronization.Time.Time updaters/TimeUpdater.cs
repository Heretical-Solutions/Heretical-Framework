using System;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.NonAlloc;
using HereticalSolutions.Delegates.NonAlloc.Factories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Synchronization.Time.Timers.FloatDelta;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Synchronization.Time.TimeUpdaters
{
    public class TimeUpdater
        : ITimeUpdater,
          ICleanuppable,
          IDisposable
    {
        private readonly TimeUpdaterDescriptor descriptor;

        private readonly IPublisherSingleArgGeneric<float> broadcaster;

        private readonly INonAllocSubscription synchronizationSubscription;

        private readonly IFloatTimer accumulator;

        private readonly ILogger logger;

        private float fixedDeltaAccumulator;

        public TimeUpdater(
            TimeUpdaterDescriptor descriptor,
            IPublisherSingleArgGeneric<float> broadcaster,
            NonAllocSubscriptionFactory subscriptionFactory,
            IFloatTimer accumulator)
        {
            this.descriptor = descriptor;

            this.broadcaster = broadcaster;

            this.accumulator = accumulator;

            synchronizationSubscription = subscriptionFactory
                .BuildSubscriptionSingleArgGeneric<float>(
                    Synchronize);

            fixedDeltaAccumulator = 0f;


            accumulator?.Reset();

            accumulator?.Start();
        }

        #region ITimeUpdater

        #region ISynchronizationWithDeltaProvider

        public void Subscribe(
            ISynchronizableWithDelta<float> synchronizable)
        {
            var subscriber = synchronizable as ISynchronizationSubscriber;

            if (subscriber == null)
            {
                throw new ArgumentException(
                    logger.TryFormatException(
                        GetType(),
                        "SYNCHRONIZABLE IS NOT SYNCHORNIZATION SUBSCRIBER"));
            }

            var subscribable = broadcaster as INonAllocSubscribable;

            subscribable.Subscribe(
                subscriber.SynchronizationSubscription);
        }

        public void Unsubscribe(
            ISynchronizableWithDelta<float> synchronizable)
        {
            var subscriber = synchronizable as ISynchronizationSubscriber;

            if (subscriber == null)
            {
                throw new ArgumentException(
                    logger.TryFormatException(
                        GetType(),
                        "SYNCHRONIZABLE IS NOT SYNCHORNIZATION SUBSCRIBER"));
            }

            var subscribable = broadcaster as INonAllocSubscribable;

            subscribable.Unsubscribe(
                subscriber.SynchronizationSubscription);
        }

        public void UnsubscribeAll()
        {
            var subscribable = broadcaster as INonAllocSubscribable;

            subscribable.UnsubscribeAll();
        }

        #endregion

        #region ISynchronizableWithDelta

        public void Synchronize(
            float delta)
        {
            if (descriptor.Togglable)
            {
                if (!descriptor.Active)
                {
                    return;
                }
            }

            float deltaActual = delta;

            if (descriptor.Scalable)
            {
                deltaActual *= descriptor.Scale;
            }

            if (!descriptor.CanHaveNegativeDelta)
            {
                deltaActual = Math.Max(
                    0.0f,
                    deltaActual);
            }

            if (deltaActual < MathHelpers.EPSILON)
            {
                return;
            }


            var accumulatorAsSynchronizable = accumulator
                as ISynchronizableWithDelta<float>;

            //Updater may as well have no accumulator
            accumulatorAsSynchronizable?.Synchronize(
                deltaActual);


            if (descriptor.HasFixedDelta)
            {
                var sign = Math.Sign(deltaActual);

                fixedDeltaAccumulator += Math.Abs(deltaActual);

                while (fixedDeltaAccumulator >= descriptor.FixedDelta)
                {
                    fixedDeltaAccumulator -= descriptor.FixedDelta;

                    var fixedDeltaActual = sign * descriptor.FixedDelta;

                    broadcaster.Publish(fixedDeltaActual);
                }
            }
            else
                broadcaster.Publish(deltaActual);
        }

        #endregion

        #region ISynchronizationSubscriber

        public INonAllocSubscription SynchronizationSubscription => 
            synchronizationSubscription;

        #endregion

        public TimeUpdaterDescriptor Descriptor => descriptor;

        public IFloatTimer Accumulator => accumulator;

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (broadcaster is ICleanuppable)
                (broadcaster as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (broadcaster is IDisposable)
                (broadcaster as IDisposable).Dispose();
        }

        #endregion
    }
}