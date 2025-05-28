namespace HereticalSolutions.Delegates.NonAlloc
{
    public interface INonAllocSubscription
    {
        bool Active { get; }

        bool Subscribe(
            INonAllocSubscribable publisher);

        bool Unsubscribe();
    }
}