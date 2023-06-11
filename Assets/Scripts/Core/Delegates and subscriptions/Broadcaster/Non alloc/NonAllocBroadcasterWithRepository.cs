using System;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Delegates.Broadcasting
{
    public class NonAllocBroadcasterWithRepository
        : IPublisherSingleArg,
          INonAllocSubscribableSingleArg
    {
        private readonly IReadOnlyObjectRepository broadcasterRepository;

        public NonAllocBroadcasterWithRepository(IReadOnlyObjectRepository broadcasterRepository)
        {
            this.broadcasterRepository = broadcasterRepository;
        }

        #region IPublisherSingleArg

        public void Publish<TValue>(TValue value)
        {
            var messageType = typeof(TValue);

            if (!broadcasterRepository.TryGet(
                    messageType,
                    out object broadcasterObject))
                throw new Exception($"[NonAllocBroadcasterWithRepository] INVALID MESSAGE TYPE: \"{messageType.ToBeautifulString()}\"");

            var broadcaster = (IPublisherSingleArgGeneric<TValue>)broadcasterObject;
            
            broadcaster.Publish(value);
        }
        
        public void Publish(Type valueType, object value)
        {
            var messageType = valueType;
            
            if (!broadcasterRepository.TryGet(
                    messageType,
                    out object broadcasterObject))
                throw new Exception($"[NonAllocBroadcasterWithRepository] INVALID MESSAGE TYPE: \"{messageType.ToBeautifulString()}\"");

            var broadcaster = (IPublisherSingleArg)broadcasterObject;
            
            broadcaster.Publish(valueType, value);
        }

        #endregion

        #region INonAllocSubscribableSingleArg
		
        public void Subscribe<TValue>(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>> subscription)
        {
            var messageType = typeof(TValue);
            
            if (!broadcasterRepository.TryGet(
                    messageType,
                    out object broadcasterObject))
                throw new Exception($"[NonAllocBroadcasterWithRepository] INVALID MESSAGE TYPE: \"{messageType.ToBeautifulString()}\"");

            var broadcaster = (INonAllocSubscribableSingleArgGeneric<TValue>)broadcasterObject;
            
            broadcaster.Subscribe(subscription);
        }

        public void Subscribe(Type valueType, ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg> subscription)
        {
            var messageType = valueType;
            
            if (!broadcasterRepository.TryGet(
                    messageType,
                    out object broadcasterObject))
                throw new Exception($"[NonAllocBroadcasterWithRepository] INVALID MESSAGE TYPE: \"{messageType.ToBeautifulString()}\"");

            var broadcaster = (INonAllocSubscribableSingleArg)broadcasterObject;
            
            broadcaster.Subscribe(messageType, subscription);
        }

        public void Unsubscribe<TValue>(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>> subscription)
        {
            var messageType = typeof(TValue);
            
            if (!broadcasterRepository.TryGet(
                    messageType,
                    out object broadcasterObject))
                throw new Exception($"[NonAllocBroadcasterWithRepository] INVALID MESSAGE TYPE: \"{messageType.ToBeautifulString()}\"");

            var broadcaster = (INonAllocSubscribableSingleArgGeneric<TValue>)broadcasterObject;
            
            broadcaster.Unsubscribe(subscription);
        }

        public void Unsubscribe(Type valueType, ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg> subscription)
        {
            var messageType = valueType;
            
            if (!broadcasterRepository.TryGet(
                    messageType,
                    out object broadcasterObject))
                throw new Exception($"[NonAllocBroadcasterWithRepository] INVALID MESSAGE TYPE: \"{messageType.ToBeautifulString()}\"");

            var broadcaster = (INonAllocSubscribableSingleArg)broadcasterObject;
            
            broadcaster.Unsubscribe(messageType, subscription);
        }

        #endregion
    }
}