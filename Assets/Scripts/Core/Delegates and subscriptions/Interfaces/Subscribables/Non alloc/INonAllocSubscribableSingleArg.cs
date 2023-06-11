using System;

namespace HereticalSolutions.Delegates
{
    public interface INonAllocSubscribableSingleArg
    {
        void Subscribe<TValue>(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>> subscription);
        
        void Subscribe(Type valueType, ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg> subscription);

        void Unsubscribe<TValue>(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>> subscription);
        
        void Unsubscribe(Type valueType, ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg> subscription);
    }
}