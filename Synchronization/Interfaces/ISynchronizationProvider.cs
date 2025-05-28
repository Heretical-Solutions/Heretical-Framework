namespace HereticalSolutions.Synchronization
{
    public interface ISynchronizationProvider
    {
        void Subscribe(
            ISynchronizable synchronizable);

        void Unsubscribe(
            ISynchronizable synchronizable);

        void UnsubscribeAll();
    }
}