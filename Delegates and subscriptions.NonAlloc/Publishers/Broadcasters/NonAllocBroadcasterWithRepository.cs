using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.NonAlloc
{
    public class NonAllocBroadcasterWithRepository
        : IPublisherSingleArg,
          INonAllocSubscribable,
          ICleanuppable,
          IDisposable
    {
        private readonly IReadOnlyRepository<Type, object> broadcasterRepository;

        private readonly ILogger logger;

        public NonAllocBroadcasterWithRepository(
            IReadOnlyRepository<Type, object> broadcasterRepository,
            ILogger logger)
        {
            this.broadcasterRepository = broadcasterRepository;

            this.logger = logger;
        }

        #region INonAllocSubscribable

        public bool Subscribe(
            INonAllocSubscription subscription)
        {
            Type valueType = null;

            switch (subscription)
            {
                case INonAllocSubscriptionContext<IInvokableSingleArg> 
                    singleArgSubscriptionContext:

                    valueType = singleArgSubscriptionContext.Delegate.ValueType;

                    break;

                default:
                
                    logger?.LogError(
                        GetType(),
                        $"INVALID SUBSCRIPTION TYPE: \"{subscription.GetType().Name}\"");

                    return false;
            }

            if (!broadcasterRepository.TryGet(
                valueType,
                out object broadcasterObject))
            {
                logger?.LogError(
                    GetType(),
                    $"INVALID VALUE TYPE: \"{valueType.Name}\"");

                return false;
            }

            var broadcaster = (INonAllocSubscribable)broadcasterObject;

            broadcaster.Subscribe(
                subscription);

            return true;
        }

        public bool Unsubscribe(
            INonAllocSubscription subscription)
        {
            Type valueType = null;

            switch (subscription)
            {
                case INonAllocSubscriptionContext<IInvokableSingleArg> 
                    singleArgSubscriptionContext:

                    valueType = singleArgSubscriptionContext.Delegate.ValueType;

                    break;

                default:

                    logger?.LogError(
                        GetType(),
                        $"INVALID SUBSCRIPTION TYPE: \"{subscription.GetType().Name}\"");

                    return false;
            }

            if (!broadcasterRepository.TryGet(
                valueType,
                out object broadcasterObject))
            {
                logger?.LogError(
                    GetType(),
                    $"INVALID VALUE TYPE: \"{valueType.Name}\"");

                return false;
            }

            var broadcaster = (INonAllocSubscribable)broadcasterObject;

            broadcaster.Unsubscribe(
                subscription);

            return true;
        }
        
        public IEnumerable<INonAllocSubscription> AllSubscriptions
        {
            get
            {
                //TODO: consider yield instead
                List<INonAllocSubscription> result = new List<INonAllocSubscription>();

                foreach (var broadcasterObject in broadcasterRepository.Values)
                {
                    var broadcaster = (INonAllocSubscribable)broadcasterObject;

                    result.AddRange(broadcaster.AllSubscriptions);
                }

                return result;
            }
        }

        public void UnsubscribeAll()
        {
            foreach (var broadcaster in broadcasterRepository.Values)
            {
                ((INonAllocSubscribable)broadcaster).UnsubscribeAll();
            }

        }

        #endregion

        #region IPublisherSingleArg

        public void Publish<TValue>(
            TValue value)
        {
            var valueType = typeof(TValue);

            if (!broadcasterRepository.TryGet(
                valueType,
                out object broadcasterObject))
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));
            }

            var broadcaster = (IPublisherSingleArgGeneric<TValue>)
                broadcasterObject;

            broadcaster.Publish(value);
        }

        public void Publish(
            Type valueType,
            object value)
        {
            if (!broadcasterRepository.TryGet(
                valueType,
                out object broadcasterObject))
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID VALUE TYPE: \"{valueType.Name}\""));
            }

            var broadcaster = (IPublisherSingleArg)broadcasterObject;

            broadcaster.Publish(
                valueType,
                value);
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (broadcasterRepository is ICleanuppable)
                (broadcasterRepository as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (broadcasterRepository is IDisposable)
                (broadcasterRepository as IDisposable).Dispose();
        }

        #endregion
    }
}