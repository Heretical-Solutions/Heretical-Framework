using System;

using HereticalSolutions.Delegates;

namespace HereticalSolutions.Messaging
{
    public interface INonAllocMessageReceiver
    {
        void SubscribeTo<TMessage>(ISubscription subscription) where TMessage : IMessage;
        
        void SubscribeTo(Type messageType, ISubscription subscription);
		
        void UnsubscribeFrom<TMessage>(ISubscription subscription) where TMessage : IMessage;
        
        void UnsubscribeFrom(Type messageType, ISubscription subscription);
    }
}