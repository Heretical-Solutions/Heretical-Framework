namespace HereticalSolutions.Delegates
{
    public interface ISubscription
    {
        bool Active { get; }

        //void Subscribe<TSubscribable>(TSubscribable publisher);
        
        //void Unsubscribe();
    }
}