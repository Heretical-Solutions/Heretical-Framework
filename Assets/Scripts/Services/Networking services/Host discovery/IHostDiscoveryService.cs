using System;
using System.Collections.Generic;


namespace HereticalSolutions.Services
{
    public interface IHostDiscoveryService
    {
        void DiscoverServers();

        void StopDiscovering();
        
        IEnumerable<long> ServersDiscovered { get; }

        HostData GetDiscoveredServerData(long hostID);
        
        Action<long> OnServerDiscovered { get; set; }
    }
}