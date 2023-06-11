namespace HereticalSolutions.Networking
{
    public interface IHostAdvertiser
    {
        bool Advertising { get; }

        void StartAdvertising();

        void StopAdvertising();
        
        void Shutdown();
    }
}