using HereticalSolutions.Delegates;

namespace HereticalSolutions.Time
{
    public interface ISynchronizationProvider
    {
        void Subscribe(ISubscription subscription);

        void Unsubscribe(ISubscription subscription);
    }
}