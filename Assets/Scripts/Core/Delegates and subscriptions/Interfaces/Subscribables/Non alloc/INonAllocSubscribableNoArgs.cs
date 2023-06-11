namespace HereticalSolutions.Delegates
{
    public interface INonAllocSubscribableNoArgs
    {
        void Subscribe(ISubscription subscription);

        void Unsubscribe(ISubscription subscription);
    }
}