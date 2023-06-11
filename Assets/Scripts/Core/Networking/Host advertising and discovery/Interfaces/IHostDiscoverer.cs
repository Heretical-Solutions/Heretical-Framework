namespace HereticalSolutions.Networking
{
    public interface IHostDiscoverer
    {
        bool Discovering { get; }

        void StartDiscovery();

        void StopDiscovery();
        
        void Shutdown();
    }
}