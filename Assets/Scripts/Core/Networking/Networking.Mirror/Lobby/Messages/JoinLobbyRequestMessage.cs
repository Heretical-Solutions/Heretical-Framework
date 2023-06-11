using System;

using HereticalSolutions.Messaging;

namespace HereticalSolutions.Networking.Mirror
{
    public class JoinLobbyRequestMessage : IMessage
    {
        public Uri URI { get; private set; }
        
        public void Write(object[] args)
        {
            URI = (Uri)args[0];
        }
    }
}