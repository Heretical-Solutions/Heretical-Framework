namespace HereticalSolutions.Delegates
{
    public interface INonAllocSubscribableSingleArgGeneric<TValue>
    {
        void Subscribe(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>> subscription);

        void Unsubscribe(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>> subscription);
    }
}