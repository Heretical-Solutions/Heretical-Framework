using System;
using System.Collections.Generic;

using HereticalSolutions.ObjectPools;

using HereticalSolutions.Bags;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.NonAlloc
{
    public class NonAllocBroadcasterMultipleArgs
        : IPublisherMultipleArgs,
          INonAllocSubscribable,
          ICleanuppable,
          IDisposable
    {
        private readonly IBag<INonAllocSubscription> subscriptionBag;

        private readonly IPool<IBag<INonAllocSubscription>> contextPool;

        private readonly ILogger logger;

        public NonAllocBroadcasterMultipleArgs(
            IBag<INonAllocSubscription> subscriptionBag,
            IPool<IBag<INonAllocSubscription>> contextPool,
            ILogger logger)
        {
            this.subscriptionBag = subscriptionBag;

            this.contextPool = contextPool;

            this.logger = logger;
        }

        #region INonAllocSubscribable

        public bool Subscribe(
            INonAllocSubscription subscription)
        {
            var subscriptionContext = subscription
                as INonAllocSubscriptionContext<IInvokableMultipleArgs>;

            if (subscriptionContext == null)
            {
                logger?.LogError(
                    GetType(),
                    $"INVALID SUBSCRIPTION TYPE: \"{subscription.GetType().Name}\"");

                return false;
            }

            if (!subscriptionContext.ValidateActivation(this))
                return false;

            if (!subscriptionBag.Push(
                subscription))
                return false;

            subscriptionContext.Activate(
                this);

            logger?.Log(
                GetType(),
                $"SUBSCRIPTION {subscription.GetHashCode()} ADDED: {this.GetHashCode()}");

            return true;
        }

        public bool Unsubscribe(
            INonAllocSubscription subscription)
        {
            var subscriptionContext = subscription
                as INonAllocSubscriptionContext<IInvokableMultipleArgs>;

            if (subscriptionContext == null)
            {
                logger?.LogError(
                    GetType(),
                    $"INVALID SUBSCRIPTION TYPE: \"{subscription.GetType().Name}\"");

                return false;
            }

            if (!subscriptionContext.ValidateTermination(this))
                return false;

            if (!subscriptionBag.Pop(
                subscription))
                return false;

            subscriptionContext.Terminate();

            logger?.Log(
                GetType(),
                $"SUBSCRIPTION {subscription.GetHashCode()} REMOVED: {this.GetHashCode()}");

            return true;
        }

        public IEnumerable<INonAllocSubscription> AllSubscriptions
        {
            get => subscriptionBag.All;
        }

        public void UnsubscribeAll()
        {
            //foreach (var subscription in subscriptionBag.All)
            //{
            //	Unsubscribe(subscription);
            //}

            while (subscriptionBag.Count > 0)
            {
                if (!subscriptionBag.Peek(out var subscription))
                    break;

                Unsubscribe(subscription);
            }

            subscriptionBag.Clear();
        }

        #endregion

        #region IPublisherMultipleArgs

        public void Publish(
            object[] values)
        {
            if (subscriptionBag.Count == 0)
                return;

            //Pop context out of the pool and initialize it with values from the bag

            var context = contextPool.Pop();

            context.Clear();

            foreach (var subscription in subscriptionBag.All)
            {
                context.Push(subscription);
            }

            //Invoke the delegates in the context

            foreach (var subscription in context.All)
            {
                if (subscription == null)
                    continue;

                if (!subscription.Active)
                    continue;

                var subscriptionContext = subscription
                    as INonAllocSubscriptionContext<IInvokableMultipleArgs>;

                if (subscriptionContext == null)
                    continue;

                subscriptionContext.Delegate?.Invoke(values);
            }


            //Clean up and push the context back into the pool
            context.Clear();

            contextPool.Push(context);
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            UnsubscribeAll();

            if (subscriptionBag is ICleanuppable)
                (subscriptionBag as ICleanuppable).Cleanup();

            if (contextPool is ICleanuppable)
                (contextPool as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            UnsubscribeAll();

            if (subscriptionBag is IDisposable)
                (subscriptionBag as IDisposable).Dispose();

            if (contextPool is IDisposable)
                (contextPool as IDisposable).Dispose();
        }

        #endregion
    }
}