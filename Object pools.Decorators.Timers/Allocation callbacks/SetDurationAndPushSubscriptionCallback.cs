using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.Delegates.NonAlloc.Factories;

using HereticalSolutions.ObjectPools.Managed;

using HereticalSolutions.Synchronization.Time.Timers.FloatDelta;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Decorators.Timers
{
    public class SetPushToPoolOnTimeoutSubscriptionCallback<T>
        : IAllocationCallback<IPoolElementFacade<T>>
    {
        private readonly NonAllocSubscriptionFactory nonAllocSubscriptionFactory;

        private ILogger logger;

        public SetPushToPoolOnTimeoutSubscriptionCallback(
            NonAllocSubscriptionFactory nonAllocSubscriptionFactory,
            ILogger logger)
        {
            this.nonAllocSubscriptionFactory = nonAllocSubscriptionFactory;

            this.logger = logger;
        }

        public void OnAllocated(
            IPoolElementFacade<T> poolElementFacade)
        {
            IPoolElementFacadeWithMetadata<T> facadeWithMetadata =
                poolElementFacade as IPoolElementFacadeWithMetadata<T>;

            if (facadeWithMetadata == null)
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "POOL ELEMENT FACADE HAS NO METADATA"));
            }
            
            var metadata = (AllocatedTimerMetadata)
                facadeWithMetadata.Metadata.Get<IContainsAllocatedTimer>();

            if (metadata == null)
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "POOL ELEMENT FACADE HAS NO ALLOCATED TIMER METADATA"));
            }

            Action<IFloatTimer> pushDelegate = (timer) =>
            {
                poolElementFacade.Push();
            };

            var pushSubscription = nonAllocSubscriptionFactory.
                BuildSubscriptionSingleArgGeneric<IFloatTimer>(
                    pushDelegate);

            metadata.PushToPoolOnTimeoutSubscription = pushSubscription;
        }
    }
}