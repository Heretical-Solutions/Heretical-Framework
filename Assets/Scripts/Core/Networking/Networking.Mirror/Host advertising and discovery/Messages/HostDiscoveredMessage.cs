using System;
using System.Net;

using HereticalSolutions.Messaging;

namespace HereticalSolutions.Networking
{
    public class HostDiscoveredMessage : IMessage
    {
        // Prevent duplicate server appearance when a connection can be made via LAN on multiple NICs
        public long ServerId { get; private set; }

        public Uri URI { get; private set; }
        
        // The server that sent this
        // this is a property so that it is not serialized,  but the
        // client fills this up after we receive it
        public IPEndPoint EndPoint { get; private set; }

        public void Write(object[] args)
        {
            ServerId = (long)args[0];
            URI = (Uri)args[1];
            EndPoint = (IPEndPoint)args[2];
        }
    }
}