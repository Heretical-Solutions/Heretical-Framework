using System;

using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Messaging;

using HereticalSolutions.Networking.Mirror;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Time;
using HereticalSolutions.Time.Factories;

using Mirror;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Networking.Factories
{
    public static class MirrorNetworkingFactory
    {
        public const string NETWORK_DISCOVERER_TIMER_ID = "Network discoverer";

        public static void InitializeNetworkingService(
            NetworkingService service,
            INonAllocMessageSender networkingBusAsSender,
            INonAllocMessageReceiver networkingBusAsReceiver)
        {
            service.Initialize(
                networkingBusAsSender,
                networkingBusAsReceiver);
        }

        public static NetworkingContext BuildNetworkingContext(Transport transport)
        {
            return new NetworkingContext(
                IDAllocationsFactory.BuildLongFromTwoRandomInts(),
                transport,
                System.Environment.MachineName);
        }

        public static MirrorHostAdvertiser<MirrorHostDiscoveryRequest, MirrorHostDiscoveryRequestResponse> BuildMirrorHostAdvertiser(
            MirrorHostAdvertisingAndDiscoverySettings settings,
            NetworkingContext context,
            INonAllocMessageReceiver networkingBusAsReceiver)
        {
            return new MirrorHostAdvertiser<MirrorHostDiscoveryRequest, MirrorHostDiscoveryRequestResponse>(
                networkingBusAsReceiver,
                settings,
                context,
                BuildHostDiscoveryRequestProcessor());
        }

        public static MirrorHostDiscoverer<MirrorHostDiscoveryRequest, MirrorHostDiscoveryRequestResponse> BuildMirrorHostDiscoverer(
            MirrorHostAdvertisingAndDiscoverySettings settings,
            NetworkingContext context,
            ISynchronizationProvider synchronizationProvider,
            INonAllocMessageSender networkingBusAsSender,
            INonAllocMessageReceiver networkingBusAsReceiver)
        {
            var timer = TimeFactory.BuildRuntimeTimer(
                NETWORK_DISCOVERER_TIMER_ID,
                settings.ActiveDiscoveryInterval);

            timer.Repeat = true;
            
            return new MirrorHostDiscoverer<MirrorHostDiscoveryRequest, MirrorHostDiscoveryRequestResponse>(
                networkingBusAsReceiver,
                
                settings,
                context,

                BuildHostDiscoveryRequestMaker(),
                BuildHostDiscoveryResponseProcessor(networkingBusAsSender),

                synchronizationProvider,
                timer,
                timer);
        }

        public static HostDiscoveryRequestMaker BuildHostDiscoveryRequestMaker()
        {
            return new HostDiscoveryRequestMaker();
        }

        public static HostDiscoveryRequestProcessor BuildHostDiscoveryRequestProcessor()
        {
            return new HostDiscoveryRequestProcessor();
        }

        public static HostDiscoveryResponseProcessor BuildHostDiscoveryResponseProcessor(
            INonAllocMessageSender networkingBus)
        {
            return new HostDiscoveryResponseProcessor(networkingBus);
        }

        public static MirrorLobbyServer BuildMirrorLobbyServer(
            DiContainer container,
            INonAllocMessageSender networkingBusAsSender,
            INonAllocMessageReceiver networkingBusAsReceiver,
            LobbySettings lobbySettings)
        {
            Func<GameObject> slotAllocationDelegate =
                () => UnityZenjectAllocationsFactory.DIResolveOrInstantiateAllocationDelegate(
                    container,
                    lobbySettings.lobbySlotBehaviourPrefab.gameObject);

            SlotData[] slots = new SlotData[lobbySettings.MaxPlayers];

            for (int i = 0; i < slots.Length; i++)
                slots[i] = new SlotData(i);
            
            return new MirrorLobbyServer(
                networkingBusAsSender,
                networkingBusAsReceiver,
                lobbySettings,
                slotAllocationDelegate,
                slots,
                RepositoriesFactory.BuildDictionaryRepository<NetworkConnectionToClient, SlotData>());
        }

        public static MirrorLobbyClient BuildMirrorLobbyClient(
            INonAllocMessageSender networkingBusAsSender,
            INonAllocMessageReceiver networkingBusAsReceiver,
            LobbySettings lobbySettings)
        {
            return new MirrorLobbyClient(
                networkingBusAsSender,
                networkingBusAsReceiver,
                lobbySettings);
        }
    }
}