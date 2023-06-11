using System;

using Mirror;

namespace HereticalSolutions.Networking.Mirror
{
    public struct MirrorHostDiscoveryRequestResponse : NetworkMessage
    {
        // Prevent duplicate server appearance when a connection can be made via LAN on multiple NICs
        public long ServerId;
        
        public Uri URI;

        public string Hostname;

        public int PlayersPresent;

        public int PlayersMax;
    }
}