using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Messaging;

using HereticalSolutions.Networking;

using HereticalSolutions.Repositories;

using UnityEngine;

namespace HereticalSolutions.Services
{
    public class HostDiscoveryService
        : MonoBehaviour,
          IHostDiscoveryService
    {
        private IHostDiscoverer hostDiscoverer;

        private INonAllocMessageReceiver networkingBus;

        private IRepository<long, HostData> discoveredServers;

        private ISubscription serverDiscoveredSubscription;

        public void Initialize(
            IHostDiscoverer hostDiscoverer,
            INonAllocMessageReceiver networkingBus,
            IRepository<long, HostData> discoveredServers)
        {
            this.hostDiscoverer = hostDiscoverer;

            this.networkingBus = networkingBus;
            
            this.discoveredServers = discoveredServers;

            serverDiscoveredSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<HostDiscoveredMessage>(OnHostDiscoveredMessage);
            
            this.networkingBus.SubscribeTo<HostDiscoveredMessage>(serverDiscoveredSubscription);
        }

        public void DiscoverServers()
        {
            //TODO: add
            //discoveredServers.RemoveAll();
            
            hostDiscoverer?.StartDiscovery();
        }

        public void StopDiscovering()
        {
            hostDiscoverer?.StopDiscovery();
        }

        public IEnumerable<long> ServersDiscovered
        {
            get => discoveredServers.Keys;
        }

        public HostData GetDiscoveredServerData(long hostID)
        {
            discoveredServers.TryGet(hostID, out var result);

            return result;
        }

        public Action<long> OnServerDiscovered { get; set; }
        

        private void OnHostDiscoveredMessage(HostDiscoveredMessage message)
        {
            // Note that you can check the versioning to decide if you can connect to the server or not using this method
            discoveredServers.AddOrUpdate(
                message.ServerId,
                new HostData()
                {
                    ID = message.ServerId,
                    EndPoint = message.EndPoint,
                    URI = message.URI
                });
            
            OnServerDiscovered?.Invoke(message.ServerId);
        }
        
        // Ensure the ports are cleared no matter when Game/Unity UI exits
        void OnApplicationQuit()
        {
            if (networkingBus != null
                && serverDiscoveredSubscription != null
                && serverDiscoveredSubscription.Active)
                networkingBus.UnsubscribeFrom<HostDiscoveredMessage>(serverDiscoveredSubscription);
            
            hostDiscoverer?.Shutdown();
        }

        void OnDisable()
        {
            if (networkingBus != null
                && serverDiscoveredSubscription != null
                && serverDiscoveredSubscription.Active)
                networkingBus.UnsubscribeFrom<HostDiscoveredMessage>(serverDiscoveredSubscription);
            
            hostDiscoverer?.Shutdown();
        }

        void OnDestroy()
        {
            if (networkingBus != null
                && serverDiscoveredSubscription != null
                && serverDiscoveredSubscription.Active)
                networkingBus.UnsubscribeFrom<HostDiscoveredMessage>(serverDiscoveredSubscription);
            
            hostDiscoverer?.Shutdown();
        }
    }
}