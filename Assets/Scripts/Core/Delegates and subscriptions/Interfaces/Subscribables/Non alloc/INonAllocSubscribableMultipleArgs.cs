namespace HereticalSolutions.Delegates
{
    public interface INonAllocSubscribableMultipleArgs
    {
        void Subscribe(ISubscriptionHandler<INonAllocSubscribableMultipleArgs, IInvokableMultipleArgs> subscription);

        void Unsubscribe(ISubscriptionHandler<INonAllocSubscribableMultipleArgs, IInvokableMultipleArgs> subscription);
    }
}