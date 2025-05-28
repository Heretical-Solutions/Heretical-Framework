using System;

using HereticalSolutions.Delegates.NonAlloc;

namespace HereticalSolutions.Messaging.NonAlloc
{
    public interface INonAllocMessageSubscribable
    {
        void SubscribeTo<TMessage>(
            INonAllocSubscription subscription)
            where TMessage : IMessage;
        
        void SubscribeTo(
            Type messageType,
            INonAllocSubscription subscription);
		
        void UnsubscribeFrom<TMessage>(
            INonAllocSubscription subscription)
            where TMessage : IMessage;
        
        void UnsubscribeFrom(
            Type messageType,
            INonAllocSubscription subscription);
    }
}