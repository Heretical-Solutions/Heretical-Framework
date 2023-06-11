using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates
{
    public interface ISubscriptionState<TInvokable>
    {
        TInvokable Invokable { get; }

        IPoolElement<TInvokable> PoolElement { get; }
    }
}