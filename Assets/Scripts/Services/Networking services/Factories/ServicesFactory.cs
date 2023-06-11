using HereticalSolutions.Messaging;
using HereticalSolutions.Networking;

using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Services.Factories
{
    public static partial class ServicesFactory
    {
        public static void InitializeHostAdvertisingService(
            HostAdvertisingService service,
            IHostAdvertiser advertiser)
        {
            service.Initialize(advertiser);
        }

        public static void InitializeHostDiscoveryService(
            HostDiscoveryService service,
            IHostDiscoverer discoverer,
            INonAllocMessageReceiver networkingBus)
        {
            service.Initialize(
                discoverer,
                networkingBus,
                RepositoriesFactory.BuildDictionaryRepository<long, HostData>());
        }
    }
}