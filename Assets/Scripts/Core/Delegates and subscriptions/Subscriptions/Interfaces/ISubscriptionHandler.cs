using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates
{
    public interface ISubscriptionHandler<TSubscribable, TInvokable>
    {
        bool ValidateActivation(TSubscribable publisher);
        
        void Activate(
            TSubscribable publisher,
            IPoolElement<TInvokable> poolElement);

        bool ValidateTermination(TSubscribable publisher);
        
        void Terminate();
    }
}